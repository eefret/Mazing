using UnityEngine;
using System.Collections;

public class DestroyAfterTime : MonoBehaviour {

    // Timers
    public float destroyTime;
    private float destroyTimer;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        destroyTimer += Time.deltaTime;
        if(destroyTimer > destroyTime) {
            Destroy(gameObject);
        }
	}
}
