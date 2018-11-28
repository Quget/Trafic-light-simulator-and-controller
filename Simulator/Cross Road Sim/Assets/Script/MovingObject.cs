using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObject : MonoBehaviour
{


    public delegate void OnRemovedEvent(MovingObject movingObject);
    public event OnRemovedEvent OnRemoved;

    [SerializeField]
    [Tooltip("Time for the object to reach its target")]
    private float timeToReachTarget = 2;
    private float bezierTimer = 0;

    //private Vector3 startPos;
    Spawner carSpawner;

    private bool stop = false;

    [SerializeField]
    private GameObject explosionPrefab;


    [SerializeField]
    private bool ignoreCollision = false;

    [SerializeField]
    private string lightName = "A";
    // Use this for initialization
    void Start ()
    {
        /*
        TraficLightGameObject[] traficLightGameObjects = FindObjectsOfType<TraficLightGameObject>();
        for(int i = 0; i < traficLightGameObjects.Length;i++)
        {
            bool isInName = false;
            for (int j = 0; j < lightName.Length; j++)
            {
                if (lightName[j] == traficLightGameObjects[i].TraficLight.light[0])
                {
                    isInName = true;
                }
            }
            //if(!isInName)
            {
                Physics2D.IgnoreCollision(traficLightGameObjects[i].GetComponent<Collider2D>(),
                    GetComponent<Collider2D>());
            }
        }
        */
    }
	
	// Update is called once per frame
	void Update ()
    {
        if(carSpawner != null)
        {
            if (!ignoreCollision)
            {
                GetComponent<Collider2D>().enabled = false;
                //RaycastHit2D raycastHit2D = Physics2D.Raycast(transform.position, transform.right, 1);
                RaycastHit2D raycastHit2D = Physics2D.CircleCast(transform.position, 0.15f, transform.right, 0.5f);
                if (raycastHit2D)
                {
                    TraficLightGameObject traficLightGameObject = raycastHit2D.collider.GetComponent<TraficLightGameObject>();
                    if(traficLightGameObject != null)
                    {
                        for(int i = 0; i < lightName.Length; i++)
                        {
                            if(lightName[i] == traficLightGameObject.TraficLight.light[0])
                            {
                                if(!stop)
                                {
                                    stop = true;
                                    string[] participation = { traficLightGameObject.TraficLight.light, traficLightGameObject.TraficLight.light };
                                    string json = JsonHelper.ToJson<string>(participation);
                                    json = json.Remove(0, 9);
                                    json = json.Remove(json.Length-1, 1);

                                    FindObjectOfType<Communication>().Send(json);
                                }
                                break;
                            }
                        }
                    }
                    else
                    {
                        if (raycastHit2D.collider.gameObject.name == gameObject.name)
                            stop = true;
                    }
                }
                else
                {
                    stop = false;
                }
                GetComponent<Collider2D>().enabled = true;
            }

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
        }
    }

    private void SpawnExplosion()
    {
        GameObject explosion = Instantiate(explosionPrefab, null);
        explosion.transform.position = transform.position;
    }
    public void Init(Spawner carSpawner)
    {
        this.carSpawner = carSpawner;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        TraficLightGameObject traficLightGameObject = collision.gameObject.GetComponent<TraficLightGameObject>();
        if(traficLightGameObject != null)
        {
            Physics2D.IgnoreCollision(traficLightGameObject.GetComponent<Collider2D>(),
                GetComponent<Collider2D>());
            return;
        }

        SpawnExplosion();
        //Debug.Log("Explosion");
        Destroy(this.gameObject);
    }

    private void OnDestroy()
    {
        if (OnRemoved != null)
            OnRemoved(this);
    }
}
