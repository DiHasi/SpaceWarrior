using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class CrosshairManager : MonoBehaviourPunCallbacks
{
    public GameObject ship;
    public RectTransform Rect; 
    private Vector3 pos;
    public float t = 0.25f;
    public float speed = 9f;
    
    private void Start()
    {
        pos = Rect.position;
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");
            // print(Rect.position.x + " " + Rect.position.y);
            var position = Rect.position;
            position += new Vector3(Mathf.Clamp(mouseX * speed, -5, 5),
                Mathf.Clamp(mouseY * speed, -5, 5));
            position = Vector3.Lerp(position, pos, 0.01f);
            Rect.position = position;
        }
    }
}
