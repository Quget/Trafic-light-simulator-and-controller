﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraficLightController : MonoBehaviour
{
    private TraficLightGameObject[] traficLightGameObjects;

    public TraficLightGameObject[] TraficLightGameObjects { get { return traficLightGameObjects; } }
    private Communication communication;
    [SerializeField]
    private Player player;
	// Use this for initialization
	void Start ()
    {
        traficLightGameObjects = FindObjectsOfType<TraficLightGameObject>();
        communication = FindObjectOfType<Communication>();

        communication.OnReceived += Communication_OnReceived;
    }

    private void Communication_OnReceived(string data)
    {
        try
        {
            data = "{\"Items\":" + data + "}";
            TraficLight[] traficLights = JsonHelper.FromJson<TraficLight>(data);
            if (traficLights != null && traficLights.Length > 0)
            {
                for (int i = 0; i < traficLights.Length; i++)
                {
                    List<TraficLightGameObject> traficLightGameObjects = FindTheLight(traficLights[i].light);
                    if (traficLightGameObjects.Count == 0)
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
        catch(Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    private List<TraficLightGameObject> FindTheLight(string lightName)
    {
        List<TraficLightGameObject> traficLights = new List<TraficLightGameObject>();
        for (int i = 0; i < traficLightGameObjects.Length; i++)
        {
            if(traficLightGameObjects[i].TraficLight.light == lightName)
            {
                traficLights.Add(traficLightGameObjects[i]);
            }
        }
        return traficLights;
    }

    // Update is called once per frame
    void Update ()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if(player != null)
            {
                player.gameObject.SetActive(!player.gameObject.activeSelf);
            }
        }

    }
}
