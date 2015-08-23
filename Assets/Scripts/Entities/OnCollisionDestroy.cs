using UnityEngine;
using System.Collections;

public class OnCollisionDestroy : MonoBehaviour {

    // Destroy self
    public bool destroyOther;
    public bool destroySelf;

    // Gameobject to filter
    public GameObject filterObject;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnCollisionEnter (Collision col) {
        if(filterObject == null || col.gameObject == filterObject) {
            if(destroyOther) { Destroy(col.gameObject); }
            if(destroySelf) { Destroy(gameObject); }
        }
    }
}
