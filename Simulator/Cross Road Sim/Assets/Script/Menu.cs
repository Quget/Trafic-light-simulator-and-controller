using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Menu : MonoBehaviour
{
    [SerializeField]
    private Communication communication;

    [SerializeField]
    private InputField inputField;

    [SerializeField]
    private Text debugText;
	// Use this for initialization
	void Start ()
    {
		if(communication == null)
        {
            communication = FindObjectOfType<Communication>();
        }
	}

    private void OnEnable()
    {
        communication.OnReceived += Communication_OnReceived;
        communication.OnError += Communication_OnError;
    }

    private void OnDisable()
    {
        communication.OnReceived -= Communication_OnReceived;
        communication.OnError -= Communication_OnError;
    }

    private void Communication_OnError(string error)
    {
        debugText.text += error + "\n";
        //throw new System.NotImplementedException();
    }

    private void Communication_OnReceived(string data)
    {
        debugText.text += data + "\n";

        try
        {
            TraficLight[] light = JsonUtility.FromJson(data, typeof(TraficLight[])) as TraficLight[];
            if (light != null)
            {
                Debug.Log(light.Length);
            }
        }
        catch(Exception e)
        {
            Debug.Log(e.Message);
        }
        //Debug.Log(light.status + ":" + light.light + ":" + light.timer);
        //throw new System.NotImplementedException();
    }



    // Update is called once per frame
    void Update ()
    {
		
	}

    public void SendTest()
    {
        TraficLight traficLight = new TraficLight();
        traficLight.light = "A1";
        traficLight.status = "Green";
        traficLight.timer = 15.5f;

        string jsonString = JsonUtility.ToJson(traficLight);
        Debug.Log(jsonString);
        communication.Send(jsonString);
    }

    public void OnConnectClick()
    {
        communication.Connect(inputField.text);
    }
}
