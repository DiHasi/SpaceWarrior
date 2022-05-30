using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;

public class Shpere : MonoBehaviourPunCallbacks, IPunObservable
{
    public int hp = 1;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Awake()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            if (hp <= 0)
            {
                Collider[] colliders = Physics.OverlapSphere(transform.position, 2000f);
                foreach (var collider1 in colliders)
                {
                    Rigidbody rigidbody = collider1.attachedRigidbody;
                    if (rigidbody)
                    {
                        rigidbody.isKinematic = false;
                        rigidbody.AddExplosionForce(3500, transform.position, 2000f);
                    }
                }
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }
    [PunRPC]
    public void TakeDamage(int dmg, string actorName)
    {
        if(!photonView.IsMine)
            return;
        if (photonView.IsMine)
        {
            hp -= dmg;
        }
        
    } 
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(hp);
        }
        else
        {
            hp = (int)stream.ReceiveNext();
        }
    }
}
