using Photon.Pun;
using TMPro;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class KillCounter : MonoBehaviourPunCallbacks
{
    public TMP_Text killsCountTextTeam1;
    public int killsCountTeam1;
    public Image killsLineTeam1;
    
    public TMP_Text killsCountTextTeam2;
    public int killsCountTeam2;
    public Image killsLineTeam2;
    public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps)
    {
        // Debug.Log(changedProps.ContainsKey("K"));
        if (changedProps.ContainsKey("K"))
        {
            if ((int) targetPlayer.CustomProperties["Team"] == 1)
            {
                killsCountTeam1++;
                killsCountTextTeam1.text = $"{killsCountTeam1}";
                killsLineTeam1.fillAmount = killsCountTeam1 / 30f;
            }
            else if ((int) targetPlayer.CustomProperties["Team"] == 2)
            {
                killsCountTeam2++;
                killsCountTextTeam2.text = $"{killsCountTeam2}";
                killsLineTeam2.fillAmount = killsCountTeam2 / 30f;
            }
        }
    }
}
