using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Exit : MonoBehaviour
{

    LemmingPool lp;
	LevelGenerator lg;
	LevelCube thisCube;
	LevelTracker lt;

	// Use this for initialization
	void Start ()
    {
        lp = GameObject.Find("Lemming Pool").GetComponent<LemmingPool>();	
		lg = GameObject.Find("LevelGenerator").GetComponent<LevelGenerator>();
		thisCube = GetComponent<LevelCube>();
		lt = GameObject.FindGameObjectWithTag("Level Tracker").GetComponent<LevelTracker>();
        transform.GetChild(2).gameObject.SetActive(true);
    }
	
	// Update is called once per frame
	void Update ()
    {
		if(thisCube.lem != null){
			SaveLem(thisCube.lem);
		}
	}

	private void SaveLem(Lemming saved)
	{
		Debug.Log("collided with lemming");
        lp.lemmingsSaved++;
		saved.Die(true);
		if (lp.lemmingsSaved >= lp.lemmingsObj) {
			if (lt.currentLevel < lt.numLevels)
				lt.currentLevel++;
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		}
	}
}