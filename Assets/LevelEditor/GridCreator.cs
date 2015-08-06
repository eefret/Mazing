using UnityEngine;
using System.Collections;

public class GridCreator : MonoBehaviour {

    // Grid map
    [HideInInspector]
    public int[] map;

    // Map size and block size
    public int mapIndex = 0;
    public int levelIndex = 0;
    public int mapWidth = 10;
    public int mapHeight = 10;
    [HideInInspector]
    public float blockWidth = 1;
    [HideInInspector]
    public float blockHeight = 1;

    // Floor material
    public Material floorMaterial;

    // Initial fill value
    public int initialFill = 0;

    // Grid container
    [HideInInspector]
    public GameObject container;
    // Selected tile
    [HideInInspector]
    public Vector2 lastSelectedTile, selectedTile;
    [HideInInspector]
    public int selectedTool = 0;

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
	    
	}

    // Map initialization function
    public void InitMap() {
        map = new int[mapWidth * mapHeight];
        
        int i, j;   
        for(i = 0; i < mapWidth; i++) {
            for(j = 0; j < mapHeight; j++) {
                map[i * mapHeight + j] = initialFill;
            }
        }
    }

    // Create current map using boxes
    public void CreateMap() {
        // Clear the map
        ClearMap();

        // Create container
        container = new GameObject();
        container.name = "Grid Container";
        container.transform.parent = transform;
        container.transform.localPosition = new Vector3(0, 0, 0);
        container.transform.hideFlags = HideFlags.HideInHierarchy;
        //container.isStatic = true;

        int i, j;
        float offsetx = blockWidth / 2;
        float offsety = blockHeight / 2;
        GameObject box;
        for(i = 0; i < mapWidth; i++) {
            for(j = 0; j < mapHeight; j++) {
                if(map[i * mapHeight + j] == 0) {
                    box = GameObject.CreatePrimitive(PrimitiveType.Quad);
                    box.transform.parent = container.transform;
                    box.name = "[" + i + "," + j + "]";
                    //box.isStatic = true;
                    box.GetComponent<Renderer>().material = floorMaterial;
                    box.transform.localPosition = new Vector3(i * blockWidth + offsetx, j * blockHeight + offsety, 0);
                }
                // Old code build walls instead of floors
                // if(map[i * mapHeight + j] == 1) {
                //     box = GameObject.CreatePrimitive(PrimitiveType.Cube);
                //     box.transform.parent = container.transform;
                //     box.name = "[" + i + "," + j + "]";
                //     box.transform.localPosition = new Vector3(i * blockWidth + offsetx, j * blockHeight + offsety, 0);
                // }
            }
        }
    }

    // Clear map
    public void ClearMap() {
        // Clear container
        GameObject.DestroyImmediate(container);        
    }

    // Return map as string
    public string mapToString() {
        string mapString = "";
        int i, j;

        for(i = 0; i < mapWidth; i++) {
            for(j = 0; j < mapHeight - 1; j++) {
                mapString += map[i * mapHeight + j] + ", ";
            }
            mapString += map[i * mapHeight + j] + "\n";
        }

        return mapString;
    }

    // Return map as string
    public string mapFromString(string mapString) {
        string[] lines = mapString.Split("\n"[0]); 
        string[] lineData;
        int i, j;
        
        for(i = 0; i < mapWidth; i++) {
            lineData = lines[i].Split(","[0]);
            for(j = 0; j < mapHeight; j++) {
                map[i * mapHeight + j] = int.Parse(lineData[j]);
            }
        }
        
        return mapString;
    }

    // Draw gizmo stuff
    void OnDrawGizmosSelected() {
        Gizmos.matrix = transform.localToWorldMatrix;

        // Draw the tiles white if empty and black if wall
        int i, j;
        float offsetx = blockWidth / 2;
        float offsety = blockHeight / 2;
        for(i = 0; i < mapWidth; i++) {
            for(j = 0; j < mapHeight; j++) {
                if(map[i * mapHeight + j] == 0) { Gizmos.color = new Color(0.8f, 0.8f, 0.8f, 0.8f); }
                else { Gizmos.color = new Color(0, 0, 0, 0.8f); }

                Gizmos.DrawCube(new Vector3(i * blockWidth + offsetx, j * blockHeight + offsety, 0), 
                                new Vector3(blockWidth, blockHeight, 1));
            }
        }

        // Draw selected tile
        if(selectedTile.x > 0) {
            Gizmos.color = new Color(0, 0.8f, 0, 0.8f);
            Gizmos.DrawCube(new Vector3(selectedTile.x * blockWidth + offsetx, selectedTile.y * blockHeight + offsety, 0), 
                            new Vector3(blockWidth, blockHeight, 1));
        }

        switch (selectedTool)
        {
            case 0: // Simple tool

                break;
            case 1: // Line tool               
                if(lastSelectedTile.x >= 0) {
                    Gizmos.color = new Color(0, 0.8f, 0, 0.8f);
                    Gizmos.DrawCube(new Vector3(lastSelectedTile.x * blockWidth + offsetx, lastSelectedTile.y * blockHeight + offsety, 0), 
                                    new Vector3(blockWidth, blockHeight, 1));
                    
                    Gizmos.color = new Color(0.8f, 0, 0, 0.8f);
                    Gizmos.DrawLine(new Vector3(selectedTile.x * blockWidth + offsetx, selectedTile.y * blockHeight + offsety, 0),
                                    new Vector3(lastSelectedTile.x * blockWidth + offsetx, lastSelectedTile.y * blockHeight + offsety, 0));
                    
                    float distance = Vector2.Distance(selectedTile, lastSelectedTile);
                    float angle = Mathf.Deg2Rad * Vector2.Angle(selectedTile - lastSelectedTile, new Vector3(0, 1));
                    float drawDistance = 0;
                    int mapX, mapY;
                    while(drawDistance <= distance) {
                        Gizmos.color = new Color(0, 0.8f, 0, 0.2f);
                        // Miror X axis to fix the angle problem
                        if(lastSelectedTile.x > selectedTile.x) {
                            mapX = Mathf.FloorToInt(lastSelectedTile.x - Mathf.Sin(angle) * drawDistance / blockWidth + offsetx);
                        } else {
                            mapX = Mathf.FloorToInt(lastSelectedTile.x + Mathf.Sin(angle) * drawDistance / blockWidth + offsetx);
                        }
                        mapY = Mathf.FloorToInt(lastSelectedTile.y + Mathf.Cos(angle) * drawDistance / blockHeight + offsety);
                        Gizmos.DrawCube(new Vector3(mapX, mapY, 0) + new Vector3(offsetx, offsety, 0), new Vector3(blockWidth, blockHeight, 1));
                        
                        drawDistance += blockWidth / 2;
                    }
                }

                break;
            case 2: // Rect tool
                if(lastSelectedTile.x >= 0) {
                    Gizmos.color = new Color(0, 0.8f, 0, 0.8f);
                    Gizmos.DrawCube(new Vector3(lastSelectedTile.x * blockWidth + offsetx, lastSelectedTile.y * blockHeight + offsety, 0), 
                                    new Vector3(blockWidth, blockHeight, 1));

                    int initX, initY, lastX, lastY;

                    if(lastSelectedTile.x < selectedTile.x) { initX = Mathf.FloorToInt(lastSelectedTile.x); lastX = Mathf.FloorToInt(selectedTile.x); }
                    else { initX = Mathf.FloorToInt(selectedTile.x); lastX = Mathf.FloorToInt(lastSelectedTile.x); }

                    if(lastSelectedTile.y < selectedTile.y) { initY = Mathf.FloorToInt(lastSelectedTile.y); lastY = Mathf.FloorToInt(selectedTile.y); }
                    else { initY = Mathf.FloorToInt(selectedTile.y); lastY = Mathf.FloorToInt(lastSelectedTile.y); }

                    for(i = initX; i <= lastX; i++) {
                        for(j = initY; j <= lastY; j++) {
                            Gizmos.color = new Color(0, 0.2f, 0, 0.8f);
                            
                            Gizmos.DrawCube(new Vector3(i * blockWidth + offsetx, j * blockHeight + offsety, 0), 
                                            new Vector3(blockWidth, blockHeight, 1));
                        }
                    }
                }

                break;
        }
    }

}
