//using System;
//using System.Windows.Forms;

namespace WindowsGame1
{
#if WINDOWS || XBOX
    static class Program
    {
      //  static Game1 game;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (Game1 game = new Game1())
            {
                game.Run();
            }

        //    Form1 form = new Form1();
        ////    form.FormClosed += (_, args) => this.Close();

 
        //    form.Show();
           
        //    game = new Game1(form.getDrawSurface());
        //    form.Hide();
        //    form.Show();

        //    form.FormClosing += new FormClosingEventHandler(gameForm_FormClosing);
        //    game.Exiting += new EventHandler< EventArgs>(game_Exiting);

        //    game.Run();
        }

        //static void game_Exiting(object sender, EventArgs e)
        //{
        //    Application.Exit();
        //}

        //static void gameForm_FormClosing(object sender, FormClosingEventArgs e)
        //{
        //    game.Exit();
        //}
    }
#endif
}

