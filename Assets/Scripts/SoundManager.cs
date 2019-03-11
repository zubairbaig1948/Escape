using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

    public AudioClip[] allClips;
    AudioSource source;

    public static SoundManager instance;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(instance);
    }
	// Use this for initialization
	void Start () {
        source = GetComponent<AudioSource>();
	}

    public void PlaySound(AudioClip clip,bool isLoop)
    {
        source.clip = clip;
        source.loop = isLoop;
        source.Play();
    }

    public void StopSound()
    {
        source.Stop();    
    }
	
}
