using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;


public enum TypeOfGun
{
    Sniper, 
    DoubleGun
};

public class GunScript : MonoBehaviourPunCallbacks
{
    public GameObject bullet;
    public float cooldown = 0.1f;
    public float trailRenderTime = 0.1f;
    
    private float _time;
    public Canvas Menu;
    private bool a = false;
    
    public List<GameObject> guns = new List<GameObject>();
    
    private void Awake()
    {
        _time = cooldown;
    }

    void LateUpdate()
    {
        if (photonView.IsMine)
        {
            
            if (Input.GetKey(KeyCode.Mouse0) && _time > cooldown)
            {
                GameObject prefab = bullet;
                prefab.GetComponent<TrailRenderer>().time = trailRenderTime;
                foreach (var gun in guns)
                {
                    var b1 = PhotonNetwork.Instantiate(prefab.name, gun.transform.position, gun.transform.rotation);
                    // b1.GetPhotonView().RPC("Set", RpcTarget.All, photonView.Owner.ActorNumber.ToString());
                    // ExitGames.Client.Photon.Hashtable h = new ExitGames.Client.Photon.Hashtable();
                    // h.Add("Sender", photonView.Owner.ActorNumber.ToString());
                    // b1.GetComponent<PhotonView>().Owner.SetCustomProperties(h);
                    _time = 0f;
                }

            }
            if (Input.GetKey(KeyCode.Mouse1) && _time > cooldown)
            {
                GameObject prefab = bullet;
                prefab.GetComponent<TrailRenderer>().time = trailRenderTime * 5;

                    var b1 = PhotonNetwork.Instantiate(prefab.name, guns[0].transform.position, guns[0].transform.rotation);
                    // b1.GetPhotonView().RPC("Set", RpcTarget.All, photonView.Owner.ActorNumber.ToString());
                    // ExitGames.Client.Photon.Hashtable h = new ExitGames.Client.Photon.Hashtable();
                    // h.Add("Sender", photonView.Owner.ActorNumber.ToString());
                    // b1.GetComponent<PhotonView>().Owner.SetCustomProperties(h);
                    _time = -5f;
            }
            
            _time += Time.deltaTime;
        }
    }
}
