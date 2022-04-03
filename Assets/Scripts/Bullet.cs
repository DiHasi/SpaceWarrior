using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Bullet : MonoBehaviourPunCallbacks
{
    public Rigidbody bullet;
    public Transform capsule;
    private float lifeTime;
    
    // Start is called before the first frame update
    void Start()
    {
        capsule.GetComponent<MeshRenderer>().enabled = false;
        bullet.AddRelativeForce(0f, 0f, 50000f); 
    }

    private void Awake()
    {
        bullet.maxDepenetrationVelocity = 100000f;
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            lifeTime += Time.deltaTime;
            if (lifeTime > 3f)
            {
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }
}
