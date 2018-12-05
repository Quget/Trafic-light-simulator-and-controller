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

    [SerializeField]
    private GameObject connectionMenu;
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
        Time.timeScale = 0;
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
        /*
        try
        {
            TraficLight[] lights = JsonUtility.FromJson<TraficLight[]>(data);
            if (lights != null)
            {
                if(lights.Length == 0)
                {
                    TraficLight light = JsonUtility.FromJson<TraficLight>(data);
                }
                Debug.Log(lights.Length);
            }
        }
        catch(Exception e)
        {
            Debug.Log(e.Message);
        }*/
        //Debug.Log(light.status + ":" + light.light + ":" + light.timer);
        //throw new System.NotImplementedException();
    }



    // Update is called once per frame
    void Update ()
    {
		if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(!connectionMenu.activeSelf)
            {
                Time.timeScale = 0.1f;
            }
            else
            {
                Time.timeScale = 1;
                Debug.Log("yo");
            }
            connectionMenu.SetActive(!connectionMenu.activeSelf);
        }
	}

    public void SendTest()
    {

        //[Serializable]
        /*
        List<string> testList = new List<string>();
        testList.Add("A1");
        testList.Add("A2");

        //string[] test = { "A1","A2","A3" };
        string jsonString = JsonUtility.ToJson(testList);
        Debug.Log(jsonString);
        communication.Send(jsonString);
        
        
        
        TraficLight traficLight = new TraficLight();
        traficLight.light = "A1";
        traficLight.status = "Green";
        traficLight.timer = 15.5f;

        string jsonString = JsonUtility.ToJson(traficLight);
        Debug.Log(jsonString);
        communication.Send(jsonString);
        */
    }

    public void OnConnectClick()
    {
        communication.Connect(inputField.text);
    }

    public void OnQuitClick()
    {
        Application.Quit();
    }
}
