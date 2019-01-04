using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTrackInit : MonoBehaviour {

    public GameObject levelTrackerObject;

	LevelGenerator lg;

	// Use this for initialization
	void Start()
    {
		lg = GameObject.Find ("LevelGenerator").GetComponent<LevelGenerator>();
        GameObject temp = GameObject.FindGameObjectWithTag("Level Tracker");
        if( temp == null)
        {
            temp = Instantiate(levelTrackerObject);
            DontDestroyOnLoad(temp);
        }
        temp.GetComponent<LevelTracker>().UpdateMusic();
		lg.Begin ();
        Destroy(this.gameObject);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
