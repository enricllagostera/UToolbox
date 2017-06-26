using UnityEngine;

[System.Serializable]
public struct V2Int {

	public int x, y;

	public V2Int(int x, int y)
	{
		this.x = x;
		this.y = y;
	}

	public Vector2 ToV2() {
		return new Vector2(this.x, this.y);
	}

    public V2Int FromV2(Vector2 v2) {
		return new V2Int((int)v2.x, (int)v2.y);
	}

    public V2Int Clamp(int minX, int maxX, int minY, int maxY){
        V2Int nova = new V2Int(0,0);
        if (x < minX) {
            nova.x = minX;
        }
        if (x > maxX) {
            nova.x = maxX;
        }
        if (y < minY) {
            nova.y = minY;
        }
        if (y > maxY) {
            nova.y = maxY;
        }
        return nova;
    }

    public bool Valido(int minX, int maxX, int minY, int maxY){
        if (x < minX) {
            return false;
        }
        if (x > maxX) {
            return false;
        }
        if (y < minY) {
            return false;
        }
        if (y > maxY) {
            return false;
        }
        return true;
    }

    public override bool Equals(object obj) {
        // Check for null values and compare run-time types.
      if (obj == null || GetType() != obj.GetType()) 
         return false;
      V2Int p = (V2Int)obj;
      return (x == p.x) && (y == p.y);
    }

    public override int GetHashCode() 
    {
        return x ^ y;
    }
}
