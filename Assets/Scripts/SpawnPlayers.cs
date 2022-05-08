using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class SpawnPlayers : MonoBehaviour
{
    public GameObject player;
    public float minX, minY, minZ, maxX, maxY, maxZ;
    // Start is called before the first frame update
    void Start()
    {
        // player.GetComponent<PlayerLocal>().force = 8000f;
        Vector3 randomPosition =
            new Vector3(0, 0, -7000f);
        // Quaternion roration = Quaternion.Euler(-90f,0f,0f);
        PhotonNetwork.Instantiate(player.name, randomPosition, Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
