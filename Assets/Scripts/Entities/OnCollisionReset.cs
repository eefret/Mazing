using UnityEngine;
using System.Collections;

public class OnCollisionReset : MonoBehaviour {
    
    // Gameobject to filter
    public GameObject filterObject;

    // Initial position
    private Vector3 initialPosition;
    
    // Use this for initialization
    void Start () {
        initialPosition = transform.position;
    }
    
    // Update is called once per frame
    void Update () {
        
    }
    
    void OnCollisionEnter (Collision col) {
        if(filterObject == null || col.gameObject == filterObject) {
            transform.position = initialPosition;
        }
    }
}
