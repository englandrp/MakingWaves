//using System;
//using System.Collections.Generic;
using System.Linq;
//using System.Text;
using Microsoft.Xna.Framework;
//using XNAnimation;
//using XNAnimation.Controllers;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
//using System.Diagnostics;
namespace WindowsGame1
{
    class World
    {
        enum State //list of possible states of gameplay
        {
            Starting,
            Ending,
            PlayingSign,
            ReplayingSign,
            WaitingForAnimation,
            WaitingForInput,
            WaitingBeforeInput,
            WaitingBeforePlaying,
            WaitingBeforeReplaying
        }

        ContentManager content;
        SpriteBatch spriteBatch;
        Model roomModel; //spacestation
        Player currentPlayer;
        Boris boris; 
    
        Texture2D correctTex; //tick texture if correct
        Texture2D incorrectTex; //cross texture if wrong
        SoundEffect correctSound; 
        SoundEffect wrongSound;

        private Matrix view;
        private Matrix projection;

        //gameplay options loaded from the playersave
        bool faceYN;
        bool audioYN;
        bool backgroundYN;
        bool scoreYN;
        bool borisDefaultColor;

        State currentState = State.PlayingSign;

        int[] signArray = {  2, 4, 6, 8, 10 }; //order of the animations for the 5 signs
        int[] correctArray = {  3, 5, 7, 9, 10 }; // order of the matching 'correct' animations to go with the 5 signs, note wave is the same for both (10)

        TimerManager timerManager; //monitors the timers set for pauses between states. 

        bool correcttimerstarted = false; //basic timer ticker for the tick sign if correct
        bool wrongtimerstarted; //basic timer ticker for the cross sign if incorrect
        public int currentSign = 4; //which sign is the current one, starts with Wave
        int prevFinished = -1; //record of previous currentsign for testing against current one
        int attemptcount; //how many times has the player got it wrong

        public World(Game game, Player theCurrentPlayer)
        {
            timerManager = new TimerManager();
            spriteBatch = new SpriteBatch(game.GraphicsDevice);
            content = game.Content;
            currentPlayer = theCurrentPlayer;

            loadSettings();
            
            view = Matrix.CreateLookAt(new Vector3(0, 12.5f, 36), new Vector3(0, 12.5f, 0), Vector3.Up);
            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, 1.77f, 1, 1000);
            boris = new Boris(game);
        }

        //updates the level options from the current playersave
        public void loadSettings()
        {
            faceYN = currentPlayer.playerSave.face;
            audioYN = currentPlayer.playerSave.audio;
            backgroundYN = currentPlayer.playerSave.background;
            scoreYN = currentPlayer.playerSave.score;
            borisDefaultColor = currentPlayer.playerSave.borisColorDefault;
            if (boris != null)
            {
                boris.ChangeColor(borisDefaultColor);
                boris.ChangeFace(faceYN);
            }
        }

        /// <summary>
        /// Load your graphics content.
        /// </summary>
        public void LoadContent()
        {
            roomModel = content.Load<Model>("Models/roomModel");

            correctSound = content.Load<SoundEffect>("done");
            wrongSound = content.Load<SoundEffect>("fail");
            correctTex = content.Load<Texture2D>("Graphics/correct");
            incorrectTex = content.Load<Texture2D>("Graphics/incorrect");
            boris.LoadContent();
            if (!borisDefaultColor)
            {
                boris.ChangeColor(borisDefaultColor);
            }
            if (!faceYN)
            {
                boris.ChangeColor(faceYN);
            }
        }

        /// <summary>
        /// only used for keyboard/gamepad override
        /// </summary>
        public void HandleInput(InputState input)
        {
            if (input.isWinning(null))
            {
                Win();
            }

            if (input.isLosing(null))
            {
                Lose();
            }
        }

        public void Win()
        {
            if (currentState == State.WaitingForInput || currentState == State.ReplayingSign) //test that it's ok to accept kinect input
            {
                boris.playAnimation(correctArray[currentSign]); //play the current sign, getting the animation number from the correctArray
                timerManager.CancelAllTimers();
                currentSign++; //move on to the next sign
               
                if (currentSign > signArray.Count() - 1) //reset to the first sign if at the end 
                {
                    currentSign = 0;
                }

                currentState = State.PlayingSign;

                if (audioYN == true)
                {
                    correctSound.Play();
                }

                if (scoreYN == true) //start the tick timer
                {
                    correcttimerstarted = true;
                }
            }
        }

        //skips to the next sign, usually if there have been too many fails, difference is wrong sound is still played when moving to next one
        public void Skip()
        {
            timerManager.CancelAllTimers();
            currentSign++;
            if (currentSign > signArray.Count() - 1)
            {
                currentSign = 0;
                wrongSound.Play();
            }
            currentState = State.PlayingSign;
        }

        public void Lose()
        {
            if (currentState == State.WaitingForInput || currentState == State.ReplayingSign)
            {
                boris.Lose(); //play lose animation
                timerManager.CancelAllTimers();

                currentState = State.WaitingBeforeReplaying;

                if (audioYN == true)
                {
                    wrongSound.Play();
                }

                if (scoreYN == true) //start timer for tick icon
                {
                    wrongtimerstarted = true;
                }

                attemptcount++;

                if (attemptcount == 5) //if the player has failed too many times, skip to the next sign
                {
                    this.Skip();
                    attemptcount = 0;
                }
            }
            
        }

        public void Update(GameTime gameTime)
        {
            if (boris.AnimatingOnRequest == false) //only check state when Boris has finished his current animation (exluding standing/bored animation)
            {
                if (currentState == State.Starting) //if game has just started, wave after 1 second
                {
                    Timer aTimer = new Timer(timerManager, () => this.Wave(), 1.0f);
                    currentState = State.WaitingForAnimation;
                }
                else if (currentState == State.PlayingSign) //in 2 seconds, play the current signs animation
                {
                    Timer aTimer = new Timer(timerManager, () => this.playAnimation(), 2.0f);
                    currentState = State.WaitingForAnimation;
                    attemptcount = 0;
                }
                else if (currentState == State.ReplayingSign) //after waiting a while replay the sign if not correct
                {
                    Timer aTimer = new Timer(timerManager, () => this.playAnimation(), 2.0f);
                    currentState = State.WaitingForInput;
                }
                else if (currentState == State.WaitingBeforePlaying) //needs this one just to give animation controller a chance to play
                {
                    currentState = State.PlayingSign;
                }
                else if (currentState == State.WaitingBeforeReplaying) //needs this one just to give animation controller a chance to play
                {
                    currentState = State.ReplayingSign;
                }

                correcttimerstarted = false; //reset
                wrongtimerstarted = false; //reset
            }
            boris.Update(gameTime);
            timerManager.Update(gameTime);
        }

        //tell Boris to wave
        public void Wave()
        {
            boris.Wave();
            currentState = State.WaitingBeforePlaying;
        }

        //play the current sign's animation
        public void playAnimation()
        {
            boris.playAnimation(signArray[currentSign]);

            if (prevFinished != currentSign) //check whether it's time to replay yet
            {
                currentState = State.WaitingBeforeReplaying;
            }
            else
            {
                currentState = State.ReplayingSign;
            }
            prevFinished = currentSign; //store record of current sign
        }

        public void Draw(GameTime gameTime)
        {
            Matrix[] transforms = new Matrix[roomModel.Bones.Count];
            roomModel.CopyAbsoluteBoneTransformsTo(transforms);

            if (backgroundYN == true) //only draw the background if UI is turned on
            {
                // Draw the model. A model can have multiple meshes, so loop.
                foreach (ModelMesh mesh in roomModel.Meshes)
                {
                    // This is where the mesh orientation is set, as well 
                    // as our camera and projection.
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();
                        effect.World = transforms[mesh.ParentBone.Index]
                            * Matrix.CreateTranslation(new Vector3(0, -0.2f, 10))
                            ;
                        effect.View = Matrix.CreateLookAt(new Vector3(100, 100, 100),
                            Vector3.Zero, Vector3.Up);
                        effect.Projection = Matrix.CreatePerspectiveFieldOfView(
                            MathHelper.ToRadians(45.0f), 1,
                            1.0f, 10000.0f);
                        effect.DirectionalLight0.Direction = Vector3.Normalize(new Vector3(0.5f, 0, -1));  // coming along the x-axis
                        //  effect.DirectionalLight0.SpecularColor = new Vector3(0.1f, 0.1f, 0.1f); // with green highlights
                        effect.AmbientLightColor = new Vector3(0.2f, 0.2f, 0.2f); // Add some overall ambient light.
                        //  effect.EmissiveColor = new Vector3(0.1f, 0.1f, 0.1f); // Sets some strange emmissive lighting.  This just looks weird.
                        effect.View = view;
                        effect.Projection = projection;
                    }
                    // Draw the mesh, using the effects set above.
                    mesh.Draw();
                }
            }
            boris.Draw(gameTime);
            spriteBatch.Begin(SpriteSortMode.Texture, BlendState.NonPremultiplied);

            //display a tick 
            if (correcttimerstarted == true)
            {
                spriteBatch.Draw(correctTex, new Rectangle(50, 50, 256, 256), Color.White);
            }
            //display a cross
            if (wrongtimerstarted == true)
            {
                spriteBatch.Draw(incorrectTex, new Rectangle(50, 50, 256, 256), Color.White);
            }
            spriteBatch.End();
        }
    }
}
