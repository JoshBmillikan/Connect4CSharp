using System;

namespace Connect4CSharp.game {
    
    /**
     * The game board class.
     * Responsible for managing the state of the game
     */
    [Serializable]
    public class Board {
        public const int Columns = 7, Rows = 6;
        private readonly Player[] _players = new Player[2];
        private uint _turnCount = 0;
        public PlayerColor[,] GameBoard { get; private set; }
        private bool _win;
        
        // Property for if the game is won
        public bool Win {
            get => _win || (_win = CheckColumns() || CheckRows() || CheckDiagonal());
            private set => _win = value;
        }

        // Current player is also accessed through a property that hides the access of the players array
        public Player CurrentPlayer {
            get => _players[_turnCount % 2];
            private set => _players[_turnCount % 2] = value;
        }

        public Board(string player1, string player2) {
            _players[0] = new Player(player1, PlayerColor.Yellow);
            _players[1] = new Player(player2, PlayerColor.Red);
            Win = false;
            GameBoard = new PlayerColor[Columns, Rows];
            for (var i = 0; i < Columns; i++) {
                for (var j = 0; j < Rows; j++)
                    GameBoard[i, j] = PlayerColor.None;
            }
        }

        /**
         * Insert into a column. Throws an exception if
         * the column is already full
         */
        public void Insert(int column) {
            for (var i = Rows - 1; i >= 0; i--) {
                if (GameBoard[column, i] == PlayerColor.None) {
                    GameBoard[column, i] = CurrentPlayer.Color;
                    return;
                }
            }
            
            throw new ArgumentOutOfRangeException();
        }

        /**
         * increments the turn count
         */
        public void NextTurn() {
            _turnCount++;
        }

        /**
         * Resets the game board
         */
        public void Reset() {
            Win = false;
            _turnCount = 0;
            for (var i = 0; i < Columns; i++) {
                for (var j = 0; j < Rows; j++)
                    GameBoard[i, j] = PlayerColor.None;
            }
        }

        /**
         * Checks for 4 in a row along the diagonals
         */
        private bool CheckDiagonal() {
            for (var i = 0; i < Columns - 3; i++) {
                for (var j = 0; j < Rows - 3; j++) {
                    var val = GameBoard[i, j];
                    if (val != PlayerColor.None && val == GameBoard[i + 1, j + 1] && val == GameBoard[i + 2, j + 2] &&
                        val == GameBoard[i + 3, j + 3])
                        return true;
                }
            }

            for (var i = 0; i < Columns - 3; i++) {
                for (var j = Rows-1; j > 3; j--) {
                    var val = GameBoard[i, j];
                    if (val != PlayerColor.None && val == GameBoard[i + 1, j - 1] && val == GameBoard[i + 2, j - 2] &&
                        val == GameBoard[i + 3, j - 3])
                        return true;
                }
            }

            return false;
        }

        /**
         * Checks for 4 in a row along the columns
         */
        private bool CheckColumns() {
            for (var i = 0; i < Columns; i++) {
                for (var j = 0; j < Rows - 3; j++) {
                    var val = GameBoard[i, j];
                    if (val != PlayerColor.None && val == GameBoard[i, j + 1] && val == GameBoard[i, j + 2] &&
                        val == GameBoard[i, j + 3])
                        return true;
                }
            }

            return false;
        }

        
        /**
         * Checks for 4 in a row along rows
         */
        private bool CheckRows() {
            for (var i = 0; i < Columns - 3; i++) {
                for (var j = 0; j < Rows; j++) {
                    var val = GameBoard[i, j];
                    if (val != PlayerColor.None && val == GameBoard[i + 1, j] && val == GameBoard[i + 2, j] &&
                        val == GameBoard[i + 3, j])
                        return true;
                }
            }

            return false;
        }
    }
}