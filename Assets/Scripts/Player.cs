using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using UnityEngine;

public class Player : MonoBehaviourPunCallbacks
{
    public int playerHp;

    public PlayerLocal PlayerLocal;
    public GameObject Canvas;
    public List<Collider> Colliders;

    void Start()
    {
        transform.name = photonView.Owner.NickName;
    }

    private void Awake()
    {
        if (!photonView.IsMine)
        {
            PlayerLocal.enabled = false;
            Destroy(PlayerLocal.camera.gameObject);
            Canvas.SetActive(false);
        }

        if (photonView.IsMine)
        {
            foreach (var collider1 in Colliders)
            {
                collider1.enabled = false;
            }
        }
    }
}
