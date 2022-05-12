using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class GameManagerTeamFight : MonoBehaviourPunCallbacks
{
    public GameObject player;
    public Camera Camera;
    private bool isStart = false;
    public Canvas Canvas;

    public float speedRot;
    // Start is called before the first frame update
    void Start()
    {
        // Camera.GetComponent<Renderer>().enabled = true;
        // shipRed.GetComponent<Renderer>().enabled = false;
        // shipBlue.GetComponent<Renderer>().enabled = false;
        // building.GetComponent<Renderer>().enabled = false;
    }

    Vector3 vec;
    // Update is called once per frame
    void Update()
    {
        
        if (!isStart)
        {
            vec = Vector3.Lerp(new Vector3(0, 0, 0), new Vector3(0, 480, 24), speedRot);
            Camera.transform.RotateAround(Vector3.zero, vec, Mathf.Lerp(0, 360, speedRot));
        }
        
        if (PhotonNetwork.PlayerList.Length > 1 && !isStart)
        {
            Canvas.enabled = false;
            isStart = true;
            Vector3 randomPosition =
                new Vector3(0, 0, -7000f);
            PhotonNetwork.Instantiate(player.name, randomPosition, Quaternion.identity);
            Camera.enabled = false;
        }
    }
}
