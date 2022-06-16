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
    
    public IEnumerator SpawnEnumerator2()
    {
        GameObject prefab = bullet;
        prefab.GetComponent<TrailRenderer>().time = trailRenderTime;
        foreach (var gun in guns)
        {
            var b = Instantiate(prefab, gun.transform.position, gun.transform.rotation);
            if (photonView.IsMine)
            {
                b.GetComponent<Bullet>().team = (int)photonView.Owner.CustomProperties["Team"];
                b.GetComponent<Bullet>().sender = photonView.Owner.NickName;
                var g = gun.transform;
                photonView.RPC("RPC_ShootSound", RpcTarget.All);
                var position = g.position;
                var rotation = g.rotation;
                
                photonView.RPC("SpawnBullet", RpcTarget.Others, 
                    position.x, position.y, position.z, rotation.x, rotation.y, rotation.z, rotation.w, trailRenderTime);
            }
            _time = 0f;
        }

        yield return null;
    }

    
}
