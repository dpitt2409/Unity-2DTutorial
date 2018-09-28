using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader : MonoBehaviour {

    public GameObject gameManager;

	// Use this for initialization
	void Awake ()
    {
        //If the game manager has not already been instantiated, instantiate it.
        if (GameManager.instance == null)
            Instantiate(gameManager);
	}
	
	
}
