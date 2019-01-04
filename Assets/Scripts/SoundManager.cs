using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

    public AudioSource[] sounds;
    public AudioSource ow;
    public AudioSource yay;

	// Use this for initialization
	void Start ()
    {
        sounds = GetComponents<AudioSource>();
        ow = sounds[5];
        yay = sounds[6];
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
