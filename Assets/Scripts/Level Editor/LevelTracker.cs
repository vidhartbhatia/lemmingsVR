using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTracker : MonoBehaviour {

    public string[] levels;
    public int currentLevel;
    public int numLevels;
    public int startingLevel = 0;
    AudioSource[] music;
    int currentMusic = -1;

    bool created = false;

    // Use this for initialization
    void Awake ()
    {
        music = GetComponents<AudioSource>();
        numLevels = levels.Length;
        currentLevel = startingLevel;
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void UpdateMusic()
    {
        if( currentMusic != -1 )
          music[currentMusic].Stop();
        currentMusic = (currentMusic + 1) % 4;
        music[currentMusic].Play();
    }
}
