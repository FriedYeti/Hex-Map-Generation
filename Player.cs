using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    public new Camera camera;
    private HexGrid map;

    public LayerMask tileLayerMask;
    public LayerMask unitLayerMask;

    private Hex currentTile; 

    public Unit selectedUnit = new Unit();

	void Start () {
        camera = Camera.main;

        selectedUnit.unitMesh = gameObject;
        selectedUnit.movementLimit = 5;

        GameObject mapGenObject = GameObject.FindGameObjectWithTag("MapGenerator");
        MapGenerator mapGen = mapGenObject.GetComponent<MapGenerator>();
        map = mapGen.map;
	}
	
	void Update () {
        RaycastHit hit;
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, camera.farClipPlane, unitLayerMask)) {
            Debug.DrawLine(new Vector3(9, 10, 5), hit.transform.position, Color.blue);            
        }
        else if (Physics.Raycast(ray, out hit, camera.farClipPlane, tileLayerMask)) {
            Transform objectHit = hit.transform;
            Debug.DrawLine(new Vector3(9,10,5), hit.transform.position + hit.transform.GetComponent<TileInfo>().unitOffset, Color.red);

            if(Input.GetMouseButtonDown(0)) {
                DrawLineBetweenHexes(currentTile, map.GetHexAt(hit.transform.position));

            }
        }
        Debug.DrawRay(gameObject.transform.position, Vector3.up * 1, Color.green);

        SetCurrentTile();
	}

    private void SetCurrentTile() {
        RaycastHit hit;
        Ray ray = new Ray(gameObject.transform.position, Vector3.down);
        
        if (Physics.Raycast(ray, out hit, camera.farClipPlane, tileLayerMask)) {
            currentTile = map.GetHexAt(hit.transform.position);
            gameObject.transform.position = hit.transform.position + hit.transform.GetComponent<TileInfo>().unitOffset;
        }
        
    }

    private void SelectUnit(GameObject newUnit) {
        this.selectedUnit.unitMesh = newUnit;
    }

    public Vector3 CubeLerp(Vector3 a, Vector3 b, float t) {
        return new Vector3(Mathf.Lerp(a.x, b.x, t), Mathf.Lerp(a.y, b.y, t), Mathf.Lerp(a.z, b.z, t));
    }

    public void DrawLineBetweenHexes(Hex a, Hex b) {
        Debug.DrawLine(new Vector3(map.HexToWorld(a).x,0,map.HexToWorld(a).y) + new Vector3(0, 0.5f, 0), new Vector3(map.HexToWorld(b).x, 0, map.HexToWorld(b).y) + new Vector3(0, 0.5f, 0), Color.cyan, 5f);

        int distance = a.Distance(b) + 1;
        List<Vector3> lerpPoints = new List<Vector3>();

        for (int i = 0; i <= distance; i++) {
            Vector3 posA = new Vector3(map.HexToWorld(a).x, 0, map.HexToWorld(a).y);
            Vector3 posB = new Vector3(map.HexToWorld(b).x, 0, map.HexToWorld(b).y);
            lerpPoints.Add(CubeLerp(posA, posB, (float)i / distance));
        }

        int rayLength = 2;

        for (int i = 0; i < (lerpPoints.Count - 1); i++) {
            Hex first = map.GetHexAt(lerpPoints[i]);
            Vector2 firstPos = map.HexToWorld(first);
            Hex second = map.GetHexAt(lerpPoints[i + 1]);
            Vector2 secondPos = map.HexToWorld(second);
            Debug.DrawRay(new Vector3(firstPos.x, 0, firstPos.y), Vector3.up * rayLength, Color.red, 5f);
            Debug.DrawLine(new Vector3(firstPos.x, 1f, firstPos.y), new Vector3(secondPos.x, 1f, secondPos.y), Color.green, 5f);
            rayLength++;
        }
    }

}


public class Unit {
    public GameObject unitMesh;
    public int movementLimit;
}
