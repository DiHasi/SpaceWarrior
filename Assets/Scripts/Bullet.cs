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
    // public GameObject bullet1;
    public Transform capsule;
    public string sender;
    public int dmg;
    public float lifeTime;
    public int team;

    public bool isStart;
    public float BulletSpeed;

    public GameObject Sparks;
    
    // Start is called before the first frame update
    void Start()
    {
        isStart = true;
        capsule.GetComponent<MeshRenderer>().enabled = false;
        bullet.AddRelativeForce(0, 0, BulletSpeed);
    }
    
    private void Awake()
    {
        bullet.maxDepenetrationVelocity = 500000f;
    }

    // Update is called once per frame
    void Update()
    {
        lifeTime += Time.deltaTime;
        if (lifeTime > 7f)
        {
            lifeTime = 0f;
            Destroy(gameObject);
        }

        
    }

    
    private void OnTriggerEnter(Collider other)
    {
        
     
        if (!other.isTrigger && !other.CompareTag("Object"))
        {
            var o = gameObject;
            var s= Instantiate(Sparks, o.transform.position, o.transform.rotation);
            Destroy(s, s.GetComponent<ParticleSystem>().main.duration); 
            Destroy(s, s.GetComponent<ParticleSystem>().main.duration);

            var a = other.gameObject.transform.parent;
            if ((int) a.GetComponent<PhotonView>().Owner.CustomProperties["Team"] != team)
            {
                    
                var p = PhotonNetwork.PlayerList.ToList()
                    .Find(p => p.NickName == a.GetComponent<PhotonView>().Owner.NickName);
                Debug.Log(p.NickName);
                a.GetComponent<PhotonView>().RPC("TakeDamage", p, dmg, sender);

            }
        }
    }

    [PunRPC]
    public void Set(string sn, int team)
    {
        sender = sn;
        this.team = team;
    }
}
