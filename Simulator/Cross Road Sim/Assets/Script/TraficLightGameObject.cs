using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraficLightGameObject : MonoBehaviour
{
    [SerializeField]
    private TraficLight traficLight;
    public TraficLight TraficLight
    {
        get
        {
            return traficLight;
        }
        set
        {
            traficLight = value;
            UpdateLight();
        }
    }
    private SpriteRenderer spriteRenderer;
    private new Collider2D collider2D;

    [SerializeField]
    private string dependsOn = "";
    /*
    private string lightName = "NULL";
    public string LightName { get { return lightName; } }

    private string status = 
    */
    // Use this for initialization
    void Start ()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        collider2D = GetComponent<Collider2D>();
        UpdateLight();

    }
	
	// Update is called once per frame
	void Update ()
    {
        Special();
    }

    private void Special()
    {
        if(string.IsNullOrEmpty(dependsOn))
        {
            return;
        }

        bool doBlock = false;
        TraficLightController traficLightController = FindObjectOfType<TraficLightController>();
        for (int i = 0; i < traficLightController.TraficLightGameObjects.Length; i++)
        {
            if (traficLightController.TraficLightGameObjects[i].traficLight.light[0] == dependsOn[0])
            {
                if (traficLightController.TraficLightGameObjects[i].traficLight.status == "green")
                {
                    doBlock = true;
                    break;
                }
            }
        }

        if (doBlock)
        {
            traficLight.status = "red";
        }
        else
        {
            traficLight.status = "green";
        }
        UpdateLight();
    }

    public void UpdateLight()
    {
        if(spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if(spriteRenderer == null)
            {
                Debug.LogError("No indicator found...");
                return;
            }
        }

        switch(traficLight.status)
        {
            case "red":
                spriteRenderer.color = Color.red;
                if (collider2D != null)
                    collider2D.enabled = true;
                break;
            case "orange":
                spriteRenderer.color = new Color32(253,106,2,255);
                if (collider2D != null)
                    collider2D.enabled = true;
                break;
            case "green":
                spriteRenderer.color = Color.green;
                if (collider2D != null)
                    collider2D.enabled = false;
                break;

            default:
                Debug.Log(traficLight.light);
                spriteRenderer.color = Color.red;
                if (collider2D != null)
                    collider2D.enabled = true;
                break;
        }
    }
}
