using UnityEngine;
using System.Collections;

public class LinearMovement : MonoBehaviour {

    // Movement type
    public Vector3 movementSpeed;

    // Bounding box
    public Rect boundBox;

    // Model
    public GameObject model;

    public bool bounceOffmap;
    public bool bounceOffbox;
    public int bouncesToDestroy;

    public bool sideBounce;

	// Use this for initialization
	void Start () {
        gizmoMatrix = transform.localToWorldMatrix;

        boundBox.position = transform.position;
	}
	
	// Update is called once per frame
    void Update () {
        Vector3 lastPosition = Vector3.MoveTowards(transform.position, transform.position + movementSpeed, Time.deltaTime * movementSpeed.magnitude);

        // Move and rotate if posible
        if(GridCreator.checkPosition(lastPosition)) {
            if(lastPosition != transform.position) {
                model.transform.LookAt(lastPosition, Vector3.up);
                if(lastPosition.x == transform.position.x) {
                    model.transform.Rotate(new Vector3(90, 90, 0));
                } else {
                    model.transform.Rotate(new Vector3(0, 90, 0));
                }
            }

            if(!AIUtils.enemyInsideBox(lastPosition, boundBox)) {
                if(bounceOffbox) { Bounce(true); }
            }

            transform.position = lastPosition;
        } else {
            if(bounceOffmap) { Bounce(false); }
        }
	}

    void Bounce(bool BoxBounce) {
        Vector3 movX = transform.position + new Vector3(movementSpeed.x * Time.deltaTime, 0);
        Vector3 movY = transform.position + new Vector3(0, movementSpeed.y * Time.deltaTime);
        if(sideBounce) {
            if(BoxBounce) {
                if(!AIUtils.enemyInsideBox(movX, boundBox)) { movementSpeed.x *= -1; }
                if(!AIUtils.enemyInsideBox(movY, boundBox)) { movementSpeed.y *= -1; }
            } else {
                if(!GridCreator.checkPosition(movX)) { movementSpeed.x *= -1; }
                if(!GridCreator.checkPosition(movY)) { movementSpeed.y *= -1; }
            }
        }
        else { movementSpeed *= -1; }

        bouncesToDestroy--;
        if(bouncesToDestroy == 0) { Destroy(gameObject); }
    }

    private Matrix4x4 gizmoMatrix;
    // Draw gizmo stuff
    void OnDrawGizmosSelected() {
        Gizmos.matrix = transform.localToWorldMatrix;
        if(movementSpeed.magnitude > 0) {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(new Vector3(), movementSpeed.normalized);
        }

        Gizmos.matrix = gizmoMatrix;
        if(boundBox.width > 0 || boundBox.height > 0) {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(new Vector3(0, 0), new Vector3(boundBox.width, boundBox.height, 1));
        }
    }
}
