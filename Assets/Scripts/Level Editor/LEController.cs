using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LEController : MonoBehaviour {

    public float speed = 0.5f;

	public LevelCube currentBlock;
    public LevelCube lastBlock;
	public Lemming currentLemming;
	public LevelGenerator LG;

	int totalMinX = 0;
	int totalMinY = 0;
	int totalMinZ = 0;
	int totalMaxX = 0;
	int totalMaxY = 0;
	int totalMaxZ = 0;


	public LevelCube startBlock = null;

	// Use this for initialization
	void Start ()
    {
		
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void Activate()
    {
		startBlock = currentBlock;
    }

	public void FillTemps(){
		int currMinX = Mathf.Min(startBlock.x, currentBlock.x);
		int currMinY = Mathf.Min(startBlock.y, currentBlock.y);
		int currMinZ = Mathf.Min(startBlock.z, currentBlock.z);
		int currMaxX = Mathf.Max(startBlock.x, currentBlock.x);
		int currMaxY = Mathf.Max(startBlock.y, currentBlock.y);
		int currMaxZ = Mathf.Max(startBlock.z, currentBlock.z);

		totalMinX = Mathf.Min(currMinX, totalMinX);
		totalMinY = Mathf.Min(currMinY, totalMinY);
		totalMinZ = Mathf.Min(currMinZ, totalMinZ);
		totalMaxX = Mathf.Max(currMaxX, totalMaxX);
		totalMaxY = Mathf.Max(currMaxY, totalMaxY);
		totalMaxZ = Mathf.Max(currMaxZ, totalMaxZ);

		for(int x = totalMinX; x <= totalMaxX; x++){
			for(int y = totalMinY; y <= totalMaxY; y++){
				for(int z = totalMinZ; z <= totalMaxZ; z++){
					LG.blocks [x, y, z].Unhighlight ();
				}
			}
		}

		for(int x = currMinX; x <= currMaxX; x++){
			for(int y = currMinY; y <= currMaxY; y++){
				for(int z = currMinZ; z <= currMaxZ; z++){
					if (gameObject.tag == "LevelEditController")
						LG.blocks [x, y, z].Highlight(0);
					if (gameObject.tag == "LevelEditErase") 
						LG.blocks [x, y, z].Highlight(1);
				}
			}
		}

	}

	public void Deactivate(int fillBlock){
		int currMinX = Mathf.Min(startBlock.x, currentBlock.x);
		int currMinY = Mathf.Min(startBlock.y, currentBlock.y);
		int currMinZ = Mathf.Min(startBlock.z, currentBlock.z);
		int currMaxX = Mathf.Max(startBlock.x, currentBlock.x);
		int currMaxY = Mathf.Max(startBlock.y, currentBlock.y);
		int currMaxZ = Mathf.Max(startBlock.z, currentBlock.z);

		totalMinX = Mathf.Min(currMinX, totalMinX);
		totalMinY = Mathf.Min(currMinY, totalMinY);
		totalMinZ = Mathf.Min(currMinZ, totalMinZ);
		totalMaxX = Mathf.Max(currMaxX, totalMaxX);
		totalMaxY = Mathf.Max(currMaxY, totalMaxY);
		totalMaxZ = Mathf.Max(currMaxZ, totalMaxZ);

		for(int x = totalMinX; x <= totalMaxX; x++){
			for(int y = totalMinY; y <= totalMaxY; y++){
				for(int z = totalMinZ; z <= totalMaxZ; z++){
					LG.blocks [x, y, z].Unhighlight ();
				}
			}
		}

		for(int x = currMinX; x <= currMaxX; x++){
			for(int y = currMinY; y <= currMaxY; y++){
				for(int z = currMinZ; z <= currMaxZ; z++){
					if (gameObject.tag == "LevelEditController") PlaceBlock(LG.blocks[x,y,z], fillBlock);
					if (gameObject.tag == "LevelEditErase") PlaceBlock(LG.blocks[x,y,z], 0);
				}
			}
		}

        startBlock = null;
	}

	private void PlaceBlock(LevelCube block, int ID)
    {
        block.Change(ID);
    }



}
