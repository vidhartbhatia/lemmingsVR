using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandTextController : MonoBehaviour {

	public LemmingPool lp;
	public bool Alive;
	public bool Need;
	private TextMesh aliveOrNeed;

	// Use this for initialization
	void Start () {
		aliveOrNeed = GetComponent<TextMesh>();
		if (!lp.lg.gameMode)
			gameObject.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () {
		if(Alive){
			aliveOrNeed.text = "Alive: " + (lp.lg.maxLemmings - lp.lemmingsDead - lp.lemmingsSaved);
		}
		else if(Need){
			aliveOrNeed.text = "Need: " + (lp.lg.lemmingsObj - lp.lemmingsSaved);
		}
	}
}
