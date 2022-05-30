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
                a.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.All, dmg, "test");
            }
            
            // ExitGames.Client.Photon.Hashtable h = new ExitGames.Client.Photon.Hashtable();
            // var rec = PhotonNetwork.PlayerList.ToList().Find(x => x.ActorNumber 
            //                                                       == other.GetComponent<PhotonView>().Owner.ActorNumber);
            //
            // if (photonView.IsMine)
            // {
            //     h.Add("hp", (int)rec.CustomProperties["hp"] - dmg);
            // }
            // // other.GetComponent<Renderer>().material.color = Color.red;
            // other.GetComponent<PhotonView>().Owner.SetCustomProperties(h);
            // var plr = PhotonNetwork.PlayerList.ToList().Find(x => x.UserId
            //                                                       == photonView.Owner.UserId);
            // Debug.Log( other.GetComponent<PhotonView>().ViewID);
            // photonView.RPC("TakeDamage", RpcTarget.All, dmg, "test");
            
        }
    }

    [PunRPC]
    public void Set(string sn)
    {
        sender = sn;
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
