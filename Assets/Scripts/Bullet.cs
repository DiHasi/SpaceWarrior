using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Bullet : MonoBehaviourPunCallbacks
{
    public Rigidbody bullet;
    public Transform capsule;
    public string sender;
    public int dmg;
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
    private void OnTriggerEnter(Collider other)
    {
        if (!other.isTrigger)
        {
            GetComponent<PhotonView>().RPC("Del", RpcTarget.AllBuffered);
        }
    }
    // private void OnCollisionEnter(Collision collision)
    // {
    //     if (!other.isTrigger)
    //     {
    //         GetComponent<PhotonView>().RPC("Del", RpcTarget.AllBuffered);
    //     }
    // }

    [PunRPC]
    public void Del()
    {
        foreach (var item in FindObjectsOfType<Player>())
        {
            item.photonView.RPC("TakeDamage", RpcTarget.AllBuffered, dmg, sender);
        }
        // Destroy(Instantiate(explode.gameObject, transform.position, transform.rotation), 2);
        Destroy(gameObject);
    }
}
