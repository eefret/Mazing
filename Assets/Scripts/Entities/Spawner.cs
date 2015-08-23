using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour {

    // Spawn timers
    public float spawnTime;
    public float spawnTimer;

    // Spawn object
    public GameObject spawnObject;

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
        spawnTimer += Time.deltaTime;
        if(spawnTimer > spawnTime) {
            spawnTimer -= spawnTime;

            GameObject spawned = Instantiate(spawnObject) as GameObject;
            spawned.transform.position = transform.position;
            spawned.transform.parent = transform;
        }
	}
}
