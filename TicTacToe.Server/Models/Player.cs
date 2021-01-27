using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToe.Server.Models
{
    // Player
    public class Player
    {
        public Player(string name, string id)
        {
            this.Name = name;
            this.Id = id;
        }

        public string Name { get; private set; }

        public string Id { get; private set; }

        public string GameId { get; set; }

        public string Piece { get; set; }

        public override string ToString()
        {
            return String.Format("(Id={0}, Name={1}, GameId={2}, Piece={3})", 
                this.Id, this.Name, this.GameId, this.Piece);
        }

        public override bool Equals(object obj)
        {
            Player other = obj as Player;

            if (other == null)
            {
                return false;
            }

            return this.Id.Equals(other.Id) && this.Name.Equals(other.Name);
        }
    }
}
