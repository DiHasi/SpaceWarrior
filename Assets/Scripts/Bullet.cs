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
        Debug.Log(other.GetComponent<PhotonView>().Owner.ActorNumber);
        if (!other.isTrigger)
        {
            // Debug.Log("trigger!!!!!!!!!!" + " " + other.GetComponent<PhotonView>().Owner.CustomProperties["hp"]);
            
            Debug.Log($"{photonView.Owner.CustomProperties["Sender"]} take damage {other.GetComponent<PhotonView>().Owner.ActorNumber}, " +
                      $"hp {other.GetComponent<PhotonView>().Owner.ActorNumber} = {(int)other.GetComponent<PhotonView>().Owner.CustomProperties["hp"] - dmg}");
            // other.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.AllBuffered, dmg, sender, -1);
            // PhotonNetwork.Destroy(gameObject);
            // photonView.RPC("Del", RpcTarget.All, );
            // photonView.RPC("Del", RpcTarget.All, other.GetComponent<PhotonView>().Owner.ActorNumber);
            // other.GetComponent<Player>().TakeDamage(dmg, sender);
            // photonView.RPC("Del", RpcTarget.AllBuffered, other.GetComponent<PhotonView>().Owner.ActorNumber);
            // other.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.AllBuffered, dmg, sender);

            ExitGames.Client.Photon.Hashtable h = new ExitGames.Client.Photon.Hashtable();
            h.Add("hp", (int)other.GetComponent<PhotonView>().Owner.CustomProperties["hp"] - dmg);
            other.GetComponent<Renderer>().material.color = Color.red;
            other.GetComponent<PhotonView>().Owner.SetCustomProperties(h);
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
        
        foreach (var item in FindObjectsOfType<Player>())
        {
            item.photonView.RPC("TakeDamage", RpcTarget.All, dmg, sender, receiver);
        }
        // Destroy(Instantiate(explode.gameObject, transform.position, transform.rotation), 2);
        if (photonView.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
