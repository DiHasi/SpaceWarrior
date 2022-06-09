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
    [Header("Online Player")]
    public int playerHp;
    public int k, d;
    public string lastDamagePlayer;
    public int myTeam;
    private List<Color> teamColor = new List<Color>() { new Color(0, 0.9f, 1f), new Color(1, 0.5f, 0f)};

    private Vector3 spawnPosition;

    [Header("Local Player")]
    public GameObject player;
    public PlayerLocal PlayerLocal;
    public Camera Camera;
    public GameObject plane;
    public GameObject Avatar;
    public bool isDead = false;
    public GameObject Collider;
    
    [Header("UI")]
    public Image HealthLine;
    public Image ForceLine;
    public TMP_Text hpCount;
    public TMP_Text name;
    public GameObject Canvas;
    public GameObject TookDamageCanvas;
    public GameObject DiedCanvas;
    public CrosshairManager CrosshairManager;
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
            // Debug.Log(gameObject.transform.position);
            if (!flag && myTeam != 0)
            {
                ExitGames.Client.Photon.Hashtable h = new ExitGames.Client.Photon.Hashtable();
                h.Add("Team", myTeam);
                photonView.Owner.SetCustomProperties(h);
                if (myTeam == 1)
                {
                    spawnPosition = new Vector3(0, Random.Range(120, 120), -4000);
                    photonView.transform.position = spawnPosition;
                }
                else if (myTeam == 2)
                {
                    spawnPosition = new Vector3(0, Random.Range(-120, 120), 4000);
                    photonView.transform.position = spawnPosition;
                    photonView.transform.rotation = Quaternion.Euler(0, 180, 0);
                }


                photonView.RPC(nameof(RPC_ChangeColor), RpcTarget.AllBuffered, teamColor[myTeam-1].r,
                teamColor[myTeam-1].g, teamColor[myTeam-1].b, 1f);
                
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
            if (gm2.CanOutputTab && PhotonNetwork.PlayerList.ToList().All(p => p.CustomProperties["Team"] != null))
            {
                gm2.GetComponent<TabControl>().TabOutput();
            }
            var gm3 = GameObject.Find("Tab").transform.Find("Parent");
            foreach (var child in gm3.GetComponentsInChildren<Outline>())
            {
                if (child.gameObject.transform.Find("Name").GetComponent<TextMeshProUGUI>().text == photonView.Owner.NickName)
                {
                    child.gameObject.GetComponent<Outline>().enabled = true;
                }
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
                    // h.Add("D", (int)ldp.CustomProperties["D"]);
                    ldp.SetCustomProperties(h);
                }
            }
            d++;
            SaveD();

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
                photonView.transform.position = spawnPosition;
            }
            else if (myTeam == 2)
            {
                photonView.transform.position = spawnPosition;
                photonView.transform.rotation = Quaternion.Euler(0, 180, 0);
            }
        }
        
        
        yield return new WaitForSeconds(1);
        if (photonView.IsMine)
        {
            playerHp = 500;
            HealthLine.fillAmount = playerHp/500f;
            gameObject.GetComponent<Renderer>().enabled = true;
            gameObject.GetComponent<PlayerLocal>().enabled = true;
            gameObject.GetComponent<PlayerLocal>().force = 10000;
            isDead = false;
            photonView.RPC(nameof(RPC_ChangeRender), RpcTarget.All, true);
            photonView.RPC(nameof(RPC_ChangeColor), RpcTarget.AllBuffered, teamColor[myTeam-1].r,
                teamColor[myTeam-1].g, teamColor[myTeam-1].b, 1f);
            CrosshairManager.enabled = false;
            gameObject.GetComponent<RayScript>().enabled = true;
            DiedCanvas.SetActive(false);
        }
        
        
        yield return null;
    }
    
    public void SaveD()
    {
        ExitGames.Client.Photon.Hashtable h = new ExitGames.Client.Photon.Hashtable();
        // h.Add("K", k);
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

    
    // [PunRPC]
    // public void RPC_GetPosition()
    // {
    //     var GM = GameObject.Find("GameManager").GetComponent<GameManagerTeamFight>();
    //     myTeam = GM.nextTeam;
    //     GM.UpdateTeam();
    //     photonView.RPC("RPC_SentTeam", RpcTarget.OthersBuffered, myTeam);
    // }
    //
    // [PunRPC]
    // public void RPC_SentPosition(int whichPosition)
    // {
    //     myTeam = whichPosition;
    // }

    // [PunRPC]
    // public void popPosition()
    // {
    //     var GM = GameObject.Find("GameManager").GetComponent<GameManagerTeamFight>();
    //     if (myTeam == 1)
    //     {
    //         spawnPosition = GM.spawnPositionsTeam1[0];
    //         GM.spawnPositionsTeam1.RemoveAt(0);
    //     }
    //     
    //     photonView.RPC("RPC_SentPosition", RpcTarget.OthersBuffered, spawnPosition);
    // }
    // [PunRPC]
    // public void RPC_SentPosition(Vector3 whichPosition)
    // {
    //     spawnPosition = whichPosition;
    // }

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
