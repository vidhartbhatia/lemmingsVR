using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelCube : MonoBehaviour {

    public int ID = 0;
	int tempID = 0;

	public int x, y, z;
	public Lemming lem;
	public Lemming blocker;
	public bool umbrellas = false;

	LevelGenerator lg;
	GameObject umbrella;
	GameObject trampoline;

    public TextMeshPro text;
    public bool textEnabled = false;

    public Material highlightMat;
    public Material gameHighlightMat;
    public Material eraseMat;
    Material oldMat;

    public Material[] blockMats = new Material[10];

    MeshRenderer mr;


	//*** LEVEL CUBE CODES ***
	// 0 - Air
	// 1 - Grass
	// 2 - Spawner xPos
	// 3 - Spawner xNeg
	// 4 - Spawner zPos
	// 5 - Spawner zNeg
    // 6 - Exit
    // 7 - Dirt
    // 8 - Dirt w/ Grass
	// 9 - Stone

    // Use this for initialization
    void Start()
    {
        lg = GameObject.Find("LevelGenerator").GetComponent<LevelGenerator>();
		if (lg.gameMode) highlightMat = gameHighlightMat;
		umbrella = transform.GetChild(0).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
		if (umbrellas && !umbrella.activeInHierarchy) {
			umbrella.SetActive (true);
		} 
		else if (!umbrellas && umbrella.activeInHierarchy){
			umbrella.SetActive (false);
		}

    }

    public void AddText( string newText )
    {
        text.gameObject.SetActive(true);
        text.text = newText;
        textEnabled = true;
    }

	public void Setup(int blockID, int newX, int newY, int newZ)
    {
        text = gameObject.transform.GetChild(1).GetComponent<TextMeshPro>();
        mr = gameObject.GetComponent<MeshRenderer>();
        x = newX;
        y = newY;
        z = newZ;
        Change(blockID);
    }

    public void Change(int blockID)
    {
        if (blockID == 0)
        {
            mr.enabled = false;
            text.gameObject.SetActive(false);
            gameObject.transform.GetChild(2).gameObject.SetActive(false);
            textEnabled = false;
        }
		if (blockID != 0) mr.enabled = true;
        ID = blockID;
        oldMat = blockMats[ID];
        mr.material = oldMat;

        LemmingSpawner s = gameObject.GetComponent<LemmingSpawner>();
        if (2 <= ID && ID <= 5)
        {
            s.enabled = true;
            switch (ID)
            {
                case 2:
                    s.SpawnerSetup("xPos");
                    break;
                case 3:
                    s.SpawnerSetup("xNeg");
                    break;
                case 4:
                    s.SpawnerSetup("zPos");
                    break;
                case 5:
                    s.SpawnerSetup("zNeg");
                    break;
            }
		}
		else
			s.enabled = false;

        if (ID == 6)
        {
            gameObject.GetComponent<Exit>().enabled = true;
            mr.enabled = false;
        }
        else
            gameObject.GetComponent<Exit>().enabled = false;
        
    }

    private void OnTriggerEnter(Collider other)
    {


        if (other.gameObject.tag.StartsWith("LevelEdit"))
        {
            LEController sphere = other.gameObject.GetComponent<LEController>();
            sphere.lastBlock = sphere.currentBlock;
            sphere.currentBlock = this;
            other.gameObject.GetComponent<ControllerSounds>().PlayTileClick();
        }

        if (lg.gameMode && (other.gameObject.tag == "LevelEditErase" || ID != 0))
			return;
		
        switch (other.gameObject.tag)
        {
            case "LevelEditController":
                Highlight(0);
                break;
            case "LevelEditErase":
                Highlight(1);
                break;
        }


		if (other.gameObject.tag.StartsWith ("LevelEdit")) {
			//Debug.Log ("entering lemming");
			other.gameObject.GetComponent<LEController> ().currentLemming = lem;
		}

        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag.StartsWith("LevelEdit"))
        {
            LEController sphere = other.gameObject.GetComponent<LEController>();
            if (sphere.currentBlock == this)
            {
                sphere.currentBlock = sphere.lastBlock;
                sphere.lastBlock = null;
            }
        }

        if (lg.gameMode && (other.gameObject.tag == "LevelEditErase" || ID != 0) )
			return;
		
        if (other.gameObject.tag == "LevelEditController" || other.gameObject.tag == "LevelEditErase" )
        {
            Unhighlight();
        }


		if (other.gameObject.tag.StartsWith ("LevelEdit")) {
			//Debug.Log ("exiting lemming");
			other.gameObject.GetComponent<LEController> ().currentLemming = lem;
		}

    }

	public void Highlight(int i)
    {
        mr.enabled = true;
        switch (i)
        {
            case 0: 
                mr.material = highlightMat;
                break;
            case 1:
                mr.material = eraseMat;
                break;
			default:
				mr.material = blockMats[i];
				break;
        }
    }

    public void Unhighlight()
    {
        mr.material = oldMat;
        if (ID == 0 || ID == 6) mr.enabled = false;
    }



	public static bool IsSolid(int ID){
		switch (ID) {
		case 0:
			return false;
		case 6:
			return false;
		default:
			return true;
		}
	}

}
