using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEditor.UIElements;
using UnityEngine;

public class Bullet : MonoBehaviourPunCallbacks
{
    public Rigidbody bullet;
    public Transform capsule;
    public string sender;
    public int dmg;
    private float lifeTime;
    public int team;

    public float BulletSpeed;
    
    // Start is called before the first frame update
    void Start()
    {
        capsule.GetComponent<MeshRenderer>().enabled = false;
        bullet.AddRelativeForce(0f, 0f, BulletSpeed); 
    }

    private void Awake()
    {
        bullet.maxDepenetrationVelocity = 500000f;
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            lifeTime += Time.deltaTime;
            if (lifeTime > 9f)
            {
                // Debug.Log($"{lifeTime} {photonView.Owner.ActorNumber}");
                PhotonNetwork.Destroy(gameObject);
                lifeTime = 0f;
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        
        if (!other.isTrigger && !other.CompareTag("Object"))
        {
            if (photonView.IsMine)
            {
                GameObject a = other.gameObject;
                if (!other.CompareTag("Sphere"))
                {
                    a = other.transform.parent.gameObject;
                }

                if ((int) a.GetComponent<PhotonView>().Owner.CustomProperties["Team"] != team)
                {
                    a.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.All, dmg, "test");
                }
            }
        }
    }

    [PunRPC]
    public void Set(string sn, int team)
    {
        sender = sn;
        this.team = team;
    }
    
    [PunRPC]
    public void Del(int receiver)
    {
        var rec = PhotonNetwork.PlayerList.ToList().Find(x => x.ActorNumber == receiver);
        
        // foreach (var item in FindObjectsOfType<Player>())
        // {
        //     if(item.photonView.Owner.)
        //     item.photonView.RPC("TakeDamage", RpcTarget.All, dmg, sender, receiver);
        // }

        if (photonView.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
