using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Text;

[CustomEditor(typeof(GridCreator))]
public class GridCreatorGUI : Editor
{
    // Grid creator reference
    private GridCreator gridCreator;
    // Last tool to restoring it when done editing
    private Tool lastTool;
    // Editing flag
    private bool editing;
    // Set tile tool ; 0 Simple ; 1 Line ; 2 Rect ; 3 Fill
    private int setTool;
    // Last change for drag
    private bool autosave = false;
    private float autosaveTimer = 5;
    private float lastTime = -1;
    // Confirmation bools
    private bool confirmInit = false;
    private bool confirmLoad = false;

    // Grid max and world position
    private Vector3 gridPos;
    private Vector3 maxgridPos;

    // Inspector GUI
	public override void OnInspectorGUI()
	{
        // Draw the base gui
		base.OnInspectorGUI ();

        // Get the component from the selected object
        gridCreator = Selection.activeGameObject.GetComponent<GridCreator>();

        // Init level button
        Rect button = EditorGUILayout.BeginHorizontal ("Button");
        if (GUI.Button (button, GUIContent.none)) {
            if(confirmInit) {
                gridCreator.ClearMap();
                gridCreator.InitMap();
                SceneView.RepaintAll();
                confirmInit = false;
            } else {
                confirmInit = true;
            }
        }
        GUILayout.Label (confirmInit ? "Confirm Init (Map will be overriden)" : "Init map");
        EditorGUILayout.EndHorizontal ();

        // Create level button
        button = EditorGUILayout.BeginHorizontal ("Button");
        if (GUI.Button (button, GUIContent.none)) {
            gridCreator.CreateMap();
            SceneView.RepaintAll();
        }
        GUILayout.Label ("Create map");
        EditorGUILayout.EndHorizontal ();

        // Clear level button
        button = EditorGUILayout.BeginHorizontal ("Button");
        if (GUI.Button (button, GUIContent.none)) {
            gridCreator.ClearMap();
            SceneView.RepaintAll();
        }
        GUILayout.Label ("Clear map");
        EditorGUILayout.EndHorizontal ();

        // Start editing button
        button = EditorGUILayout.BeginHorizontal ("Button");
        if (GUI.Button (button, GUIContent.none)) {
            gridCreator = Selection.activeGameObject.GetComponent<GridCreator>();

            gridPos = gridCreator.transform.position;
            maxgridPos = gridPos + new Vector3(gridCreator.mapWidth * gridCreator.blockWidth, gridCreator.mapHeight * gridCreator.blockHeight, 0);

            lastTool = Tools.current;
            editing = true;
            SceneView.RepaintAll();
        }
        GUILayout.Label ("Start Editing");
        EditorGUILayout.EndHorizontal ();

        // Stop editing button
        button = EditorGUILayout.BeginHorizontal ("Button");
        if (GUI.Button (button, GUIContent.none)) {
            gridCreator = Selection.activeGameObject.GetComponent<GridCreator>();
            gridCreator.selectedTool = -1;

            Tools.current = lastTool;
            editing = false;
            SceneView.RepaintAll();
        }
        GUILayout.Label ("Stop Editing");
        EditorGUILayout.EndHorizontal ();

        // Save
        button = EditorGUILayout.BeginHorizontal ("Button");
        if (GUI.Button (button, GUIContent.none)) {
            SaveLevel();
        }
        GUILayout.Label ("Save Level");
        EditorGUILayout.EndHorizontal ();
        
        // Autosave toggle button
        EditorGUILayout.BeginHorizontal ("Button");
        autosave = GUILayout.Toggle(autosave, "Autosave");
        autosaveTimer = GUILayout.HorizontalSlider(autosaveTimer, 5f, 180f);
        GUILayout.Label(autosaveTimer.ToString("000") + "s");
        EditorGUILayout.EndHorizontal ();

        // Load
        button = EditorGUILayout.BeginHorizontal ("Button");
        if (GUI.Button (button, GUIContent.none)) {
            if(confirmLoad) {
                LoadLevel();
                confirmLoad = false;
            } else {
                confirmLoad = true;
            }
        }
        GUILayout.Label (confirmLoad ? "Confirm Load" : "Load Level");
        EditorGUILayout.EndHorizontal ();

        // Autosave if time is correct
        float time = (float)EditorApplication.timeSinceStartup;
        if(autosave && time > lastTime + autosaveTimer) {
            SaveLevel();
            lastTime = time;
        }

        Event ev = Event.current;
        if (ev.type == EventType.MouseDown) {
            confirmInit = false;
            confirmLoad = false;
        }
    }
    
    void OnSceneGUI() {
        if(editing) {
            // Setup to start editing
            Tools.current = Tool.None;
            // Current event
            Event ev = Event.current;

            // Draw tools window
            GUILayout.Window(0, new Rect(10, 20, 100, 110), (id)=> {
                if(GUILayout.Button("Simple")) {   // Set tile button
                    setTool = 0;
                }
                if(GUILayout.Button("Line")) { // Clean tile button
                    setTool = 1;
                    gridCreator.lastSelectedTile = new Vector2(-1, -1);
                }
                if(GUILayout.Button("Rect")) { // Clean tile button
                    setTool = 2;
                }
                /*if(GUILayout.Button("Fill")) { // Clean tile button
                    setTile = 3;
                }*/
                gridCreator.selectedTool = setTool;
            }, "Tools");

            // Get mouse screen position
            Vector2 mouseScreenPos = Event.current.mousePosition;
            mouseScreenPos.y = SceneView.lastActiveSceneView.camera.pixelHeight - mouseScreenPos.y;
            Vector3 mousePos = SceneView.lastActiveSceneView.camera.ScreenPointToRay(mouseScreenPos).origin;

            // Get selected tile
            gridCreator.selectedTile = new Vector2(Mathf.FloorToInt((mousePos.x - gridPos.x) / gridCreator.blockWidth),
                                                   Mathf.FloorToInt((mousePos.y - gridPos.y) / gridCreator.blockHeight));

            // On mouse movement repaint
            if (ev.type == EventType.MouseMove || ev.type == EventType.MouseDrag) {
                SceneView.RepaintAll();
            }

            // Get events
            // Mouse down event
            if(ev.button == 0 || ev.button == 1) {
                switch (ev.type) {
                    case EventType.MouseDown: // On mouse button down
                        gridCreator.lastSelectedTile = gridCreator.selectedTile;
                        
                        // See if insde map, if inside set overlaping tile to setTile value
                        if(mousePos.x > gridPos.x && mousePos.x < maxgridPos.x && mousePos.y > gridPos.y && mousePos.y < maxgridPos.y) {                
                            if(setTool == 0) {
                                useTool(ev);
                            }
                        }
                        
                        ev.Use();
                        break;
                    case EventType.MouseDrag: // On mouse drag
                        // Set map tile if inside map area
                        if(mousePos.x > gridPos.x && mousePos.x < maxgridPos.x && mousePos.y > gridPos.y && mousePos.y < maxgridPos.y) {
                            if(setTool == 0) {
                                useTool(ev);
                            }
                        }
                        
                        ev.Use();
                        break;
                    case EventType.MouseUp: // On mouse button up
                        if(setTool == 1 || setTool == 2) {
                            useTool(ev);
                        }
                        
                        gridCreator.lastSelectedTile = new Vector2(-1, -1);
                        
                        ev.Use();
                        break;
                    case EventType.Layout: // Needed so events are used properly
                        // Somehow this allows ev.Use() to actually function and block mouse input
                        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
                        break;
                }
            }
        }
    }

    void useTool (Event ev) {
        int mapX, mapY;

        switch (setTool)
        {
            case 0:
                mapX = Mathf.FloorToInt(gridCreator.selectedTile.x);
                mapY = Mathf.FloorToInt(gridCreator.selectedTile.y);
                
                gridCreator.map[mapX * gridCreator.mapWidth + mapY] = ev.button;  
                break;
            case 1:
                float offsetx = gridCreator.blockWidth / 2;
                float offsety = gridCreator.blockHeight / 2;
                
                float distance = Vector2.Distance(gridCreator.selectedTile, gridCreator.lastSelectedTile);
                float angle = Mathf.Deg2Rad * Vector2.Angle(gridCreator.selectedTile - gridCreator.lastSelectedTile, new Vector3(0, 1));
                float drawDistance = 0;
                while(drawDistance <= distance) {
                    if(gridCreator.lastSelectedTile.x > gridCreator.selectedTile.x) {
                        mapX = Mathf.FloorToInt(gridCreator.lastSelectedTile.x - Mathf.Sin(angle) * drawDistance / gridCreator.blockWidth + offsetx);
                    } else {
                        mapX = Mathf.FloorToInt(gridCreator.lastSelectedTile.x + Mathf.Sin(angle) * drawDistance / gridCreator.blockWidth + offsetx);
                    }
                    mapY = Mathf.FloorToInt(gridCreator.lastSelectedTile.y + Mathf.Cos(angle) * drawDistance / gridCreator.blockHeight + offsety);
                    
                    gridCreator.map[mapX * gridCreator.mapWidth + mapY] = ev.button;  
                    
                    drawDistance += gridCreator.blockWidth / 2;
                }
                break;
            case 2:                
                int initX, initY, lastX, lastY;

                if(gridCreator.lastSelectedTile.x < gridCreator.selectedTile.x) { initX = Mathf.FloorToInt(gridCreator.lastSelectedTile.x);
                    lastX = Mathf.FloorToInt(gridCreator.selectedTile.x); }
                else { initX = Mathf.FloorToInt(gridCreator.selectedTile.x); lastX = Mathf.FloorToInt(gridCreator.lastSelectedTile.x); }

                if(gridCreator.lastSelectedTile.y < gridCreator.selectedTile.y) { initY = Mathf.FloorToInt(gridCreator.lastSelectedTile.y); 
                    lastY = Mathf.FloorToInt(gridCreator.selectedTile.y); }
                else { initY = Mathf.FloorToInt(gridCreator.selectedTile.y); lastY = Mathf.FloorToInt(gridCreator.lastSelectedTile.y); }
                
                int i, j;
                for(i = initX; i <= lastX; i++) {
                    for(j = initY; j <= lastY; j++) {
                        gridCreator.map[i * gridCreator.mapHeight + j] = ev.button;
                    }
                }

                break;
        }
    }
    
    void SaveLevel () {
        string fileDir = "Assets/LevelEditor/Saves/" + gridCreator.mapIndex + "_" + gridCreator.levelIndex + ".csv";
        System.IO.File.WriteAllText(fileDir, gridCreator.mapToString());
    }

    void LoadLevel () {
        string fileDir = "Assets/LevelEditor/Saves/" + gridCreator.mapIndex + "_" + gridCreator.levelIndex + ".csv";
        gridCreator.mapFromString(System.IO.File.ReadAllText(fileDir));
        gridCreator.CreateMap();
    }

}