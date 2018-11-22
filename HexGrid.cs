using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexGrid {
    public Vector2Int gridSize;
    // TODO upgrade grid storage to Hash table
    private Hex[,] hexGrid;
    private Layout gridLayout;

    public HexGrid(Vector2Int gridSize, Layout gridLayout) {
        this.gridSize = gridSize;
        this.hexGrid = new Hex[gridSize.x, gridSize.y];
        this.gridLayout = gridLayout;
    }

    public Hex GetHex(Vector2Int axialCoords) {
        return hexGrid[axialCoords.x, axialCoords.y];
    }

    public Hex GetHexAt(Vector2 worldCoords) {
        FractionalHex fractHex = gridLayout.WorldToHex(worldCoords);
        return GetHex(fractHex.HexRound().axialCoords);
    }

    public Hex GetHexAt(Vector3 worldCoords) {
        return GetHexAt(new Vector2(worldCoords.x, worldCoords.z));
    }

    public List<Hex> GetHexNeighbors(Vector2Int axialCoords) {
	List<Hex> neighbors = new List<Hex>;
	currentHex = GetHex(axialCoords);
        for(int i = 0; i < 6; i++) {
	    Vector3 neighborCoords = currentHex.GetNeighbor(i);
	    if(GetHex(neighborCoords) != null) {
	        neighbors.Add(GetHex(neighborCoords));
	    }
	}
	return neighbors;
    }

    public List<Hex> GetHexNeighbors(Vector3 worldCoords) {
	return GetHexNeighbors(new Vector2Int(worldCoords.x, 0, worldCoords.z));
    }

    public List<Hex> GetHexNeighbors(Hex currentHex) {
	return GetHexNeighbors(currentHex.axialCoords);
    }

    public void SetHex(Vector2Int axialCoords, Hex hex) {
        this.hexGrid[axialCoords.x, axialCoords.y] = hex;
    }

    public void SetHexAt(Vector2 worldCoords, Hex hex) {
        FractionalHex fractHex = gridLayout.WorldToHex(worldCoords);
        SetHex(fractHex.HexRound().axialCoords, hex);
    }

    public void SetHexAt(Vector3 worldCoords, Hex hex) {
        SetHexAt(new Vector2(worldCoords.x, worldCoords.z), hex);
    }

    public Vector2Int WorldToHex(Vector2 worldCoords) {
        return this.gridLayout.WorldToHex(worldCoords).HexRound().axialCoords;
    }

    public Vector2Int WorldToHex(Vector3 worldCoords) {
        return WorldToHex(new Vector2(worldCoords.x, worldCoords.z));
    }

    public Vector2 HexToWorld(Vector2Int hexCoords) {
        return this.gridLayout.HexToWorld(GetHex(hexCoords));
    }

    public Vector2 HexToWorld(Hex hex) {
        return this.gridLayout.HexToWorld(hex);
    }
}


public class Hex {
    // vectorized cube constructor
    public Hex(Vector3Int coords) {
        this.coords = coords;
        //this.hexTile = new GameObject("Blank Tile (" + coords.x + ", " + coords.z +")");
        if (coords.x + coords.y + coords.z != 0) throw new ArgumentException("x + y + z must be 0");
    }

    // vectorized axial constructor
    public Hex(Vector2Int axialCoords) : this(new Vector3Int(axialCoords.x, -axialCoords.x - axialCoords.y, axialCoords.y)) {
    }

    // cube constructor
    public Hex(int x, int y, int z) : this(new Vector3Int(x, y, z)) {
    }

    // axial constructor
    public Hex(int q, int r) : this(new Vector3Int(q, -q -r, r)) {
    }

    
    public readonly Vector3Int coords;
    public Vector2Int axialCoords {
        get {
            return new Vector2Int(this.coords.x, this.coords.z);
        }
    }
    private GameObject hexTile;

    public void SetHexTile(GameObject newTile) {
        UnityEngine.Object.Destroy(hexTile);
        this.hexTile = newTile;
    }

    public GameObject GetHexTile() {
        return this.hexTile;
    }

    public override bool Equals(object obj) {
        return this.Equals(obj);
    }

    public bool Equals(Hex obj) {
            return this.coords == obj.coords;
    }

    public override int GetHashCode() {
        return (coords.x * 0x100000) + (coords.y * 0x1000) + coords.z;
    }

    public static bool operator ==(Hex h1, Hex h2) {
        return h1.Equals(h2);
    }

    public static bool operator !=(Hex h1, Hex h2) {
        return !h1.Equals(h2);
    }

    public Hex Add(Hex b) {
        return new Hex(coords + b.coords);
    }

    public Hex Subtract(Hex b) {
        return new Hex(coords - b.coords);
    }

    public Hex Scale(int k) {
        return new Hex(coords.x * k, coords.y * k, coords.z * k);
    }

    public int Length() {
        return (int)((Math.Abs(coords.x) + Math.Abs(coords.y) + Math.Abs(coords.z)) / 2);
    }

    public int Distance(Hex b) {
        return Subtract(b).Length();
    }

    static public List<Hex> directions = new List<Hex>{new Hex(1, 0, -1), new Hex(1, -1, 0), new Hex(0, -1, 1), new Hex(-1, 0, 1), new Hex(-1, 1, 0), new Hex(0, 1, -1)};

    static public Hex Direction(int direction) {
        return Hex.directions[direction];
    }


    public Hex Neighbor(int direction) {
        return Add(Hex.Direction(direction));
    }
    
    static public List<Vector3Int> directionOffsets = new List<Vector3Int>(new Vector3Int(1, 0, -1), new Vector3Int(1, -1, 0), new Vector3Int(0, -1, 1), new Vector3Int(-1, 0, 1), new Vector3Int(-1, 1, 0), new Vector3Int(0, 1, -1));

    static public Vector3 GetDirectionOffset(int direction) {
        return Hex.directionOffsets[direction];
    }

    public Vector3 GetNeighborCoords(int direction) {
        return this.coords + Hex.DirectionOffset(direction);
    }
}

public struct Orientation {
    public Orientation(double f0, double f1, double f2, double f3, double b0, double b1, double b2, double b3, double start_angle) {
        this.f0 = f0;
        this.f1 = f1;
        this.f2 = f2;
        this.f3 = f3;
        this.b0 = b0;
        this.b1 = b1;
        this.b2 = b2;
        this.b3 = b3;
        this.start_angle = start_angle;
    }
    public readonly double f0;
    public readonly double f1;
    public readonly double f2;
    public readonly double f3;
    public readonly double b0;
    public readonly double b1;
    public readonly double b2;
    public readonly double b3;
    public readonly double start_angle;

    static public readonly Orientation pointy = new Orientation(Math.Sqrt(3.0), Math.Sqrt(3.0) / 2.0, 0.0, 3.0 / 2.0, Math.Sqrt(3.0) / 3.0, -1.0 / 3.0, 0.0, 2.0 / 3.0, 0.5);
    static public readonly Orientation flat = new Orientation(3.0 / 2.0, 0.0, Math.Sqrt(3.0) / 2.0, Math.Sqrt(3.0), 2.0 / 3.0, 0.0, -1.0 / 3.0, Math.Sqrt(3.0) / 3.0, 0.0);
}

public struct Layout {
    public Layout(Orientation orientation, Vector2 size, Vector2 origin) {
        this.orientation = orientation;
        this.size = size;
        this.origin = origin;
    }
    public readonly Orientation orientation;
    public readonly Vector2 size;
    public readonly Vector2 origin;

    public Vector2 HexToWorld(Hex h) {
        Orientation M = orientation;
        float x = (float)(M.f0 * h.coords.x + M.f1 * h.coords.z) * size.x;
        float y = (float)(M.f2 * h.coords.x + M.f3 * h.coords.z) * size.y;
        return new Vector2(x + origin.x, y + origin.y);
    }


    public FractionalHex WorldToHex(Vector2 p) {
        Orientation M = orientation;
        Vector2 pt = new Vector2((p.x - origin.x) / size.x, (p.y - origin.y) / size.y);
        float q = (float)(M.b0 * pt.x + M.b1 * pt.y);
        float r = (float)(M.b2 * pt.x + M.b3 * pt.y);
        return new FractionalHex(q, -q - r, r);
    }


    public Vector2 HexCornerOffset(int corner) {
        Orientation M = orientation;
        float angle = (float)(2.0 * Math.PI * (M.start_angle - corner) / 6.0);
        return new Vector2((float)(size.x * Math.Cos(angle)), (float)(size.y * Math.Sin(angle)));
    }


    public List<Vector2> PolygonCorners(Hex h) {
        List<Vector2> corners = new List<Vector2> { };
        Vector2 center = HexToWorld(h);
        for (int i = 0; i < 6; i++) {
            Vector2 offset = HexCornerOffset(i);
            corners.Add(new Vector2(center.x + offset.x, center.y + offset.y));
        }
        return corners;
    }

}

public struct FractionalHex {
    public FractionalHex(double x, double y, double z) {
        this.x = x;
        this.y = y;
        this.z = z;
        if (Math.Round(x + y + z) != 0) throw new ArgumentException("x + y + z must be 0");
    }
    public readonly double x;
    public readonly double y;
    public readonly double z;

    public Hex HexRound() {
        int xi = (int)(Math.Round(x));
        int yi = (int)(Math.Round(y));
        int zi = (int)(Math.Round(z));
        double x_diff = Math.Abs(xi - x);
        double r_diff = Math.Abs(yi - y);
        double s_diff = Math.Abs(zi - z);
        if (x_diff > r_diff && x_diff > s_diff) {
            xi = -yi - zi;
        }
        else
            if (r_diff > s_diff) {
            yi = -xi - zi;
        }
        else {
            zi = -xi - yi;
        }
        return new Hex(xi, yi, zi);
    }


    public FractionalHex HexLerp(FractionalHex b, double t) {
        return new FractionalHex(x * (1.0 - t) + b.x * t, y * (1.0 - t) + b.y * t, z * (1.0 - t) + b.z * t);
    }


    static public List<Hex> HexLineDraw(Hex a, Hex b) {
        int N = a.Distance(b);
        FractionalHex a_nudge = new FractionalHex(a.coords.x + 0.000001, a.coords.y + 0.000001, a.coords.z - 0.000002);
        FractionalHex b_nudge = new FractionalHex(b.coords.x + 0.000001, b.coords.y + 0.000001, b.coords.z - 0.000002);
        List<Hex> results = new List<Hex> { };
        double step = 1.0 / Math.Max(N, 1);
        for (int i = 0; i <= N; i++) {
            results.Add(a_nudge.HexLerp(b_nudge, step * i).HexRound());
        }
        return results;
    }

}
