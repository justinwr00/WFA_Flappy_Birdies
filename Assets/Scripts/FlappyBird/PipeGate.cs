using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeGate : MonoBehaviour {

    private AudioSource Asource;

	// Use this for initialization
	void Start () {
        Asource = GetComponent<AudioSource>();
	}
	
	public void PlayScoreSound()
    {
        Asource.Play();
    }
}
