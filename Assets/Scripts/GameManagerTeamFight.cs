using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class GameManagerTeamFight : MonoBehaviourPunCallbacks
{
    public static GameManagerTeamFight GS;
    public GameObject player;
    public Camera Camera;
    // private bool isStart = false;
    public Canvas Canvas;
    public Canvas KillCounterCanvas; 

    public Player body;

    public int nextTeam;
    public bool isStart = false;
    public float speedRot;
    
    
    public List<Vector3> spawnPositionsTeam1 = new List<Vector3>()
    {
        new Vector3(0, 60, 4000),
        new Vector3(0, 30, 4000),
        new Vector3(0, 0, 4000),
        new Vector3(0, -40, 4000),
    };
    public List<Vector3> spawnPositionsTeam2 = new List<Vector3>()
    {
        new Vector3(0, 60, -4000),
        new Vector3(0, 30, -4000),
        new Vector3(0, 0, -4000),
        new Vector3(0, -40, -4000),
    };
    // Start is called before the first frame update
    void Start()
    {
        FindObjectsOfType<Player>().ToList().ForEach(p => p.photonView.RPC("Ready", RpcTarget.AllBuffered));
        PhotonNetwork.CurrentRoom.PlayerTtl = 0;
    }

    private void Awake()
    {
        var Avatar = PhotonNetwork.Instantiate(player.name, new Vector3(0, 0, 0), Quaternion.identity);
        var body3 = Avatar.transform.Find("Body3");
        Canvas.enabled = true;
        Camera.enabled = true;
        
        body3.GetComponent<Renderer>().enabled = false;
        body3.GetComponent<PlayerLocal>().enabled = false;
        body3.GetComponent<PlayerLocal>().camera.SetActive(false);
        body3.GetComponent<Player>().Canvas.SetActive(false);
    }
    
    public void UpdateTeam()
    {
        if (nextTeam == 1)
        {
            nextTeam = 2;
        }
        else if(nextTeam == 2)
        {
            nextTeam = 1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Camera.transform.RotateAround(Vector3.zero, new Vector3(0, 
            Mathf.Lerp(-120, 120, speedRot), 
            Mathf.Lerp(-100, 100, speedRot)), 
            Mathf.Lerp(0, 360, speedRot));
        
        if (PhotonNetwork.PlayerList.Length > 1 && !isStart)
        {
            StartCoroutine(StartCor());
        }
    }

    public IEnumerator StartCor()
    {
        yield return new WaitForSeconds(1);
        if (PhotonNetwork.PlayerList.ToList().All(p => p.CustomProperties["Ready"] != null))
        {
            if (PhotonNetwork.PlayerList.ToList().All(p => (bool) p.CustomProperties["Ready"]))
            {
                Canvas.enabled = false;
                Camera.enabled = false;
                isStart = true;
                KillCounterCanvas.enabled = true;
            }
        }
        else
        {
            yield return null;
        }
    }
}
