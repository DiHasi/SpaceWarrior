using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class MeteorSpawn : MonoBehaviourPunCallbacks
{
    public int spawnCount = 300;
    public float spawnRadius;

    public GameObject prefab;

    public Transform spawnPoint;

    public Vector3 volume;
    // Start is called before the first frame update
    void Start()
    {
        GameObject parent = new GameObject();
        while (spawnCount-- > 0)
        {
            // spawnCount--;
            PhotonNetwork.Instantiate(prefab.name, Random.insideUnitSphere * spawnRadius, Random.rotation);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
