using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    public new Camera camera;

    // TODO Store ref to map instead of MapGenerator
    public MapGenerator mapRef;

    public LayerMask tileLayerMask;
    public LayerMask unitLayerMask;

    private Hex currentTile;

    

    public Unit selectedUnit;

	void Start () {
        camera = Camera.main;
        selectedUnit.unitMesh = gameObject;
        selectedUnit.movementLimit = 5;
	}
	
	void Update () {
        RaycastHit hit;
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, camera.farClipPlane, unitLayerMask)) {
            Debug.DrawLine(new Vector3(9, 10, 5), hit.transform.position, Color.blue);
            //if(Input.GetMouseButtonDown(0)) {
                //SelectUnit(hit.transform.GetComponentInParent<GameObject>());
            //}
            
        }
        else if (Physics.Raycast(ray, out hit, camera.farClipPlane, tileLayerMask)) {
            Transform objectHit = hit.transform;
            Debug.DrawLine(new Vector3(9,10,5), hit.transform.position + hit.transform.GetComponent<TileInfo>().unitOffset, Color.red);

            if(Input.GetMouseButtonDown(0)) {
                TileInfo tileInfo = hit.transform.GetComponent<TileInfo>();
                //Debug.Log("Distance from unit: " + currentTile.GetComponent<Hex>().Distance(hit.transform.GetComponent<Hex>()));
                //Debug.Log("Length :" + mapRef.map.GetHexAt(hit.transform.position).Length());

                DrawLineBetweenHexes(currentTile, mapRef.map.GetHexAt(hit.transform.position));

            }
        }
        Debug.DrawRay(gameObject.transform.position, Vector3.up * 1, Color.green);
        SetCurrentTile();
	}

    private void SetCurrentTile() {
        RaycastHit hit;
        Ray ray = new Ray(gameObject.transform.position, Vector3.down);
        
        if (Physics.Raycast(ray, out hit, camera.farClipPlane, tileLayerMask)) {
            currentTile = mapRef.map.GetHexAt(hit.transform.position);
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
        Debug.DrawLine(new Vector3(mapRef.mapLayout.HexToPixel(a).x,0,mapRef.mapLayout.HexToPixel(a).y) + new Vector3(0, 0.5f, 0), new Vector3(mapRef.mapLayout.HexToPixel(b).x, 0, mapRef.mapLayout.HexToPixel(b).y) + new Vector3(0, 0.5f, 0), Color.cyan, 5f);

        int distance = a.Distance(b) + 1;
        List<Vector3> lerpPoints = new List<Vector3>();

        for (int i = 0; i <= distance; i++) {
            Vector3 posA = new Vector3(mapRef.mapLayout.HexToPixel(a).x, 0, mapRef.mapLayout.HexToPixel(a).y);
            Vector3 posB = new Vector3(mapRef.mapLayout.HexToPixel(b).x, 0, mapRef.mapLayout.HexToPixel(b).y);
            lerpPoints.Add(CubeLerp(posA, posB, (float)i / distance));
        }

        int rayLength = 2;

        for (int i = 0; i < (lerpPoints.Count - 1); i++) {
            Hex _a = mapRef.map.GetHexAt(lerpPoints[i]);
            Vector2 aPos = mapRef.mapLayout.HexToPixel(_a);
            Hex _b = mapRef.map.GetHexAt(lerpPoints[i + 1]);
            Vector2 bPos = mapRef.mapLayout.HexToPixel(_b);
            Debug.DrawRay(new Vector3(aPos.x, 0, aPos.y), Vector3.up * rayLength, Color.red, 5f);
            Debug.DrawLine(new Vector3(aPos.x, 1f, aPos.y), new Vector3(bPos.x, 1f, bPos.y), Color.green, 5f);
            rayLength++;
        }
    }

}


public class Unit {
    public GameObject unitMesh;
    public int movementLimit;
}
