using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateDestroy : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
		
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
