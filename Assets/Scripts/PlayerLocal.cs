using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PlayerLocal : MonoBehaviourPunCallbacks
{
    public Rigidbody Rigidbody;
    
    public RectTransform Rect;

    public GameObject camera;

    public GameObject plane;

    public float force; 
    // public GameObject bullet;
    // public float cooldown = 0.5f;
    // public float time ;
    // public Transform bulletPoint;

    public float rotationSpeed = 1f;
    public float step = 1f;
    // public float rotationСoefficient = 1;
    // public float rotationСoefficient2 = 1;

    // public RayScript ray;

    public float sensitivity = 1f;

    public float Limit1 = 10f;
    public float Limit2 = 90f;

    private Vector3 StartPosition;
    private void Awake()
    {
        if (photonView.IsMine)
        {
            StartPosition = Rect.position;
            Cursor.visible = false;
            Rigidbody = GetComponent<Rigidbody>();
            Rigidbody.maxAngularVelocity = 500f;
        }

        if (!photonView.IsMine)
        {
            plane.SetActive(true);
        }
    }
    
    void Update()
    {
        if (photonView.IsMine)
        {
            Mover2();
        }
    }
    void Mover()
    {
        float Xpos = 0f;
        float Ypos = 0f;
        if (Mathf.Abs(StartPosition.x - Rect.position.x) < Limit1 && Mathf.Abs(StartPosition.y - Rect.position.y) < Limit1)
        {
            Xpos = Ypos = 0f;
        }
        if (Mathf.Abs(StartPosition.x - Rect.position.x) >= Limit1 && Mathf.Abs(StartPosition.y - Rect.position.y) >= Limit1)
        {
            Xpos = Ypos = 1f;
        }
        if (Mathf.Abs(StartPosition.x - Rect.position.x) >= Limit2 && Mathf.Abs(StartPosition.y - Rect.position.y) >= Limit2)
        {
            Xpos = Ypos = 3f;
        }
        
        print(Xpos + " " + Ypos);
        Rigidbody.AddRelativeTorque(-sensitivity * rotationSpeed * Ypos, 0f, -sensitivity * rotationSpeed * Xpos);

        if (Input.GetKey("w"))
        {
            force += step;
            if (force > 700)
                force = 700;
        }
        
        if (Input.GetKey("s") && force > 0) 
        {
            force -= step;
            if (force < 0)
                force = 0;
        }

        if (Input.GetKey("a"))
        {
            // var position = Rect.position;
            // position += new Vector3((-sensitivity * rotationSpeed), 0f);
            // Rect.position = position;
            Rigidbody.AddRelativeTorque(0f, (-sensitivity * rotationSpeed/3), 0f);
        }
        if (Input.GetKey("d"))
        {
            // var position = Rect.position;
            // position += new Vector3((sensitivity * rotationSpeed), 0f);
            // Rect.position = position;
            Rigidbody.AddRelativeTorque(0f, (sensitivity * rotationSpeed/3), 0f);
        }
        
        if (Input.GetKey("i"))
            sensitivity -= 0.1f;
        if (Input.GetKey("o"))
            sensitivity += 0.1f;
        
        Rigidbody.AddRelativeForce(0f, 0f, force);
    }
    void Mover2()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        if (mouseX != 0 || mouseY != 0)
        {
            Rigidbody.AddRelativeTorque(-sensitivity * rotationSpeed * mouseY, 0f, -sensitivity * rotationSpeed * mouseX);
        }
        if (Input.GetKey("w"))
        {
            force += step;
            if (force > 2000)
                force = 2000;
        }
        
        if (Input.GetKey("s") && force > 0) 
        {
            force -= step;
            if (force < 0)
                force = 0;
        }

        if (Input.GetKey("a"))
        {
            // var position = Rect.position;
            // position += new Vector3((-sensitivity * rotationSpeed), 0f);
            // Rect.position = position;
            Rigidbody.AddRelativeTorque(0f, (-sensitivity * rotationSpeed/3), 0f);
        }
        if (Input.GetKey("d"))
        {
            // var position = Rect.position;
            // position += new Vector3((sensitivity * rotationSpeed), 0f);
            // Rect.position = position;
            Rigidbody.AddRelativeTorque(0f, (sensitivity * rotationSpeed/3), 0f);
        }
        
        if (Input.GetKey("i"))
            sensitivity -= 0.1f;

        if (Input.GetKey("o"))
            sensitivity += 0.1f;
        
        Rigidbody.AddRelativeForce(0f, 0f, force);
    }
}
