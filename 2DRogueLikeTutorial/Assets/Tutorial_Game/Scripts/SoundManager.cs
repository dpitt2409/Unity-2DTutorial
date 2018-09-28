using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

    public AudioSource efxSource;
    public AudioSource musicSource;
    public static SoundManager instance = null;

    public float lowPitchRange = 0.95f;
    public float highPitchRange = 1.05f;

	// Use this for initialization
	void Awake () {
        //Ensure single instance of this object
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
	}
	

    public void PlaySingle(AudioClip clip)
    {
        efxSource.clip = clip;
        efxSource.Play();
    }

    //params keyword allows parameter to be an indetermined number of AudioClip objects
    public void RandomizeSfx (params AudioClip [] clips)
    {
        //Select a random clip from the given AudioClips
        int randomIndex = Random.Range(0, clips.Length);
        //Select a random pitch within the specified range
        float randomPitch = Random.Range(lowPitchRange, highPitchRange);
        //Set the randomized values to the efxSource and play it
        efxSource.pitch = randomPitch;
        efxSource.clip = clips[randomIndex];
        efxSource.Play();
    }

}
