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
    
    public List<GameObject> guns = new List<GameObject>();
    
    private void Awake()
    {
        _time = cooldown;
    }

    void LateUpdate()
    {
        if (photonView.IsMine)
        {
            GameObject prefab = bullet;
            prefab.GetComponent<TrailRenderer>().time = trailRenderTime;
            if (Input.GetKey(KeyCode.Mouse0) && _time > cooldown)
            {
                foreach (var gun in guns)
                {
                    PhotonNetwork.Instantiate(bullet.name, gun.transform.position, gun.transform.rotation);
                    _time = 0f;
                }

            }
            _time += Time.deltaTime;
        }
    }
}
