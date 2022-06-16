#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;


public class Manager : MonoBehaviourPunCallbacks
{

    
    public GameObject rect;
    public PlayerLocal PlayerLocal;
    public GameObject gun;
    public bool PauseMenuActive = false;
    public Canvas Menu;
    public CrosshairManager CrosshairManager;
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
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        GameObject? gm = GameObject.Find("GameManager");
        gm.GetComponent<TabControl>().TabOutput();
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                SwitchPauseMenu();
            }
        }
    }
    private void SwitchPauseMenu()
    {
        if (photonView.IsMine)
        {
            CrosshairManager.enabled = PauseMenuActive;
            rect.SetActive(PauseMenuActive);
            gun.gameObject.GetComponent<GunScript>().enabled = PauseMenuActive;
            PauseMenuActive = !PauseMenuActive;
            Menu.enabled = PauseMenuActive;
            Cursor.visible = PauseMenuActive;
        }
    }
    public void Exit()
    {
        Application.Quit();
    }
}
