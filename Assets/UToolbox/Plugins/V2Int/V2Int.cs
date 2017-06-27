/* Enric Llagostera <http://enric.llagostera.com.br> */

using UnityEngine;

namespace UToolbox.V2IntSystem
{
    public enum EDirection
    {
        N,
        NE,
        E,
        SE,
        S,
        SW,
        W,
        NW
    }


    [System.Serializable]
    public struct V2Int
    {

        public int x, y;

        public V2Int(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public Vector2 ToV2()
        {
            return new Vector2(this.x, this.y);
        }

        public static V2Int FromV2(Vector2 v2)
        {
            return new V2Int((int)v2.x, (int)v2.y);
        }

        public void ClampSelf(int minX, int maxX, int minY, int maxY)
        {
            if (x < minX)
            {
                x = minX;
            }
            if (x > maxX)
            {
                x = maxX;
            }
            if (y < minY)
            {
                y = minY;
            }
            if (y > maxY)
            {
                y = maxY;
            }
        }

        public bool IsValid(int minX, int maxX, int minY, int maxY)
        {
            if (x < minX)
            {
                return false;
            }
            if (x > maxX)
            {
                return false;
            }
            if (y < minY)
            {
                return false;
            }
            if (y > maxY)
            {
                return false;
            }
            return true;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;
            V2Int p = (V2Int)obj;
            return (x == p.x) && (y == p.y);
        }

        public override int GetHashCode()
        {
            return x ^ y;
        }

        public static bool operator ==(V2Int a, V2Int b)
        {
            return a.x == b.x && a.y == b.y;
        }

        public static bool operator !=(V2Int a, V2Int b)
        {
            return !(a == b);
        }

        public static V2Int AtDirection(V2Int position, EDirection direction)
        {
            var newPosition = position;
            switch (direction)
            {
                case EDirection.N: newPosition.y++; break;
                case EDirection.NE: newPosition.y++; newPosition.x++; break;
                case EDirection.E: newPosition.x++; break;
                case EDirection.SE: newPosition.y--; newPosition.x++; break;
                case EDirection.S: newPosition.y--; break;
                case EDirection.SW: newPosition.y--; newPosition.x--; break;
                case EDirection.W: newPosition.x--; break;
                case EDirection.NW: newPosition.y++; newPosition.x--; break;
            }
            return newPosition;
        }
    }
}