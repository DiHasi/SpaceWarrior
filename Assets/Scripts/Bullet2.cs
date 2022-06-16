using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEditor.UIElements;
using UnityEngine;

public class Bullet2 : MonoBehaviourPunCallbacks
{
    public Rigidbody bullet;
    // public GameObject bullet1;
    public Transform capsule;
    // public string sender;
    // public int dmg;
    public float lifeTime;
    // public int team;
    //
    // public bool isStart;
    public float BulletSpeed;

    // public GameObject Sparks;
    
    // Start is called before the first frame update
    void Start()
    {
        // isStart = true;
        capsule.GetComponent<MeshRenderer>().enabled = false;
        bullet.AddRelativeForce(0, 0, BulletSpeed);
    }
    
    private void Awake()
    {
        bullet.maxDepenetrationVelocity = 500000f;
    }

    // Update is called once per frame
    void Update()
    {
        lifeTime += Time.deltaTime;
        if (lifeTime > 7f)
        {
            lifeTime = 0f;
            Destroy(gameObject);
        }

        
    }

    
    private void OnTriggerEnter(Collider other)
    {
        // Destroy(gameObject);
    }


    
}
