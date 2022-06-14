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
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using WebSocketSharp;
using static System.Random;
using Hashtable = ExitGames.Client.Photon.Hashtable;
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
    public GameObject TrailRenderer1;
    public GameObject TrailRenderer2;

    [Header("Audio")]
    public AudioClip fireClip;
    public AudioSource fireSource;
    public AudioMixer AudioMixer;
    public AudioClip fightClip;
    
    [Header("UI")]
    public Image HealthLine;
    public Image ForceLine;
    public TMP_Text hpCount;
    public TMP_Text name;
    public GameObject Canvas;
    public GameObject TookDamageCanvas;
    public GameObject DiedCanvas;
    public GameObject KilledCanvas;
    public CrosshairManager CrosshairManager;
    public TMP_Text winner;
    public Canvas WinnerCanvas;
    public Canvas KillCounterCanvas;
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
            // PlayerLocal.enabled = false;
            // Canvas.SetActive(false); 
            
            
            // gameObject.GetComponent<PlayerLocal>().force = 0;
            // PlayerLocal.enabled = false;
            // gameObject.GetComponent<RayScript>().enabled = false;
            // CrosshairManager.enabled = false;
            // Canvas.SetActive(false);
            //
            
            
            photonView.RPC("RPC_GetTeam", RpcTarget.MasterClient);
            Collider.SetActive(false);
        }
    }

    private bool flag1 = false;
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
                    photonView.transform.rotation = Quaternion.Euler(0, 0, 0);
                }
                else if (myTeam == 2)
                {
                    spawnPosition = new Vector3(0, Random.Range(-120, 120), 4000);
                    photonView.transform.position = spawnPosition;
                    photonView.transform.rotation = Quaternion.Euler(0, 180, 0);
                }

                AudioSource audio = gameObject.AddComponent<AudioSource> ();
                audio.clip = fightClip;
                audio.playOnAwake = false;
                audio.outputAudioMixerGroup = AudioMixer.FindMatchingGroups("Master")[2];

                audio.volume = 0.1f;
                // audioRPC.spatialBlend = 1;
                // audioRPC.minDistance = 25;
                // audioRPC.maxDistance = 250;
                audio.Play();
                
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
                // GameObject? gm = GameObject.Find("GameManager");
                // if (flag && gm.GetComponent<GameManagerTeamFight>().isStart)
                // {
                //     gameObject.GetComponent<Renderer>().enabled = true;
                //     gameObject.GetComponent<PlayerLocal>().enabled = true;
                //     gameObject.GetComponent<PlayerLocal>().camera.SetActive(true);
                //     Canvas.SetActive(true);
                // }
            }

            GameObject? gm = GameObject.Find("GameManager");
            if (flag && gm.GetComponent<GameManagerTeamFight>().isStart && !flag1)
            {
                
                // gameObject.GetComponent<Renderer>().enabled = true;
                // gameObject.GetComponent<PlayerLocal>().enabled = true;
                // gameObject.GetComponent<PlayerLocal>().camera.SetActive(true);
                // Canvas.SetActive(true);
                
                gameObject.GetComponent<Renderer>().enabled = true;
                gameObject.GetComponent<PlayerLocal>().enabled = false;
                gameObject.GetComponent<PlayerLocal>().camera.SetActive(true);
                Canvas.SetActive(false);
                flag1 = true;
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
                    photonView.RPC(nameof(KilledOutput), ldp, photonView.Owner.NickName);
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

        photonView.RPC(nameof(RPC_ChangeTrailRender1), RpcTarget.AllBuffered, false);
        photonView.RPC(nameof(RPC_ChangeTrailRender2), RpcTarget.AllBuffered, false);
        
        DiedCanvas.SetActive(true);
        yield return new WaitForSeconds(3);
        photonView.RPC(nameof(RPC_ChangeRender), RpcTarget.All, false);
        if (photonView.IsMine)
        {
            if (myTeam == 1)
            {
                photonView.transform.position = spawnPosition;
                photonView.transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            else if (myTeam == 2)
            {
                photonView.transform.position = spawnPosition;
                photonView.transform.rotation = Quaternion.Euler(0, 180, 0);
            }
        }
        photonView.RPC(nameof(RPC_ChangeTrailRender1), RpcTarget.AllBuffered, true);
        photonView.RPC(nameof(RPC_ChangeTrailRender2), RpcTarget.AllBuffered, true);

        
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
            CrosshairManager.enabled = true;
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
    public void RPC_ShootSound()
    {
        // Debug.Log("jisfdjgi");
        // gameObject.GetComponent<AudioSource>().PlayOneShot(fireClip);
        // fireSource.PlayOneShot(fireClip);
        AudioSource audioRPC = gameObject.AddComponent<AudioSource> ();
        audioRPC.clip = fireClip;
        audioRPC.playOnAwake = false;
        audioRPC.outputAudioMixerGroup = AudioMixer.FindMatchingGroups("Master")[1];

        audioRPC.volume = 0.1f;
        audioRPC.spatialBlend = 1;
        audioRPC.minDistance = 25;
        audioRPC.maxDistance = 500;
        audioRPC.PlayOneShot(fireClip);
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
    public void KilledOutput(string Name)
    {
        KilledCanvas.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"You killed {Name}";
        StartCoroutine(Killed());
    }

    public IEnumerator Killed()
    {
        KilledCanvas.SetActive(true);
        yield return new WaitForSeconds(4);
        KilledCanvas.SetActive(false);
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
    
    [PunRPC]
    public void RPC_ChangeTrailRender1(bool b)
    {
        TrailRenderer1.SetActive(b);
    }
    [PunRPC]
    public void RPC_ChangeTrailRender2(bool b)
    {
        TrailRenderer2.SetActive(b);
    }

    [PunRPC]
    public void RPC_Finish(string text, float r, float g, float b)
    {
        GameObject.Find("WinCanvas").transform.GetChild(1).GetComponent<TMP_Text>().text = text;
        GameObject.Find("WinCanvas").transform.GetChild(1).GetComponent<TMP_Text>().color = new Color(r, g, b);
        GameObject.Find("WinCanvas").GetComponent<Canvas>().enabled = true;
        GameObject.Find("KillCounter").GetComponent<Canvas>().enabled = false;
    
        StartCoroutine(Win());
        gameObject.GetComponent<PlayerLocal>().force = 0;
        PlayerLocal.enabled = false;
        gameObject.GetComponent<RayScript>().enabled = false;
        CrosshairManager.enabled = false;
        Canvas.SetActive(false);
    }

    [PunRPC]
    public void Ready()
    {
        ExitGames.Client.Photon.Hashtable h = new ExitGames.Client.Photon.Hashtable();
        h.Add("Ready", true);
        photonView.Owner.SetCustomProperties(h);
    }
    public IEnumerator Win()
    {
        yield return new WaitForSeconds(6);
        PhotonNetwork.LeaveRoom();
        while (PhotonNetwork.InRoom)
        {
            yield return null;
        }
        PhotonNetwork.LoadLevel("Menu");
    }
    // public override void OnRoomListUpdate(List<RoomInfo> roomList)
    // {
    //     GameObject? gm1 = GameObject.Find("GameManager");
    //     gm1.GetComponent<TabControl>().TabOutput();
    // }

    public void TabOutline()
    {
        if (photonView.IsMine)
        {
            GameObject.Find("Tab").transform.GetChild(1).GetComponentsInChildren<Outline>().ToList()
                .ForEach(p => p.enabled = photonView.Owner.NickName ==
                                             p.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text);
            
        }
    }
    [PunRPC]
    public void RPC_Respawn()
    {
        
        // photonView.RPC(nameof(RPC_ChangeRender), RpcTarget.All, false);
        // if (photonView.IsMine)
        // {
        //     if (myTeam == 1)
        //     {
        //         photonView.transform.position = spawnPosition;
        //         photonView.transform.rotation = Quaternion.Euler(0, 0, 0);
        //     }
        //     else if (myTeam == 2)
        //     {
        //         photonView.transform.position = spawnPosition;
        //         photonView.transform.rotation = Quaternion.Euler(0, 180, 0);
        //     }
        // }
        //
        //
        // if (photonView.IsMine)
        // {
        //     playerHp = 500;
        //     photonView.RPC(nameof(RPC_ChangeRender), RpcTarget.All, true);
        //     photonView.RPC(nameof(RPC_ChangeColor), RpcTarget.AllBuffered, teamColor[myTeam-1].r,
        //         teamColor[myTeam-1].g, teamColor[myTeam-1].b, 1f);
        // }
        // gameObject.GetComponent<PlayerLocal>().force = 0;
        if (photonView.IsMine)
        {
            PlayerLocal.enabled = true;
            gameObject.GetComponent<RayScript>().enabled = true;
            CrosshairManager.enabled = true;
            Canvas.SetActive(true);
            gameObject.GetComponent<PlayerLocal>().force = 10000;
        }
        
    }
}
