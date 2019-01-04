using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerSounds : MonoBehaviour {

    public AudioSource[] tileClicks = new AudioSource[3];
    public AudioSource[] jobClicks = new AudioSource[3];
    public AudioSource jobSelect;

	// Use this for initialization
	void Start ()
    {
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void PlayTileClick()
    {
        tileClicks[Random.Range(0, 3)].Play();
    }

    public void PlayJobClick()
    {
        jobClicks[Random.Range(0, 3)].Play();
    }

    public void PlayJobSelect()
    {
        jobSelect.Play();
    }
}
