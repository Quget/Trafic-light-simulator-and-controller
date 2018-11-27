﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Communication : MonoBehaviour
{

    public delegate void OnReceivedEvent(string data);
    public event OnReceivedEvent OnReceived;


    public delegate void OnErrorEvent(string error);
    public event OnErrorEvent OnError;

    [SerializeField]
    private string webSocketUrl = "ws://141.252.207.71:54000";

    private WebSocket webSocket;
    // Use this for initialization
    void Start ()
    {
        //webSocket = new WebSocket(new Uri(webSocketUrl));
        //Connect(webSocketUrl);
        //StartCoroutine(StartConnection());
	}
	
	// Update is called once per frame
	void Update ()
    {
		if(Input.GetKeyDown(KeyCode.E))
        {
            List<TraficLight> traficLights = new List<TraficLight>();

            TraficLight traficLight = new TraficLight();
            traficLights.Add(TestLight("A1","green"));
            traficLights.Add(TestLight("A2", "green"));
            traficLights.Add(TestLight("A3", "green"));

            traficLights.Add(TestLight("C3.1", "red"));
            traficLights.Add(TestLight("C3.2", "red"));
            traficLights.Add(TestLight("B3", "red"));

            string json = JsonHelper.ToJson(traficLights.ToArray());
            json = json.Remove(0, 9);
            json = json.Remove(json.Length - 1, 1);

            TestReceive(json);
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            List<TraficLight> traficLights = new List<TraficLight>();

            TraficLight traficLight = new TraficLight();
            traficLights.Add(TestLight("A1", "red"));
            traficLights.Add(TestLight("A2", "red"));
            traficLights.Add(TestLight("A3", "red"));

            traficLights.Add(TestLight("C3.1", "green"));
            traficLights.Add(TestLight("C3.2", "green"));
            traficLights.Add(TestLight("B3", "green"));

            string json = JsonHelper.ToJson(traficLights.ToArray());
            json = json.Remove(0, 9);
            json = json.Remove(json.Length - 1, 1);

            TestReceive(json);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            List<TraficLight> traficLights = new List<TraficLight>();

            TraficLight traficLight = new TraficLight();
            traficLights.Add(TestLight("E1", "red"));

            string json = JsonHelper.ToJson(traficLights.ToArray());
            json = json.Remove(0, 9);
            json = json.Remove(json.Length - 1, 1);

            TestReceive(json);
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            List<TraficLight> traficLights = new List<TraficLight>();

            TraficLight traficLight = new TraficLight();
            traficLights.Add(TestLight("E1", "green"));

            string json = JsonHelper.ToJson(traficLights.ToArray());
            json = json.Remove(0, 9);
            json = json.Remove(json.Length - 1, 1);

            TestReceive(json);
        }
    }
    private TraficLight TestLight(string name, string status, float timer = 0)
    {
        TraficLight traficLight = new TraficLight();
        traficLight.light = name;
        traficLight.status = status;
        traficLight.timer = timer;
        return traficLight;
    }

    public void Connect(string webSocketUrl)
    {
        if(webSocket != null)
        {
            webSocket.Close();
            webSocket = null;
        }
        webSocket = new WebSocket(new Uri(webSocketUrl));
        StartCoroutine(StartConnection());
    }

    IEnumerator StartConnection()
    {   
        yield return StartCoroutine(webSocket.Connect());
        while (webSocket != null)
        {
            string reply = webSocket.RecvString();
            if (reply != null)
            {
                Received(reply);
            }
            if (webSocket.error != null)
            {
                Debug.LogError("Error: " + webSocket.error);
                if(OnError != null)
                {
                    OnError(webSocket.error);
                }
                break;
            }
            yield return 0;
        }
        webSocket.Close();
    }

    private void Received(string data)
    {
        Debug.Log("Received: " + data);
        
        if(OnReceived != null)
        {
            OnReceived(data);
        }

    }
    public void TestReceive(string data)
    {
        Received(data);
    }

    public void Send(string data)
    {
        if(webSocket == null)
        {
            if(OnError != null)
            {
                OnError("Cannot send, websocket is null");
            }
            Debug.Log("Sending" + data);
            return;
        }
        webSocket.SendString(data);
    }
}
