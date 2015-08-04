using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {

    public float movement = 1;
    public float rotation = 1;
    public float distanceModifier = 5;

    public GridCreator level;


	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
        float movSpeed = movement * Time.deltaTime;
        float rotSpeed = rotation * Time.deltaTime;
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition + new Vector3(0, 0, 50));
        mousePos.z = transform.position.z;
        
        Vector3 lastPosition = transform.position;
        float distance = Vector3.Distance(transform.position, mousePos);
        if(Input.GetMouseButton(0)) {
            lastPosition = Vector3.MoveTowards(transform.position, mousePos, movSpeed * distance / distanceModifier);
        }

        if(checkPosition(lastPosition)) {
            transform.position = lastPosition;
        }
	}

    bool checkPosition (Vector3 Position) {
        Vector3 gridPos = level.transform.position;
        Vector2 levelTile = new Vector2(Mathf.FloorToInt((Position.x - gridPos.x) / level.blockWidth),
                                        Mathf.FloorToInt((Position.y - gridPos.y) / level.blockHeight));

        if(level.map[(int)levelTile.x * level.mapWidth + (int)levelTile.y] == 0) {
            return true;
        } else {
            return false;
        }
    }
}
