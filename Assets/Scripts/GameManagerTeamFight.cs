using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;

public class GameManagerTeamFight : MonoBehaviourPunCallbacks
{
    public static GameManagerTeamFight GS;
    public GameObject player;
    public Camera Camera;
    // private bool isStart = false;
    public Canvas Canvas;

    public Player body;

    public int nextTeam = 2;

    public float speedRot;
    // Start is called before the first frame update
    void Start()
    {
        
        
    }

    private void Awake()
    {
        Canvas.enabled = false;
        Camera.enabled = false;
        ExitGames.Client.Photon.Hashtable h = new ExitGames.Client.Photon.Hashtable();
        var Avatar = PhotonNetwork.Instantiate(player.name, new Vector3(0, 0, 0), Quaternion.identity);
        
        
        var body3 = Avatar.transform.Find("Body3");
        h.Add("Team", nextTeam);
        body3.GetComponent<PhotonView>().Owner.SetCustomProperties(h);
        body3.GetComponent<Renderer>().enabled = false;
        body3.GetComponent<PlayerLocal>().enabled = false;
    }

    public void UpdateTeam()
    {
        if (nextTeam == 1)
        {
            nextTeam = 2;
        }
        else
        {
            nextTeam = 1;
        }
    }
    Vector3 vec;
    // Update is called once per frame
    void Update()
    {
       
    }
}
