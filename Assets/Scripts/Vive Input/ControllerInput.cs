using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class ControllerInput : MonoBehaviour {

	public LevelGenerator lg;
	public LemmingPool lp;
    LEController sphere;
    public string whichHand;
	public RightHandUISelector rightHand;
	public LeftHandUISelector leftHand;
    public GameObject tutorialMenu;

    SoundManager sm;

	private bool held = false;

	private bool placingSpawner = false;
	private int spawnerDirection = 2;

	private bool placingLem = false;
	private string lemDirection = "";

	public GameObject parent;
	private bool leftHeld = false;
	// for moving the level
	private float handY;
	private float parentY;
    private float originalParentY;

    private float handX;
    private float parentX;
    private float originalParentX;

    void Start()
	{
        sm = GameObject.Find("SoundManager").GetComponent<SoundManager>();
		lg = GameObject.Find("LevelGenerator").GetComponent<LevelGenerator>();
		lp = GameObject.Find("Lemming Pool").GetComponent<LemmingPool>();
        sphere = gameObject.transform.GetChild(1).gameObject.GetComponent<LEController>();
        originalParentY = parent.transform.position.y;
    }

    // Update is called once per frame
    void Update ()
	{
        if (whichHand == "Left" && SteamVR_Input._default.inActions.MenuPush.GetStateDown(SteamVR_Input_Sources.LeftHand)
                || whichHand == "Right" && SteamVR_Input._default.inActions.MenuPush.GetStateDown(SteamVR_Input_Sources.RightHand))
        {
            if (tutorialMenu.activeInHierarchy)
                tutorialMenu.SetActive(false);
            else
                tutorialMenu.SetActive(true);
        }

        //************      LEVEL EDITOR INPUT      ****************//
        if (!lg.gameMode) {
			if (whichHand == "Left" && SteamVR_Input._default.inActions.GrabPinch.GetStateDown (SteamVR_Input_Sources.LeftHand)
			    || whichHand == "Right" && SteamVR_Input._default.inActions.GrabPinch.GetStateDown (SteamVR_Input_Sources.RightHand)
                && leftHand.touchIndex == -1 && rightHand.hoverIndex == -1) {
				sphere.Activate ();
				held = true;
			}
            if (whichHand == "Left" && SteamVR_Input._default.inActions.GrabPinch.GetStateUp(SteamVR_Input_Sources.LeftHand)
                || whichHand == "Right" && SteamVR_Input._default.inActions.GrabPinch.GetStateUp(SteamVR_Input_Sources.RightHand)
                && held) {
				int blockNum = 1;
				for(int i=0; i<rightHand.BlockHandObjects.Length; i++){
					if (rightHand.BlockHandObjects [i].activeInHierarchy) {
						switch (i) {
						case 0:
							blockNum = 7;
							break;
						case 1:
							blockNum = 8;
							break;
						case 2:
							blockNum = 1;
							break;
						case 3:
							blockNum = 9;
							break;
						}
					}
				}
				sphere.Deactivate (blockNum);
				held = false;
			} else if (held) {
				sphere.FillTemps ();
			}

			if (whichHand == "Right" && SteamVR_Input._default.inActions.Teleport.GetStateDown (SteamVR_Input_Sources.RightHand)) {
				sphere.currentBlock.Change(6);
			}

            if (whichHand == "Left" && SteamVR_Input._default.inActions.Teleport.GetStateDown(SteamVR_Input_Sources.LeftHand))
            {
                sphere.currentBlock.AddText(lg.inputText);
            }


            if (whichHand == "Right" && SteamVR_Input._default.inActions.GrabGrip.GetStateDown (SteamVR_Input_Sources.RightHand)) {
				sphere.Activate ();
				placingSpawner = true;
			}
			if (whichHand == "Right" && SteamVR_Input._default.inActions.GrabGrip.GetStateUp (SteamVR_Input_Sources.RightHand)) {
				sphere.startBlock.Unhighlight ();
				sphere.startBlock.Change (spawnerDirection);
				placingSpawner = false;
			} else if (placingSpawner) {
				int xDiff = sphere.currentBlock.x - sphere.startBlock.x;
				int zDiff = sphere.currentBlock.z - sphere.startBlock.z;

				if (Mathf.Abs (xDiff) > Mathf.Abs (zDiff)) {
					if (xDiff > 0) {
						spawnerDirection = 2;
					} else {
						spawnerDirection = 3;
					}
				} else {
					if (zDiff > 0) {
						spawnerDirection = 4;
					} else {
						spawnerDirection = 5;
					}
				}

				sphere.startBlock.Highlight (spawnerDirection);

			}
		} 
        //*********************     GAME INPUT      *************************//
		else {
            //Extra blocker code that needs to happen even if last blocker has been placed
            if (whichHand == "Right" && SteamVR_Input._default.inActions.GrabPinch.GetStateUp(SteamVR_Input_Sources.RightHand) && placingLem)
            {
                print("End blocker placement");
                if (sphere.startBlock.blocker != null)
                    sphere.startBlock.blocker.direction = lemDirection;
				placingLem = false;
				sphere.startBlock.blocker.transform.GetChild (4).gameObject.SetActive (false);
            }
            else if (placingLem)
            {
				sphere.startBlock.blocker.transform.GetChild (4).gameObject.SetActive (true);

                int xDiff = sphere.currentBlock.x - sphere.startBlock.x;
                int zDiff = sphere.currentBlock.z - sphere.startBlock.z;

                if (Mathf.Abs(xDiff) > Mathf.Abs(zDiff))
                {
                    if (xDiff > 0)
                    {
						lemDirection = "xPos";
						sphere.startBlock.blocker.transform.GetChild(0).eulerAngles = new Vector3(0, 90, 0);
						sphere.startBlock.blocker.transform.GetChild(4).eulerAngles = new Vector3(0, 180, 0);
                        sphere.startBlock.blocker.armature.transform.eulerAngles = new Vector3(0, 90, 0);
                    }
                    else
                    {
						lemDirection = "xNeg";
						sphere.startBlock.blocker.transform.GetChild(0).eulerAngles = new Vector3(0, 270, 0);
						sphere.startBlock.blocker.transform.GetChild(4).eulerAngles = new Vector3(0, 0, 0);
                        sphere.startBlock.blocker.armature.transform.eulerAngles = new Vector3(0, 270, 0);
                    }
                }
                else
                {
                    if (zDiff > 0)
                    {
						lemDirection = "zPos";
						sphere.startBlock.blocker.transform.GetChild(0).eulerAngles = new Vector3(0, 0, 0);
						sphere.startBlock.blocker.transform.GetChild(4).eulerAngles = new Vector3(0, 90, 0);
                        sphere.startBlock.blocker.armature.transform.eulerAngles = new Vector3(0, 0, 0);
                    }
                    else
                    {
                        lemDirection = "zNeg";
						sphere.startBlock.blocker.transform.GetChild(0).eulerAngles = new Vector3(0, 180, 0);
						sphere.startBlock.blocker.transform.GetChild(4).eulerAngles = new Vector3(0, 270, 0);
                        sphere.startBlock.blocker.armature.transform.eulerAngles = new Vector3(0, 180, 0);
                    }
                }
            }


            //Blocker code
            if (rightHand.HandObjects [0].activeInHierarchy) {
				if (whichHand == "Right" && SteamVR_Input._default.inActions.GrabPinch.GetStateDown (SteamVR_Input_Sources.RightHand) 
                    && sphere.currentBlock.blocker == null) {
                    sphere.Activate ();
					if (sphere.currentLemming != null && LevelCube.IsSolid(lg.blocks[sphere.startBlock.x, sphere.startBlock.y-1, sphere.startBlock.z].ID)) {
						print ("Begin blocker placement");
						sphere.currentLemming.SetJob ("blocker", sphere.startBlock.x, sphere.startBlock.y, sphere.startBlock.z);
						sphere.startBlock.blocker = sphere.currentLemming;
						placingLem = true;
						lg.jobs [0]--;
						leftHand.UpdateCounts();
						if (lg.jobs [0] == 0)
							rightHand.HandObjects [0].SetActive (false);
                        sm.sounds[0].Play();
					}
				}
			}
            //Bomb code
			else if(rightHand.HandObjects [1].activeInHierarchy){
				if (whichHand == "Right" && SteamVR_Input._default.inActions.GrabPinch.GetStateDown (SteamVR_Input_Sources.RightHand) && sphere.currentBlock.blocker == null) {
                    sphere.Activate ();
					if (sphere.currentLemming != null) {
						print ("Bomb placement");

                        for( int x = -1; x < 2; x++)
                        {
                            for( int y = 0; y < 3; y++ )
                            {
                                for( int z = -1; z < 2; z++ )
                                {
                                    if(lg.blocks[sphere.startBlock.x + x, sphere.startBlock.y + y, sphere.startBlock.z + z].ID < 2 
                                        || lg.blocks[sphere.startBlock.x + x, sphere.startBlock.y + y, sphere.startBlock.z + z].ID > 6)
                                        lg.blocks[sphere.startBlock.x + x, sphere.startBlock.y + y, sphere.startBlock.z + z].Change(0);
                                }
                            }
                        }

						sphere.currentLemming.Explode();
						sphere.currentLemming.Die(false);
						sphere.currentLemming = null;

						lg.jobs [1]--;
						leftHand.UpdateCounts();
						if (lg.jobs [1] == 0)
							rightHand.HandObjects [1].SetActive (false);
                        sm.sounds[1].Play();
					}
				}
			}
            //Digger code
			else if(rightHand.HandObjects [2].activeInHierarchy){
				if (whichHand == "Right" && SteamVR_Input._default.inActions.GrabPinch.GetStateDown (SteamVR_Input_Sources.RightHand) && sphere.currentBlock.blocker == null) {
					sphere.Activate ();
					if (sphere.currentLemming != null) {
						print ("Digger placement");
						sphere.currentLemming.digging = true;

						lg.jobs [2]--;
						leftHand.UpdateCounts();
						if (lg.jobs [2] == 0)
							rightHand.HandObjects [2].SetActive (false);
					}
                    sm.sounds[2].Play();
				}
			}
            //Umbrella code
			else if(rightHand.HandObjects [3].activeInHierarchy){
				if (whichHand == "Right" && SteamVR_Input._default.inActions.GrabPinch.GetStateDown (SteamVR_Input_Sources.RightHand)) {
					sphere.Activate ();
					if (!LevelCube.IsSolid(sphere.currentBlock.ID) && rightHand.hoverIndex == -1) {
						print ("Umbrella placement");
						sphere.currentBlock.umbrellas = true;

						lg.jobs [3]--;
						leftHand.UpdateCounts();
						if (lg.jobs [3] == 0)
							rightHand.HandObjects [3].SetActive (false);
					}
				}
			}
            //Bridge code
			else if(rightHand.HandObjects [4].activeInHierarchy){
				if (whichHand == "Right" && SteamVR_Input._default.inActions.GrabPinch.GetStateDown (SteamVR_Input_Sources.RightHand)) {
					sphere.Activate ();
					if (sphere.currentLemming != null) {
						print ("Wood placement");
						sphere.currentLemming.wood = 3;
						sphere.currentLemming.transform.GetChild(2).gameObject.SetActive(true);

						lg.jobs [4]--;
						leftHand.UpdateCounts();
						if (lg.jobs [4] == 0)
							rightHand.HandObjects [4].SetActive (false);
                        sm.sounds[4].Play();
                    }
				}

			}
			//Trampoline code
			/*else if(rightHand.HandObjects [5].activeInHierarchy){
				if (whichHand == "Right" && SteamVR_Input._default.inActions.GrabPinch.GetStateDown (SteamVR_Input_Sources.RightHand)) {
					sphere.Activate ();
					if (!LevelCube.IsSolid(sphere.currentBlock.ID) && rightHand.hoverIndex == -1) {
						print ("Trampoline placement");
						sphere.currentBlock.trampolines = true;

						lg.jobs [5]--;
						leftHand.UpdateCounts();
						if (lg.jobs [5] == 0)
							rightHand.HandObjects [5].SetActive (false);
					}
				}
			}*/



			// code for moving level vertically
			if (whichHand == "Left" && SteamVR_Input._default.inActions.GrabPinch.GetStateDown(SteamVR_Input_Sources.LeftHand))
			{
				handY = sphere.transform.position.y;
				parentY = parent.transform.position.y;
				leftHeld = true;
				Debug.Log("1");
			}
			if (whichHand == "Left" && SteamVR_Input._default.inActions.GrabPinch.GetStateUp(SteamVR_Input_Sources.LeftHand))
			{
				leftHeld = false;
				Debug.Log("2");
			}
			else if (leftHeld)
			{
				float yDiff = sphere.transform.position.y - handY;
				parent.transform.position = new Vector3 (parent.transform.position.x, parentY + yDiff, parent.transform.position.z);
			}

            lg.yDiff = parent.transform.position.y - originalParentY;

        }
	}

}
