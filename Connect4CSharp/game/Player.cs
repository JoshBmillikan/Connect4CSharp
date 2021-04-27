using System;

namespace Connect4CSharp.game {
    
    /**
     * A class containing a player name and color,
     * Supports equality comparisons and is serializable
     */
    [Serializable]
    public class Player {

        public PlayerColor Color { get;}
        public string Name { get;}

        public Player(string name, PlayerColor color) {
            Color = color;
            Name = name;
        }

        public static bool operator ==(Player left, Player right) {
            return left is not null && left.Name.Equals(right?.Name) && left.Color == right?.Color;
        }
        
        public static bool operator !=(Player left, Player right) {
            return !(left == right);
        }
        
        protected bool Equals(Player other) {
            return Color == other.Color && Name == other.Name;
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == this.GetType() && Equals((Player) obj);
        }
        
        public override int GetHashCode() {
            unchecked {
                return ((int) Color * 397) ^ (Name != null ? Name.GetHashCode() : 0);
            }
        }

        public override string ToString() {
            return Name;
        }
    }
    [Serializable]
    public enum PlayerColor : byte {
        None,
        Yellow,
        Red
    }
}