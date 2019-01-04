using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LemmingPool : MonoBehaviour {

    public int numLemmings;
    public int startLemmings;
    public int lemmingsSaved = 0;
    public int lemmingsDead = 0;
    public int lemmingsObj;
	public LevelGenerator lg;

    private Stack<GameObject> lemmings;

    public GameObject lemmingObject;

	// Use this for initialization
	void Start ()
	{
        lemmings = new Stack<GameObject>();
        for (int i = 0; i < 100; i++)
        {
			GameObject newLemming = Instantiate(lemmingObject);
			newLemming.transform.parent = lg.parent.transform;
            lemmings.Push(newLemming);
        }
        startLemmings = numLemmings;
	}
	
    public GameObject SpawnLemming()
    {
		if (numLemmings > 0) {
			numLemmings--;
			GameObject temp = lemmings.Pop ();
			return temp;
		} 
		else
			return null;
    }

    public void Take( GameObject lemming )
    {
        lemmings.Push(lemming);
    }

	// Update is called once per frame
	void Update () {
		
	}
}
