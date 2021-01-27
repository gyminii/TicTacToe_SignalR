using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToe.Server.Models
{
    public class Game
    {
        private bool isFirstPlayersTurn;
        // creates game
        public Game(Player player1, Player player2)
        {
            this.Player1 = player1;
            this.Player2 = player2;
            this.Id = Guid.NewGuid().ToString("d");
            this.Board = new Board();

            this.isFirstPlayersTurn = true;

            // Registering plyaer
            this.Player1.GameId = this.Id;
            this.Player2.GameId = this.Id;

            // Assign piece types to each player
            this.Player1.Piece = "X";
            this.Player2.Piece = "O";
        }

        public string Id { get; set; }

        public Player Player1 { get; set; }

        public Player Player2 { get; set; }

        public Board Board { get; set; }
        //Turn
        public Player WhoseTurn
        {
            get
            {
                return (this.isFirstPlayersTurn) ?
                    this.Player1 :
                    this.Player2;
            }
        }

        /// <summary>
        /// Returns whether the game is ongoing or has completed.
        /// Over states include either a tie or a player has won.
        /// </summary>
        public bool IsOver
        {
            get
            {
                return this.IsTie || this.Board.IsThreeInRow;
            }
        }

        // Tie
        public bool IsTie
        {
            get
            {
                return !this.Board.AreSpacesLeft;
            }
        }

        // Piece location
        public void PlacePiece(int row, int col)
        {
            string pieceToPlace = this.isFirstPlayersTurn ?
                this.Player1.Piece :
                this.Player2.Piece;
            this.Board.PlacePiece(row, col, pieceToPlace);
            
            this.isFirstPlayersTurn = !this.isFirstPlayersTurn;
        }

        // Check valid moves in the game.
        public bool IsValidMove(int row, int col)
        {
            // TODO: Make the board dimensions public properties
            return  row < this.Board.Pieces.GetLength(0) &&
                    col < this.Board.Pieces.GetLength(1) &&
                    string.IsNullOrWhiteSpace(this.Board.Pieces[row, col]);
        }
        
        public override string ToString()
        {
            return String.Format("(Id={0}, Player1={1}, Player2={2}, Board={3})",
                this.Id, this.Player1, this.Player2, this.Board);
        }
    }
}

