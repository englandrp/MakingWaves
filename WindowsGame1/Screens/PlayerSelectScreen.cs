#region File Description
//-----------------------------------------------------------------------------
// PauseMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
//using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Content;
//using System.IO;
//using System.Xml.Serialization;
//using System.Diagnostics;
#endregion

namespace WindowsGame1
{
    /// <summary>
    /// 
    /// 
    /// </summary>
    class PlayerSelectScreen : MenuScreen
    {
        #region Initialization

        List<MenuEntry> menuEntries = new List<MenuEntry>();
        string menuTitle;
        ContentManager content;
        Texture2D mousePointer;
        Texture2D selected;
        Texture2D frontTex;
        Vector2 selectedEntry2D = Vector2.Zero;
        Vector2 gridSize = new Vector2(3, 2);
        int controlmethod = 0;

        public Vector2 mouseVec;
        float previousZPos;
        float zSpeed;
        public bool pushing;
        private KeyboardState previousKeyboard;

        List<Vector3> pushlocations;

        protected IList<MenuEntry> MenuEntries
        {
            get { return menuEntries; }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public PlayerSelectScreen(ScreenManager screenManager)
            : base("Select Player", screenManager)
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.Zero;
        }

        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(screenManager.Game.Services, "Content");

            selected = content.Load<Texture2D>("Graphics/selected");
            frontTex = content.Load<Texture2D>("Graphics/front");
            int i = 1;
            foreach (Player thePlayer in screenManager.game1.players)
            {
                MenuEntry aPlayerEntry = new MenuEntry(thePlayer.playerSave.playerName);
                aPlayerEntry.Selected += OnPlayer;
                aPlayerEntry.Position = new Vector2(50, i * 50);
                i++;
                MenuEntries.Add(aPlayerEntry);
            }
            mousePointer = content.Load<Texture2D>("Graphics/Bone");
            pushlocations = new List<Vector3>();

            for (int j = 0; j < 10; j++)
            {
                pushlocations.Add(Vector3.Zero);
            }
        }

        public override void UnloadContent()
        {

        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                    bool coveredByOtherScreen)
        {
       //     kinectMouse.Update(gameTime);
        }

        public override void HandleInput(InputState input)
        {
            KeyboardState newState = Keyboard.GetState();
            Vector3 handPos = SkeletonStreamRenderer.rightHandPos; 
            if (input.deleteEntry(ControllingPlayer))
            {
                int playerNum = (int)((3 * selectedEntry2D.Y) + selectedEntry2D.X);
                if (playerNum < screenManager.game1.players.Count)
                {
                    if (screenManager.game1.players[playerNum] != null)
                    {
                        screenManager.game1.images.Remove(screenManager.game1.players[playerNum].playerSave.playerName);
                        screenManager.game1.players.RemoveAt(playerNum);
                    }
                }
            }
            else
            {
                if (input.ChangeInput(null))
                {
                    controlmethod++;
                    if (controlmethod > 4)
                        controlmethod = 0;
                }

                if (newState.IsKeyDown(Keys.B) &&
               previousKeyboard.IsKeyUp(Keys.B))
                {
                }

                // Move to the previous menu entry?
                if (input.IsMenuUp(ControllingPlayer))
                {
                    selectedEntry2D.Y--;

                    if (selectedEntry2D.Y < 0)
                    {
                        selectedEntry2D.Y = gridSize.Y - 1;
                        if (selectedEntry2D.X == 0)
                        {
                            selectedEntry2D.X = gridSize.X - 1; ;
                        }
                        else
                        {
                            selectedEntry2D.X--; ;
                        }
                    }
                }

                // Move to the next menu entry?
                if (input.IsMenuDown(ControllingPlayer))
                {
                    selectedEntry2D.Y++;

                    if (selectedEntry2D.Y >= gridSize.Y)
                    {
                        selectedEntry2D.Y = 0;
                        if (selectedEntry2D.X < gridSize.X - 1)
                        {
                            selectedEntry2D.X++;
                        }
                        else
                        {
                            selectedEntry2D.X = 0;
                        }
                    }
                }
                if (input.IsMenuLeft(ControllingPlayer))
                {
                    selectedEntry2D.X--;

                    if (selectedEntry2D.X < 0)
                    {
                        selectedEntry2D.X = gridSize.X - 1;
                        if (selectedEntry2D.Y == 0)
                        {
                            selectedEntry2D.Y = gridSize.Y - 1; ;
                        }
                        else
                        {
                            selectedEntry2D.Y--; ;
                        }
                    }
                }
                if (input.IsMenuRight(ControllingPlayer))
                {
                    selectedEntry2D.X++;

                    if (selectedEntry2D.X >= gridSize.X)
                    {
                        selectedEntry2D.X = 0;
                        if (selectedEntry2D.Y < gridSize.Y - 1)
                        {
                            selectedEntry2D.Y++;
                        }
                        else
                        {
                            selectedEntry2D.Y = 0;
                        }
                    }
                }

                int thirdX = 1280 / 3;
                int halfY = 720 / 2;

                if (controlmethod != 0)
                {
                    if (controlmethod == 1 || controlmethod == 3)
                    {
                        mouseVec = new Vector2(handPos.X, handPos.Y);
                        mouseVec = new Vector2(1280 * ((mouseVec.X + 0.5f)), 720 * (0.5f - mouseVec.Y));
                    }
                    else if (controlmethod == 2 || controlmethod == 4)
                    {
                        mouseVec = new Vector2(handPos.X, handPos.Y);
                        mouseVec = new Vector2(1280 * ((mouseVec.X + 0.5f)), 720 * (0.5f - mouseVec.Y));

                        if (mouseVec.Y < halfY)
                        {

                            if (mouseVec.X < 550)
                            {
                                mouseVec = new Vector2(230, 190);
                            }
                            else if (mouseVec.X > 730)
                            {
                                mouseVec = new Vector2(1030, 190);
                            }
                            else
                            {
                                mouseVec = new Vector2(630, 190);
                            }
                        }
                        else
                        {
                            if (mouseVec.X < 550)
                            {
                                mouseVec = new Vector2(230, 510);
                            }
                            else if (mouseVec.X > 730)
                            {
                                mouseVec = new Vector2(1030, 510);
                            }
                            else
                            {
                                mouseVec = new Vector2(630, 510);
                            }
                        }

                    }
                    pushlocations.RemoveAt(0);
                    pushlocations.Add(handPos);

                    Vector3 minvec = Vector3.Zero;
                    Vector3 maxvec = Vector3.Zero;

                    foreach (Vector3 thevec in pushlocations)
                    {
                        if (thevec.X < minvec.X || minvec.X == 0)
                            minvec.X = thevec.X;
                        if (thevec.Y < minvec.Y || minvec.Y == 0)
                            minvec.Y = thevec.Y;
                        if (thevec.Z < minvec.Z || minvec.Z == 0)
                            minvec.Z = thevec.Z;

                        if (thevec.X > maxvec.X || maxvec.X == 0)
                            maxvec.X = thevec.X;
                        if (thevec.Y > maxvec.Y || maxvec.Y == 0)
                            maxvec.Y = thevec.Y;
                        if (thevec.Z > maxvec.Z || maxvec.Z == 0)
                            maxvec.Z = thevec.Z;
                    }

                    Vector3 totalvec = new Vector3(maxvec.X - minvec.X, maxvec.Y - minvec.Y, maxvec.Z - minvec.Z);
                    if (totalvec.Z > 0.15f && totalvec.X < 0.05f && totalvec.Y < 0.05f)
                    {
                        //Trace.WriteLine("HIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIT");
                        pushing = true;
                    }
                    else
                    {
                        pushing = false;
                    }

                    if (mouseVec.Y < halfY)
                    {
                        selectedEntry2D.Y = 0;
                        if (mouseVec.X < thirdX)
                        {
                            selectedEntry2D.X = 0;
                        }
                        else if (mouseVec.X > 2 * thirdX)
                        {
                            selectedEntry2D.X = 2;
                        }
                        else
                        {
                            selectedEntry2D.X = 1;
                        }
                    }
                    else
                    {
                        selectedEntry2D.Y = 1;
                        if (mouseVec.X < thirdX)
                        {
                            selectedEntry2D.X = 0;
                        }
                        else if (mouseVec.X > 2 * thirdX)
                        {
                            selectedEntry2D.X = 2;
                        }
                        else
                        {
                            selectedEntry2D.X = 1;
                        }
                    }
                }

                if (input.IsMenuSelect2(ControllingPlayer) || pushing == true)
                {
                    int playerNum = (int)((3 * selectedEntry2D.Y) + selectedEntry2D.X);
                    if (playerNum < screenManager.game1.players.Count)
                    {
                        if (screenManager.game1.players[playerNum] != null)
                        {
                            if (screenManager.worldType == 1)
                            {
                                LoadingScreen.Load(screenManager, true, PlayerIndex.One,
                                new GameplayScreenSigning(screenManager.game1.players[playerNum]));
                                ExitScreen();
                            }
                            else if (screenManager.worldType == 2)
                            {
                                //RE
                                SignPack theSignPack = screenManager.game1.signPacks[screenManager.game1.currentSignPack];
                                SkeletonStreamRenderer skelStream = screenManager.game1.kinectManager.skeletonStream;
                                LoadingScreen.Load(screenManager, true, PlayerIndex.One,
                                new GameplayScreenCards(screenManager.game1.players[playerNum], theSignPack));
                                //is this the intended place for this? Oh well, here goes...
                                // also skeletonStream probably shouldn't be public, but this is a quick fix to get signs working for me
                                skelStream.SetSignPack(theSignPack);
                                Console.WriteLine(" ### Set sign pack in skelStream  ### " + theSignPack.nameFile);
                                ExitScreen();
                            }
                            else if (screenManager.worldType == 3)
                            {
                                //RE
                                SignPack theSignPack = screenManager.game1.signPacks[screenManager.game1.currentSignPack];
                                SkeletonStreamRenderer skelStream = screenManager.game1.kinectManager.skeletonStream;
                                LoadingScreen.Load(screenManager, true, PlayerIndex.One,
                                new GameplayScreenIsometric(screenManager.game1.players[playerNum]));

                                //RE
                                skelStream.SetSignPack(theSignPack);
                                Console.WriteLine(" ### Set sign pack in skelStream  ### " + theSignPack.nameFile);
                                ExitScreen();
                            }
                            else if (screenManager.worldType == 4)
                            {
                                //RE
                                SignPack theSignPack = screenManager.game1.signPacks[screenManager.game1.currentSignPack];
                                SkeletonStreamRenderer skelStream = screenManager.game1.kinectManager.skeletonStream;

                                LoadingScreen.Load(screenManager, true, PlayerIndex.One,
                                new GameplayScreenCards(screenManager.game1.players[playerNum], screenManager.game1.signPacks[screenManager.game1.currentSignPack]));
                                //RE
                                skelStream.SetSignPack(theSignPack);
                                Console.WriteLine(" ### Set sign pack in skelStream  ### " + theSignPack.nameFile);

                                ExitScreen();
                            }
                        }
                    }
                }
                else if (input.IsMenuCancel2(ControllingPlayer))
                {
                    // Raise the cancelled event, then exit the message box.
                    ExitScreen();
                }
            }
            this.previousKeyboard = newState;
        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice graphics = screenManager.GraphicsDevice;
            SpriteBatch spriteBatch = screenManager.SpriteBatch;
            SpriteFont font = screenManager.Font;

            spriteBatch.Begin();

            int posX = 0;
            int posY = 0;
            int xdist = 420;
            int ydist = 330;

            foreach (Player thePlayer in screenManager.game1.players)
            {

                spriteBatch.Draw(screenManager.game1.images[thePlayer.playerSave.playerName], new Rectangle(50 + (posX * xdist), 60 + (posY * ydist), 350, 278), Color.White);

                posX++;
                if (posX == 3)
                {
                    posY++;
                    posX = 0;
                }
            }

            // Make the menu slide into place during transitions, using a
            // power curve to make things look more interesting (this makes
            // the movement slow down as it nears the end).
            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

            menuTitle = @"test";
            // Draw the menu title centered on the screen
            Vector2 titlePosition = new Vector2(graphics.Viewport.Width / 2, 80);
            Vector2 titleOrigin = font.MeasureString(menuTitle) / 2;
            Color titleColor = new Color(192, 192, 192) * TransitionAlpha;
            float titleScale = 1.25f;

            titlePosition.Y -= transitionOffset * 100;

            spriteBatch.DrawString(font, menuTitle, titlePosition, titleColor, 0,
                                   titleOrigin, titleScale, SpriteEffects.None, 0);

            //spriteBatch.DrawString(font, selectedEntry2D.ToString(), new Vector2(150, 150), new Color(192, 192, 192), 0,
            //                       titleOrigin, 1, SpriteEffects.None, 0);

            //spriteBatch.DrawString(font, SkeletonStreamRenderer.rightHandPos.ToString(), new Vector2(150, 200), new Color(192, 192, 192), 0,
            //                       titleOrigin, 1, SpriteEffects.None, 0);

            spriteBatch.Draw(frontTex, new Rectangle(0, 0, 1280, 720), Color.White);

            spriteBatch.Draw(selected, new Rectangle(-35 + (int)selectedEntry2D.X * 424 , -15 + (int)selectedEntry2D.Y * 352, 509, 413), Color.White);
            spriteBatch.Draw(mousePointer, new Rectangle((int)mouseVec.X, (int)mouseVec.Y, 20, 20), Color.White);
            spriteBatch.DrawString(font, "controlmethod: " + controlmethod.ToString(), new Vector2(150, 680), new Color(192, 192, 192), 0,
                           titleOrigin, 1, SpriteEffects.None, 0);
            spriteBatch.End();
        }


        #endregion

        #region Handle Input


        /// <summary>
        /// Event handler for when the Quit Game menu entry is selected.
        /// </summary>
        void OnPlayer(object sender, PlayerIndexEventArgs e)
        {
            const string message = "test1";

            MessageBoxScreen confirmPlayerMessageBox = new MessageBoxScreen(message);

            confirmPlayerMessageBox.Accepted += ConfirmQuitMessageBoxAccepted;

            screenManager.AddScreen(confirmPlayerMessageBox, ControllingPlayer);
        }

        void QuitGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            const string message = "Are you sure you want to quit this game?";

            MessageBoxScreen confirmQuitMessageBox = new MessageBoxScreen(message);

            confirmQuitMessageBox.Accepted += ConfirmPlayerMessageBoxAccepted;

            screenManager.AddScreen(confirmQuitMessageBox, ControllingPlayer);
        }

        void ConfirmPlayerMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
        {
            LoadingScreen.Load(screenManager, false, null, new BackgroundScreen(screenManager),
                                                           new MainMenuScreen(screenManager));
        }

        /// <summary>
        /// Event handler for when the user selects ok on the "are you sure
        /// you want to quit" message box. This uses the loading screen to
        /// transition from the game back to the main menu screen.
        /// </summary>
        void ConfirmQuitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
        {
            LoadingScreen.Load(screenManager, false, null, new BackgroundScreen(screenManager),
                                                           new MainMenuScreen(screenManager));
        }
        #endregion
    }
}
