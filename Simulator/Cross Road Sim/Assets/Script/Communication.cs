using System;
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
        Connect(webSocketUrl);
        StartCoroutine(StartConnection());
	}
	
	// Update is called once per frame
	void Update ()
    {
		
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

    public void Send(string data)
    {
        if(webSocket == null)
        {
            if(OnError != null)
            {
                OnError("Cannot send, websocket is null");
            }
            return;
        }
        webSocket.SendString(data);
    }
}
