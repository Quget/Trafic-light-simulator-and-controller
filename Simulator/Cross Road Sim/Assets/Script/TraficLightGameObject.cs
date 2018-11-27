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
		if(Input.GetKeyDown(KeyCode.E))
        {
            if(traficLight.status == "green")
            {
                traficLight.status = "red";
            }
            else
            {
                traficLight.status = "green";
            }

            UpdateLight();
        }
	}

    private void UpdateLight()
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
                spriteRenderer.color = Color.red;
                if (collider2D != null)
                    collider2D.enabled = true;
                break;
        }
    }
}
