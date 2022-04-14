using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using UnityEngine;

public class Player : MonoBehaviourPunCallbacks
{
    public int playerHp;
    public int k, d;
    
    float hpadd = 0;
    bool dead = false;

    public PlayerLocal PlayerLocal;
    public GameObject Canvas;
    
    public string lastDamagePlayer;

    void Start()
    {
        transform.name = photonView.Owner.NickName;
    }

    private void Awake()
    {
        if (!photonView.IsMine)
        {
            PlayerLocal.enabled = false;
            Destroy(PlayerLocal.camera.gameObject);
            Canvas.SetActive(false);
        }
    }

    public void Update()
    {
        if (playerHp > 100)
        {
            playerHp = 100;
        }
        if (photonView.IsMine)
        {
            // if (playerHp < playerHpOld)
            // {
            //     playerHpOld = playerHp;
            //     print("Damage");
            // }
            if (photonView.Owner.CustomProperties["K"] != null)
            {
                k = (int)photonView.Owner.CustomProperties["K"];
                d = (int)photonView.Owner.CustomProperties["D"];
            }
            if (playerHp <= 0 )
            {
                Dead();
                playerHp = 100;
            }
        }
        else
        {
            PlayerLocal.plane.transform.LookAt(gameObject.transform);
            if (playerHp <= 0)
            {
                dead = true;
            }
        }
        
    }
    public void Dead()
    {
        if (photonView.IsMine)
        {
            if (dead == false)
            {
                if (lastDamagePlayer != "" && lastDamagePlayer != photonView.Owner.NickName)
                {
                    var ldp = PhotonNetwork.PlayerList.ToList().Find(x => x.NickName == lastDamagePlayer);
                    if (ldp != null)
                    {
                        ExitGames.Client.Photon.Hashtable h = new ExitGames.Client.Photon.Hashtable();
                        h.Add("K", (int)(((int)ldp.CustomProperties["K"]) + 1));
                        h.Add("D", (int)ldp.CustomProperties["D"]);
                        ldp.SetCustomProperties(h);
                    }
                }
                d++;
                SaveKD();
                PhotonNetwork.Destroy(gameObject);
                dead = true;
                // FindObjectOfType<GameManager>().StartCoroutine(FindObjectOfType<GameManager>().Respawn());
            }
        }
    }
    public void SaveKD()
    {
        ExitGames.Client.Photon.Hashtable h = new ExitGames.Client.Photon.Hashtable();
        h.Add("K", k);
        h.Add("D", d);
        photonView.Owner.SetCustomProperties(h);
    }
}
