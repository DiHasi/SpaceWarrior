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

    private void Start()
    {
        // fireSource = gameObject.AddComponent<AudioSource>();
        // fireSource.clip = fireClip;
    }
    
    void LateUpdate()
    {
        if (photonView.IsMine)
        {
            
            if (Input.GetKey(KeyCode.Mouse0) && _time > cooldown)
            {
                StartCoroutine(SpawnEnumerator2());
                // StartCoroutine(SpawnEnumerator());
            }
            _time += Time.deltaTime;
        }
        
    }

    public IEnumerator SpawnEnumerator()
    {
        GameObject prefab = bullet;
        prefab.GetComponent<TrailRenderer>().time = trailRenderTime;
        foreach (var gun in guns)
        {
            var b1 = PhotonNetwork.Instantiate(prefab.name, gun.transform.position, gun.transform.rotation);
            b1.GetComponent<PhotonView>().RPC("Set", RpcTarget.All, photonView.Owner.NickName, 
                photonView.Owner.CustomProperties["Team"]);
            _time = 0f;
        }

        yield return null;
    }
    public IEnumerator SpawnEnumerator2()
    {
        // GameObject prefab = bullet;
        // prefab.GetComponent<TrailRenderer>().time = trailRenderTime;
        foreach (var gun in guns)
        {
            var bulletsList = GameObject.Find("Bullets").GetComponent<Bullets>().BulletsList;
            var prefab = bulletsList[0];
            bulletsList.RemoveAt(0);
            // prefab.GetComponent<Rigidbody>().position = gun.transform.position;
            // prefab.GetComponent<Rigidbody>().rotation = gun.transform.rotation;
            prefab.transform.position = gun.transform.position;
            prefab.transform.rotation = gun.transform.rotation;
            prefab.GetComponent<Bullet>().Shoot();
            if (photonView.IsMine)
            {
                photonView.RPC("RPC_ShootSound", RpcTarget.All);
            }
            
            // var b1 = PhotonNetwork.Instantiate(prefab.name, gun.transform.position, gun.transform.rotation);
            prefab.GetComponent<PhotonView>().RPC("Set", RpcTarget.All, photonView.Owner.NickName, 
                photonView.Owner.CustomProperties["Team"]);
            _time = 0f;
        }

        yield return null;
    }


}
