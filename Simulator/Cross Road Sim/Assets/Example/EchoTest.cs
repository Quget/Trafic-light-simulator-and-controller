using UnityEngine;
using System.Collections;
using System;

public class EchoTest : MonoBehaviour
{
    
	// Use this for initialization
	IEnumerator Start ()
    {
		WebSocket webSocket = new WebSocket(new Uri("ws://echo.websocket.org"));
		yield return StartCoroutine(webSocket.Connect());
		webSocket.SendString("Hi there");
		int i=0;
		while (true)
		{
			string reply = webSocket.RecvString();
			if (reply != null)
			{
				Debug.Log ("Received: "+reply);
				webSocket.SendString("Hi there"+i++);
			}
			if (webSocket.error != null)
			{
				Debug.LogError ("Error: "+webSocket.error);
				break;
			}
			yield return 0;
		}
		webSocket.Close();
	}
}
