using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class Pig : MonoBehaviour
{


    public bool isDrag;
    
    public float maxLength;
    public float force;
    
    private Vector3 currentPosition;
    
    private Vector2 idlePosition;
    private sShot sshot;
    
    private RVManager rvManager;
    
   
    private Rigidbody2D rb;
    private Collider2D cr;



    private void Awake()
    {
        rvManager = FindObjectOfType<RVManager>();
        sshot = FindObjectOfType<sShot>();
        rb = GetComponent<Rigidbody2D>();
        cr = GetComponent<Collider2D>();
        
    }
    
    
    void Start()
    {
        
        // idlePosition = ;
        rb.isKinematic = true;
        rb.simulated = false;
        cr.enabled = true;
        
        //idlePosition = rb.position;
        idlePosition = rvManager.spawnPoint.position;
        
        currentPosition = idlePosition;

    }


    

    void Update()
    {
        

        if (isDrag)
        {
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = 10; // 카메라의 z축 거리
            currentPosition = Camera.main.ScreenToWorldPoint(mousePosition);

            //sshot = new sShot();
            Vector3 Center = sshot.center.position;
            currentPosition = Center + Vector3.ClampMagnitude
                               (currentPosition - Center, maxLength);
            //currentPosition += new Vector3(-0.5f, 0, 0);
           

            rvManager.pig.transform.position = currentPosition;
            
            if (rvManager.pig)
            {
                Vector3 pigPos = rvManager.pig.transform.position 
                                 + new Vector3(-0.5f, 0, 0);
                Vector3 dir = currentPosition - Center;
                pigPos = currentPosition + dir.normalized;
                rvManager.pig.transform.right = -dir.normalized;
            }
           

            
        }

    }

    public void Drag()
    {
        
        isDrag = true;
        Debug.Log("Drag 시작");
    }

    public void Drop()
    {
        
        isDrag = false;
        Debug.Log("Drag 종료");
        
        Shoot();
        
        //코루틴으로 5초후 사망
        StartCoroutine(DestroyPig());
    }

    void Shoot()
    {
        rb.simulated = true;
        rb.isKinematic = false;
        Vector3 pigForce = (currentPosition - sshot.center.position) * force * -1;
        rb.velocity = pigForce;
    }

    IEnumerator DestroyPig()
    {
        yield return new WaitForSeconds(5f);
        
        Destroy(gameObject);
    }
    
    
}
