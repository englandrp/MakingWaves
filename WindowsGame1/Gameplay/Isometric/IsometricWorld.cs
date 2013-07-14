using System;
using System.Collections.Generic;
//using System.Linq;
//using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using XNAnimation;
using XNAnimation.Controllers;
using System.Diagnostics;
using Microsoft.Kinect;
using Microsoft.Xna.Framework.Audio; 

namespace WindowsGame1
{
    class IsometricWorld
    {
        Texture2D correctTex;
        Texture2D incorrectTex;

        Game1 game1;
        ContentManager content;
        SpriteBatch spriteBatch;

        private Matrix view;
        private Matrix projection;
        
        TimerManager timerManager;
        public Camera camera;
        public KitchenLevel kitchenLevel;
        Player currentPlayer;
        BorisIso borisIso;

        BorisModel borisModel;
      
        float alarmAlpha;
        public static Rectangle mouseRect = Rectangle.Empty;
        Texture2D mousePointer;
        TimeSpan elapsedTime = TimeSpan.Zero;
        private SpriteFont font;

        bool correcttimerstarted = false;
        bool wrongtimerstarted;
        
        bool alarmOn = false;
        bool cancelAlarm;
        int currentHotSpot;



        SoundEffect soundTrack;/////////////////////*********
        //Set the sound effects to use

        SoundEffectInstance soundEngineInstance;


        public Stars stars;

        public IsometricWorld(Game1 game1, Player theCurrentPlayer)
        {
            Point gridOffset = new Point(1, 0);
            kitchenLevel = new KitchenLevel(game1, gridOffset );
            camera = new Camera(gridOffset);

            borisIso = new BorisIso(game1, this, kitchenLevel);
            timerManager = new TimerManager();
            
            this.game1 = game1;
            spriteBatch = new SpriteBatch(game1.GraphicsDevice);
            content = game1.Content;
            stars = new Stars();
        }

        /// <summary>
        /// Load your graphics content.
        /// </summary>
        public void LoadContent()
        {
            mousePointer = content.Load<Texture2D>("Graphics/gradient");
            this.font = content.Load<SpriteFont>("Arial");
            //correctSound = content.Load<SoundEffect>("done");
            //wrongSound = content.Load<SoundEffect>("home");
            correctTex = content.Load<Texture2D>("Graphics/correct");
            incorrectTex = content.Load<Texture2D>("Graphics/incorrect");
            borisIso.LoadContent();
            kitchenLevel.LoadContent(content);
            stars.LoadContent(content);

            soundTrack = content.Load<SoundEffect>("soundTrack");

            soundEngineInstance = soundTrack.CreateInstance();
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //UPDATE        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
   
        public void Update(GameTime gameTime)
        {
            borisIso.Update(gameTime);
            timerManager.Update(gameTime);
            camera.Update(borisIso.borisMover.borisPos);
            kitchenLevel.Update(gameTime);



            int thesign = game1.kinectManager.GetCurrentSign(true); //pass true to get the actual sign ID




          //Sign aciton for Boris...............bad
            if (thesign == 0)
            {
                //borisIso.Sleep(); //should be "Home"
            }
            else if (thesign == 1)
            {
                borisIso.Toilet();//Toilet
            }
            else if (thesign == 2)
            {
                borisIso.Eat();//Eat
            }
            else if (thesign == 3)
            {
                borisIso.Drink();//Drink
            }
            else if (thesign == 4)
            {
                //borisIso.Hello(); //should be "Hello"
            }
            else if (thesign == 5)
            {
                borisIso.Sleep(); //should be "Sleep"
            }
            else if (thesign == 6)
            {
                borisIso.Clean(); //should be "Wash"
            }

            //Sign aciton for Boris...............bad
            if (thesign == -1)
            {
                //borisIso.Lose();



            }
            else
            {
                //borisIso.DoSignAction(thesign);///////////uncomment.
            }

            stars.Update(borisIso.barGoldstar);
            //PLay Sound in here
            //playLoopSoundTrack();


        }

        public void SoundAlarm()
        {
            alarmOn = true;
            Timer aTimer = new Timer(timerManager, () => this.CancelAlarm(), 5.0f);
        }

        public void CancelAlarm()
        {
            cancelAlarm = true;
        }

        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public void HandleInput(InputState input)
        {
          //  quadManager.HandleInput(input);
            PlayerIndex index;
            if (input.IsNewKeyPress(Microsoft.Xna.Framework.Input.Keys.D1, null, out index))
            {
                borisIso.Eat();
            }
            else
            if (input.IsNewKeyPress(Microsoft.Xna.Framework.Input.Keys.D2, null, out index))
            {
                borisIso.Drink();
            }
            else
            if (input.IsNewKeyPress(Microsoft.Xna.Framework.Input.Keys.D3, null, out index))
            {
                borisIso.Toilet();
            }
            else
            if (input.IsNewKeyPress(Microsoft.Xna.Framework.Input.Keys.D4, null, out index))
            {
                borisIso.Clean();
            }
            else if (input.IsNewKeyPress(Microsoft.Xna.Framework.Input.Keys.D5, null, out index))
            {
                borisIso.Sleep();
            }
        }



        public void playLoopSoundTrack()
        {
            if (soundEngineInstance.State == SoundState.Stopped)
            {

                soundEngineInstance.Volume = 0.30f;
               // soundEngineInstance.IsLooped = true;
               // soundEngineInstance.Play();

            }
        }

        public void stopLoopSoundTrack()
        {
            if (soundEngineInstance.State == SoundState.Playing)
            {
                soundEngineInstance.Stop();
            }
        } 

        public void ShowStar()
        {
            stars.Animate();
        }

        public void ClearStars()
        {
            stars.ClearStars();
            
        }

        public void Draw(GameTime gameTime)
        {
            projection = camera.projection;
            view = camera.view;

            
            kitchenLevel.Draw(view, projection);
            borisIso.Draw(gameTime, view, projection);

            spriteBatch.Begin();

            int inc = 1;

            //this.spriteBatch.DrawString(this.font,
                                       // "Need to wash: " + borisIso.needWash.ToString(),
                                       // new Vector2(100, inc * 100),
                                       // Color.Red);
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Texture, BlendState.NonPremultiplied);
            if (correcttimerstarted == true)
            {
                spriteBatch.Draw(correctTex, new Rectangle(50, 50, 256, 256), Color.White);
            }
            if (wrongtimerstarted == true)
            {
                spriteBatch.Draw(incorrectTex, new Rectangle(50, 50, 256, 256), Color.White);
            }

            stars.Draw(spriteBatch, borisIso.barGoldstar);
            spriteBatch.End();


            //Alarm /****************************************************************
            if (alarmOn == true)
            {
                float thealph = (float)Math.Sin(alarmAlpha);
                thealph = (thealph + 1.0f) / 4.0f;
                spriteBatch.Begin();

                Trace.WriteLine(thealph);
                spriteBatch.Draw(mousePointer, new Rectangle(0, 0, 1280, 720), Color.Red * thealph);
                spriteBatch.End();

                alarmAlpha += 0.1f;
                if (cancelAlarm == true)
                {
                    if (thealph < 0.01f)
                    {
                        alarmAlpha = 0;
                        alarmOn = false;
                        cancelAlarm = false;
                    }
                }
            }
            else
            {
                alarmAlpha = 0;
            }


            //Stats Bar***********************
            spriteBatch.Begin();
            foreach (StatBar statBar in borisIso.statBars)
            {
                int incf = 1;
                this.spriteBatch.DrawString(
                 this.font,
                 //statBar.name + ": " + statBar.currentValue + " / " + statBar.size + " " + statBar.Percentage().ToString("N2") + ".",

                  statBar.name + ": " + statBar.Percentage().ToString("N2") + "%.",

                 new Vector2(0 + inc * 170, 650),
                // new Vector2(20, 200 + inc * 40),//Old stuff
                 Color.LightBlue);
                inc++;
            }
            spriteBatch.End();
        }
    }
}
