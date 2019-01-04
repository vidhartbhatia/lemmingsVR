using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class RightHandUISelector : MonoBehaviour {

    ControllerSounds cs;
	public LevelGenerator lg;
	public GameObject handUI;
	public GameObject[] UIObjects;
	public GameObject[] HandObjects;
	public GameObject blockHandUI;
	public GameObject[] BlockUIObjects;
	public GameObject[] BlockHandObjects;
	public int hoverIndex = -1;
	public int trackPadIndex = -1;

	// Use this for initialization
	void Start () {
		lg = GameObject.Find ("LevelGenerator").GetComponent<LevelGenerator> ();
		hoverIndex = -1;
        cs = gameObject.GetComponent<ControllerSounds>();

		if (!lg.gameMode) {
			handUI.SetActive(false);
			blockHandUI.SetActive(true);
		} 
		else {
			handUI.SetActive(true);
			blockHandUI.SetActive(false);
		}

	}

	// Update is called once per frame
	void Update () {

		if(trackPadIndex != -1){
			return;
		}


		if (!lg.gameMode) {
			doHands(BlockUIObjects, BlockHandObjects);
		} 
		else {
			doHands(UIObjects, HandObjects);
		}

			
	}

	void doHands(GameObject[] UIObj, GameObject[] HandObj){
		double minDist = 0.06;
		int minIndex = -1;
		for(int i = 0; i < UIObj.Length; i++){
			double currDist = Vector3.Distance(transform.position, UIObj[i].transform.position);
			if (currDist < minDist) {
				minDist = currDist;
				minIndex = i;
			}
		}


		if (minIndex < 0) {
			if(hoverIndex >= 0){
				UIObj[hoverIndex].transform.localScale /= 1.5f;
			}
			hoverIndex = -1;
		} 
		else {
			if (hoverIndex < 0) {
				UIObj[minIndex].transform.localScale *= 1.5f;
				if(UIObj[minIndex].activeInHierarchy) cs.PlayJobClick();
			}
			else if (hoverIndex != minIndex) {
				UIObj[minIndex].transform.localScale *= 1.5f;
				UIObj[hoverIndex].transform.localScale /= 1.5f;
				if(UIObj[minIndex].activeInHierarchy) cs.PlayJobClick();
			}
			hoverIndex = minIndex;
		}

		if (SteamVR_Input._default.inActions.GrabPinch.GetStateDown (SteamVR_Input_Sources.RightHand) && hoverIndex >= 0 && (!lg.gameMode || lg.jobs[hoverIndex] > 0)) {
			cs.PlayJobSelect();
			HandObj[hoverIndex].SetActive (true);
			for(int i = 0; i < HandObj.Length; i++){
				if(i != hoverIndex)
					HandObj[i].SetActive (false);
			}
		}
	}

	/*void OnTriggerEnter(Collider other){
		Debug.Log("Collided...");
		if(other.gameObject.CompareTag("UIObject")){
			Debug.Log("...and changed.");
			other.gameObject.transform.localScale = other.transform.localScale * 1.5f;
		}
	}

	void OnTriggerExit(Collider other){
		Debug.Log("Exited...");
		if(other.gameObject.CompareTag("UIObject")){
			Debug.Log("...and changed.");
			other.gameObject.transform.localScale = other.transform.localScale / 1.5f;
		}
	}*/

}
