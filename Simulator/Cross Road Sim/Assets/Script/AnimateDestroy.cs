using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateDestroy : MonoBehaviour {

    [SerializeField]
    private AudioClip explosionSound;
	// Use this for initialization
	void Start ()
    {
        AudioSource.PlayClipAtPoint(explosionSound, transform.position);
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}
    public void KillMe()
    {
        Destroy(transform.parent.gameObject);
    }
}
