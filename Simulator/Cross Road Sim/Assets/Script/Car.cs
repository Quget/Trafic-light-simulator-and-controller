using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{

    [SerializeField]
    [Tooltip("Time for the object to reach its target")]
    private float timeToReachTarget = 2;
    private float bezierTimer = 0;

    //private Vector3 startPos;
    CarSpawner carSpawner;

    private bool stop = false;

    [SerializeField]
    private GameObject explosionPrefab;
    // Use this for initialization
    void Start ()
    {

    }
	
	// Update is called once per frame
	void Update ()
    {
        if(carSpawner != null)
        {

            GetComponent<Collider2D>().enabled = false;
            //RaycastHit2D raycastHit2D = Physics2D.Raycast(transform.position, transform.right, 1);
            RaycastHit2D raycastHit2D = Physics2D.CircleCast(transform.position, 0.15f, transform.right, 0.5f);
            if (raycastHit2D)
            {
                //Debug.Log(raycastHit2D.collider.name);
                stop = true;
            }
            else
            {
                stop = false;
            }
            GetComponent<Collider2D>().enabled = true;

            if (!stop)
            {
                bezierTimer += Time.deltaTime / timeToReachTarget;
                if (bezierTimer > 1f)
                    bezierTimer = 1f;
            }
            else
            {
                return;
            }

            Vector3 bCurvePos = carSpawner.CalculateBezierPoint(bezierTimer, carSpawner.transform.position, carSpawner.curveOne.position, carSpawner.curveTwo.position, carSpawner.endTarget.position);
            Vector3 lookPos = bCurvePos;
            transform.right = bCurvePos - transform.position;
            //lookPos.x= transform.position.x;
            //transform.LookAt(Vector3.forward, Vector3.Cross(Vector3.forward, bCurvePos));
            //transform.LookAt(lookPos, transform.up);
            transform.position = bCurvePos;


            if(transform.position == carSpawner.endTarget.position)
            {
                Debug.Log("Target reached");
                Destroy(this.gameObject);
            }

            if(Input.GetKeyDown(KeyCode.Q))
            {
                stop = !stop;
            }
        }

    }

    private void SpawnExplosion()
    {
        GameObject explosion = Instantiate(explosionPrefab, null);
        explosion.transform.position = transform.position;
    }
    public void Init(CarSpawner carSpawner)
    {
        this.carSpawner = carSpawner;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        SpawnExplosion();
        //Debug.Log("Explosion");
        Destroy(this.gameObject);
    }
}
