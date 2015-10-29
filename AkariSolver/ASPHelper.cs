using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Reflection;

namespace AkariSolver
{
    /// <summary>
    /// ASP helper functions.
    /// </summary>
    public static class ASPHelper
    {
        /// <summary>
        /// Solve this instance.
        /// </summary>
        public static bool Solve()
        {
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            path = path.Substring(0, path.LastIndexOf('/') + 1).Replace('/', System.IO.Path.DirectorySeparatorChar);
            if (!File.Exists(path + @"akari.lp"))
            {
                File.WriteAllText(path + @"akari.lp",
                    "spaceX(0..xmax).\n" +
                    "spaceY(0..ymax).\n" +
                    "black(X,Y) :- hint(X,Y,Z).\n" +
                    "white(X,Y) :- not black(X,Y), spaceX(X), spaceY(Y).\n" +
                    "adjacent_h(X,Y,X,Y-1) :- Y > 0, spaceX(X), spaceY(Y).\n" +
                    "adjacent_h(X,Y,X,Y+1) :- Y < ymax, spaceX(X), spaceY(Y).\n" +
                    "adjacent_v(X,Y,X-1,Y) :- X > 0, spaceX(X), spaceY(Y).\n" +
                    "adjacent_v(X,Y,X+1,Y) :- X < xmax, spaceX(X), spaceY(Y).\n" +
                    "adjacent(X1,Y1,X2,Y2) :- adjacent_h(X1,Y1,X2,Y2).\n" +
                    "adjacent(X1,Y1,X2,Y2) :- adjacent_v(X1,Y1,X2,Y2).\n" +
                    "can_see_h(X,Y1,X,Y2) :- adjacent_h(X,Y1,X,Y2), not black(X,Y1; X,Y2), spaceX(X), spaceY(Y1; Y2), Y1!=Y2.\n" +
                    "can_see_h(X,Y1,X,Y2) :- adjacent_h(X,Y1,X,Y3), can_see_h(X,Y3,X,Y2), not black(X,Y1; X,Y2; X,Y3), spaceX(X), spaceY(Y1; Y2; Y3), Y1!=Y2, Y2!=Y3.\n" +
                    "can_see_v(X1,Y,X2,Y) :- adjacent_v(X1,Y,X2,Y), not black(X1,Y; X2,Y), spaceX(X1; X2), spaceY(Y), X1!=X2.\ncan_see_v(X1,Y,X2,Y) :- adjacent_v(X1,Y,X3,Y), can_see_v(X3,Y,X2,Y), not black(X1,Y; X2,Y; X3,Y), spaceX(X1; X2; X3), spaceY(Y), X1!=X2, X2!=X3.\n" +
                    "can_see(X1,Y1,X2,Y2) :- can_see_v(X1,Y1,X2,Y2).\ncan_see(X1,Y1,X2,Y2) :- can_see_h(X1,Y1,X2,Y2).\n" +
                    ":- lamp(X,Y), black(X,Y).\n" +
                    ":- lamp(X1,Y1), lamp(X2,Y2), can_see(X1,Y1,X2,Y2).\n" +
                    "I{lamp(X1,Y1) : adjacent(X,Y,X1,Y1)}I :- hint(X,Y,I).\n" +
                    "lit(X,Y) :- lamp(X1,Y1), can_see(X,Y,X1,Y1), not black(X,Y), spaceX(X;X1), spaceY(Y;Y1).\n" +
                    "lamp(X,Y) :- spaceX(X), spaceY(Y), not -lamp(X,Y).\n" +
                    "-lamp(X,Y) :- spaceX(X), spaceY(Y), not lamp(X,Y).\n" +
                    ":- not lit(X,Y), not lamp(X,Y), not black(X,Y), spaceX(X), spaceY(Y).\n" +
                    "#show lamp/2.\n");
            }
            GameBoard.Spaces[,] board = MainWindow.board.board;
            int rows = board.GetLength(0);
            int cols = board.GetLength(1);
            string write = "#const xmax = " + (rows - 1) + ".\n" +
                           "#const ymax = " + (cols - 1) + ".\n";
            for (int x = 0; x < rows; x++)
            {
                for (int y = 0; y < cols; y++)
                {
                    switch (board[x, y])
                    {					
                        case GameBoard.Spaces.Black:
                            WriteBlack(x, y, ref write);
                            break;

                        case GameBoard.Spaces.Zero:
                            WriteHint(x, y, 0, ref write);
                            break;

                        case GameBoard.Spaces.One:
                            WriteHint(x, y, 1, ref write);
                            break;

                        case GameBoard.Spaces.Two:
                            WriteHint(x, y, 2, ref write);
                            break;

                        case GameBoard.Spaces.Three:
                            WriteHint(x, y, 3, ref write);
                            break;

                        case GameBoard.Spaces.Four:
                            WriteHint(x, y, 4, ref write);
                            break;
                    }
                }
            }
            WriteFile(ref write);
            return RunCommand(0);
        }

        /// <summary>
        /// Writes the black.
        /// </summary>
        /// <param name="x">Row.</param>
        /// <param name="y">Column.</param>
        /// <param name="write">The ASP string.</param>
        static void WriteBlack(int x, int y, ref string write)
        {
            write += "black(" + x + "," + y + ").\n";
        }

        /// <summary>
        /// Writes the hint.
        /// </summary>
        /// <param name="x">Row.</param>
        /// <param name="y">Column.</param>
        /// <param name="z">The z coordinate.</param>
        /// <param name="write">The ASP string.</param>
        static void WriteHint(int x, int y, int z, ref string write)
        {
            write += "hint(" + x + "," + y + "," + z + ").\n";
        }

        /// <summary>
        /// Writes the lamp.
        /// </summary>
        /// <param name="x">Row.</param>
        /// <param name="y">Column.</param>
        /// <param name="write">The ASP string.</param>
        static void WriteLamp(int x, int y, ref string write)
        {
            write += "lamp(" + x + "," + y + ").\n";
        }

        /// <summary>
        /// Writes the file.
        /// </summary>
        /// <param name="write">The ASP string.</param>
        static void WriteFile(ref string write)
        {
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            path = path.Substring(0, path.LastIndexOf('/') + 1).Replace('/', System.IO.Path.DirectorySeparatorChar);
            System.IO.File.WriteAllText(path + @"akarispaces.lp", write);
        }

        /// <summary>
        /// Check this instance.
        /// </summary>
        public static bool Check()
        {
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            path = path.Substring(0, path.LastIndexOf('/') + 1).Replace('/', System.IO.Path.DirectorySeparatorChar);
            if (!File.Exists(path + @"akaricheck.lp"))
            {
                File.WriteAllText(path + @"akaricheck.lp",
                    "spaceX(0..xmax).\n" +
                    "spaceY(0..ymax).\n" +
                    "black(X,Y) :- hint(X,Y,Z).\n" +
                    "white(X,Y) :- not black(X,Y), spaceX(X), spaceY(Y).\n" +
                    "adjacent_h(X,Y,X,Y-1) :- Y > 0, spaceX(X), spaceY(Y).\n" +
                    "adjacent_h(X,Y,X,Y+1) :- Y < ymax, spaceX(X), spaceY(Y).\n" +
                    "adjacent_v(X,Y,X-1,Y) :- X > 0, spaceX(X), spaceY(Y).\n" +
                    "adjacent_v(X,Y,X+1,Y) :- X < xmax, spaceX(X), spaceY(Y).\n" +
                    "adjacent(X1,Y1,X2,Y2) :- adjacent_h(X1,Y1,X2,Y2).\n" +
                    "adjacent(X1,Y1,X2,Y2) :- adjacent_v(X1,Y1,X2,Y2).\n" +
                    "can_see_h(X,Y1,X,Y2) :- adjacent_h(X,Y1,X,Y2), not black(X,Y1; X,Y2), spaceX(X), spaceY(Y1; Y2), Y1!=Y2.\n" +
                    "can_see_h(X,Y1,X,Y2) :- adjacent_h(X,Y1,X,Y3), can_see_h(X,Y3,X,Y2), not black(X,Y1; X,Y2; X,Y3), spaceX(X), spaceY(Y1; Y2; Y3), Y1!=Y2, Y2!=Y3.\n" +
                    "can_see_v(X1,Y,X2,Y) :- adjacent_v(X1,Y,X2,Y), not black(X1,Y; X2,Y), spaceX(X1; X2), spaceY(Y), X1!=X2.\ncan_see_v(X1,Y,X2,Y) :- adjacent_v(X1,Y,X3,Y), can_see_v(X3,Y,X2,Y), not black(X1,Y; X2,Y; X3,Y), spaceX(X1; X2; X3), spaceY(Y), X1!=X2, X2!=X3.\n" +
                    "can_see(X1,Y1,X2,Y2) :- can_see_v(X1,Y1,X2,Y2).\ncan_see(X1,Y1,X2,Y2) :- can_see_h(X1,Y1,X2,Y2).\n" +
                    ":- lamp(X,Y), black(X,Y).\n" +
                    ":- lamp(X1,Y1), lamp(X2,Y2), can_see(X1,Y1,X2,Y2).\n" +
                    "%I{lamp(X1,Y1) : adjacent(X,Y,X1,Y1)}I :- hint(X,Y,I).\n" +
                    "lit(X,Y) :- lamp(X1,Y1), can_see(X,Y,X1,Y1), not black(X,Y), spaceX(X;X1), spaceY(Y;Y1).\n" +
                    "%lamp(X,Y) :- spaceX(X), spaceY(Y), not -lamp(X,Y).\n" +
                    "%-lamp(X,Y) :- spaceX(X), spaceY(Y), not lamp(X,Y).\n" +
                    ":- not lit(X,Y), not lamp(X,Y), not black(X,Y), spaceX(X), spaceY(Y).\n" +
                    "#show lit/2.\n");
            }
            GameBoard.Spaces[,] board = MainWindow.board.board;
            int rows = board.GetLength(0);
            int cols = board.GetLength(1);
            string write = "#const xmax = " + (rows - 1) + ".\n" +
                           "#const ymax = " + (cols - 1) + ".\n";
            for (int x = 0; x < rows; x++)
            {
                for (int y = 0; y < cols; y++)
                {
                    switch (board[x, y])
                    {                   
                        case GameBoard.Spaces.Black:
                            WriteBlack(x, y, ref write);
                            break;

                        case GameBoard.Spaces.Zero:
                            WriteHint(x, y, 0, ref write);
                            break;

                        case GameBoard.Spaces.One:
                            WriteHint(x, y, 1, ref write);
                            break;

                        case GameBoard.Spaces.Two:
                            WriteHint(x, y, 2, ref write);
                            break;

                        case GameBoard.Spaces.Three:
                            WriteHint(x, y, 3, ref write);
                            break;

                        case GameBoard.Spaces.Four:
                            WriteHint(x, y, 4, ref write);
                            break;

                        case GameBoard.Spaces.Lamp:
                            WriteLamp(x, y, ref write);
                            break;
                    }
                }
            }
            WriteFile(ref write);
            return RunCommand(1);
        }

        /// <summary>
        /// Light this instance.
        /// </summary>
        public static bool Lit()
        {
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            path = path.Substring(0, path.LastIndexOf('/') + 1).Replace('/', System.IO.Path.DirectorySeparatorChar);
            if (!File.Exists(path + @"akarilit.lp"))
            {
                File.WriteAllText(path + @"akarilit.lp",
                    "spaceX(0..xmax).\n" +
                    "spaceY(0..ymax).\n" +
                    "black(X,Y) :- hint(X,Y,Z).\n" +
                    "white(X,Y) :- not black(X,Y), spaceX(X), spaceY(Y).\n" +
                    "adjacent_h(X,Y,X,Y-1) :- Y > 0, spaceX(X), spaceY(Y).\n" +
                    "adjacent_h(X,Y,X,Y+1) :- Y < ymax, spaceX(X), spaceY(Y).\n" +
                    "adjacent_v(X,Y,X-1,Y) :- X > 0, spaceX(X), spaceY(Y).\n" +
                    "adjacent_v(X,Y,X+1,Y) :- X < xmax, spaceX(X), spaceY(Y).\n" +
                    "adjacent(X1,Y1,X2,Y2) :- adjacent_h(X1,Y1,X2,Y2).\n" +
                    "adjacent(X1,Y1,X2,Y2) :- adjacent_v(X1,Y1,X2,Y2).\n" +
                    "can_see_h(X,Y1,X,Y2) :- adjacent_h(X,Y1,X,Y2), not black(X,Y1; X,Y2), spaceX(X), spaceY(Y1; Y2), Y1!=Y2.\n" +
                    "can_see_h(X,Y1,X,Y2) :- adjacent_h(X,Y1,X,Y3), can_see_h(X,Y3,X,Y2), not black(X,Y1; X,Y2; X,Y3), spaceX(X), spaceY(Y1; Y2; Y3), Y1!=Y2, Y2!=Y3.\n" +
                    "can_see_v(X1,Y,X2,Y) :- adjacent_v(X1,Y,X2,Y), not black(X1,Y; X2,Y), spaceX(X1; X2), spaceY(Y), X1!=X2.\ncan_see_v(X1,Y,X2,Y) :- adjacent_v(X1,Y,X3,Y), can_see_v(X3,Y,X2,Y), not black(X1,Y; X2,Y; X3,Y), spaceX(X1; X2; X3), spaceY(Y), X1!=X2, X2!=X3.\n" +
                    "can_see(X1,Y1,X2,Y2) :- can_see_v(X1,Y1,X2,Y2).\ncan_see(X1,Y1,X2,Y2) :- can_see_h(X1,Y1,X2,Y2).\n" +
                    "%:- lamp(X,Y), black(X,Y).\n" +
                    "%:- lamp(X1,Y1), lamp(X2,Y2), can_see(X1,Y1,X2,Y2).\n" +
                    "%I{lamp(X1,Y1) : adjacent(X,Y,X1,Y1)}I :- hint(X,Y,I).\n" +
                    "lit(X,Y) :- lamp(X1,Y1), can_see(X,Y,X1,Y1), not black(X,Y), spaceX(X;X1), spaceY(Y;Y1).\n" +
                    "-lit(X,Y) :- white(X,Y), not lit(X,Y), not black(X,Y), not lamp(X,Y).\n" +
                    "%lamp(X,Y) :- spaceX(X), spaceY(Y), not -lamp(X,Y).\n" +
                    "%-lamp(X,Y) :- spaceX(X), spaceY(Y), not lamp(X,Y).\n" +
                    "%:- not lit(X,Y), not lamp(X,Y), not black(X,Y), spaceX(X), spaceY(Y).\n" +
                    "#show lit/2.\n" +
                    "#show -lit/2.\n");
            }
            GameBoard.Spaces[,] board = MainWindow.board.board;
            int rows = board.GetLength(0);
            int cols = board.GetLength(1);
            string write = "#const xmax = " + (rows - 1) + ".\n" +
                           "#const ymax = " + (cols - 1) + ".\n";
            for (int x = 0; x < rows; x++)
            {
                for (int y = 0; y < cols; y++)
                {
                    switch (board[x, y])
                    {                   
                        case GameBoard.Spaces.Black:
                            WriteBlack(x, y, ref write);
                            break;

                        case GameBoard.Spaces.Zero:
                            WriteHint(x, y, 0, ref write);
                            break;

                        case GameBoard.Spaces.One:
                            WriteHint(x, y, 1, ref write);
                            break;

                        case GameBoard.Spaces.Two:
                            WriteHint(x, y, 2, ref write);
                            break;

                        case GameBoard.Spaces.Three:
                            WriteHint(x, y, 3, ref write);
                            break;

                        case GameBoard.Spaces.Four:
                            WriteHint(x, y, 4, ref write);
                            break;

                        case GameBoard.Spaces.Lamp:
                            WriteLamp(x, y, ref write);
                            break;
                    }
                }
            }
            WriteFile(ref write);
            return RunCommand(2);
        }

        /// <summary>
        /// Runs clingo.
        /// </summary>
        /// <returns><c>true</c>, if solutions were found, <c>false</c> otherwise.</returns>
        /// <param name="mode">Mode.</param>
        static bool RunCommand(int mode)
        {
            string output = "";
            string[] outputs;
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            path = path.Substring(0, path.LastIndexOf('/') + 1).Replace('/', System.IO.Path.DirectorySeparatorChar);
            Console.WriteLine(path);
            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.WorkingDirectory = path;
            if (mode == 0)
            {
                proc.StartInfo.FileName = "clingo";
                proc.StartInfo.Arguments = "akarispaces.lp " + "akari.lp";
                proc.Start();
                proc.OutputDataReceived += (object sender, DataReceivedEventArgs e) => output += e.Data + "\n";
                proc.BeginOutputReadLine();
                proc.WaitForExit();
                outputs = output.Split('\n');
                foreach (var str in outputs)
                {
                    if (str == "UNSATISFIABLE")
                        return false;
                }
                System.IO.File.WriteAllText(path + @"akari.sol", output);
                return true;
            }
            else if (mode == 1)
            {
                proc.StartInfo.FileName = "clingo";
                proc.StartInfo.Arguments = "akarispaces.lp " + "akaricheck.lp";
                proc.Start();
                proc.OutputDataReceived += (object sender, DataReceivedEventArgs e) => output += e.Data + "\n";
                proc.BeginOutputReadLine();
                proc.WaitForExit();
                outputs = output.Split('\n');
                foreach (var str in outputs)
                {
                    if (str == "UNSATISFIABLE")
                        return false;
                }
                System.IO.File.WriteAllText(path + @"akari.sol", output);
                return true;
            }
            else if (mode == 2)
            {
                proc.StartInfo.FileName = "clingo";
                proc.StartInfo.Arguments = "akarispaces.lp " + "akarilit.lp";
                proc.Start();
                proc.OutputDataReceived += (object sender, DataReceivedEventArgs e) => output += e.Data + "\n";
                proc.BeginOutputReadLine();
                proc.WaitForExit();
                outputs = output.Split('\n');
                foreach (var str in outputs)
                {
                    if (str == "UNSATISFIABLE")
                        return false;
                }
                System.IO.File.WriteAllText(path + @"akarilit.sol", output);
                return true;
            }
            else
                return false;
        }
    }
}

