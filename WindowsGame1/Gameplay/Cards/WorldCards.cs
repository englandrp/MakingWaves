using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;//shouldnt be needed

namespace WindowsGame1
{
    //World class for card game, see animated sign game for more comments as the 
    //content is similar, main difference is state stuff has been shifted into the Boris class
    class WorldCards
    {
        Game1 game1;
        ContentManager content;
        SpriteBatch spriteBatch;
        private Matrix view;
        private Matrix projection;

        private Model roomModel;
        BorisCards boris;

        bool faceYN;
        bool audioYN;
        bool backgroundYN;
        bool scoreYN;
        bool borisDefaultColor;
        bool videosYN;
        bool diagramsYN;

        Texture2D correctTex;
        Texture2D incorrectTex;
        Texture2D currentCardTex;
        SoundEffect correctSound;
        SoundEffect wrongSound;
        bool correcttimerstarted;
        bool wrongtimerstarted;

        Player currentPlayer;
        int attemptcount;
        int prevFinished = -1;
        int ticktimer = 0;

        public WorldCards(Game1 game1, Player theCurrentPlayer, SignPack signPack)
        {
            this.game1 = game1;
            spriteBatch = new SpriteBatch(game1.GraphicsDevice);
            content = game1.Content;
            currentPlayer = theCurrentPlayer;
            loadSettings();
            //view = Matrix.CreateLookAt(new Vector3(0, 12.5f, 26), new Vector3(0, 10.5f, 0), Vector3.Up);
            view = Matrix.CreateLookAt(new Vector3(0, 12.5f, 80), new Vector3(0, 10.5f, 0), Vector3.Up);
            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, 1.77f, 1, 1000);
            boris = new BorisCards(game1, signPack, this,currentPlayer);
        }

        /// <summary>
        /// Load your graphics content.
        /// </summary>
        public void LoadContent()
        {
            roomModel = content.Load<Model>("Models/FlashCard_Background_2");
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
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public void HandleInput(InputState input)
        {
            if (input.isWinning(null))
            {
                boris.Win();
            }

            if (input.isLosing(null))
            {
                boris.Lose();
            }

            if (input.IsMenuLeft(null))
            {
                boris.ToggleVids();
            }

            if (input.IsMenuRight(null))
            {
                boris.ToggleDiagrams();
            }
        }


     public void Update(GameTime gameTime)
     {
         boris.Update(gameTime);

         if (ticktimer > 100)
         {
             ticktimer = 0;
                    correcttimerstarted = false;
                    wrongtimerstarted = false;
         }
         if (correcttimerstarted == true || wrongtimerstarted == true)
         {
             ticktimer++;
         }
     }

        
     public  void Win()
     {
             if (audioYN == true)
             {
                 correctSound.Play();
             }
             if (scoreYN == true)
             {
                 correcttimerstarted = true;
             }
     }

     public void Lose()
     {
             if (audioYN == true)
             {
                 wrongSound.Play();
             }

             if (scoreYN == true)
             {
                 wrongtimerstarted = true;
             }
     }

        public void loadSettings()
        {
            faceYN = currentPlayer.playerSave.face;
            audioYN = currentPlayer.playerSave.audio;
            backgroundYN = currentPlayer.playerSave.background;
            scoreYN = currentPlayer.playerSave.score;
            borisDefaultColor = currentPlayer.playerSave.borisColorDefault;
            videosYN = currentPlayer.playerSave.cardvideos;
            diagramsYN = currentPlayer.playerSave.carddiagrams;
            if (boris != null)
            {
                boris.ChangeColor(borisDefaultColor);
                boris.ChangeFace(faceYN);
                boris.ChangeVideos(videosYN);
                boris.ChangeDiagrams(diagramsYN);
            }
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////////////


        //        /// <summary>
        ///// Draws the gameplay screen.
        ///// </summary>
        public void Draw(GameTime gameTime)
        {
            Matrix[] transforms = new Matrix[roomModel.Bones.Count];
            roomModel.CopyAbsoluteBoneTransformsTo(transforms);

            if (backgroundYN == true)
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
            boris.Draw(gameTime,currentCardTex);
            spriteBatch.Begin(SpriteSortMode.Texture, BlendState.NonPremultiplied);
            if (correcttimerstarted == true)
            {

                spriteBatch.Draw(correctTex, new Rectangle(50, 50, 256, 256), Color.White);
            }

            if (wrongtimerstarted == true)
            {

                spriteBatch.Draw(incorrectTex, new Rectangle(50, 50, 256, 256), Color.White);
            }
            spriteBatch.End();
        }
    }
}
