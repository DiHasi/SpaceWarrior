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
    public GameObject bullet1;
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
        isStart = false;
        // gameObject.GetComponent<TrailRenderer>().enabled = false;
        capsule.GetComponent<MeshRenderer>().enabled = false;
        
        // bullet.AddRelativeForce(0,0,BulletSpeed);
        // bullet.AddForce(bullet.gameObject.transform.forward * BulletSpeed);

    }

    public void Shoot()
    {
        photonView.RPC(nameof(RPC_ChangeCondition), RpcTarget.AllBuffered, true);
        gameObject.GetComponent<Rigidbody>().AddForce(gameObject.transform.forward * BulletSpeed);
    }
    private void Awake()
    {
        bullet.maxDepenetrationVelocity = 500000f;
    }

    // Update is called once per frame
    void Update()
    {
        if (isStart)
        {
            capsule.GetComponent<Collider>().enabled = true;
            // gameObject.GetComponent<TrailRenderer>().enabled = true;
            photonView.RPC(nameof(RPC_ChangeTrailRender), RpcTarget.All, true);
            if (photonView.IsMine)
            {
                lifeTime += Time.deltaTime;
                if (lifeTime > 1f)
                {
                    // StartCoroutine(Del());
                    lifeTime = 0f;
                    photonView.RPC(nameof(RPC_ChangeCondition), RpcTarget.AllBuffered, false);
                    capsule.GetComponent<Collider>().enabled = false;
                    GameObject o;
                    // .GetComponent<TrailRenderer>().enabled = false;
                    photonView.RPC(nameof(RPC_ChangeTrailRender), RpcTarget.All, false);
                    gameObject.GetComponent<TrailRenderer>().enabled = false;
                    (o = gameObject).GetComponent<Rigidbody>().velocity = Vector3.zero;
                    o.GetComponent<Rigidbody>().isKinematic = true;
                    o.GetComponent<Rigidbody>().isKinematic = false;
                    GameObject.Find("Bullets").GetComponent<Bullets>().BulletsList.Add(o);
                }
            }
        }
        
    }

    [PunRPC]
    public void RPC_ChangeTrailRender(bool b)
    {
        gameObject.GetComponent<TrailRenderer>().enabled = b;
    }
    public IEnumerator Del()
    {
        PhotonNetwork.Destroy(gameObject);
        yield return null;
    }
    private void OnTriggerEnter(Collider other)
    {
        
        if (!other.isTrigger && !other.CompareTag("Object"))
        {
            var o = gameObject;
            var s= Instantiate(Sparks, o.transform.position, o.transform.rotation);
            Destroy(s, s.GetComponent<ParticleSystem>().main.duration); 
            Destroy(s, s.GetComponent<ParticleSystem>().main.duration); 
            if (photonView.IsMine)
            {
                GameObject a = other.gameObject;
                if (!other.CompareTag("Sphere"))
                {
                    a = other.transform.parent.gameObject;
                }

                if ((int) a.GetComponent<PhotonView>().Owner.CustomProperties["Team"] != team)
                {
                    a.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.All, dmg, 
                        photonView.Owner.NickName);
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

    [PunRPC]
    public void RPC_ChangeCondition(bool condition)
    {
        isStart = condition;
    }
}
