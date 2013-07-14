using System;
using System.Threading;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using XNAnimation;
using XNAnimation.Controllers;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System.Diagnostics;

namespace WindowsGame1
{
    class BorisCards
    {
        enum State //card game states 
        {
            Breathe,
            Showcard,
            Wait,
            Changecard,
            Win,
            Lose
        }

        Game1 game1;
        Player currentPlayer; 
        ContentManager content;
        private Matrix view;
        private Matrix projection;
        WorldCards worldCards;
        SignPack signPack;
        TimerManager timerManager;

        Texture2D blueTex;
        Texture2D pinkTex;
        Texture2D currentBorisTex; //boris current texture
        Texture2D headTex;
        Texture2D cardFaceTex; 

        private SkinnedModel currentModel; //which model is currently in use, can be either borismodel or facelessmodel
        private SkinnedModel borisModel; 
        private SkinnedModel borisFacelessModel;
        private SkinnedModel cardModel; //model for Boris' card
        private AnimationController borisAnimController;
        private AnimationController cardAnimController;

        private int activeAnimationClip; //which is the current animation/sign
        public bool hascard = false;
        int currentSign = 0;
        int nextSign;

        float cardFaceAlpha = 1.0f; //for fading the card to black, stard at 100% visible
        public int cardFadingStage = 0; //different stages to the fading animation (fade in, out, pause etc)
        int cardAnimStage = 0; //different stages to animating the card(move forard, back, pause etc)
        float cardFadeTime = 0.1f; //how long the fade should last

        bool boredTimer = true; //whether Boris should show his bored animation yet.
        bool playable;          //whether accepting kinect input
        bool diagrams   = true; //should show diagrams or pictures on the card?
        bool videos     = true; //should play videos?

        //threading variables for videos. 
        private volatile List<Video> thevideos;
        private volatile List<VideoPlayer> videoplayers; //each video has it's own video player, experiment to avoid performance issues
        private volatile List<MediaState> mediaStates;
        private volatile bool _loop;
        private volatile Texture2D _texture;
        private Thread _videoThread;

        public BorisCards(Game1 game1, SignPack signPack, WorldCards worldCards, Player currentPlayer)
        {
            this.signPack = signPack;
            this.currentPlayer = currentPlayer;
            timerManager = new TimerManager();
            content = game1.Content;
            this.game1 = game1;
            this.worldCards = worldCards;
            view = Matrix.CreateLookAt(new Vector3(0, 12.5f, 26), new Vector3(0, 10.5f, 0), Vector3.Up);
            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, 1.77f, 1, 1000);
            videoplayers = new List<VideoPlayer>();
            mediaStates = new List<MediaState>();
            thevideos = new List<Video>();

            for (int i = 0; i < signPack.noOfSigns; i++)// Video vid in signPack.GetVideo())
            {
                videoplayers.Add(new VideoPlayer());
                videoplayers[i].IsLooped = true;
                thevideos.Add(signPack.GetVideo(i));
                videoplayers[i].Play(thevideos[i]);
                videoplayers[i].Pause();
                mediaStates.Add(MediaState.Paused);
            }

            _videoThread = new Thread(DoVideoThread) { IsBackground = true, Name = "VideoThread" }; //Create and start the new thread
            _videoThread.Start();
        }

        /// <summary>
        /// Load your graphics content.
        /// </summary>
        public void LoadContent()
        {
            cardModel = content.Load<SkinnedModel>("Models/Card_s");
            borisModel = content.Load<SkinnedModel>("Models/Boris_Cardgame");
            borisFacelessModel = content.Load<SkinnedModel>("Models/Boris_Cardgame_nofacial");
            currentModel = borisModel;
            headTex = content.Load<Texture2D>("Textures/head");
            blueTex = content.Load<Texture2D>("Textures/blue");
            pinkTex = content.Load<Texture2D>("Textures/pink");
            currentBorisTex = blueTex;

            // Create an animation controller and start a clip
            borisAnimController = new AnimationController(currentModel.SkeletonBones);
            //borisAnimController.Speed = 0.5f;
            borisAnimController.TranslationInterpolation = InterpolationMode.Linear;
            borisAnimController.OrientationInterpolation = InterpolationMode.Linear;
            borisAnimController.ScaleInterpolation = InterpolationMode.Linear;
            borisAnimController.StartClip(currentModel.AnimationClips["standingnocard"]);
            borisAnimController.LoopEnabled = false;
            cardAnimController = new AnimationController(cardModel.SkeletonBones);
            //borisAnimController.Speed = 0.5f;
            cardAnimController.TranslationInterpolation = InterpolationMode.Linear;
            cardAnimController.OrientationInterpolation = InterpolationMode.Linear;
            cardAnimController.ScaleInterpolation = InterpolationMode.Linear;
            cardAnimController.StartClip(cardModel.AnimationClips["standingnocard"]);
            cardAnimController.LoopEnabled = false;
            cardAnimController.TranslationInterpolation = InterpolationMode.None;
            cardAnimController.OrientationInterpolation = InterpolationMode.None;
            cardAnimController.ScaleInterpolation = InterpolationMode.None;

            activeAnimationClip = 1;
            if (diagrams == true)
            {
                cardFaceTex = signPack.GetDiagram(0);
            }
            else
            {
                cardFaceTex = signPack.GetPhotos(0);
            }
            this.ShowCard();
        }

        //Seperate thread created to get around performance problems of XNA videos. 
        //Videos will pause game before playing under normal conditions, 
        //but this is removed by moving the code to another thread. 
        private void DoVideoThread()
        {
            int sleepamount = 1000 / 60; //how often to update the thread (in milliseconds), currently 60fps (ish)
            while (true)                 //start thread loop, will remain in this indefinitely. 
            {
                for (int i = 0; i < videoplayers.Count; i++) 
                {
                    if (mediaStates[i] == MediaState.Stopped) //if the videoplayer's own videostate is set to stop, the video must be rewound...
                    {
                        videoplayers[i].IsMuted = true; //start by muting it, as it will still be playing after fading away
                        if (videoplayers[i].PlayPosition < new TimeSpan(0, 0, 0, 0, 500)) //once it has looped back to the beginning, pause it ready to play next time
                        {
                            mediaStates[i] = MediaState.Paused;
                            videoplayers[i].Pause();
                            videoplayers[i].IsMuted = false;
                        }
                    }
                }

                if (mediaStates[currentSign] != videoplayers[currentSign].State) //if a request has been made to change state...
                {
                    switch (mediaStates[currentSign])
                    {
                        case MediaState.Paused:
                            videoplayers[currentSign].Pause(); 
                            break;

                        case MediaState.Playing:
                            videoplayers[currentSign].Resume(); //if it's set to play now, use Resume so it doesn't reload from the start
                            break;
                    }
                }
                if (mediaStates[currentSign] == MediaState.Playing)
                {
                    UpdateTexture(); //update the texture for the card face
                }
                Thread.Sleep(sleepamount);//this stops the thread updating for x amount of frames, otherwise it will max the cpu
            }
        }

        private void UpdateTexture() //get a new image from the current videoplayer
        {
            _texture = videoplayers[currentSign].GetTexture();
        }



        //what to do if the correct sign is played
        public bool Win()
        {
            if (playable == true) //if the current state allows input
            {
                mediaStates[currentSign] = MediaState.Stopped; //set the video thread to stop playing videos
                worldCards.Win(); //tell the world that Boris has won
                timerManager.CancelAllTimers();
                playable = false;

                if (cardFadingStage != 0 && videos == true) //set to fade instantly if playing video
                {
                    cardFadingStage = 4;
                }
                borisAnimController.CrossFade(currentModel.AnimationClips["correct"], TimeSpan.FromSeconds(0.05f));
                cardAnimController.CrossFade(cardModel.AnimationClips["correct"], TimeSpan.FromSeconds(0.05f));

                if (currentSign == signPack.noOfSigns - 1) //check if we are at the end...
                {
                    HideCard(); //and put the card away
                }
                else
                {
                    ShowCard(); //show the next card
                    nextSign++;
                }
                return true;
            }
            return false;
        }

        //what to do if the incorrect sign is played
        public bool Lose()
        {
            if (playable == true)
            {
                worldCards.Lose(); //tell Boris we have lost
                timerManager.CancelAllTimers();
                playable = false;
                cardAnimStage = 3;
                borisAnimController.CrossFade(currentModel.AnimationClips["wrong"], TimeSpan.FromSeconds(0.05f));
                cardAnimController.CrossFade(cardModel.AnimationClips["wrong"], TimeSpan.FromSeconds(0.05f));
                return true;
            }
            return false;
        }

        //set Boris to show a new card after the timer countdown
        public void ShowCard()
        {
            if (cardFadingStage != 0 && videos == true)
            {
                cardFadingStage = 4;
            }
            cardAnimStage = 0;
            nextSign = currentSign;
            timerManager.CancelAllTimers();
            Timer aTimer = new Timer(timerManager, () => ShowingCard(), 2.0f);
        }

        //Actually animate showing the card
        public void ShowingCard()
        {
            timerManager.CancelAllTimers();
            Timer theTimer = new Timer(timerManager, () => PrepareCardFade(), 4.0f); //after timer start fading the card
            if (hascard == true) //If he is already holding the card, just show a new face on it...
            {
                cardAnimStage = 1; //start animating card from the 1st stage
                borisAnimController.CrossFade(currentModel.AnimationClips["changecard"], TimeSpan.FromSeconds(0.05f));
                cardAnimController.CrossFade(cardModel.AnimationClips["changecard"], TimeSpan.FromSeconds(0.05f));
            }
            else //otherwise he will have to make the card appear from thin air
            {
                borisAnimController.CrossFade(currentModel.AnimationClips["showcard"], TimeSpan.FromSeconds(0.05f)); //plays the first half of the animation, hiding the face of the card
                cardAnimController.CrossFade(cardModel.AnimationClips["showcard"], TimeSpan.FromSeconds(0.05f));
                hascard = true;
                cardAnimStage = 3; 
                currentSign = 0;
                if (diagrams == true)
                {
                    cardFaceTex = signPack.GetDiagram(currentSign);
                }
                else
                {
                    cardFaceTex = signPack.GetPhotos(currentSign);
                }
            }
        }

        public void Update(GameTime gameTime)
        {
            //check with the black box whether the current sign has been played
            int hasWon = this.game1.kinectManager.CheckForSign(currentSign, this.currentPlayer.GetPrecision(signPack.packName), this.currentPlayer.playerSave.leftHanded);
            if (hasWon != -1)
            {
                if (hasWon == 1)
                    Win();
                else if (hasWon == 0)
                    Lose();
            }
            this.fadecard(); //update the card's fading stage
            
            if (borisAnimController.IsPlaying == false && borisAnimController.CrossFading == false) //If he has stopped animating
            {
                this.animateCard(); //update the card's movement
            }

            borisAnimController.Update(gameTime.ElapsedGameTime, Matrix.Identity);
            cardAnimController.Update(gameTime.ElapsedGameTime, Matrix.Identity);
            timerManager.Update(gameTime);
        }

        //cycles through a number of stages to animate the card moving. 
        //Starts when the card has already been turned to face away from the camera so the texture switch isn't seen
        public void animateCard()
        {
            if (hascard == true)
            {
                if (cardAnimStage == 1) //if he is in the process of changing card, and it's now facing away from the camera
                {
                    currentSign = nextSign; //update the sign
                    if (diagrams == true) //get the new texture
                    {
                        cardFaceTex = signPack.GetDiagram(currentSign);
                    }
                    else
                    {
                        cardFaceTex = signPack.GetPhotos(currentSign);
                    }

                    //finish the animation to reshow the face of the card
                    borisAnimController.CrossFade(currentModel.AnimationClips["changecard2"], TimeSpan.FromSeconds(0.05f));
                    cardAnimController.CrossFade(cardModel.AnimationClips["changecard2"], TimeSpan.FromSeconds(0.05f));
                    cardAnimStage = 2;
                }
                else if (cardAnimStage == 2) //set card to fade in
                {
                    cardAnimStage = 3;
                    if (videos == true)
                    {
                        boredTimer = true;
                    }
                    timerManager.CancelAllTimers();
                    Timer aTimer = new Timer(timerManager, () => PrepareCardFade(), 1.0f);
                }
                else if (cardAnimStage == 3) //while animating update the texture on the card
                {
                    if (videos == false)
                    {
                    //    boredTimer = false;
                    }

                    UpdateTexture();
                    cardAnimStage = 0;
                    playable = true;
                }
                else
                {
                    if (boredTimer == false && cardFadingStage == 0)
                    {
                        boredTimer = true;
                        Timer aTimer = new Timer(timerManager, () => BeBored(), 3.0f);
                    }
                  //  Timer aTimer = new Timer(timerManager, () => BeBored(), 3.0f);
                    borisAnimController.CrossFade(currentModel.AnimationClips["standingcard"], TimeSpan.FromSeconds(0.05f));
                    cardAnimController.CrossFade(cardModel.AnimationClips["standingcard"], TimeSpan.FromSeconds(0.05f));
                }
            }
            else
            {
                Trace.WriteLine("else2");
                borisAnimController.CrossFade(currentModel.AnimationClips["standingnocard"], TimeSpan.FromSeconds(0.05f));
                cardAnimController.CrossFade(cardModel.AnimationClips["standingnocard"], TimeSpan.FromSeconds(0.05f));
            }
            
        }

        //Go through the stages of fading the card, for switch between diagrams/photos or for starting/finishing videos
        public void fadecard()
        {
            if (cardFadingStage == 1) //fade to black
            {
                if (cardFaceAlpha >= 0.0f)
                {
                    cardFaceAlpha -= cardFadeTime;
                }
                else
                {
                    cardFadingStage = 2;
                }
            }
            else if (cardFadingStage == 2) //unfade to visible face
            {
                mediaStates[currentSign] = MediaState.Playing; //tell video thread to start playing
                cardFaceAlpha += cardFadeTime;
                if (cardFaceAlpha >= 1.0f)
                {
                    cardFadingStage = 3;
                }
            }
            else if (cardFadingStage == 3) //play video
            {
                if (videoplayers[currentSign].PlayPosition >= videoplayers[currentSign].Video.Duration.Subtract(new TimeSpan(0, 0, 0, 0, 200)))
                {
                    mediaStates[currentSign] = MediaState.Stopped;
                    cardFadingStage = 4;
                }
            }
            else if (cardFadingStage == 4)//face to black again 
            {
                if (cardFaceAlpha >= 0.0f)
                {
                    cardFaceAlpha -= cardFadeTime;
                }
                else //once faded change the face texture 
                {
                    if (diagrams == true)
                    {
                        cardFaceTex = signPack.GetDiagram(currentSign);
                    }
                    else
                    {
                        cardFaceTex = signPack.GetPhotos(currentSign);
                    }
                    cardFadingStage = 5;
                }
            }
            else if (cardFadingStage == 5) //unfade to show the new card
            {
                cardFaceAlpha += cardFadeTime;
                if (cardFaceAlpha >= 1.0f)
                {
                  //  boredTimer = false;
                    //if (boredTimer == false && cardFadingStage == 0)
                    //{
                        boredTimer = true;
                        Timer aTimer = new Timer(timerManager, () => BeBored(), 3.0f);
                    //}
                    cardFadingStage = 0;
                }
            }
        }

        //toggle whether the videos will play from debug menu
        public void ToggleVids()
        {
            if (videos == true)
            {
                if (cardFadingStage != 0)
                {
                    cardFadingStage = 4; //if already playing, jump to stage 4
                }
                videos = false;
            }
            else //prepare to fade in the video
            {
                timerManager.CancelAllTimers();
                Timer aTimer = new Timer(timerManager, () => PrepareCardFade(), 1.0f);
                videos = true;
            }
        }

        //toggle whether the card shows the diagrams or photos
        public void ToggleDiagrams()
        {
            if (diagrams == true)
            {
                diagrams = false;
            }
            else
            {
                diagrams = true;
            }
            if (cardFadingStage == 0)
            {
                cardFadingStage = 4;
            }
        }

        //set the fade stage to start fading next update
        public void PrepareCardFade()
        {
            if (videos == true)
            {
                cardFadingStage = 1;
                cardAnimStage = 0;
            }
        }

        //randomly choose between 2 bored animations to play
        public void BeBored()
        {
            boredTimer = false;

            Random random = new Random();
            int randomNumber = random.Next(0, 2);
            if (randomNumber == 0)
            {
                borisAnimController.CrossFade(currentModel.AnimationClips["random1"], TimeSpan.FromSeconds(0.05f));
                cardAnimController.CrossFade(cardModel.AnimationClips["random1"], TimeSpan.FromSeconds(0.05f));
            }
            else
            {
                borisAnimController.CrossFade(currentModel.AnimationClips["random2"], TimeSpan.FromSeconds(0.05f));
                cardAnimController.CrossFade(cardModel.AnimationClips["random2"], TimeSpan.FromSeconds(0.05f));
            }
        }

        //set up throwing the card away
        public void HideCard()
        {
            Timer aTimer = new Timer(timerManager, () => HideCard2(), 2.0f);
        }

        //actually throw the card away
        public void HideCard2()
        {
            hascard = false;
            borisAnimController.CrossFade(currentModel.AnimationClips["hidecard"], TimeSpan.FromSeconds(0.05f));
            cardAnimController.CrossFade(cardModel.AnimationClips["hidecard"], TimeSpan.FromSeconds(0.05f));
            this.ShowCard(); //reset to the beginning and show the new card
        }

        //change the color of Boris' uniform.
        public void ChangeColor(bool defaultColor)
        {
            if (defaultColor)
                currentBorisTex = blueTex;
            else
                currentBorisTex = pinkTex;
        }

        //switch out the models between one that has facial animations and one that doesn't. 
        public void ChangeFace(bool faceYN)
        {
            if (faceYN)
                currentModel = borisModel;
            else
                currentModel = borisFacelessModel;
        }

        //turn on videos from settings being changed, start fading
        public void ChangeVideos(bool videosYN)
        {
            if (videos == true && videosYN == false)
            {
                if (cardFadingStage != 0)
                {
                    cardFadingStage = 4;
                }
                videos = false;

            }
            else if (videos == false && videosYN == true)
            {
                timerManager.CancelAllTimers();
                Timer aTimer = new Timer(timerManager, () => PrepareCardFade(), 1.0f);
                videos = true;
            }
        }

        //fade for change of picture
        public void ChangeDiagrams(bool diagramsYN)
        {
            diagrams = diagramsYN;

            if (cardFadingStage == 0)
            {
                cardFadingStage = 4;
            }
        }

/////////////////////////////////////////////////////////////////////////////////////////////////
// DRAW CODE                                                                                   //
/////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public void Draw(GameTime gameTime, Texture2D cardTex2jnhhfg)
        {
            foreach (ModelMesh mesh in currentModel.Model.Meshes)
            {
                foreach (SkinnedEffect effect in mesh.Effects)
                {
                    if (mesh.Name == "BODY")
                    {
                        effect.Texture = currentBorisTex;
                    }
                    else if (mesh.Name == "HEAD1")
                    {
                        effect.Texture = headTex;
                    }
                    effect.SetBoneTransforms(borisAnimController.SkinnedBoneTransforms);
                    effect.AmbientLightColor = new Vector3(0.4f, 0.4f, 0.4f); // Add some overall ambient light.
                    effect.DirectionalLight0.Direction = Vector3.Normalize(new Vector3(0.5f, 0, -1));  // coming along the x-axis
                    effect.DirectionalLight0.DiffuseColor = new Vector3(1, 1, 1);
                    effect.View = view;
                    effect.Projection = projection;
                }
                mesh.Draw();
            }

            foreach (ModelMesh mesh in cardModel.Model.Meshes)
            {
                foreach (SkinnedEffect effect in mesh.Effects)
                {
                    effect.SetBoneTransforms(cardAnimController.SkinnedBoneTransforms);
                    effect.AmbientLightColor = new Vector3(0.4f, 0.4f, 0.4f); // Add some overall ambient light.
                    effect.DirectionalLight0.Direction = Vector3.Normalize(new Vector3(0.5f, 0, -1));  // coming along the x-axis
                    effect.DirectionalLight0.DiffuseColor = new Vector3(1, 1, 1);

                    effect.World = Matrix.CreateTranslation(new Vector3(-2.7f, 1.3f, 4.0f));

                    if (mesh.Name == "pCube1") //then it's the mesh of just the face of the card
                    {
                        effect.Alpha = cardFaceAlpha;
                        if (cardFadingStage == 2 || cardFadingStage == 3 || cardFadingStage == 4)
                        {
                            effect.Texture = _texture;
                        }
                        else
                        {
                            effect.Texture = cardFaceTex;
                        }
                    }
                    effect.View = view;
                    effect.Projection = projection;
                }
                mesh.Draw();
            }
        }
    }
}
