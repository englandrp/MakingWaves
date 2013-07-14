using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Storage;
using System.IO;
using System.Xml.Serialization;
using CppCLINativeDllWrapper;

namespace WindowsGame1
{
    public class FileSaver
    {
        Game1 game1; 
        IAsyncResult result;
        bool GameSaveRequested = false;
        bool GameLoadRequested = false;

        //This class contains all the xbox/windows code for loading and saving gamepacks and player details. 
        public FileSaver(Game1 game1)
        {
            this.game1 = game1;
        }

        //loads signpacks and player images
        public void LoadData()
        {
            // Set the request flag
            if ((!Guide.IsVisible) && (GameLoadRequested == false))
            {
                GameLoadRequested = true;
                result = StorageDevice.BeginShowSelector(
                        PlayerIndex.One, null, null);
            }
            if ((GameLoadRequested) && (result.IsCompleted))
            {
                StorageDevice device = StorageDevice.EndShowSelector(result);
                if (device != null && device.IsConnected)
                {
                    DoLoadIcons(device);
                    DoLoadSignPacks(device, game1);
                }
                // Reset the request flag
                GameLoadRequested = false;
            }
        }

        //loads the signpacks from My Documents
        private void DoLoadSignPacks(StorageDevice device, Game1 game1)
        {
            ContentManager content = game1.Content;

            // Open a storage container.
            IAsyncResult result =
                device.BeginOpenContainer("StorageDemo", null, null);

            // Wait for the WaitHandle to become signaled.
            result.AsyncWaitHandle.WaitOne();

            StorageContainer container = device.EndOpenContainer(result);

            // Close the wait handle.
            result.AsyncWaitHandle.Close();
            string root = Path.GetDirectoryName("SignPacks\\");

            if (root != "")
            {
                root += "/";
            }

            // Retrieve directories from folder 'signpacks'.
            List<String> directoryList = new List<String>(container.GetDirectoryNames("SignPacks\\"));

            //for each signpack...
            foreach (String directory in directoryList)
            {
                String[] parts = directory.Split(' ');
                String lastWord = parts[parts.Length - 1]; //seperate out the last word of the folder, this will be the signpack name

                //info for SignPack
                int noOfSigns; 
                String classFile = null; 
                String nameFile = null; 
                List<int> signOrder = new List<int>(); 
                List<int> cardOrder = new List<int>();
                List<Video> videos = new List<Video>();
                List<Texture2D> photos = new List<Texture2D>();
                List<Texture2D> diagrams = new List<Texture2D>();

                foreach (String s2 in container.GetFileNames(directory + "\\Diagrams\\"))
                {
                    if (Path.GetExtension(s2) == ".png")
                    {
                        using (Stream fileStream = container.OpenFile(s2, FileMode.Open))
                        {
                            Texture2D tex = Texture2D.FromStream(game1.GraphicsDevice, fileStream);
                            diagrams.Add(tex);
                        }
                    }
                }
                foreach (String s2 in container.GetFileNames(directory + "\\Photos\\"))
                {
                    if (Path.GetExtension(s2) == ".png")
                    {
                        using (Stream fileStream = container.OpenFile(s2, FileMode.Open))
                        {
                            Texture2D tex = Texture2D.FromStream(game1.GraphicsDevice, fileStream);
                            photos.Add(tex);
                        }
                    }
                }
                foreach (String s2 in container.GetFileNames(directory + "\\Videos\\"))
                {
                    if (Path.GetExtension(s2) == ".wmv")
                    {
                        String thexnb = s2;
                        int fileExtPos = s2.LastIndexOf(".");//get video name without .wmv at the end
                        if (fileExtPos >= 0)
                        {
                            thexnb = s2.Substring(0, fileExtPos) + ".xnb";//add .xnb to the video name to get the associated file
                        }
                        //If we have a matching xmv and xnb, move them into the build folder.
                        if (container.FileExists(thexnb))
                        {
                            string filename = Path.GetFileName(s2);
                            string copyfilename = Path.GetFileName(s2);
                            Stream file = container.OpenFile(s2, FileMode.Open);
                            String combined = Environment.CurrentDirectory + "\\Content\\" + copyfilename; //merge the file's path and name 
                            using (Stream fs = System.IO.File.Create(combined))
                            {
                                file.CopyTo(fs);
                                fs.Close();
                            }
                            file.Close();
                            file.Dispose();
                            filename = Path.GetFileName(thexnb);
                            copyfilename = Path.GetFileName(thexnb);
                            file = container.OpenFile(thexnb, FileMode.Open);
                            combined = Environment.CurrentDirectory + "\\Content\\" + copyfilename;//merge the path and name 
                            using (Stream fs = System.IO.File.Create(combined))
                            {
                                file.CopyTo(fs);
                                fs.Close();
                            }
                            file.Close();
                            file.Dispose();
                            videos.Add(content.Load<Video>(Path.GetFileNameWithoutExtension(s2)));
                        }
                    }
                }

                List<String> boxfiles = new List<String>(container.GetFileNames(directory + "\\"));

                foreach (String s1 in boxfiles)
                {
                    if (Path.GetExtension(s1) == ".cls") //class file
                    {
                        // Add the container path to our file name.
                        string filename = Path.GetFileName(s1);
                        string copyfilename = Path.GetFileName(s1);
                            Stream file = container.OpenFile(s1, FileMode.Open);
                            String combined = Environment.CurrentDirectory + "\\" + copyfilename;
                            using (Stream fs = System.IO.File.Create(combined))
                            {
                                file.CopyTo(fs);
                                fs.Close();
                            }
                            file.Close();
                            file.Dispose();
                        String text = Path.GetFileName(s1); 
                        classFile = text;
                        Console.WriteLine(" - classFile ------ " + classFile);
                    }
                    else if (Path.GetExtension(s1) == ".name") //name file
                    {
                        string filename = Path.GetFileName(s1);
                        string copyfilename = Path.GetFileName(s1);
                        Stream file = container.OpenFile(s1, FileMode.Open);
                        String combined = Environment.CurrentDirectory + "\\" + copyfilename;
                        using (Stream fs = System.IO.File.Create(combined))
                        {
                            file.CopyTo(fs);
                            fs.Close();
                        }
                        file.Close();
                        String text = Path.GetFileName(s1); 
                        nameFile = text;
                    }
                    else if (Path.GetExtension(s1) == ".so") //sign order file
                    {
                        Stream file = container.OpenFile(s1, FileMode.Open);
                        StreamReader reader = new StreamReader(file);
                        String line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            signOrder.Add(Convert.ToInt32(line));
                        }
                    }
                    else if (Path.GetExtension(s1) == ".co") // card order file
                    {
                        Stream file = container.OpenFile(s1, FileMode.Open);
                        StreamReader reader = new StreamReader(file);
                        String line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            cardOrder.Add(Convert.ToInt32(line));
                        }
                    }
                }
                noOfSigns = diagrams.Count;
                SignPack thePack = new SignPack(noOfSigns, classFile, nameFile, signOrder, cardOrder, videos, photos, diagrams, lastWord);
                game1.signPacks.Add(thePack); //add the signpack to the game's list of packs
            }
            container.Dispose();
        }

        /// <summary>
        /// Loads the player saves 
        /// </summary>
        /// <param name="device"></param>
        private void DoLoadIcons(StorageDevice device)
        {
            ContentManager content = game1.Content;

            // Open a storage container.
            IAsyncResult result =
                device.BeginOpenContainer("StorageDemo", null, null);

            // Wait for the WaitHandle to become signaled.
            result.AsyncWaitHandle.WaitOne();

            StorageContainer container = device.EndOpenContainer(result);

            // Close the wait handle.
            result.AsyncWaitHandle.Close();

            string filename = "playersave.sav";

            // Check to see whether the save exists.
            if (!container.FileExists(filename))
            {
                // If not, dispose of the container and create the default player profiles.
                container.Dispose();

                game1.players.Add(new Player("0", true, true, true, true, true, true, true, 0, false));
                game1.players.Add(new Player("1", true, true, true, true, true, true, true, 0, false));
                game1.players.Add(new Player("2", true, true, true, true, true, true, true, 0, false));
                game1.players.Add(new Player("3", true, true, true, true, true, true, true, 0, false));
                game1.players.Add(new Player("4", true, true, true, true, true, true, true, 0, false));
                game1.players.Add(new Player("5", true, true, true, true, true, true, true, 0, false));

                foreach (Player aPlayer in game1.players)
                {
                    string name = aPlayer.playerSave.playerName;
                    game1.images.Add(name, content.Load<Texture2D>("Graphics/d" + name));
                }
                DoSaveIcons(device);
                return;
            }

            // Open the file.
            Stream stream = container.OpenFile(filename, FileMode.Open);

            // Read the data from the file.
            XmlSerializer serializer = new XmlSerializer(typeof(List<PlayerSave>));
            int i = 0;

            List<PlayerSave> list1 = new List<PlayerSave>();
            while (stream.Position < stream.Length)
            {
                list1 = (List<PlayerSave>)serializer.Deserialize(stream); //read the stream contents into playersaves
            }

            foreach (PlayerSave save in list1)
            {
                Player player = new Player(save.playerName, save.face, save.audio, save.background, save.score, save.borisColorDefault, save.cardvideos, save.carddiagrams, save.precision, save.leftHanded);
                game1.players.Add(player);
            }

            // Close the file.
            stream.Close();
            foreach (Player aPlayer in game1.players)
            {
                string name = aPlayer.playerSave.playerName;
                using (Stream fileStream = container.OpenFile("Images/" + name + ".png", FileMode.Open))
                {
                    Texture2D tex = Texture2D.FromStream(game1.GraphicsDevice, fileStream);
                    if (game1.images.ContainsKey(name))
                    {
                        game1.images.Remove(name);
                    }
                    game1.images.Add(name, tex);
                }
            }
            // Dispose the container.
            container.Dispose();
        }

        public void SaveIcons()
        {
            // Set the request flag
            if ((!Guide.IsVisible) && (GameSaveRequested == false))
            {
                GameSaveRequested = true;
                result = StorageDevice.BeginShowSelector(
                        PlayerIndex.One, null, null);
            }
            // If a save is pending, save as soon as the
            // storage device is chosen
            if ((GameSaveRequested) && (result.IsCompleted))
            {
                StorageDevice device = StorageDevice.EndShowSelector(result);
                if (device != null && device.IsConnected)
                {
                    DoSaveIcons(device);
                }
                // Reset the request flag
                GameSaveRequested = false;
            }
        }


        /// <summary>
        /// This method serializes a data object into
        /// the StorageContainer for this game.
        /// </summary>
        /// <param name="device"></param>
        private  void DoSaveIcons(StorageDevice device)
        {
            // Open a storage container.
            IAsyncResult result = device.BeginOpenContainer("StorageDemo", null, null);

            // Wait for the WaitHandle to become signaled.
            result.AsyncWaitHandle.WaitOne();

            StorageContainer container = device.EndOpenContainer(result);

            // Close the wait handle.
            result.AsyncWaitHandle.Close();

            string filename = "playersave.sav";

            // Check to see whether the save exists.
            if (container.FileExists(filename))
            {
                // Delete it so that we can create one fresh.
                container.DeleteFile(filename);
            }
            // Create the file.
            Stream stream = container.CreateFile(filename);


            // Convert the object to XML data and put it in the stream.
            XmlSerializer serializer = new XmlSerializer(typeof(List<PlayerSave>));
            List<PlayerSave> list1 = new List<PlayerSave>();
            foreach (Player aPlayer in game1.players)
            {
                list1.Add(aPlayer.playerSave);
            }
            serializer.Serialize(stream, list1);
            // Close the file.
            stream.Close();

            foreach (KeyValuePair<string, Texture2D> pair in game1.images)
            {
                container.CreateDirectory("Images");
                using (Stream fileStream = container.OpenFile("Images/" + pair.Key + ".png",FileMode.OpenOrCreate))
                {
                    pair.Value.SaveAsPng(fileStream, pair.Value.Width, pair.Value.Height);
                } 
            }            
            // Dispose the container, to commit changes.
            container.Dispose();
        }
    }
}
