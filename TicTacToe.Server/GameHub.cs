using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;
using TicTacToe.Server.Models;

namespace TicTacToe.Server
{
    public class GameHub : Hub
    {
        // Client joining game
        public async Task FindGame(string username)
        {
            if (GameState.Instance.IsUsernameTaken(username))
            {
                this.Clients.Caller.usernameTaken();
                return;
            }

            Player joiningPlayer = GameState.Instance.CreatePlayer(username, this.Context.ConnectionId);
            this.Clients.Caller.playerJoined(joiningPlayer);
            
            Player opponent = GameState.Instance.GetWaitingOpponent();
            if (opponent == null)
            {
                GameState.Instance.AddToWaitingPool(joiningPlayer);
                this.Clients.Caller.waitingList();
            }
            else
            {
                Game newGame = await GameState.Instance.CreateGame(opponent, joiningPlayer);
                Clients.Group(newGame.Id).start(newGame);
            }
        }

        // Client required to mark on the game. Make move
        public void PlacePiece(int row, int col)
        {
            Player playerMakingTurn = GameState.Instance.GetPlayer(playerId: this.Context.ConnectionId);
            Player opponent;
            Game game = GameState.Instance.GetGame(playerMakingTurn, out opponent);

            if (game == null || !game.WhoseTurn.Equals(playerMakingTurn))
            {
                this.Clients.Caller.notPlayersTurn();
                return;
            }

            if (!game.IsValidMove(row, col))
            {
                this.Clients.Caller.notValidMove();
                return;
            }

            game.PlacePiece(row, col);

            this.Clients.Group(game.Id).piecePlaced(row, col, playerMakingTurn.Piece);

            if (!game.IsOver)
            {
                this.Clients.Group(game.Id).updateTurn(game);
            }
            else
            {
                if (game.IsTie)
                {
                    this.Clients.Group(game.Id).tieGame();
                }
                else
                {
                    this.Clients.Group(game.Id).winner(playerMakingTurn.Name);
                }
                
                GameState.Instance.RemoveGame(game.Id);
            }
        }

        // if one player leaves, game finish
        public override async Task OnDisconnected(bool stopCalled)
        {
            Player leavingPlayer = GameState.Instance.GetPlayer(playerId: this.Context.ConnectionId);

            if (leavingPlayer != null)
            {
                Player opponent;
                Game ongoingGame = GameState.Instance.GetGame(leavingPlayer, out opponent);
                if (ongoingGame != null)
                {
                    this.Clients.Group(ongoingGame.Id).opponentLeft();
                    GameState.Instance.RemoveGame(ongoingGame.Id);
                }
            }
            
            await base.OnDisconnected(stopCalled);
        }
    }
}