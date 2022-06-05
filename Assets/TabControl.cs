using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class TabControl : MonoBehaviourPunCallbacks
{
    public GameObject TabCanvas;

    public GameObject PanelTemplate1;
    public GameObject PanelTemplate2;
    public GameObject Parent;


    // Start is called before the first frame update
    void Start()
    {
        PanelTemplate1.SetActive(false);
        PanelTemplate2.SetActive(false);
        TabCanvas.GetComponent<Canvas>().enabled = false;
    }

    void TabOutput()
    {

        for (int i = 0; i < Parent.transform.childCount; i++) {
            Destroy (Parent.transform.GetChild(i).gameObject);
        }

        
        var playersTeam1 = PhotonNetwork.PlayerList.Where(p => (int)p.CustomProperties["Team"] == 1).ToList();  
        var playersTeam2 = PhotonNetwork.PlayerList.Where(p => (int)p.CustomProperties["Team"] == 2).ToList();
        // Debug.Log(playersTeam2.Count);
        GameObject g;
        GameObject planeTemplate = TabCanvas.transform.GetChild(0).gameObject;
        
        GameObject PT1 = Instantiate(PanelTemplate1, Parent.transform);
        GameObject PT2 = Instantiate(PanelTemplate2, Parent.transform);
        
        foreach (var player in playersTeam1)
        {
            int i = playersTeam1.IndexOf(player);
            g = Instantiate(PT1, Parent.transform);
            g.SetActive(true);
            g.transform.position = PT1.transform.position + new Vector3(0, (445-60 * i - (i)), 0);
            g.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = player.NickName;
            g.transform.Find("Kills").GetComponent<TextMeshProUGUI>().text = player.CustomProperties["K"].ToString();
            g.transform.Find("Deaths").GetComponent<TextMeshProUGUI>().text = player.CustomProperties["D"].ToString();
        }
        Destroy(PT1);
        foreach (var player in playersTeam2)
        {
            int i = playersTeam2.IndexOf(player);
            g = Instantiate(PT2, Parent.transform);
            g.SetActive(true);
            g.transform.position = PT2.transform.position + new Vector3(0, (445-60 * i - (i)), 0);
            g.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = player.NickName;
            g.transform.Find("Kills").GetComponent<TextMeshProUGUI>().text = player.CustomProperties["K"].ToString();
            g.transform.Find("Deaths").GetComponent<TextMeshProUGUI>().text = player.CustomProperties["D"].ToString();
        }
        Destroy(PT2);
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Tab))
        {
            TabCanvas.GetComponent<Canvas>().enabled = true;
        }
        else
        {
            TabCanvas.GetComponent<Canvas>().enabled = false;
        }
    }

    public override void OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics)
    {
        TabOutput();
    }

    public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps)
    {
        TabOutput();
    }
}
