using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LemmingSpawner : MonoBehaviour {

    LemmingPool lp;
    LevelGenerator lg;
	LevelCube parentCube;

    public int spawnrate = 1;
    int spawnFrames;

	public int x, y, z;

    //"xPos", "xNeg", "zPos", "zNeg"
    string direction;

	// Use this for initialization
	void Start ()
    {
		parentCube = this.GetComponent<LevelCube>();
        lg = GameObject.Find("LevelGenerator").GetComponent<LevelGenerator>();
        lp = GameObject.Find("Lemming Pool").GetComponent<LemmingPool>();
		x = parentCube.x;
		y = parentCube.y;
		z = parentCube.z;
        spawnFrames = spawnrate * 60;
	}

    public void SpawnerSetup( string dir )
    {
        direction = dir;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (!lg.gameMode) return;
        if (spawnFrames > 0) spawnFrames--;
        else
        {
            if (lp.numLemmings > 0)
            {
                spawnFrames = spawnrate * 60;
				Spawn();
            }
            else
            {
                spawnFrames = -1;
            }
        }
	}

    void Spawn()
    {
		//Debug.Log ("Spawning Lemming");
        GameObject temp = lp.SpawnLemming();
		if(temp == null) return;

        temp.transform.position = gameObject.transform.position;
		temp.GetComponent<Lemming>().direction = direction;
		temp.GetComponent<Lemming>().x = x;
		temp.GetComponent<Lemming>().y = y;
		temp.GetComponent<Lemming>().z = z;
		temp.SetActive(true);
    }
}
