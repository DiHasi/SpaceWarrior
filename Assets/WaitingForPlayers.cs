using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class WaitingForPlayers : MonoBehaviour
{
    public TMP_Text Text;
    private GameManagerTeamFight GM;

    private void Start()
    {
         GM = GameObject.Find("GameManager").GetComponent<GameManagerTeamFight>();
    }

    void Update()
    {
        if (!GM.isStart)
        {
            Text.text = $"waiting for players ({PhotonNetwork.PlayerList.Length}/{GM.playersCount})";
        }
    }
}
