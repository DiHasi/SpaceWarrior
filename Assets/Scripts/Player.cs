#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;
using static System.Random;
using Random = UnityEngine.Random;

public class Player : MonoBehaviourPunCallbacks, IPunObservable
{
    public int playerHp;
    public int k, d;
    public GameObject player;
    
    float hpadd = 0;
    // bool dead = false;

    public PlayerLocal PlayerLocal;
    public GameObject Canvas;

    public GameObject TookDamageCanvas;
    public GameObject DiedCanvas;

    public CrosshairManager CrosshairManager;

    public TMP_Text hpCount;
    public TMP_Text name;

    public Camera Camera;
    public GameObject plane;

    public string lastDamagePlayer;

    public int myTeam;
    public GameObject Avatar;

    public bool isDead = false;


    public GameObject Collider;

    public Image HealthLine;
    
    public Image ForceLine;


    private bool flag = false;
    void Start()
    {
        
    }
    
    private void Awake()
    {
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

            // Debug.Log(PhotonNetwork.PlayerList.Length);
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
                //
                if (photonView.IsMine)
                {
                    if (char.ConvertToUtf32(photonView.Owner.NickName, 0) == 8203)
                    {
                        photonView.Owner.NickName = "Player_" + Random.Range(0, 1000);
                    }
                    while (PhotonNetwork.PlayerList.ToList().All(p => p.NickName != photonView.Owner.NickName))
                    {
                        photonView.Owner.NickName = photonView.Owner.NickName +"_"+ Random.Range(0, 100);
                    }
                }
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
            var gm1 = GameObject.Find("GameManager");
            var gm2 = gm1.GetComponent<TabControl>();
            if (gm2.CanOutputTab)
            {
                gm2.GetComponent<TabControl>().TabOutput();
            }
        }
        if (photonView.IsMine)
        {
            if (isDead)
            {
                gameObject.GetComponent<Renderer>().material.color = Color.black;
                gameObject.GetComponent<PlayerLocal>().enabled = false;
                gameObject.GetComponent<RayScript>().enabled = false;
                gameObject.GetComponent<PlayerLocal>().force = 0;
                CrosshairManager.enabled = false;
                photonView.RPC(nameof(RPC_ChangeColor), RpcTarget.All, 1.0f, 0.0f, 0.0f, 1f);
            }
            
            hpCount.text = $"hp: {playerHp}\nteam: {myTeam}";
            name.text = $"name: {photonView.Owner.NickName}";

            ForceLine.fillAmount = gameObject.GetComponent<PlayerLocal>().force / (gameObject.GetComponent<PlayerLocal>().maxForce);
            
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

        DiedCanvas.SetActive(true);
        yield return new WaitForSeconds(3);
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
            CrosshairManager.enabled = false;
            gameObject.GetComponent<RayScript>().enabled = true;
            DiedCanvas.SetActive(false);
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
                StartCoroutine(TookDamage());
                HealthLine.fillAmount = playerHp/500f;
                // Debug.Log(playerHp/500f);
            }
        }
    }

    public IEnumerator TookDamage()
    {
        TookDamageCanvas.SetActive(true);
        yield return new WaitForSeconds(0.6f);
        TookDamageCanvas.SetActive(false);
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

    // public override void OnRoomListUpdate(List<RoomInfo> roomList)
    // {
    //     GameObject? gm1 = GameObject.Find("GameManager");
    //     gm1.GetComponent<TabControl>().TabOutput();
    // }
}
