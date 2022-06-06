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

    public bool isDead = false;


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
        // float a = 1f;
        // while (a > 0)
        // {
        //     a = Mathf.Lerp(a, 0, 0.5f);
        //     Debug.Log(a);
        //     // photonView.RPC(nameof(RPC_ChangeColor), RpcTarget.Others, 0f, 0f, 0f, a);
        // }
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
            photonView.RPC("RPC_GetTeam", RpcTarget.MasterClient);
            Collider.SetActive(false);
        }
    }
    
    
    public void Update()
    {
        if (photonView.IsMine)
        {
            // Debug.LogError(lastDamagePlayer);
            // var rec = PhotonNetwork.PlayerList.ToList().Find(x => x.UserId
            //                                                       == photonView.Owner.UserId);
            if (!flag && myTeam != 0)
            {
                ExitGames.Client.Photon.Hashtable h = new ExitGames.Client.Photon.Hashtable();
                h.Add("Team", myTeam);
                photonView.Owner.SetCustomProperties(h);
                if (myTeam == 1)
                {
                    photonView.transform.position = new Vector3(0, 0, -4000);
                }
                else if (myTeam == 2)
                {
                    photonView.transform.position = new Vector3(0, 0, 4000);
                    photonView.transform.rotation = Quaternion.Euler(0, 180, 0);
                }
                
                
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
            if (isDead)
            {
                gameObject.GetComponent<Renderer>().material.color = Color.black;
                gameObject.GetComponent<PlayerLocal>().enabled = false;
                gameObject.GetComponent<RayScript>().enabled = false;
                gameObject.GetComponent<PlayerLocal>().force = 0;
                photonView.RPC(nameof(RPC_ChangeColor), RpcTarget.All, 1.0f, 0.0f, 0.0f, 1f);
            }
            
            
            
            hpCount.text = $"hp: {playerHp} team: {myTeam}";
            name.text = $"name: {photonView.Owner.UserId}";
            // if (photonView.Owner.CustomProperties["hp"] != null)
            // {
            //     playerHp = (int)photonView.Owner.CustomProperties["hp"];
            // }
            if (photonView.Owner.CustomProperties["K"] != null)
            {
                k = (int)photonView.Owner.CustomProperties["K"];
                d = (int)photonView.Owner.CustomProperties["D"];
            }
            if (playerHp <= 0 && !isDead)
            {
                playerHp = 0;
                Dead();
                isDead = true;
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
            if (lastDamagePlayer != "")
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

            StartCoroutine(Respawn());
        }

        
    }
    
    public IEnumerator Respawn()
    {
        if (photonView.IsMine)
        {
            playerHp = 0;
        }

        yield return new WaitForSeconds(5);
        photonView.RPC(nameof(RPC_ChangeRender), RpcTarget.All, false);
        if (photonView.IsMine)
        {
            if (myTeam == 1)
            {
                photonView.transform.position = new Vector3(0, 0, -4000);
            }
            else if (myTeam == 2)
            {
                photonView.transform.position = new Vector3(0, 0, 4000);
                photonView.transform.rotation = Quaternion.Euler(0, 180, 0);
            }
        
            
            // gameObject.GetComponent<Renderer>().enabled = false;
        }
        
        
        yield return new WaitForSeconds(1);
        if (photonView.IsMine)
        {
            playerHp = 500;
            gameObject.GetComponent<Renderer>().enabled = true;
            gameObject.GetComponent<PlayerLocal>().enabled = true;
            gameObject.GetComponent<PlayerLocal>().force = 10000;
            isDead = false;
            photonView.RPC(nameof(RPC_ChangeRender), RpcTarget.All, true);
            photonView.RPC(nameof(RPC_ChangeColor), RpcTarget.All, 1f, 0.9215686f, 0.01568628f, 1f);
            gameObject.GetComponent<RayScript>().enabled = true;
        }
        
        
        yield return null;
    }

    // public IEnumerator FadeOpacity()
    // {
    //     float a = 1f;
    //     while (a > 0)
    //     {
    //         Material mat = GetComponent<Renderer>().material;
    //         
    //         a = Mathf.Lerp(a, 0, 0.5f);
    //         var matColor = mat.color;
    //         matColor.a = a;
    //         if (photonView.IsMine)
    //         {
    //             photonView.RPC(nameof(RPC_ChangeColor), RpcTarget.Others, 1f, 0f, 0f, a);
    //         }
    //         
    //         yield return new WaitForSeconds(0.5f);
    //     }
    // }
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
            if (!isDead)
            {
                lastDamagePlayer = actorName;
                playerHp -= dmg;
            }
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
        var GM = GameObject.Find("GameManager").GetComponent<GameManagerTeamFight>();
        myTeam = GM.nextTeam;
        GM.UpdateTeam();
        photonView.RPC("RPC_SentTeam", RpcTarget.OthersBuffered, myTeam);
}

    [PunRPC]
    public void RPC_SentTeam(int whichTeam)
    {
        myTeam = whichTeam;
    }

    [PunRPC]
    public void RPC_ChangeColor(float r, float g, float b, float a = 1f)
    {
        gameObject.GetComponent<Renderer>().material.color = new Color(r, g, b, a);
    }
    [PunRPC]
    public void RPC_ChangeRender(bool b)
    {
        gameObject.GetComponent<Renderer>().enabled = b;
    }
}
