using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Bullets : MonoBehaviour
{
    public GameObject bulletPrefab;
    public List<GameObject> BulletsList = new List<GameObject>();
    private void Awake()
    {
        for (int i = 0; i < 110; i++)
        {
            var g = PhotonNetwork.Instantiate(bulletPrefab.name, Vector3.zero, Quaternion.identity);
            g.name = $"bul_{i + 1}";
            BulletsList.Add(g);
        }
    }
}
