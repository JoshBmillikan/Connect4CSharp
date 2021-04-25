using System;
using System.IO;
using System.Windows.Forms;
using Connect4CSharp.game;
using Board = Connect4CSharp.game.Board;

namespace Connect4CSharp {
    public partial class GameForm : Form {
        private Board gameBoard;

        public GameForm() {
            InitializeComponent();
            var players = GetPlayerNames();
            gameBoard = new Board(players[0], players[1]);
            CreateGrid();
        }

        private void CreateGrid() {
            var grid = new DataGridView {DataSource = gameBoard.GameBoard};
            BoardPanel.Controls.Add(grid);
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