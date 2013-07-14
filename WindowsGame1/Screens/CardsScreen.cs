using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsGame1
{
    class CardsScreen : MenuScreen
    {
        public CardsScreen(ScreenManager screenManager, Game1 game1)
            : base("Card Sets", screenManager)
        {
            for (int i = 0; i < game1.signPacks.Count(); i++)
            {
                MenuEntry theEntry = new MenuEntry(string.Empty);
                theEntry.Text = game1.signPacks[i].packName;
                theEntry.Selected += StartMenuEntrySelected;
                MenuEntries.Add(theEntry);
            }
        }

        /// <summary>
        /// Event handler for when the Elf menu entry is selected.
        /// </summary>
        void StartMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            screenManager.worldType = 2;
            for (int i = 0; i < screenManager.game1.signPacks.Count(); i++)
            {
                if(sender.GetType() == typeof(MenuEntry))
                {
                    if(((MenuEntry)sender).Text == screenManager.game1.signPacks[i].packName)
                    {
                    screenManager.game1.currentSignPack = i;
                    screenManager.AddScreen(new PlayerSelectScreen(this.screenManager), e.PlayerIndex);
                    }
                }
            }
        } 
    }
}
