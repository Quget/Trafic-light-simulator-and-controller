using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    [SerializeField]
    private Bullet bulletPrefab;

    [SerializeField]
    private float delayTimer = 0.15f;

    private float timer = 0;

    [SerializeField]
    private AudioClip bulletSound;

    [SerializeField]
    private AudioSource shootSource;

    [SerializeField]
    private AudioClip kra;

    [SerializeField]
    private AudioClip ka;

    [SerializeField]
    private AudioClip kah;

    private bool first = true;

    [SerializeField]
    private bool destroyMode = true;

    [SerializeField]
    private float delayTimerNonDestroy = 1f;

    public bool DestroyMode { get { return destroyMode; } }
	// Use this for initialization
	void Start ()
    {
        shootSource.clip = kra;

    }
	
	// Update is called once per frame
	void Update ()
    {
        var objectPos = Camera.main.WorldToScreenPoint(transform.position);
        var dir = Input.mousePosition - objectPos;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg));

        if(Input.GetKey(KeyCode.W))
        {
            
            Vector3 goToPos = Vector3.MoveTowards(transform.position, 
                Camera.main.ScreenToWorldPoint(Input.mousePosition), 
                10 * Time.deltaTime);
            goToPos.z = 0;

            transform.position = goToPos;
        }

        if(Input.GetKeyDown(KeyCode.L))
        {
            destroyMode = !destroyMode;
        }

        if(Input.GetMouseButtonDown(0))
        {
            timer = delayTimer;
            if(!destroyMode)
            {
                timer = delayTimerNonDestroy;
            }
        }

        if(Input.GetMouseButton(0))
        {
            timer += Time.deltaTime;
            if ((destroyMode && timer > delayTimer) ||
                (!destroyMode && timer > delayTimerNonDestroy))
            {
                Bullet bullet = Instantiate(bulletPrefab, null);

                Bullet[] bullets = FindObjectsOfType<Bullet>();
                Physics2D.IgnoreCollision(bullet.GetComponent<Collider2D>(), GetComponent<Collider2D>());
                for (int i = 0; i < bullets.Length; i++)
                {
                    Physics2D.IgnoreCollision(bullet.GetComponent<Collider2D>(), bullets[i].GetComponent<Collider2D>());
                }

                bullet.transform.position = transform.position + (transform.right * 0.5f);
                bullet.transform.rotation = transform.rotation;
                timer = 0;
                if(bulletSound != null)
                {
                    //AudioSource.PlayClipAtPoint(bulletSound, transform.position);
                    if(shootSource != null && !shootSource.isPlaying)
                    {
                        if(!first)
                        {
                            shootSource.clip = ka;
                        }
                        else
                        {
                            shootSource.clip = kra;
                        }
                        shootSource.Play();
                        first = false;
                    }
                }
            }
            return;
        }
        if (!first && shootSource != null && !shootSource.isPlaying)
        {
            Debug.Log("yo");
            shootSource.clip = kah;
            shootSource.Play();
            first = true;
        }

    }
}
