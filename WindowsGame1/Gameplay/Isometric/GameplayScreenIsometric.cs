#region File Description
//-----------------------------------------------------------------------------
// GameplayScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
//using System.Collections.Generic;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
//using Microsoft.Xna.Framework.Media;
//using System.Diagnostics;

#endregion

namespace WindowsGame1
{
    /// <summary>
    /// This screen implements the actual game logic. It is just a
    /// placeholder to get the idea across: you'll probably want to
    /// put some more interesting gameplay in here!
    /// </summary>
    class GameplayScreenIsometric : GameScreen
    {
        #region Fields

        ContentManager content;
        SpriteFont gameFont;
       
        float pauseAlpha;


        IsometricWorld isometricWorld;
        Player theCurrentPlayer;
        bool wasActive;

        #endregion

        #region Initialization

        /// <summary>
        /// Constructor.
        /// </summary>
        public GameplayScreenIsometric(Player theCurrentPlayer)
        {
            this.theCurrentPlayer = theCurrentPlayer;
  
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }

        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(screenManager.Game.Services, "Content");

            gameFont = content.Load<SpriteFont>("gamefont");

            // A real game would probably have more content than this sample, so
            // it would take longer to load. We simulate that by delaying for a
            // while, giving you a chance to admire the beautiful loading screen.
           // Thread.Sleep(1000);

            // once the load has finished, we use ResetElapsedTime to tell the game's
            // timing mechanism that we have just finished a very long frame, and that
            // it should not try to catch up.
            screenManager.Game.ResetElapsedTime();

            isometricWorld = new IsometricWorld(this.screenManager.game1, theCurrentPlayer);
            isometricWorld.LoadContent();
        }

        /// <summary>
        /// Unload graphics content used by the game.
        /// </summary>
        public override void UnloadContent()
        {
            content.Unload();
        }

        #endregion

        #region Update and Draw


        /// <summary>
        /// Updates the state of the game. This method checks the GameScreen.IsActive
        /// property, so the game will stop updating when the pause menu is active,
        /// or if you tab away to a different application.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            // Gradually fade in or out depending on whether we are covered by the pause screen.
            if (coveredByOtherScreen)
                pauseAlpha = Math.Min(pauseAlpha + 1f / 32, 1);
            else
                pauseAlpha = Math.Max(pauseAlpha - 1f / 32, 0);

            if (IsActive)
            {
                if (wasActive == false)
                {
                //    isometricWorld.loadSettings();
                    //       Trace.WriteLine("CALLED AGAIN");
                }
                isometricWorld.Update(gameTime);
                wasActive = true;
            }
            else
            {
                wasActive = false;
            }

            //if (prevState != ScreenState.Active) 
            //{
            //    pauseAlpha = 10;
            //}
            base.Update(gameTime, otherScreenHasFocus, false);
        }

        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(InputState input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            // Look up inputs for the active player profile.
            int playerIndex = (int)ControllingPlayer.Value;

            KeyboardState keyboardState = input.CurrentKeyboardStates[playerIndex];
            GamePadState gamePadState = input.CurrentGamePadStates[playerIndex];

            // The game pauses either if the user presses the pause button, or if
            // they unplug the active gamepad. This requires us to keep track of
            // whether a gamepad was ever plugged in, because we don't want to pause
            // on PC if they are playing with a keyboard and have no gamepad at all!
            bool gamePadDisconnected = !gamePadState.IsConnected &&
                                       input.GamePadWasConnected[playerIndex];

            if (input.IsPauseGame(ControllingPlayer) || gamePadDisconnected)
            {
                screenManager.AddScreen(new PauseMenuScreen(screenManager), ControllingPlayer);
            }
            else if (input.IsAdminMenu(ControllingPlayer))
            {
                screenManager.AddScreen(new PlayerSelectScreen(screenManager), ControllingPlayer);
            }else if (input.OpenPlayerSettings(ControllingPlayer))
            {
                screenManager.AddScreen(new CreatePlayerMenuScreen(theCurrentPlayer,screenManager), ControllingPlayer);
            }

            isometricWorld.HandleInput(input);
        }

        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            // This game has a blue background. Why? Because!
            screenManager.GraphicsDevice.Clear(ClearOptions.Target,
                                               Color.CornflowerBlue, 0, 0);
            screenManager.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            screenManager.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            isometricWorld.Draw(gameTime);

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0 || pauseAlpha > 0)
            {
                float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, pauseAlpha / 2);
                screenManager.FadeBackBufferToBlack(alpha);
            }

        }
        #endregion
    }
}
