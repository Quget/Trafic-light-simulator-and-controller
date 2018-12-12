using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        Player player = FindObjectOfType<Player>();
        if (player != null && !player.DestroyMode)
        {
            MovingObject[] movingObjects = GameObject.FindObjectsOfType<MovingObject>();
            for (int i = 0; i < movingObjects.Length; i++)
            {
                Physics2D.IgnoreCollision(GetComponent<Collider2D>(),
                    movingObjects[i].GetComponent<Collider2D>());

                Physics2D.IgnoreCollision(player.GetComponent<Collider2D>(),
                    movingObjects[i].GetComponent<Collider2D>());
            }

            TraficLightGameObject[] traficLightGameObject = GameObject.FindObjectsOfType<TraficLightGameObject>();
            for (int i = 0; i < traficLightGameObject.Length; i++)
            {
                Physics2D.IgnoreCollision(GetComponent<Collider2D>(),
                    traficLightGameObject[i].GetComponent<Collider2D>());
            }
        }
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
        Player player = FindObjectOfType<Player>();
        if (player != null && !player.DestroyMode)
        {
            TraficLightGameObject[] traficLightGameObject = GameObject.FindObjectsOfType<TraficLightGameObject>();
            for (int i = 0; i < traficLightGameObject.Length; i++)
            {

                float distance = Vector3.Distance(transform.position,
                    traficLightGameObject[i].transform.position);
                if (distance < 0.25f)
                {
                    if (traficLightGameObject[i].TraficLight.status == "green")
                    {
                        traficLightGameObject[i].TraficLight.status = "red";
                    }
                    else
                    {
                        traficLightGameObject[i].TraficLight.status = "green";

                    }
                    traficLightGameObject[i].UpdateLight();
                    Destroy(this.gameObject);
                    return;
                }
            }
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Player player = FindObjectOfType<Player>();
        if (player != null && player.DestroyMode)
        {
            Destroy(collision.collider.gameObject);
        }
        //Destroy(this.gameObject);
    }
}
