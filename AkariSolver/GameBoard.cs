using System;

namespace AkariSolver
{
	/// <summary>
	/// Game board object
	/// </summary>
    public class GameBoard
    {
		/// <summary>
		/// The board.
		/// </summary>
        public Spaces[,] board;

        /// <summary>
        /// Initializes a new instance of the <see cref="AkariSolver.GameBoard"/> class.
        /// </summary>
        /// <param name="r">Rows.</param>
        /// <param name="c">Columns.</param>
        public GameBoard(int r, int c)
        {
            board = new Spaces[r, c];
            for (int x = 0; x < r; x++)
            {
                for (int y = 0; y < c; y++)
                {
                    board[x, y] = Spaces.White;
                }
            }
        }

        public enum Spaces
        {
            White,
            Black,
            Zero,
            One,
            Two,
            Three,
            Four,
            Lit,
            Lamp
        }
    }
}

