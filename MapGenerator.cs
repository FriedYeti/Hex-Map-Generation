using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour {
    public Vector2Int mapSize = new Vector2Int(8, 8);
    public Vector2 unitSize = new Vector2(1, 1);
    public Vector2 mapOrigin = new Vector2(0, 0);

    public GameObject[] hexPrefabs;

    public Layout mapLayout;

    public int prefabSelect = 0;

    public Vector2 noiseOrigin;

    public HexGrid map;

    private void Awake() {
	    // TODO set mapOrigin to object's origin instead of public variable
        mapLayout = new Layout(Layout.pointy, unitSize, mapOrigin);
        map = new HexGrid(mapSize, mapLayout);

        noiseOrigin = new Vector2(Random.Range(0f, 10f),Random.Range(0f, 10f));
    }

    // Use this for initialization
    void Start () {
		for(int i = 0; i < mapSize.x; i++) {
            for(int j = 0; j < mapSize.y; j++) {
                map.SetHex(new Vector2Int(i, j), new Hex(new Vector2Int(i, j)));
            }
        }

        for (int i = 0; i < mapSize.x; i++) {
            for (int j = 0; j < mapSize.y; j++) {
                float perlinNoise = Mathf.Clamp(Mathf.PerlinNoise((float)i / hexPrefabs.Length + noiseOrigin.x, (float)j / hexPrefabs.Length + noiseOrigin.y), 0, 0.999f);
                prefabSelect = Mathf.FloorToInt(perlinNoise * hexPrefabs.Length);
                Hex tile = map.GetHex(new Vector2Int(i, j));
                Vector2 worldCoords = mapLayout.HexToPixel(tile);
                Vector3 pos = new Vector3(worldCoords.x, (float)prefabSelect / hexPrefabs.Length, worldCoords.y);
                GameObject newHex = Instantiate(hexPrefabs[prefabSelect], pos, Quaternion.identity);
                newHex.transform.parent = gameObject.transform;
                newHex.gameObject.name = ("Tile (" + i + ", " + j + ")");
                

                tile.SetHexTile(newHex);

                
                print("(" + i + ", " + j + "): " + Mathf.PerlinNoise((float)i / hexPrefabs.Length + noiseOrigin.x, (float)j / hexPrefabs.Length + noiseOrigin.y));
            }
        }

    }

	// Update is called once per frame
	void Update () {

    }
}
