using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WindowsGame1
{
    class CreatePlayerMenuScreen:MenuScreen
    {
        MenuEntry takePhoto;
        MenuEntry facesOnOffEntry;
        MenuEntry audioOnOffEntry;
        MenuEntry backgroundOnOffEntry;
        MenuEntry scoresOnOffEntry;
        MenuEntry saveEntry;
        MenuEntry borisColor;
        MenuEntry cardvideos;
        MenuEntry carddiagrams;
        MenuEntry precisionValue;
        MenuEntry back;

        bool facesOn = true;
        bool audioOn = true;
        bool backgroundOn = true;
        bool scoresOn = true;
        bool borisColorDefault = true;
        bool takingPhoto = true;
        bool videosOn = true;
        bool diagramsOn = true;
        int precisionNo = 0;
        bool leftHandedOn = false;

        bool imageTaken;
        Texture2D newImage;
        Player thePlayer;
        /// <summary>
        /// This Xna effect is used to swap the Red and Blue bytes of the color stream data.
        /// </summary>
        private Effect kinectColorVisualizer;
        int incr = 0;


        public CreatePlayerMenuScreen(Player thePlayer, ScreenManager screenManager)
            : base("Edit", screenManager)
        {
            this.thePlayer = thePlayer;
            startingYPos = 380;
            takePhoto = new MenuEntry(string.Empty);
            facesOnOffEntry = new MenuEntry(string.Empty);
            audioOnOffEntry = new MenuEntry(string.Empty);
            backgroundOnOffEntry = new MenuEntry(string.Empty);
            scoresOnOffEntry = new MenuEntry(string.Empty);
            saveEntry = new MenuEntry(string.Empty);
            borisColor = new MenuEntry(string.Empty);
            cardvideos = new MenuEntry(string.Empty);
            carddiagrams = new MenuEntry(string.Empty);
            precisionValue = new MenuEntry(string.Empty);
             back = new MenuEntry("Back");

            takePhoto.Selected += TakePhotoEntrySelected;
            facesOnOffEntry.Selected += FacesOnOffEntrySelected;
            audioOnOffEntry.Selected += AudioOnOffEntrySelected;
            backgroundOnOffEntry.Selected += BackgroundOnOffEntrySelected;
            scoresOnOffEntry.Selected += ScoresOnOffEntrySelected;
            saveEntry.Selected += SaveEntrySelected;
            borisColor.Selected += ChangeColorSelected;
            cardvideos.Selected += ChangeVideosSelected;
            carddiagrams.Selected += ChangeDiagramsSelected;
            precisionValue.Selected += ChangePrecisionSelected;
            back.Selected += OnCancel;

            MenuEntries.Add(takePhoto);
            MenuEntries.Add(facesOnOffEntry);
            MenuEntries.Add(audioOnOffEntry);
            MenuEntries.Add(backgroundOnOffEntry);
            MenuEntries.Add(scoresOnOffEntry);
            MenuEntries.Add(borisColor);
            MenuEntries.Add(cardvideos);
            MenuEntries.Add(carddiagrams);
            MenuEntries.Add(saveEntry);
            MenuEntries.Add(precisionValue);
            MenuEntries.Add(back);
        }

        public override void LoadContent()
        {
            this.kinectColorVisualizer = screenManager.Game.Content.Load<Effect>("KinectColorVisualizer");
            if (thePlayer != null)
            {
                imageTaken = true;
                takingPhoto = false;
                newImage = screenManager.game1.images[thePlayer.playerSave.playerName];

                facesOn = thePlayer.playerSave.face; ;
                audioOn = thePlayer.playerSave.audio;
                backgroundOn = thePlayer.playerSave.background;
                scoresOn = thePlayer.playerSave.score;
                borisColorDefault = thePlayer.playerSave.borisColorDefault;
                videosOn = thePlayer.playerSave.cardvideos;
                diagramsOn = thePlayer.playerSave.carddiagrams;
                precisionNo = thePlayer.playerSave.precision;
            }
            SetMenuEntryText();
        }

        void SetMenuEntryText()
        {
            facesOnOffEntry.Text = "Facial animations enabled: " + (facesOn ? "On" : "Off");
            audioOnOffEntry.Text = "Audio enabled: " + (audioOn ? "On" : "Off");
            backgroundOnOffEntry.Text = "Background enabled: " + (backgroundOn ? "On" : "Off");
            scoresOnOffEntry.Text = "HUD enabled: " + (scoresOn ? "On" : "Off");
            borisColor.Text = "Boris' colour: " + (borisColorDefault ? "Blue" : "Pink");
            cardvideos.Text = "Card game: Videos enabled: " + (videosOn ? "On" : "Off");
            carddiagrams.Text = "Card diagrams enabled: " + (diagramsOn ? "On" : "Off");
            precisionValue.Text = "Difficulty Level: " + precisionNo;
                if (imageTaken == true)
                {
                    if (takingPhoto == true)
                    {
                        takePhoto.Text = "Retake photo";
                        saveEntry.Text = "Cancel new player image";

                    }else{
                        takePhoto.Text = "Create new player image";
                        saveEntry.Text = "Save and exit";
                    }
                }
                else
                {
                    if (takingPhoto == true)
                    {
                        takePhoto.Text = "Take photo";
                        saveEntry.Text = "No photo";
                    }
                    else
                    {
                        takePhoto.Text = "Take Photo";
                        saveEntry.Text = "No photo";
                    }
                }
        }

        void SaveEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            if (takingPhoto == false && imageTaken == true)
            {
                if (thePlayer == null)
                {
                    screenManager.AddPlayer(newImage, facesOn, audioOn, backgroundOn, scoresOn, borisColorDefault, videosOn, diagramsOn, precisionNo, leftHandedOn);
                }
                else
                {
                    if(newImage != null)
                    {
                        screenManager.game1.images[thePlayer.playerSave.playerName] = newImage;
                    }
                    thePlayer.playerSave.audio = audioOn;
                    thePlayer.playerSave.background = backgroundOn;
                    thePlayer.playerSave.face = facesOn;
                    thePlayer.playerSave.score = scoresOn;
                    thePlayer.playerSave.borisColorDefault = borisColorDefault;
                    thePlayer.playerSave.cardvideos = videosOn;
                    thePlayer.playerSave.carddiagrams = diagramsOn;
                    thePlayer.playerSave.precision = precisionNo;
                }
                screenManager.game1.fileSaver.SaveIcons();
                ExitScreen();
            }
            else
            {
                if (imageTaken == true)
                {
                    takingPhoto = false;
                }
                else
                {
                }
            }

            SetMenuEntryText();
        }

        void TakePhotoEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            if (takingPhoto == true)
            {
                takingPhoto = false;
                Texture2D colortex = screenManager.game1.kinectManager.getColorTexture();
                newImage = new Texture2D(screenManager.GraphicsDevice, colortex.Width, colortex.Height);
                Color[] bits = new Color[colortex.Width * colortex.Height];

                colortex.GetData(bits);

                byte temp = 0;
                for (int i = 0; i < bits.Length; i++) //have to change colour to match formats
                {
                    temp = bits[i].B;
                    bits[i].A = 255;
                    bits[i].B = bits[i].R;
                    bits[i].R = temp;
                }
                newImage.SetData(bits);
                imageTaken = true;
                takingPhoto = false;
            }
            else
            {
                takingPhoto = true;
            }
            SetMenuEntryText();
        }

        void ChangePrecisionSelected( object sender, PlayerIndexEventArgs e)
        {
            precisionNo++;
            if (precisionNo > 2)
            {
                precisionNo = 0;
            }
            SetMenuEntryText();
        }
        
        void ChangeColorSelected(object sender, PlayerIndexEventArgs e)
        {
            borisColorDefault = !borisColorDefault;

            SetMenuEntryText();
        }

        void FacesOnOffEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            facesOn = !facesOn;

            SetMenuEntryText();
        }

        void AudioOnOffEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            audioOn = !audioOn;

            SetMenuEntryText();
        }

        void BackgroundOnOffEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            backgroundOn = !backgroundOn;

            SetMenuEntryText();
        }

        void ScoresOnOffEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            scoresOn = !scoresOn;

            SetMenuEntryText();
        }

        void ChangeVideosSelected(object sender, PlayerIndexEventArgs e)
        {
            videosOn = !videosOn;
            SetMenuEntryText();
        }
        void ChangeDiagramsSelected(object sender, PlayerIndexEventArgs e)
        {
            diagramsOn = !diagramsOn;
            SetMenuEntryText();
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            SpriteBatch spriteBatch = screenManager.SpriteBatch;
            spriteBatch.Begin();
             
            if (takingPhoto == true)
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, this.kinectColorVisualizer);
                      spriteBatch.GraphicsDevice.BlendState = BlendState.Opaque;
                      Texture2D colortex = screenManager.game1.kinectManager.getColorTexture();
                      if (colortex != null)
                      {
                          spriteBatch.Draw(colortex, new Rectangle((int)back.Position.X - 125, 120, 320, 240), Color.White);//needs transition alpha
                      }
            }
            else
            {
                    spriteBatch.Draw(newImage, new Rectangle((int)back.Position.X - 125, 120, 320, 240), Color.White);//needs transition alpha
            }
                spriteBatch.End();
        }
    }
}
