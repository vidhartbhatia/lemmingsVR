using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Lemming : MonoBehaviour {

    LemmingPool lp;
    LevelTracker lt;
	LevelGenerator lg;
	MeshRenderer mr;
    SoundManager sm;

    //"xPos", "xNeg", "zPos", "zNeg"
    public string direction;
	public int x, y, z;

	public string job = "walker";
	private int fallingFor = 0;
	public bool digging = false;
	public bool hasUmbrella = false;
	public int wood = -1;

	public Animator anim;
    public GameObject armature;

	public int maxWalkFrames = 60;
	private int walkFrames;

	public GameObject explosionPrefab;

    // Use this for initialization
    void Start()
    {
        lt = GameObject.FindGameObjectWithTag("Level Tracker").GetComponent<LevelTracker>();
        sm = GameObject.Find("SoundManager").GetComponent<SoundManager>();
        lp = GameObject.Find("Lemming Pool").GetComponent<LemmingPool>();
		lg = GameObject.Find("LevelGenerator").GetComponent<LevelGenerator>();
        //anim = armature.GetComponent<Animator>();
        anim.Play("Walking");
		mr = GetComponent<MeshRenderer>();
		walkFrames = maxWalkFrames;
    }   
	
	// Update is called once per frame
	void Update ()
	{

		if (y == 0)
			Die (false);
		//transform.position = new Vector3(x*lg.scale, y*lg.scale, z*lg.scale); //update pos every frame, not most efficient, but easy
		//if(case on anim){
			
		//}
		//else
		if (job == "walker") {
			if (walkFrames > 0) {
				//walking
				walkFrames--;
				if (fallingFor != 0) {
                    anim.SetBool("Falling", true);
					transform.position -= new Vector3 (0, lg.scale / maxWalkFrames, 0);
				} else {
                    anim.SetBool("Falling", false);
					switch (direction) {
					case "xPos":
                            transform.position += new Vector3(lg.scale / maxWalkFrames, 0, 0);
                            transform.rotation = Quaternion.LookRotation(new Vector3(1,0,0), new Vector3(0,1,0));
						break;
					case "xNeg":
						transform.position -= new Vector3 (lg.scale / maxWalkFrames, 0, 0);
                            transform.rotation = Quaternion.LookRotation(new Vector3(-1, 0, 0), new Vector3(0, 1, 0));
                            break;
					case "zPos":
						transform.position += new Vector3 (0, 0, lg.scale / maxWalkFrames);
                            transform.rotation = Quaternion.LookRotation(new Vector3(0, 0, 1), new Vector3(0, 1, 0));
                            break;
					case "zNeg":
						transform.position -= new Vector3 (0, 0, lg.scale / maxWalkFrames);
                            transform.rotation = Quaternion.LookRotation(new Vector3(0, 0, -1), new Vector3(0, 1, 0));
                            break;
					}
				}
			} 
			else {
				//not walking
				walkFrames = 60;

                //change coords
				if (fallingFor == 0) {
					anim.SetBool("Parachuting",false);
					anim.SetBool("Falling",false);
					switch (direction) {
					case "xPos":
                        //step forward
                        //also set anim here
						lg.blocks [x, y, z].lem = null;
						lg.blocks [x + 1, y, z].lem = this;
						x++;
						break;
					case "xNeg":
                        //step forward
                        //also set anim here
						lg.blocks [x, y, z].lem = null;
						lg.blocks [x - 1, y, z].lem = this;
						x--;
						break;
					case "zPos":
                        //step forward
                        //also set anim here
						lg.blocks [x, y, z].lem = null;
						lg.blocks [x, y, z + 1].lem = this;
						z++;
						break;
					case "zNeg":
                        //step forward
                        //also set anim here
						lg.blocks [x, y, z].lem = null;
						lg.blocks [x, y, z - 1].lem = this;
						z--;
						break;
					}
				} 

				if (lg.blocks [x, y, z].umbrellas && !hasUmbrella) {
					hasUmbrella = true;
					transform.GetChild(1).gameObject.SetActive(true);
                    sm.sounds[3].Play();
				}

                //falling
				if (!LevelCube.IsSolid (lg.blocks [x, y - 1, z].ID) || digging) {
                    if (digging && LevelCube.IsSolid(lg.blocks[x, y - 1, z].ID))
					{
						anim.SetBool ("Digging", true);
                        lg.blocks[x, y - 1, z].Change(0);
                        transform.GetChild(5).gameObject.SetActive(true);
                    }
                    else if (digging)
					{
						anim.SetBool ("Digging", false);
                        digging = false;
                        transform.GetChild(5).gameObject.SetActive(false);
                    }
            

					if (fallingFor == 0 && wood > 0) {
						anim.SetBool ("Bridging", true);
						lg.blocks [x, y - 1, z].Change (7);//wood material?
						wood--;
						if (wood == 0) {
							transform.GetChild (2).gameObject.SetActive (false);
                            anim.SetBool("Bridging", false);
							wood = -1;
						}
					} else {
						anim.SetBool ("Bridging", false);
						lg.blocks [x, y, z].lem = null;
						y--;
						lg.blocks [x, y - 1, z].lem = this;
						fallingFor++;
					}
				}
				else{
				//if (LevelCube.IsSolid (lg.blocks [x, y - 1, z].ID)) {
					//die if fell too far
					if (fallingFor > 5) {
						if(!hasUmbrella){
							lg.blocks [x, y, z].lem = null;
							Die (false);
							return;
						}
						else{
							hasUmbrella = false;
							transform.GetChild(1).gameObject.SetActive(false);
						}
					}
					fallingFor = 0;


					//check if in blocker
					if (lg.blocks [x, y, z].blocker != null)
						direction = lg.blocks [x, y, z].blocker.direction;
					
					//turn around if in front of wall
					switch (direction) {
					case "xPos":
						if (x == lg.dimensions.x - 1 || LevelCube.IsSolid (lg.blocks [x + 1, y, z].ID)) {
							//turn around
							direction = "xNeg";
						}
						break;
					case "xNeg":
						if (x == 0 || LevelCube.IsSolid (lg.blocks [x - 1, y, z].ID)) {
							//turn around
							direction = "xPos";
						}
						break;
					case "zPos":
						if (z == lg.dimensions.z - 1 || LevelCube.IsSolid (lg.blocks [x, y, z + 1].ID)) {
							//turn around
							direction = "zNeg";
						}
						break;
					case "zNeg":
						if (z == 0 || LevelCube.IsSolid (lg.blocks [x, y, z - 1].ID)) {
							//turn around
							direction = "zPos";
						}
						break;
					}

					//update real world position
					//transform.position = new Vector3(x*lg.scale, y*lg.scale, z*lg.scale);
				}

				if (fallingFor > 0) {
					if (hasUmbrella) {
						anim.SetBool("Parachuting",true);
					} 
					else {
						anim.SetBool("Falling",true);
					}
				}

			}
		}
		else if(job == "blocker"){

		}
	}

	public void Die(bool success)
    {
        if (!success)
        {
            lp.lemmingsDead++;
            if (lp.lemmingsDead > lp.startLemmings - lp.lemmingsObj)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
            sm.ow.Play();
        }
        else sm.yay.Play();
		lg.blocks [x, y, z].lem = null;
		x = 0;
		y = 0;
		z = 0;
		gameObject.transform.position = new Vector3(0, 0, 0);
		lp.Take(this.gameObject);
        gameObject.SetActive(false);
    }

	public void Explode(){
		GameObject explosion = Instantiate(explosionPrefab, this.transform.position, this.transform.rotation);
		explosion.GetComponent<Explosion>().Play();
	}

	public void SetJob(string newJob, int newX, int newY, int newZ){
		job = newJob;
		if(job == "walker"){
			mr.material = lg.job_materials[0];
		}
		else if(job == "blocker"){
            lp.lemmingsDead++;
            if (lp.lemmingsDead > lp.startLemmings - lp.lemmingsObj)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
            mr.material = lg.job_materials[1];
            x = newX;
            y = newY;
            z = newZ;
			transform.position = new Vector3(newX*lg.scale, newY*lg.scale + lg.yDiff, newZ*lg.scale);
			transform.localScale = new Vector3(0.97f*transform.localScale.x,0.97f*transform.localScale.y,0.97f*transform.localScale.z);
			transform.GetChild(0).gameObject.SetActive(true);
			transform.GetChild(1).gameObject.SetActive(false);
			anim.SetBool("Blocking",true);
		}
		else if(job == "digger"){
			//enable the shovel
		}
	}

	void OnTriggerEnter(Collider other){
		//change tag when using future objects
		if (other.gameObject.tag.StartsWith ("LevelEdit")) {
			//Debug.Log ("entering lemming");
			other.gameObject.GetComponent<LEController> ().currentLemming = this;
		}
	}

	void OnTriggerExit(Collider other){
		//change tag when using future objects
		/*if (other.gameObject.tag.StartsWith ("LevelEdit")) {
			//Debug.Log ("exiting lemming");
			other.gameObject.GetComponent<LEController> ().currentLemming = null;
		}*/
	}

}
