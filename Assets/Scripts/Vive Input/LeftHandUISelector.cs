using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class LeftHandUISelector : MonoBehaviour {

	[SteamVR_DefaultAction("TrackPadTouch", "default")]
	public SteamVR_Action_Vector2 menu_select;

	private SteamVR_Input_Sources hand;
	public RightHandUISelector rightHand;
	public int touchIndex = -1;

	private LevelGenerator lg;
	private TextMesh[] counts;

	// Use this for initialization
	void Start () {
		hand = SteamVR_Input_Sources.LeftHand;
		lg = GameObject.Find ("LevelGenerator").GetComponent<LevelGenerator> ();
		counts = GetComponentsInChildren<TextMesh>();
        UpdateCounts();
	}
	
	// Update is called once per frame
	void Update () {
		
		if (!lg.gameMode) {
			doHands(rightHand.BlockUIObjects, rightHand.BlockHandObjects);
		} 
		else {
			doHands(rightHand.UIObjects, rightHand.HandObjects);
		}

	}

	void doHands(GameObject[] UIObj, GameObject[] HandObj){
		Vector2 thumb_pos = menu_select.GetAxis(hand);

		float r = Mathf.Sqrt(thumb_pos.x*thumb_pos.x + thumb_pos.y*thumb_pos.y);
		float theta = thumb_pos.y > 0 ? Mathf.Acos(thumb_pos.x/r) : 2*Mathf.PI - Mathf.Acos(thumb_pos.x/r);

		touchIndex = -1;
		if (r > 0.0) {
			if (0 <= theta && theta < Mathf.PI / 3)
				touchIndex = lg.gameMode ? 5 : 2;
			else if (Mathf.PI / 3 <= theta && theta < 2 * Mathf.PI / 3)
				touchIndex = lg.gameMode ? 4 : 1;
			else if (2 * Mathf.PI / 3 <= theta && theta < Mathf.PI)
				touchIndex = lg.gameMode ? 3 : 0;
			else if (Mathf.PI <= theta && theta < 4 * Mathf.PI / 3)
				touchIndex = lg.gameMode ? 0 : 3;
			else if (4 * Mathf.PI / 3 <= theta && theta < 5 * Mathf.PI / 3)
				touchIndex = lg.gameMode ? 1 : -1;
			else
				touchIndex = lg.gameMode ? 2 : -1;
		}




		if (touchIndex != -1) {
			if(rightHand.hoverIndex != -1){
				UIObj[rightHand.hoverIndex].transform.localScale /= 1.5f;
				rightHand.hoverIndex = -1;
			}

			if(rightHand.trackPadIndex == -1){
				rightHand.trackPadIndex = touchIndex;
				UIObj[rightHand.trackPadIndex].transform.localScale *= 1.5f;
			}
			else if(rightHand.trackPadIndex != touchIndex){
				UIObj[rightHand.trackPadIndex].transform.localScale /= 1.5f;
				rightHand.trackPadIndex = touchIndex;
				UIObj[rightHand.trackPadIndex].transform.localScale *= 1.5f;
			}

		} 
		else if(touchIndex == -1 && rightHand.trackPadIndex != -1){
			UIObj[rightHand.trackPadIndex].transform.localScale /= 1.5f;
			rightHand.trackPadIndex = -1;
		}

		if(SteamVR_Input._default.inActions.Teleport.GetStateDown(hand) && rightHand.trackPadIndex != -1 && (!lg.gameMode || lg.jobs[rightHand.trackPadIndex] > 0)){
			HandObj[rightHand.trackPadIndex].SetActive (true);
			for(int i = 0; i < HandObj.Length; i++){
				if(i != rightHand.trackPadIndex)
					HandObj[i].SetActive (false);
			}
		}

	}

	public void UpdateCounts() {
		if(!lg.gameMode){
			for(int i = 0; i < rightHand.HandObjects.Length; i++ )
			{
				rightHand.HandObjects[i].SetActive(false);
			}
			return;
		}

        for(int i = 0; i < rightHand.UIObjects.Length; i++ )
        {
            rightHand.UIObjects[i].SetActive(false);
        }
		for(int i = 0; i < Mathf.Min(counts.Length,lg.jobs.Length); i++){
			counts[i].text = "" + lg.jobs [i/*rightHand.trackPadIndex*/];
			if (lg.jobs [i/*rightHand.trackPadIndex*/] > 0) {
				rightHand.UIObjects[i].SetActive (true);
			}
		}
	}

}
