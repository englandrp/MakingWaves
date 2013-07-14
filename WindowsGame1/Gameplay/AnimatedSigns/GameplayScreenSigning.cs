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
    class GameplayScreenSigning : GameScreen
    {
        #region Fields
        ContentManager content;
        SpriteFont gameFont;
        float pauseAlpha;

        World world;
        Player theCurrentPlayer;
        bool wasActive;

        #endregion

        #region Initialization

        /// <summary>
        /// Constructor.
        /// </summary>
        public GameplayScreenSigning(Player theCurrentPlayer)
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
            screenManager.Game.ResetElapsedTime();
            world = new World(screenManager.Game, theCurrentPlayer);
            world.LoadContent();
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
                    world.loadSettings(); //reload the current settings if coming from another screen, eg diag box
                }
                int hasWon = screenManager.game1.kinectManager.CheckForSign(world.currentSign, theCurrentPlayer.playerSave.precision, false); //precision should be level, not player based

                if (hasWon != -1) //-1 would be no sign found
                {
                    if (hasWon == 1) //if sign matches the one sent to the black box..
                        world.Win();
                    else if (hasWon == 0)//otherwise it doesn't match
                        world.Lose();
                }
                world.Update(gameTime);

                wasActive = true;
            }
            else
            {
                wasActive = false;
            }
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
                screenManager.AddScreen(new PauseMenuScreen( screenManager), ControllingPlayer);
            }
            else if (input.IsAdminMenu(ControllingPlayer))
            {
                screenManager.AddScreen(new PlayerSelectScreen(screenManager), ControllingPlayer);
            }
            else if (input.OpenPlayerSettings(ControllingPlayer))
            {
                screenManager.AddScreen(new CreatePlayerMenuScreen(theCurrentPlayer, screenManager), ControllingPlayer);
            }
            world.HandleInput(input);
        }

        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            // This game has a blue background. Why? Because!
            screenManager.GraphicsDevice.Clear(ClearOptions.Target,
                                               Color.White, 0, 0);

            // Our player and enemy are both actually just text strings.
            SpriteBatch spriteBatch = screenManager.SpriteBatch;

            screenManager.GraphicsDevice.BlendState = BlendState.Opaque;
            screenManager.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            screenManager.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
          
            world.Draw(gameTime);

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
