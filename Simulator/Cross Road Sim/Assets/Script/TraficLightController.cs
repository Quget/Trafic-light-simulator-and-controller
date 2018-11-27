using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraficLightController : MonoBehaviour
{
    private TraficLightGameObject[] traficLightGameObject;
    private Communication communication;
	// Use this for initialization
	void Start ()
    {
        traficLightGameObject = FindObjectsOfType<TraficLightGameObject>();
        communication = FindObjectOfType<Communication>();

        communication.OnReceived += Communication_OnReceived;
    }

    private void Communication_OnReceived(string data)
    {

        data = "{\"Items\":" + data + "}";
        TraficLight[] traficLights = JsonHelper.FromJson<TraficLight>(data);
        if(traficLights != null && traficLights.Length > 0)
        {
            for(int i =0; i < traficLights.Length; i++)
            {
                List<TraficLightGameObject> traficLightGameObjects = FindTheLight(traficLights[i].light);
                if(traficLightGameObjects.Count == 0)
                {
                    Debug.Log("ADD THIS LIGHT!" + traficLights[i].light);
                }
                else
                {
                    for (int j = 0; j < traficLightGameObjects.Count; j++)
                    {
                        traficLightGameObjects[j].TraficLight = traficLights[i];
                    }
                }
            }

        }
    }

    private List<TraficLightGameObject> FindTheLight(string lightName)
    {
        List<TraficLightGameObject> traficLights = new List<TraficLightGameObject>();
        for (int i = 0; i < traficLightGameObject.Length; i++)
        {
            if(traficLightGameObject[i].TraficLight.light == lightName)
            {
                traficLights.Add(traficLightGameObject[i]);
            }
        }
        return traficLights;
    }

    // Update is called once per frame
    void Update ()
    {
		
	}
}
