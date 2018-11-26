using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{

    [SerializeField]
    [Tooltip("Time for the camera to reach its target")]
    private float timeToReachTarget = 2;
    private float bezierTimer = 0;

    //private Vector3 startPos;
    CarSpawner carSpawner;
    // Use this for initialization
    void Start ()
    {

    }
	
	// Update is called once per frame
	void Update ()
    {
        if(carSpawner != null)
        {
            bezierTimer += Time.deltaTime / timeToReachTarget;
            if (bezierTimer > 1f)
                bezierTimer = 1f;

            Vector3 bCurvePos = carSpawner.CalculateBezierPoint(bezierTimer, carSpawner.transform.position, carSpawner.curveOne.position, carSpawner.curveTwo.position, carSpawner.endTarget.position);
            Vector3 lookPos = bCurvePos;
            lookPos.y = transform.position.y;
            transform.position = bCurvePos;
        }

    }

    public void Init(CarSpawner carSpawner)
    {
        this.carSpawner = carSpawner;
    }
}
