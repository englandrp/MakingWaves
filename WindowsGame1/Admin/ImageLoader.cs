using System;
using System.Windows.Forms;
using System.IO;
using System.IO.IsolatedStorage;
using Microsoft.Xna.Framework.Graphics;

namespace WindowsGame1
{
    //Uses c# libraries to open a file dialog, load an image then return it as an XNA texture2d. Likely currently over complicated. CURRENTLY NOT IN USE
    public partial class ImageLoader
    {
        public Texture2D openDialog(GraphicsDevice device)
        {
            OpenFileDialog frm = new OpenFileDialog(); //The standard windows open file dialog
            String Location = String.Empty;
  
            frm.InitializeLifetimeService();
            frm.Filter = "Images (*.BMP;*.JPG;*.GIF,*.PNG,*.TIFF)|*.BMP;*.JPG;*.GIF;*.PNG;*.TIFF|" +
                         "All files (*.*)|*.*";
            frm.Title = "Browse for replacement image file";

            //The dialog state must be called from an STA thread, not from an MTA thread that XNA uses. 
            DialogResult ret = STAShowDialog(frm); 

            //Load image only if the dialog popup has worked correctly
            if (ret == DialogResult.OK)
            {
                Location = frm.FileName;
                FileStream fs = File.OpenRead(Location);
                IsolatedStorageScope scope = IsolatedStorageScope.User | IsolatedStorageScope.Assembly | IsolatedStorageScope.Roaming;
                
                using (var store = IsolatedStorageFile.GetStore(scope, null, null))
                {
                    // Create a directory at the root of the store.
                    if (!store.DirectoryExists("Images"))
                    {
                        store.CreateDirectory("Images");
                    }


                    using (IsolatedStorageFileStream isoStream = store.OpenFile(@"Images\UserImageFile.png", FileMode.OpenOrCreate))
                    {
                        byte[] bytes = new byte[fs.Length];
                        fs.Read(bytes, 0, (int)fs.Length);
                        isoStream.Write(bytes, 0, (int)fs.Length);
                    }
                }

                using (var store = IsolatedStorageFile.GetStore(scope, null, null))
                {
                    if (store.FileExists(@"Images\UserImageFile.png"))
                    {
                        using (var isoStream = store.OpenFile(@"Images\UserImageFile.png", FileMode.Open, FileAccess.Read))
                        {
                            Texture2D tex = Texture2D.FromStream(device, isoStream);
                            return tex;
                        }
                    }
                }
            }
            return null;
        }
            /* STAShowDialog takes a FileDialog and shows it on a background STA thread and returns the results.
         * Usage:
         *   OpenFileDialog d = new OpenFileDialog();
         *   DialogResult ret = STAShowDialog(d);
         *   if (ret == DialogResult.OK)
         *      MessageBox.Show(d.FileName);
         */
        private DialogResult STAShowDialog(FileDialog dialog)
        {
            DialogState state = new DialogState();
            state.dialog = dialog;
            System.Threading.Thread t = new System.Threading.Thread(state.ThreadProcShowDialog);
            t.SetApartmentState(System.Threading.ApartmentState.STA);
            t.Start();
            t.Join();
            return state.result;
        }
    }
}
        /* Helper class to hold state and return value in order to call FileDialog.ShowDialog on a background thread.
     * Usage:
     *   DialogState state = new DialogState();
     *   state.dialog = // <any class that derives from FileDialog>
     *   System.Threading.Thread t = new System.Threading.Thread(state.ThreadProcShowDialog);
     *   t.SetApartmentState(System.Threading.ApartmentState.STA);
     *   t.Start();
     *   t.Join();
     *   return state.result;
     */
  public class DialogState
    {
        public DialogResult result;
        public FileDialog dialog;
 
        public void ThreadProcShowDialog()
        {
            result = dialog.ShowDialog();
        }
    }

