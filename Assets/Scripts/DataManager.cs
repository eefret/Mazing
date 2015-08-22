using UnityEngine;
using System.Collections;

public class DataManager : MonoBehaviour {

    public GridCreator InitialLevel;

	// Use this for initialization
	void Start () {
        GridCreator.currentLevel = InitialLevel;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
