#nullable enable
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class Manager : MonoBehaviourPunCallbacks
{
    public void ExitToMenu()
    {
        StartCoroutine(LeaveAndLoad());
    }

    public IEnumerator LeaveAndLoad()
    {
        PhotonNetwork.LeaveRoom();
        while (PhotonNetwork.InRoom)
        {
            yield return null;
        }
        PhotonNetwork.LoadLevel("Menu");
    }
    public void ExitToWindow()
    {
        Application.Quit();
    }

    public override void OnLeftRoom()
    {
        GameObject? gm = GameObject.Find("GameManager");
        gm.GetComponent<TabControl>().TabOutput();
        // Debug.Log(12345678);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        GameObject? gm = GameObject.Find("GameManager");
        gm.GetComponent<TabControl>().TabOutput();
        // Debug.Log(87654321);
    }
}
