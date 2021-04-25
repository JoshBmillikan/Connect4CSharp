using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Connect4CSharp.game;
using Board = Connect4CSharp.game.Board;

namespace Connect4CSharp {
    public partial class GameForm : Form {
        private readonly Board _gameBoard;
        private readonly Pen _pen = new Pen(Color.FromArgb(255, 0, 0, 0));

        public GameForm() {
            InitializeComponent();
            var players = GetPlayerNames();
            _gameBoard = new Board(players[0], players[1]);
            BoardPanel.Paint += Draw;
            BoardPanel.MouseClick += OnClick;
        }

        private void Draw(object _, PaintEventArgs args) {
            var graphics = args.Graphics;
            var xOffset = BoardPanel.Width / Board.Columns;
            var yOffset = BoardPanel.Height / Board.Rows;
            
            for (var x = 0; x < BoardPanel.Width; x += xOffset) {
                var p1 = new Point(x, 0);
                var p2 = new Point(x, BoardPanel.Height);
                graphics.DrawLine(_pen, p1,p2);
            }
            
            for (var y = 0; y < BoardPanel.Height; y += yOffset) {
                var p1 = new Point(0, y);
                var p2 = new Point(BoardPanel.Width,y);
                graphics.DrawLine(_pen, p1,p2);
            }
            
            for (var i = 0; i < Board.Columns; i++) {
                for (var j = 0; j < Board.Rows; j++) {
                    var cell = _gameBoard.GameBoard[i, j];
                    var xPos = Lerp(xOffset * i,xOffset * (i+1),0.25f);
                    var yPos = Lerp(yOffset * j,yOffset * (j+1),0.25f);
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

            CurrentPlayer.Text = $@"Current player: {_gameBoard.CurrentPlayer.Name}";
        }

        private void OnClick(object sender, MouseEventArgs args) {
            var x = args.X;
            var xOffset = BoardPanel.Width / Board.Columns;
            var i = x / xOffset;
            
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

        /**
         * linear interpolation
         * A very useful function for a variety of applications
         * returns value t percent of the way between a and b
         * eg, if a is 1, b is 3, and t is 0.5, it will return 2
         */
        private static float Lerp(float a, float b, float t) {
            return a * (1 - t) + b * t;
        }

        private static Form WinPrompt(string winner) {
            var prompt = new Form() {
                Name = @"Win Prompt",
                Height = 150,
                Width = 500,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterScreen
            };
            var textLabel = new Label() { Left = 50, Top=20, Width = 100, Text= $@"Player {winner} is the Winner!"};
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
        
        private string[] GetPlayerNames() {
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
    }
}