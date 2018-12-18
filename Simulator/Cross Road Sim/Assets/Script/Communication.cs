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

    [SerializeField]
    private bool testMode = false;

    [SerializeField]
    private float spawnMultiplier = 1;

    public float SpawnMultiplier { get { return spawnMultiplier; } }

    private bool isRunning = false;
    private Queue<Action> testActions = new Queue<Action>();
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
            testMode = !testMode;
        }
        /*
		if(Input.GetKeyDown(KeyCode.E))
        {
            List<TraficLight> traficLights = new List<TraficLight>();

            TraficLight traficLight = new TraficLight();
            traficLights.Add(TestLight("A1","green"));
            traficLights.Add(TestLight("A2", "green"));
            traficLights.Add(TestLight("A3", "green"));

            traficLights.Add(TestLight("A8", "red"));
            traficLights.Add(TestLight("A9", "red"));
            traficLights.Add(TestLight("A10", "red"));

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

            traficLights.Add(TestLight("A8", "green"));
            traficLights.Add(TestLight("A9", "green"));
            traficLights.Add(TestLight("A10", "green"));

            string json = JsonHelper.ToJson(traficLights.ToArray());
            json = json.Remove(0, 9);
            json = json.Remove(json.Length - 1, 1);

            TestReceive(json);
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            List<TraficLight> traficLights = new List<TraficLight>();

            TraficLight traficLight = new TraficLight();
            traficLights.Add(TestLight("D1", "green"));
            traficLights.Add(TestLight("A5", "green"));
            traficLights.Add(TestLight("A4", "green"));

            traficLights.Add(TestLight("A6", "red"));
            traficLights.Add(TestLight("A7", "red"));

            string json = JsonHelper.ToJson(traficLights.ToArray());
            json = json.Remove(0, 9);
            json = json.Remove(json.Length - 1, 1);

            TestReceive(json);
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            List<TraficLight> traficLights = new List<TraficLight>();

            TraficLight traficLight = new TraficLight();
            traficLights.Add(TestLight("D1", "red"));
            traficLights.Add(TestLight("A5", "red"));
            traficLights.Add(TestLight("A4", "red"));

            traficLights.Add(TestLight("A6", "green"));
            traficLights.Add(TestLight("A7", "green"));

            string json = JsonHelper.ToJson(traficLights.ToArray());
            json = json.Remove(0, 9);
            json = json.Remove(json.Length - 1, 1);

            TestReceive(json);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            List<TraficLight> traficLights = new List<TraficLight>();

            TraficLight traficLight = new TraficLight();
            traficLights.Add(TestLight("F1", "red"));

            string json = JsonHelper.ToJson(traficLights.ToArray());
            json = json.Remove(0, 9);
            json = json.Remove(json.Length - 1, 1);

            TestReceive(json);
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            List<TraficLight> traficLights = new List<TraficLight>();

            TraficLight traficLight = new TraficLight();
            traficLights.Add(TestLight("F1", "green"));

            string json = JsonHelper.ToJson(traficLights.ToArray());
            json = json.Remove(0, 9);
            json = json.Remove(json.Length - 1, 1);

            TestReceive(json);
        }
        */
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

            if(testMode)
            {
                testActions.Enqueue(() =>
                {
                    StartCoroutine(TestLight(data));
                });
                //StartCoroutine(TestLight(data));
                if(testActions.Count == 1 && !isRunning)
                {
                    testActions.Dequeue().Invoke();
                }
            }

            return;
        }
        Debug.Log("Sending" + data);
        webSocket.SendString(data);
    }

    private IEnumerator TestLight(string data)
    {
        isRunning = true;
        data = data.Remove(0, 2);
        data = data.Remove(data.Length - 2, 2);
        List<TraficLight> traficLights = new List<TraficLight>();
        TraficLightGameObject[] traficLightGameObjects = FindObjectsOfType<TraficLightGameObject>();
        for (int i = 0; i < traficLightGameObjects.Length; i++)
        {
            if (traficLightGameObjects[i].TraficLight.light == data)
            {
                traficLightGameObjects[i].TraficLight.status = "green";
            }
            else
            {
                traficLightGameObjects[i].TraficLight.status = "red";
            }
            traficLights.Add(traficLightGameObjects[i].TraficLight);
        }
        string json = JsonHelper.ToJson(traficLights.ToArray());
        json = json.Remove(0, 9);
        json = json.Remove(json.Length - 1, 1);
        TestReceive(json);
        yield return new WaitForSeconds(UnityEngine.Random.Range(2f, 5));
        isRunning = false;
        if (testActions.Count != 0)
            testActions.Dequeue().Invoke();

    }
}
