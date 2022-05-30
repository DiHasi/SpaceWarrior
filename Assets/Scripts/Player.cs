#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using TMPro;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class Player : MonoBehaviourPunCallbacks, IPunObservable
{
    public int playerHp;
    public int k, d;
    public GameObject player;

    public int Team;
    float hpadd = 0;
    bool dead = false;

    public PlayerLocal PlayerLocal;
    public GameObject Canvas;

    public TMP_Text hpCount;
    public TMP_Text name;

    public Camera Camera;
    public GameObject plane;

    public string lastDamagePlayer;

    public int myTeam;
    public GameObject Avatar;


    public GameObject Collider;


    private bool flag = false;
    void Start()
    {
        // if (photonView.IsMine)
        // {
        //     // photonView.RPC("RPC_GetTeam", RpcTarget.MasterClient);
        // }
    }
    
    private void Awake()
    {
        
        // Debug.Log(661 + " awake " + (int)photonView.Owner.CustomProperties["hp"]);
        photonView.RPC("RPC_GetTeam", RpcTarget.MasterClient);
        if (!photonView.IsMine)
        {
            PlayerLocal.enabled = false;
            Destroy(PlayerLocal.camera.gameObject);
            Canvas.SetActive(false);
            gameObject.layer = 0;
            gameObject.GetComponent<MeshCollider>().isTrigger = false;
            plane.SetActive(true);
        }

        if (photonView.IsMine)
        {
            Collider.SetActive(false);
        }
    }
    
    
    public void Update()
    {
        // photonView.Owner.CustomProperties.ToList().ForEach(p => Debug.Log("55" + p));
        if (photonView.IsMine)
        {
            // Debug.Log(661 + " start " + (int)photonView.Owner.CustomProperties["hp"]);
            var rec = PhotonNetwork.PlayerList.ToList().Find(x => x.UserId
                                                                  == photonView.Owner.UserId);
            if (rec.CustomProperties["Team"] != null && !flag)
            {
                if ((int) rec.CustomProperties["Team"] == 1)
                {
                    photonView.transform.position = new Vector3(0, 0, -4000);
                }
                else
                {
                    photonView.transform.position = new Vector3(0, 0, 4000);
                    photonView.transform.rotation = Quaternion.Euler(0, 180, 0);
                }
                
                photonView.RPC("RPC_GetTeam", RpcTarget.MasterClient);
                // Debug.LogError(photonView.Owner.ActorNumber + " - " + 2 + " " + GameObject.Find("GameManager").GetComponent<GameManagerTeamFight>().nextTeam);
                flag = true;
            }

            GameObject? gm = GameObject.Find("GameManager");
            if (flag && gm.GetComponent<GameManagerTeamFight>().isStart)
            {
                gameObject.GetComponent<Renderer>().enabled = true;
                gameObject.GetComponent<PlayerLocal>().enabled = true;
                gameObject.GetComponent<PlayerLocal>().camera.SetActive(true);
                Canvas.SetActive(true);
            }
        }
        
        if (playerHp > 500)
        {
            playerHp = 500;
        }
        if (photonView.IsMine)
        {
            hpCount.text = $"hp: {playerHp} team: {photonView.Owner.CustomProperties["Team"]}";
            name.text = $"name: {photonView.ViewID}";
            // if (photonView.Owner.CustomProperties["hp"] != null)
            // {
            //     playerHp = (int)photonView.Owner.CustomProperties["hp"];
            // }
            if (photonView.Owner.CustomProperties["K"] != null)
            {
                k = (int)photonView.Owner.CustomProperties["K"];
                d = (int)photonView.Owner.CustomProperties["D"];
            }
            if (playerHp <= 0)
            {
                PlayerLocal.GetComponent<Renderer>().material.color = Color.red;
                // Dead();
                playerHp = 499;
            }
        }
        else
        {
            if (playerHp <= 0)
            {
                dead = true;
            }
        }
        
        plane.GetComponent<Renderer>().material.color = Color.cyan;
        // if (Camera == null)
        Camera = FindObjectOfType<Camera>();
        if (Camera != null)
        {
            plane.transform.LookAt(Camera.transform);
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
                        h.Add("K", ((int)ldp.CustomProperties["K"]) + 1);
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
    [PunRPC]
    public void TakeDamage(int dmg, string actorName)
    {
        if(!photonView.IsMine)
            return;
        if (photonView.IsMine)
        {
            lastDamagePlayer = actorName;
            playerHp -= dmg;
        }
        
    } 
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(playerHp);
            stream.SendNext(k);
            stream.SendNext(d);
        }
        else
        {
            playerHp = (int)stream.ReceiveNext();
            k = (int)stream.ReceiveNext();
            d = (int)stream.ReceiveNext();
        }
    }

    [PunRPC]
    public void RPC_GetTeam()
    {
        // myTeam = GameObject.Find("GameManager").GetComponent<GameManagerTeamFight>().nextTeam;
        // myTeam = GameManagerTeamFight.GS.nextTeam;
        // GameManagerTeamFight.GS.UpdateTeam();
        // photonView.RPC("RPC_SentTeam", RpcTarget.OthersBuffered, myTeam);
        
        GameObject.Find("GameManager").GetComponent<GameManagerTeamFight>().UpdateTeam();
    }
    // [PunRPC]
    // void RPC_SentTeam(int whichTeam)
    // {
    //     myTeam = whichTeam;
    // }
}
