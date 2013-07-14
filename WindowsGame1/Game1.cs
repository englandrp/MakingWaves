#region File Description
//-----------------------------------------------------------------------------
// Game.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

//#define SKIPMENU
#define EXPOMODE

#region Using Statements
using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.GamerServices;
using System.Collections.Generic;
#endregion



namespace WindowsGame1
{
    /// <summary>
    /// Sample showing how to manage different game states, with transitions
    /// between menu screens, a loading screen, the game itself, and a pause
    /// menu. This main game class is extremely simple: all the interesting
    /// stuff happens in the screenManager component.
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        #region Fields

        public static GraphicsDeviceManager graphics;
        public ScreenManager screenManager;
        public FileSaver fileSaver;
        public List<Player> players = new List<Player>();
        public Dictionary<string, Texture2D> images = new Dictionary<string, Texture2D>();

        public  KinectManager kinectManager;

        public List<SignPack> signPacks = new List<SignPack>();
        public int currentSignPack;
       // public static int[] signOrder = { 4, 0, 1, 2, 3 };

        //private IntPtr drawSurface;

        // By preloading any assets used by UI rendering, we avoid framerate glitches
        // when they suddenly need to be loaded in the middle of a menu transition.
        static readonly string[] preloadAssets =
        {
            "gradient",
        };

        
        #endregion

        #region Initialization

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            this.Components.Add(new GamerServicesComponent(this));
            this.IsFixedTimeStep = true;
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            graphics.PreferMultiSampling = true;
            graphics.PreparingDeviceSettings += this.GraphicsDevicePreparingDeviceSettings;
            graphics.SynchronizeWithVerticalRetrace = true;
            //graphics.IsFullScreen = true;
            //Create the screen manager component.
            fileSaver = new FileSaver(this);
            screenManager = new ScreenManager(this);
            Components.Add(screenManager);

            #if SKIPMENU
                LoadingScreen.Load(screenManager, true, PlayerIndex.One, new GameplayScreen(this));
            #endif
        }

        protected override void Initialize()
        {
            base.Initialize();
        }


        protected override void LoadContent()
        {
            // Activate the first screens.
            screenManager.AddScreen(new BackgroundScreen(screenManager), null);
            screenManager.AddScreen(new MainMenuScreen(screenManager), null);
            fileSaver.LoadData();
            kinectManager = new KinectManager(this);
        }

        #endregion

        #region Draw


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.Black);

            // The real drawing happens inside the screen manager component.
            base.Draw(gameTime);
        }

        /// <summary>
        /// This method ensures that we can render to the back buffer without
        /// losing the data we already had in our previous back buffer.  This
        /// is necessary for the SkeletonStreamRenderer.
        /// </summary>
        /// <param name="sender">The sending object.</param>
        /// <param name="e">The event args.</param>
        private void GraphicsDevicePreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
        {
            // This is necessary because we are rendering to back buffer/render targets and we need to preserve the data
            e.GraphicsDeviceInformation.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;
        }

        #endregion
    }
}
