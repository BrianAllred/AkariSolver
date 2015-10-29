using System;
using Gdk;
using Gtk;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Reflection;

namespace AkariSolver
{
    /// <summary>
    /// Grid constructor window for making and solving Akari boards.
    /// </summary>
    public partial class GridConstructor : Gtk.Window
    {
        /// <summary>
        /// The board's GUI structure.
        /// </summary>
        public ListStore board;

        /// <summary>
        /// The mode.
        /// </summary>
        protected int mode = 0;

        /// <summary>
        /// The lib path.
        /// </summary>
        protected string libPath;

        /// <summary>
        /// The assembly path.
        /// </summary>
        protected string path;

        /// <summary>
        /// The height of each cell.
        /// </summary>
        protected const int CELL_HEIGHT = 35;

        /// <summary>
        /// The width of each cell.
        /// </summary>
        protected const int CELL_WIDTH = 35;

        /// <summary>
        /// The number rows.
        /// </summary>
        protected int Rows;

        /// <summary>
        /// The number of columns.
        /// </summary>
        protected int Cols;

        /// <summary>
        /// The parent window.
        /// </summary>
        public MainWindow main;

        /// <summary>
        /// Initializes a new instance of the <see cref="AkariSolver.GridConstructor"/> class.
        /// </summary>
        /// <param name="r">Rows.</param>
        /// <param name="c">Columns.</param>
        /// <param name="mainWindow">Reference to constructing window.</param>
        public GridConstructor(int r, int c, MainWindow mainWindow)
            : base(Gtk.WindowType.Toplevel)
        {
            this.Build();

            //Save parent window for handling later.
            this.main = mainWindow;

            Rows = r;
            Cols = c;

            //The following 5 lines are all OS-agnostic path grabbing.
            //The default for Mono C# is for paths to be broken by '/' char
            //This is problematic for Windows, since it uses the '\' char.
            //Solution is to do a blanket replace of '/' with the System
            //level DirectorySeparatorChar. Slightly overkill since all other
            //OSes use '/', but it prevents a system call to check the OS,
            //which may or may not be able to be OS-agnostic.
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            path = Uri.UnescapeDataString(uri.Path);
            path = path.Substring(0, path.LastIndexOf('/') + 1).Replace('/', System.IO.Path.DirectorySeparatorChar);
            libPath = path + @"lib" + System.IO.Path.DirectorySeparatorChar;

            /*The rest is algorithmic generation of the game board
              as specified by given rows and columns.
              I used a GTK TreeView as my board. The way a GTK TV works
              is it takes multiple ListStore structures initialized with
              specific types across the columns. Each ListStore is appended
              as a row of the TV.
            */
            this.SetSizeRequest(40 * c + 20, 40 * r + 50);
            gridBoard.SetSizeRequest(40 * c, 40 * r);
            System.Type[] types = new System.Type[c];
            Gdk.Pixbuf[] spaces = new Gdk.Pixbuf[c];
            for (int x = 0; x < c; x++)
            {
                var cell = new Gtk.CellRendererPixbuf();
                cell.Height = CELL_HEIGHT;
                cell.Width = CELL_WIDTH;
                gridBoard.AppendColumn("", cell, "pixbuf", x);
                types[x] = typeof(Gdk.Pixbuf);
                spaces[x] = new Gdk.Pixbuf(libPath + @"WhiteSpace.png");
            }
            board = new Gtk.ListStore(types);
            for (int x = 0; x < r; x++)
            {
                board.AppendValues(spaces);
            }
            gridBoard.Model = board;
            gridBoard.EnableTreeLines = true;
            gridBoard.EnableGridLines = Gtk.TreeViewGridLines.Both;
            global::Gtk.Fixed.FixedChild PlayChild = ((global::Gtk.Fixed.FixedChild)(this.fixed1[this.buttonPlay]));
            PlayChild.X = (40 * c + 20) / 2 - 25;
            PlayChild.Y = 40 * r + 25;
            global::Gtk.Fixed.FixedChild HelpChild = ((global::Gtk.Fixed.FixedChild)(this.fixed1[this.buttonHelp]));
            HelpChild.X = (40 * c + 20) - 50;
            HelpChild.Y = 40 * r + 25;
            global::Gtk.Fixed.FixedChild SolveChild = ((global::Gtk.Fixed.FixedChild)(this.fixed1[this.buttonSolve]));
            SolveChild.X = 0;
            SolveChild.Y = 40 * r + 25;
            gridBoard.AddEvents((int)EventMask.ButtonPressMask);
            gridBoard.ButtonPressEvent += new ButtonPressEventHandler(SpaceClick);
        }

        /// <summary>
        /// Sets the grid mode and unselects all rows.
        /// </summary>
        public void SetGrid()
        {
            gridBoard.Selection.UnselectAll();
            gridBoard.Selection.Mode = SelectionMode.None;
        }

        #region Buttons

        /// <summary>
        /// Grid click event. Handles all relevant clicks.
        /// </summary>
        /// <param name="obj">Object.</param>
        /// <param name="args">Mouse event arguments.</param>
        [GLib.ConnectBefore]
        protected void SpaceClick(object obj, ButtonPressEventArgs args)
        {
            /* A TreeViews ListStore structure is accessed using an iterator structure. 
             * This function grabs the current cell using X/Y coords of mouse then 
             * dividing by the cell width and height. The row is used to grab
             * the correct ListStore structure, then column is used to grab the
             * specific cell in the list.
             */
            TreeIter iter;
            int column = (int)args.Event.X / CELL_WIDTH;
            int row = (int)args.Event.Y / CELL_HEIGHT;

            /* Due to algorithmic generation of board, the rightmost and bottommost cells
             * may be a few pixels bigger, so implement bounds checking. 
             */
            if (column >= Cols)
            {
                column = Cols - 1;
            }
            if (row >= Rows)
            {
                row = Rows - 1;
            }
            board.IterNthChild(out iter, row);
            //If left click and in build mode.
            if (args.Event.Type == EventType.ButtonPress && args.Event.Button == 1 && mode == 0)
            {
                GameBoard.Spaces curValue = MainWindow.board.board[row, column];
                switch (curValue)
                {
                    case GameBoard.Spaces.White:
                        MainWindow.board.board[row, column] = GameBoard.Spaces.Black;
                        board.SetValue(iter, column, new Pixbuf(libPath + @"BlackSpace.png"));
                        break;
                    case GameBoard.Spaces.Black:
                        MainWindow.board.board[row, column] = GameBoard.Spaces.Zero;
                        board.SetValue(iter, column, new Pixbuf(libPath + @"ZeroSpace.png"));
                        break;
                    case GameBoard.Spaces.Zero:
                        MainWindow.board.board[row, column] = GameBoard.Spaces.One;
                        board.SetValue(iter, column, new Pixbuf(libPath + @"OneSpace.png"));
                        break;
                    case GameBoard.Spaces.One:
                        MainWindow.board.board[row, column] = GameBoard.Spaces.Two;
                        board.SetValue(iter, column, new Pixbuf(libPath + @"TwoSpace.png"));
                        break;
                    case GameBoard.Spaces.Two:
                        MainWindow.board.board[row, column] = GameBoard.Spaces.Three;
                        board.SetValue(iter, column, new Pixbuf(libPath + @"ThreeSpace.png"));
                        break;
                    case GameBoard.Spaces.Three:
                        MainWindow.board.board[row, column] = GameBoard.Spaces.Four;
                        board.SetValue(iter, column, new Pixbuf(libPath + @"FourSpace.png"));
                        break;
                    case GameBoard.Spaces.Four:
                        MainWindow.board.board[row, column] = GameBoard.Spaces.White;
                        board.SetValue(iter, column, new Pixbuf(libPath + @"WhiteSpace.png"));
                        break;
                }
            }
            //if right click in build mode
            else if (args.Event.Type == EventType.ButtonPress && args.Event.Button == 3 && mode == 0)
            {
                MainWindow.board.board[row, column] = GameBoard.Spaces.White;
                board.SetValue(iter, column, new Pixbuf(libPath + @"WhiteSpace.png"));
            }
            //if left click in play mode
            else if (args.Event.Type == EventType.ButtonPress && args.Event.Button == 1 && mode == 1)
            {
                GameBoard.Spaces curValue = MainWindow.board.board[row, column];
                if (curValue == GameBoard.Spaces.White)
                {
                    MainWindow.board.board[row, column] = GameBoard.Spaces.Lamp;
                    board.SetValue(iter, column, new Pixbuf(libPath + @"LampSpace.png"));
                }
                else if (curValue == GameBoard.Spaces.Lamp)
                {
                    MainWindow.board.board[row, column] = GameBoard.Spaces.White;
                    board.SetValue(iter, column, new Pixbuf(libPath + @"WhiteSpace.png"));
                }
                if (ASPHelper.Lit())
                {
                    Light();
                }
            }
            //if right click in play mode
            else if (args.Event.Type == EventType.ButtonPress && args.Event.Button == 3 && mode == 1)
            {
                GameBoard.Spaces curValue = MainWindow.board.board[row, column];
                if (curValue == GameBoard.Spaces.Lamp)
                {
                    MainWindow.board.board[row, column] = GameBoard.Spaces.White;
                    board.SetValue(iter, column, new Pixbuf(libPath + @"WhiteSpace.png"));
                }
                if (ASPHelper.Lit())
                {
                    Light();
                }
            }
        }

        /// <summary>
        /// Raises the button solve clicked event.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        protected void OnButtonSolveClicked(object sender, EventArgs e)
        {
            ClearLamps();
            Solve();
        }

        /// <summary>
        /// Raises the button help clicked event.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        protected void OnButtonHelpClicked(object sender, EventArgs e)
        {
            HelpDialog help = new HelpDialog();
            help.Show();
        }

        /// <summary>
        /// Raises the button play clicked event. Checks to make sure the game is solveable.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        protected void OnButtonPlayClicked(object sender, EventArgs e)
        {
            if (mode == 0)
            {
                if (ASPHelper.Solve())
                {
                    mode = 1;
                    buttonPlay.Label = "Check!";
                    buttonSolve.Visible = true;
                }
                else
                {
                    ErrorDialog error = new ErrorDialog(0);
                    error.Show();
                }
            }
            else
            {
                if (ASPHelper.Check())
                {
                    VictoryDialog victory = new VictoryDialog(this);
                    victory.Show();
                }
                else
                {
                    ErrorDialog error = new ErrorDialog(1);
                    error.Show();
                }
            }
        }

        #endregion

        #region Board Manipulation

        /// <summary>
        /// Solve the current grid.
        /// </summary>
        void Solve()
        {
            StringBuilder sb = new StringBuilder();
            using (StreamReader sr = new StreamReader(path + @"akari.sol"))
            {
                String line;
                while ((line = sr.ReadLine()) != null)
                {
                    sb.AppendLine(line);
                }
            }
            string[] allines = sb.ToString().Split(new char[] { '\n', ' ' });
            List<string> lamps = new List<string>();
            foreach (var str in allines)
            {
                if (str.Contains("lamp"))
                {
                    lamps.Add(str);
                }
            }
            ParseLamps(lamps);
        }

        /// <summary>
        /// Parses the lamps by using primitive bounds checking, string splitting, and catching
        /// failed integer conversions.
        /// </summary>
        /// <param name="lamps">Lamps.</param>
        void ParseLamps(object lamps)
        {
            List<string> lampList = new List<string>((IEnumerable<string>)lamps);
            string[] space;
            foreach (var str in lampList)
            {
                int x = -1;
                int y = -1;
                space = str.Split(new char[] { '(', ',', ')' });
                for (int i = 0; i < space.Length; i++)
                {
                    try
                    {
                        if (x < 0)
                            x = Convert.ToInt32(space[i]);
                        else if (y < 0)
                            y = Convert.ToInt32(space[i]);
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
                if (x >= 0 && y >= 0)
                    PlaceLamp(x, y);
            }
            LightWhites();
        }

        /// <summary>
        /// Places the lamp.
        /// </summary>
        /// <param name="row">Row.</param>
        /// <param name="column">Column.</param>
        void PlaceLamp(int row, int column)
        {
            TreeIter iter;
            board.IterNthChild(out iter, row);
            MainWindow.board.board[row, column] = GameBoard.Spaces.Lamp;
            board.SetValue(iter, column, new Pixbuf(libPath + @"LampSpace.png"));
        }

        /// <summary>
        /// Places lit spaces.
        /// </summary>
        /// <param name="row">Row.</param>
        /// <param name="column">Column.</param>
        void PlaceLight(int row, int column)
        {
            TreeIter iter;
            board.IterNthChild(out iter, row);
            MainWindow.board.board[row, column] = GameBoard.Spaces.Lit;
            board.SetValue(iter, column, new Pixbuf(libPath + @"LitSpace.png"));
        }

        /// <summary>
        /// Places white spaces.
        /// </summary>
        /// <param name="row">Row.</param>
        /// <param name="column">Column.</param>
        void PlaceWhite(int row, int column)
        {
            TreeIter iter;
            board.IterNthChild(out iter, row);
            MainWindow.board.board[row, column] = GameBoard.Spaces.White;
            board.SetValue(iter, column, new Pixbuf(libPath + @"WhiteSpace.png"));
        }

        /// <summary>
        /// Light the board according to current state.
        /// </summary>
        void Light()
        {
            StringBuilder sb = new StringBuilder();
            using (StreamReader sr = new StreamReader(path + @"akarilit.sol"))
            {
                String line;
                while ((line = sr.ReadLine()) != null)
                {
                    sb.AppendLine(line);
                }
            }
            string[] allines = sb.ToString().Split(new char[] { '\n', ' ' });
            List<string> lights = new List<string>();
            List<string> whites = new List<string>();
            foreach (var str in allines)
            {
                if (str.Contains("lit") && str[0] != '-')
                {
                    lights.Add(str);
                }
                else if (str.Contains("lit"))
                {
                    whites.Add(str);
                }
            }
            ParseLights(lights);
            ParseWhites(whites);
        }

        /// <summary>
        /// Parses the lights by using primitive bounds checking, string splitting, and catching
        /// failed integer conversions.
        /// </summary>
        /// <param name="lights">Lights.</param>
        void ParseLights(List<string> lights)
        {
            List<string> lightList = new List<string>((IEnumerable<string>)lights);
            string[] space;
            foreach (var str in lightList)
            {
                int x = -1;
                int y = -1;
                space = str.Split(new char[] { '(', ',', ')' });
                for (int i = 0; i < space.Length; i++)
                {
                    try
                    {
                        if (x < 0)
                            x = Convert.ToInt32(space[i]);
                        else if (y < 0)
                            y = Convert.ToInt32(space[i]);
                    }
                    catch (Exception)
                    {
                        //Console.WriteLine(e.ToString());
                        continue;
                    }
                }
                if (x >= 0 && y >= 0)
                    PlaceLight(x, y);
            }
        }

        /// <summary>
        /// Parses the whites by using primitive bounds checking, string splitting, and catching
        /// failed integer conversions.
        /// </summary>
        /// <param name="whites">Whites.</param>
        void ParseWhites(List<string> whites)
        {
            List<string> whiteList = new List<string>((IEnumerable<string>)whites);
            string[] space;
            foreach (var str in whiteList)
            {
                int x = -1;
                int y = -1;
                space = str.Split(new char[] { '(', ',', ')' });
                for (int i = 0; i < space.Length; i++)
                {
                    try
                    {
                        if (x < 0)
                            x = Convert.ToInt32(space[i]);
                        else if (y < 0)
                            y = Convert.ToInt32(space[i]);
                    }
                    catch (Exception)
                    {
                        //Console.WriteLine(e.ToString());
                        continue;
                    }
                }
                if (x >= 0 && y >= 0)
                    PlaceWhite(x, y);
            }
        }

        /// <summary>
        /// Lights all whites. Only used by Solve().
        /// </summary>
        void LightWhites()
        {
            int row = MainWindow.board.board.GetLength(0);
            int column = MainWindow.board.board.GetLength(1);
            GameBoard.Spaces curValue;
            TreeIter iter;
            board.GetIterFirst(out iter);
            for (int x = 0; x < row; x++)
            {
                for (int y = 0; y < column; y++)
                {
                    curValue = MainWindow.board.board[x, y];
                    if (curValue == GameBoard.Spaces.White)
                    {
                        MainWindow.board.board[x, y] = GameBoard.Spaces.Lit;
                        board.SetValue(iter, y, new Pixbuf(libPath + @"LitSpace.png"));
                    }
                }
                board.IterNext(ref iter);
            }
        }

        

        /// <summary>
        /// Clears the lamps.
        /// </summary>
        void ClearLamps()
        {
            int row = MainWindow.board.board.GetLength(0);
            int column = MainWindow.board.board.GetLength(1);
            GameBoard.Spaces curValue;
            TreeIter iter;
            board.GetIterFirst(out iter);
            for (int x = 0; x < row; x++)
            {
                for (int y = 0; y < column; y++)
                {
                    curValue = MainWindow.board.board[x, y];
                    if (curValue == GameBoard.Spaces.Lamp)
                    {
                        MainWindow.board.board[x, y] = GameBoard.Spaces.White;
                        board.SetValue(iter, y, new Pixbuf(libPath + @"WhiteSpace.png"));
                    }
                }
                board.IterNext(ref iter);
            }
        }

        #endregion

    }
}

