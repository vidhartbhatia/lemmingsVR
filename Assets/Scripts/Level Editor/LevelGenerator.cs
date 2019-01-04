using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using Valve.VR;

public class LevelGenerator : MonoBehaviour {

	[SerializeField]
    string levelID;
    TextAsset file;

	public bool gameMode = false;
    public GameObject parent;

    public string inputText = "";

	public int maxLemmings = 20;
    public int numLemmings = 20;
    public int lemmingsObj = 10;
	public Material[] job_materials;
    LemmingPool lp;

    LevelTracker lt;

    public float scale = 1.0f;

    public GameObject levelCubeObject;
    public GameObject indicatorObject;
    GameObject indicator;
	public LeftHandUISelector leftHand;

    public float yDiff = 0.0f;


    //**** JOB CODES ****//
    // 0 - Blocker
    // 1 - Bomb
    // 2 - Digger
    // 3 - Parachute
    // 4 - Bridger
    public int[] jobs;
    public int numJobs = 5;

    public Vector3 dimensions;

    public LevelCube[,,] blocks;

    //*** LEVEL CUBE CODES ***
    // 0 - Air
    // 1 - Grass
    // 2 - Spawner xPos
    // 3 - Spawner xNeg
    // 4 - Spawner zPos
    // 5 - Spawner zNeg
    // 6 - Exit

	// Use this for initialization
	public void Begin ()
    {
        lt = GameObject.FindGameObjectWithTag("Level Tracker").GetComponent<LevelTracker>();
        if( gameMode ) levelID = lt.levels[lt.currentLevel];
        lp = GameObject.Find("Lemming Pool").GetComponent<LemmingPool>();
        blocks = new LevelCube[(int)dimensions.x, (int)dimensions.y, (int)dimensions.z];
        file = (TextAsset)Resources.Load("Levels/" + levelID, typeof(TextAsset));
        if (!gameMode) 
			EditorStart();
        else
		{
            if( file == null )
            {
                print("Fail - No level!");
            }
            else
            {
                LoadLevel();
            }
        }
	}

    void EditorStart()
    {
        indicator = Instantiate(indicatorObject);
        indicator.transform.position = new Vector3(scale * (dimensions.x - 1) / 2, scale * (dimensions.y - 1) / 2, scale * (dimensions.z - 1) / 2);
        indicator.transform.localScale = new Vector3(scale * dimensions.x, scale * dimensions.y, scale * dimensions.z);
        if (file == null) NewLevelEditor();
        else LoadLevel();
    }

	void OnApplicationQuit(){
        if (!gameMode) Save();
	}
	
    private void NewLevelEditor()
    {
        print("New Level");
        jobs = new int[numJobs];
        for (int x = 0; x < dimensions.x; x++)
        {
            for (int y = 0; y < dimensions.y; y++)
            {
                for (int z = 0; z < dimensions.z; z++)
                {
                    blocks[x, y, z] = Instantiate(levelCubeObject).GetComponent<LevelCube>();
                    // parent to parent
                    blocks[x, y, z].gameObject.transform.localScale = new Vector3(scale, scale, scale);
					blocks[x, y, z].Setup(0, x, y, z);
                    blocks[x, y, z].gameObject.transform.position = new Vector3(x*scale, y*scale, z*scale);
                }
            }
        }
    }

    private void LoadLevel()
    {
        print("Load");
        string level = file.ToString();
        string[] levelTextSigns = level.Split(new char[] { '`' });
        string[] levelJobs = levelTextSigns[0].Split(new char[] { '=' });
        jobs = new int[numJobs];
        for( int i = 0; i < levelJobs.Length-1; i++)
        {
            jobs[i] = int.Parse(levelJobs[i]);
        }
        string[] info = levelJobs[levelJobs.Length - 1].Split(new char[] { '~' });
        lp.numLemmings = int.Parse(info[0]);
        numLemmings = lp.numLemmings;
		maxLemmings = numLemmings;
        lp.lemmingsObj = int.Parse(info[1]);
        lemmingsObj = lp.lemmingsObj;
        string[] planes = info[2].Split(new char[] {'|'});
        string[,] rows = new string[(int)dimensions.x, (int)dimensions.y];
        for( int x = 0; x < planes.Length; x++ )
        {
            string[] temp = planes[x].Split(new char[] {'\n'});
            for( int y = 0; y < temp.Length; y++ )
            {
                rows[x, y] = temp[y];
            }
        }
        string[,,] blockStrings = new string[(int)dimensions.x, (int)dimensions.y, (int)dimensions.z];
        for( int x = 0; x < dimensions.x; x++ )
        {
            for( int y = 0; y < dimensions.y; y++ )
            {
                string[] temp = rows[x,y].Split(new char[] { ',' });
                for( int z = 0; z < dimensions.z; z++ )
                {
                    blockStrings[x, y, z] = temp[z];
                }
            }
        }
        for (int x = 0; x < dimensions.x; x++)
        {
            for (int y = 0; y < dimensions.y; y++)
            {
                for (int z = 0; z < dimensions.z; z++)
                {
                    blocks[x, y, z] = Instantiate(levelCubeObject).GetComponent<LevelCube>();
                    // parent to parent
                    blocks[x, y, z].gameObject.transform.parent = parent.transform;
                    blocks[x, y, z].gameObject.transform.localScale = new Vector3(scale, scale, scale);
					blocks[x, y, z].Setup(int.Parse(blockStrings[x,y,z]), x, y, z);
                    blocks[x, y, z].gameObject.transform.position = new Vector3(x*scale, y*scale, z*scale);
                }
            }
        }
        //Add text signs
        for( int i = 1; i < levelTextSigns.Length; i++ )
        {
            string[] levelText = levelTextSigns[i].Split(new char[] { '>' });
            int x = int.Parse(levelText[1]);
            int y = int.Parse(levelText[2]);
            int z = int.Parse(levelText[3]);
			blocks[x, y, z].AddText(levelText[0]);
        }
    }

    // Update is called once per frame
    void Update ()
    {
	    if( Input.GetKeyDown(KeyCode.X) )
            Save();
		if (SteamVR_Input._default.inActions.GrabGrip.GetStateDown(SteamVR_Input_Sources.LeftHand)) {
			if(!gameMode) Save();
            else SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
		
	}

    void Save()
    {
        int index = 0;
        string[] textStrings = new string[50];
        for( int i = 0; i < 50; i++ )
        {
            textStrings[i] = "";
        }
        int[] textStringXs = new int[50];
        int[] textStringYs = new int[50];
        int[] textStringZs = new int[50];
        string output = "";
        for( int i = 0; i < jobs.Length; i++)
        {
            output += jobs[i].ToString() + "=";
        }
        output += numLemmings.ToString() + "~" + lemmingsObj.ToString() + "~";
        //Read block IDs
        for( int x  = 0; x < dimensions.x; x++ )
        {
            for( int y = 0; y < dimensions.y; y++ )
            {
                for( int z = 0; z < dimensions.z; z++ )
                {
                    output += blocks[x, y, z].ID;
                    if ( blocks[x, y, z].textEnabled)
                    {
                        textStrings[index] = blocks[x, y, z].text.text;
                        textStringXs[index] = x;
                        textStringYs[index] = y;
                        textStringZs[index] = z;
                        index++;
                    }
                    if (z < dimensions.z - 1) output += ",";
                }
                if( y < dimensions.y-1 ) output += "\n";
            }
            if( x < dimensions.x-1 ) output += "|";
        }
        //Add text signs
        for( int i = 0; i < 50; i++ )
        {
            if( textStrings[i] != "" )
            {
                output += "`" + textStrings[i] + ">" + textStringXs[i].ToString() + ">" + textStringYs[i].ToString() + ">" + textStringZs[i].ToString();
            }
        }
        System.IO.File.WriteAllText("Assets/Resources/Levels/" + levelID + ".txt", output);
        AssetDatabase.ImportAsset("Assets/Resources/Levels/" + levelID + ".txt");
    }


}
