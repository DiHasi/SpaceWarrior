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

    public Player body;

    public int nextTeam;
    public bool isStart = false;
    public float speedRot;
    // Start is called before the first frame update
    void Start()
    {
        
        
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
        if (PhotonNetwork.PlayerList.Length > 0 && !isStart)
        {
            Canvas.enabled = false;
            Camera.enabled = false;
            isStart = true;
        }
    }
    
}
