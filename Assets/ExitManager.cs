using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class ExitManager : MonoBehaviourPunCallbacks
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
        
    
}
