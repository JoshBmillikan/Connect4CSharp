using System;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using Connect4CSharp.game;
using Board = Connect4CSharp.game.Board;

namespace Connect4CSharp {
    public partial class GameForm : Form {
        private Board _gameBoard;
        private readonly Pen _pen = new(Color.FromArgb(255, 0, 0, 0));

        public GameForm() {
            InitializeComponent();
            var players = GetPlayerNames();
            _gameBoard = new Board(players[0], players[1]);
            BoardPanel.Paint += Draw;
            BoardPanel.MouseClick += OnClick;
        }

        /**
         * Performs the drawing of the game board.
         * also updates the current player text
         */
        private void Draw(object sender, PaintEventArgs args) {
            var graphics = args.Graphics;
            var xOffset = BoardPanel.Width / Board.Columns;
            var yOffset = BoardPanel.Height / Board.Rows;
            
            // Draw vertical lines
            for (var x = 0; x < BoardPanel.Width; x += xOffset) {
                var p1 = new Point(x, 0);
                var p2 = new Point(x, BoardPanel.Height);
                graphics.DrawLine(_pen, p1,p2);
            }
            
            // Draw horizontal lines
            for (var y = 0; y < BoardPanel.Height; y += yOffset) {
                var p1 = new Point(0, y);
                var p2 = new Point(BoardPanel.Width,y);
                graphics.DrawLine(_pen, p1,p2);
            }
            
            // Draw ellipses for all the cells that are filled in
            for (var i = 0; i < Board.Columns; i++) {
                for (var j = 0; j < Board.Rows; j++) {
                    var cell = _gameBoard.GameBoard[i, j];
                    
                    // ellipse coordinates start at the upper left of it, this ensures they are centered
                    var xPos = Lerp(xOffset * i,xOffset * (i+1),0.25f);
                    var yPos = Lerp(yOffset * j,yOffset * (j+1),0.25f);
                    
                    // Draw the ellipse of the appropriate color
                    if (cell == PlayerColor.Yellow) {
                        var brush = new SolidBrush(Color.Yellow);
                        graphics.FillEllipse(brush, xPos, yPos, xOffset/2f, yOffset/2f);
                    }
                    else if (cell == PlayerColor.Red) {
                        var brush = new SolidBrush(Color.Red);
                        graphics.FillEllipse(brush,xPos,yPos,xOffset/2f,yOffset/2f);
                    }
                }
            }

            // Update current player name
            CurrentPlayer.ForeColor = _gameBoard.CurrentPlayer.Color == PlayerColor.Red ? Color.Red : Color.Yellow;
            CurrentPlayer.Text = $@"Current player: {_gameBoard.CurrentPlayer.Name}";
        }

        /**
         * Processes the player input any time they click somewhere on the board
         */
        private void OnClick(object sender, MouseEventArgs args) {
            var x = args.X;
            var xOffset = BoardPanel.Width / Board.Columns;
            
            // Get the index of the corresponding column to where the user clicked
            var i = x / xOffset;

            // Try to insert into that column
            try {
                _gameBoard.Insert(i);
                if (_gameBoard.Win) {
                    var prompt = WinPrompt(_gameBoard.CurrentPlayer.ToString());
                    if (prompt.ShowDialog() == DialogResult.Yes)
                        _gameBoard.Reset();
                    else Close();
                }
                else _gameBoard.NextTurn();

                BoardPanel.Refresh();
            }
            
            // If the selected column is full, display a message to the user
            catch (ArgumentOutOfRangeException) {
                MessageBox.Show(@"That column is full, please select an different one", @"Invalid column",
                    MessageBoxButtons.OK);
            }
        }

        /**
         * linear interpolation,
         * A very useful function for a variety of applications.
         * It returns value t percent of the way between a and b.
         * eg, if a is 1, b is 3, and t is 0.5, it will return 2
         */
        private static float Lerp(float a, float b, float t) {
            return a * (1 - t) + b * t;
        }

        /**
         * Helper function to create the win prompt displayed after someone has won the game
         */
        private static Form WinPrompt(string winner) {
            var prompt = new Form() {
                Name = @"Win Prompt",
                Height = 150,
                Width = 500,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterScreen
            };
            var textLabel = new Label() { Left = 50, Top=20, Width = 200, Text= $@"{winner} is the Winner!"};
            prompt.Controls.Add(textLabel);
            
            var yes = new Button() {
                DialogResult = DialogResult.Yes, Left=150, Width=100, Top=120,
                Text = @"Yes",
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right
            };   
            
            var no = new Button() {
                DialogResult = DialogResult.No, Left=350, Width=100, Top=120,
                Text = @"No",
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right
            };
            prompt.Controls.Add(yes);
            prompt.Controls.Add(no);
            yes.Click += (_, _) => { prompt.Close(); };
            no.Click += (_, _) => { prompt.Close(); };
            prompt.AcceptButton = yes;
            return prompt;
        }
        
        /**
         * Displays a prompt to get the player names
         */
        private static string[] GetPlayerNames() {
            var prompt = new Form() {
                Name = @"Enter Player Names",
                Height = 200,
                Width = 500,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterScreen
            };
            
            var textLabel = new Label() { Left = 50, Top=20, Text=@"Enter player names" };
            prompt.Controls.Add(textLabel);
            var player1 = new TextBox() {Left = 50, Top = 50, Width = 400};
            var player2 = new TextBox() {Left = 50, Top = 80, Width = 400};
            prompt.Controls.Add(player1);
            prompt.Controls.Add(player2);

            var ok = new Button() {
                DialogResult = DialogResult.OK, Left=350, Width=100, Top=120,
                Text = @"Ok",
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right
            };
            
            ok.Click += (_, _) => { prompt.Close(); };
            prompt.Controls.Add(ok);
            prompt.AcceptButton = ok;
            var result = prompt.ShowDialog();
            if (result != DialogResult.OK)
                throw new IOException();
            
            return new[] {player1.Text, player2.Text};
        }

        /**
         * Starts a new game when new game is selected from the menu
         */
        private void newGameToolStripMenuItem_Click(object sender, EventArgs e) {
            try {
                var names = GetPlayerNames();
                _gameBoard = new Board(names[0], names[1]);
                Refresh();
            } catch (IOException) {}
        }

        /**
         * Quits the game
         */
        private void toolStripMenuItem1_Click(object sender, EventArgs e) {
            Close();
        }

        /**
         * Saves the game as a binary file. Originally, I tried to save to an
         * XML file, but the xml serializer doesn't work with multidimensional arrays
         */
        private void saveToolStripMenuItem_Click(object sender, EventArgs e) {
            var dialog = new SaveFileDialog();
            var result = dialog.ShowDialog();
            if (result == DialogResult.OK) {
                var stream = dialog.OpenFile();
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream,_gameBoard);
                stream.Close();
            }
        }

        /**
         * Load the game from a binary file
         */
        private void loadToolStripMenuItem_Click(object sender, EventArgs e) {
            var dialog = new OpenFileDialog();
            var result = dialog.ShowDialog();
            if (result == DialogResult.OK) {
                var stream = dialog.OpenFile();
                var formatter = new BinaryFormatter();
                _gameBoard = formatter.Deserialize(stream) as Board;
                Refresh();
                stream.Close();
            }
        }
    }
}