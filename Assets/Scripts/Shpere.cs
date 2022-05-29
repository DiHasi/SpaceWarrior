using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;

public class Shpere : MonoBehaviourPunCallbacks, IPunObservable
{
    public int hp = 1;
    public StationControl StationControl;
    // Start is called before the first frame update
    void Start()
    {
        // ExitGames.Client.Photon.Hashtable h = new ExitGames.Client.Photon.Hashtable();
        // if (photonView.IsMine)
        // {
        //     h.Add("hp", 1);
        //     photonView.Owner.SetCustomProperties(h);
        // }
    }

    private void Awake()
    {

        // ExitGames.Client.Photon.Hashtable h = new ExitGames.Client.Photon.Hashtable();
        // h.Add("hp", 1);
        // // h.ToList().ForEach(p => Debug.Log(p));
        // h.Add("Team", 0);
        // gameObject.GetComponent<PhotonView>().Owner.SetCustomProperties(h);
        // gameObject.GetComponent<Renderer>().material.color = Color.black;
        // Debug.Log(771 + " awake " + photonView.Owner.CustomProperties["hp"]);
        // // if (photonView.IsMine)
        // // {
        // //     
        // //     if (photonView.IsMine)
        // //     {
        // //         h.Add("hp", 1);
        // //         photonView.Owner.SetCustomProperties(h);
        // //     }
        // // }
        
        // if (photonView.IsRoomView)
        // {
        //     gameObject.GetComponent<PhotonView>().Owner.SetCustomProperties(h);
        // }
    }

    // Update is called once per frame
    void Update()
    {
        photonView.Owner.CustomProperties.ToList().ForEach(p => Debug.Log("88" + p));
        // if (photonView.IsMine)
        // {
        //     if ((int) gameObject.GetComponent<PhotonView>().Owner.CustomProperties["hp"] <= 0)
        //     {
        //         Collider[] colliders = Physics.OverlapSphere(transform.position, 10000f);
        //         // foreach (var collider1 in colliders)
        //         // {
        //         //     collider1.GetComponent<Rigidbody>().isKinematic = false;
        //         // }
        //         foreach (var collider1 in colliders)
        //         {
        //             Rigidbody rigidbody = collider1.attachedRigidbody;
        //             if (rigidbody)
        //             {
        //                 rigidbody.isKinematic = false;
        //                 rigidbody.AddExplosionForce(1500, transform.position, 10000f);
        //             }
        //         }
        //         PhotonNetwork.Destroy(gameObject);
        //     }
        // }
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
