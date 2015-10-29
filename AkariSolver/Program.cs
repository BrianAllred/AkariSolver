using System;
using Gtk;

namespace AkariSolver
{
	/// <summary>
	/// Main class.
	/// </summary>
    class MainClass
    {
		/// <summary>
		/// The entry point of the program, where the program control starts and ends.
		/// </summary>
		/// <param name="args">The command-line arguments.</param>
        public static void Main(string[] args)
        {
            try
            {
                Application.Init();
                GLib.Thread.Init();
                MainWindow win = new MainWindow();
                win.Show();
                Application.Run();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return;
            }
        }
    }
}
