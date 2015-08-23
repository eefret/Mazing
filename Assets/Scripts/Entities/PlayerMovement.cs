using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {

    public enum PlayerMovType { Mouse, KeyboardWASD, KeyboardTank }

    public PlayerMovType playerMovType = PlayerMovType.Mouse;

    public GameObject model;

    public float movement = 1;
    public float rotation = 1;

    private float angle = 3.14f;

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
        // Get movement speed and rotation speed
        float movSpeed = movement * Time.deltaTime;
        float rotSpeed = rotation * Time.deltaTime;
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition + new Vector3(0, 0, 50));
        mousePos.z = transform.position.z;

        // Get current position
        Vector3 lastPosition = transform.position;

        // Move using current move type
        switch (playerMovType)
        {
            case PlayerMovType.Mouse:
                float distance = Vector3.Distance(transform.position, mousePos);
                float maxDistance = Vector3.Distance(transform.position, Camera.main.ScreenToWorldPoint(new Vector3(1, 1, 50)));
                Debug.Log(maxDistance);
                Debug.Log(distance);
                if(Input.GetMouseButton(0)) {
                    lastPosition = Vector3.MoveTowards(transform.position, mousePos, Mathf.Max(movSpeed * (distance / maxDistance * 2), movSpeed));
                }
                break;
            case PlayerMovType.KeyboardWASD:
                if(Input.GetKey(KeyCode.W)) { lastPosition.y += movSpeed; }
                if(Input.GetKey(KeyCode.S)) { lastPosition.y -= movSpeed; }
                if(Input.GetKey(KeyCode.D)) { lastPosition.x += movSpeed; }
                if(Input.GetKey(KeyCode.A)) { lastPosition.x -= movSpeed; }
                break;
            case PlayerMovType.KeyboardTank:
                if(Input.GetKey(KeyCode.W)) { lastPosition += new Vector3(Mathf.Sin(angle) * movSpeed, Mathf.Cos(angle) * movSpeed, 0); }
                if(Input.GetKey(KeyCode.S)) { lastPosition -= new Vector3(Mathf.Sin(angle) * movSpeed, Mathf.Cos(angle) * movSpeed, 0); }
                if(Input.GetKey(KeyCode.D)) { angle += rotSpeed; lastPosition += new Vector3(Mathf.Sin(angle) * 0.0001f, Mathf.Cos(angle) * 0.0001f, 0); }
                if(Input.GetKey(KeyCode.A)) { angle -= rotSpeed; lastPosition += new Vector3(Mathf.Sin(angle) * 0.0001f, Mathf.Cos(angle) * 0.0001f, 0); }

                break;
            default:
                break;
        }

        if(GridCreator.checkPosition(lastPosition)) {
            if(lastPosition != transform.position && model != null) {
                model.transform.LookAt(lastPosition, Vector3.up);
                if(lastPosition.x == transform.position.x) {
                    model.transform.Rotate(new Vector3(90, 90, 0));
                } else {
                    model.transform.Rotate(new Vector3(0, 90, 0));
                }
            }

            transform.position = lastPosition;
        }
	}
}
