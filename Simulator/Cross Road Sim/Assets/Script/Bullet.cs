using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        GetComponent<Rigidbody2D>().AddForce(transform.right * 20);
        if(!GetComponent<Renderer>().isVisible)
        {
            Destroy(this.gameObject);
        }
        //transform.Translate(transform.forward * (50 * Time.deltaTime));
	}
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(collision.collider.gameObject);
        Destroy(this.gameObject);
    }
}
