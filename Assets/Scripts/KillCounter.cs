using System;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class KillCounter : MonoBehaviourPunCallbacks, IPunObservable
{
    public TMP_Text killsCountTextTeam1;
    public int killsCountTeam1;
    public Image killsLineTeam1;
    
    public TMP_Text killsCountTextTeam2;
    public int killsCountTeam2;
    public Image killsLineTeam2;


    public void Output()
    {
        killsCountTextTeam1.text = $"{killsCountTeam1}";
        killsLineTeam1.fillAmount = killsCountTeam1 / 30f;
        killsCountTextTeam2.text = $"{killsCountTeam2}";
        killsLineTeam2.fillAmount = killsCountTeam2 / 30f;
    }
    public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps)
    {
        Output();
        // Debug.Log(changedProps.ContainsKey("K"));
        if (changedProps.ContainsKey("K"))
        {
            if ((int) targetPlayer.CustomProperties["Team"] == 1)
            {
                killsCountTeam1++;
                // killsCountTextTeam1.text = $"{killsCountTeam1}";
                // killsLineTeam1.fillAmount = killsCountTeam1 / 30f;
            }
            else if ((int) targetPlayer.CustomProperties["Team"] == 2)
            {
                killsCountTeam2++;
                // killsCountTextTeam2.text = $"{killsCountTeam2}";
                // killsLineTeam2.fillAmount = killsCountTeam2 / 30f;
            }
        }
        Output();
    }

    public bool isStart = true;
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting && PhotonNetwork.IsMasterClient)
        {
            stream.SendNext(killsCountTeam1);
            stream.SendNext(killsCountTeam2);
        }
        else if(stream.IsReading && isStart)
        {
            isStart = false;    
            killsCountTeam1 = (int) stream.ReceiveNext();
            killsCountTeam2 = (int) stream.ReceiveNext();
        }
    }
}
