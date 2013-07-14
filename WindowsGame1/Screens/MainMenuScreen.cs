#region File Description
//-----------------------------------------------------------------------------
// MainMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
#endregion

namespace WindowsGame1
{
    /// <summary>
    /// The main menu screen is the first thing displayed when the game starts up.
    /// </summary>
    class MainMenuScreen : MenuScreen
    {
        #region Initialization

        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        public MainMenuScreen(ScreenManager screenManager)
            : base("Main Menu",screenManager)
        {
            // Create our menu entries.
            MenuEntry playGameMenuEntry = new MenuEntry("Play Game");
            MenuEntry playCardGameMenuEntry = new MenuEntry("Play Card Game");
            MenuEntry playIsoGameMenuEntry = new MenuEntry("Play Isometric Game");
            //MenuEntry optionsMenuEntry = new MenuEntry("Options");
            MenuEntry exitMenuEntry = new MenuEntry("Exit");
            MenuEntry createPlayerMenuEntry = new MenuEntry("Create Player");

            // Hook up menu event handlers.
            playGameMenuEntry.Selected += PlayGameMenuEntrySelected;
            playCardGameMenuEntry.Selected += PlayCardGameMenuEntrySelected;
            playIsoGameMenuEntry.Selected += PlayIsoGameMenuEntrySelected;
            //optionsMenuEntry.Selected += OptionsMenuEntrySelected;
            createPlayerMenuEntry.Selected += CreatePlayerMenuEntrySelected;
            exitMenuEntry.Selected += OnCancel;

            // Add entries to the menu.
            MenuEntries.Add(playGameMenuEntry);
            MenuEntries.Add(playCardGameMenuEntry);
            MenuEntries.Add(playIsoGameMenuEntry);
            //MenuEntries.Add(optionsMenuEntry);
            MenuEntries.Add(createPlayerMenuEntry);
            MenuEntries.Add(exitMenuEntry);
        }

        #endregion

        #region Handle Input


        /// <summary>
        /// Event handler for when the Play Game menu entry is selected.
        /// </summary>
        void PlayGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            screenManager.worldType = 1;
          //  LoadingScreen.Load(screenManager, true, e.PlayerIndex, new GameplayScreen());
            screenManager.AddScreen(new PlayerSelectScreen(this.screenManager), e.PlayerIndex);
        }

        void PlayCardGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            screenManager.worldType = 2;
            //  LoadingScreen.Load(screenManager, true, e.PlayerIndex, new GameplayScreen());
            screenManager.AddScreen(new CardsScreen(screenManager,screenManager.game1), e.PlayerIndex);
        }

        void PlayIsoGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            screenManager.worldType = 3;
            //  LoadingScreen.Load(screenManager, true, e.PlayerIndex, new GameplayScreen());
            screenManager.AddScreen(new PlayerSelectScreen(screenManager), e.PlayerIndex);
        }

        void CreatePlayerMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            screenManager.AddScreen(new CreatePlayerMenuScreen(null, screenManager), e.PlayerIndex);
        }

        /// <summary>
        /// Event handler for when the Options menu entry is selected.
        /// </summary>
        void OptionsMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            screenManager.AddScreen(new OptionsMenuScreen(screenManager), e.PlayerIndex);
        }

        /// <summary>
        /// When the user cancels the main menu, ask if they want to exit the sample.
        /// </summary>
        protected override void OnCancel(PlayerIndex playerIndex)
        {
            const string message = "Are you sure you want to exit this sample?";

            MessageBoxScreen confirmExitMessageBox = new MessageBoxScreen(message);

            confirmExitMessageBox.Accepted += ConfirmExitMessageBoxAccepted;

            screenManager.AddScreen(confirmExitMessageBox, playerIndex);
        }

        /// <summary>
        /// Event handler for when the user selects ok on the "are you sure
        /// you want to exit" message box.
        /// </summary>
        void ConfirmExitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
        {
            screenManager.Game.Exit();
          //  Application.Exit();
        }


        #endregion
    }
}
