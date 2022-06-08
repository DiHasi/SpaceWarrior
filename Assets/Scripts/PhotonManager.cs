using Photon.Pun;
using Photon.Realtime;
using TMPro;
using WebSocketSharp;
using UnityEngine;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    public string region;
    private TMP_InputField RoomName;
    public TMP_Text Name;

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.ConnectToRegion(region);
    }

    public override void OnConnectedToMaster()
    {
        
    }

    public void CreateRoom()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 8;
        PhotonNetwork.CreateRoom(RoomName.text, roomOptions, TypedLobby.Default);
    }

    public void RandomRoom()
    {
        PhotonNetwork.JoinRandomOrCreateRoom();
    }
    public void ExitToWindow()
    {
        Application.Quit();
    }

    public override void OnJoinedRoom()
    {
        if (!Name.text.IsNullOrEmpty())
        {
            ExitGames.Client.Photon.Hashtable h = new ExitGames.Client.Photon.Hashtable();
            h.Add("K", 0);
            h.Add("D", 0);
            h.Add("Team", 0);
            h.Add("Sender", "");
            PhotonNetwork.LocalPlayer.SetCustomProperties(h);
            PhotonNetwork.NickName = Name.text;
            PhotonNetwork.LoadLevel("Game");
        }
    }
}

