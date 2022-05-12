using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerLocal : MonoBehaviourPunCallbacks
{
    public Rigidbody Rigidbody;
    
    public GameObject rect;

    public GameObject camera;

    public GameObject plane;
    public GameObject gun;

    public float force;
    public bool PauseMenuActive = false;
    public Canvas Menu;
    public CrosshairManager CrosshairManager;
    
    // public GameObject bullet;
    // public float cooldown = 0.5f;
    // public float time ;
    // public Transform bulletPoint;

    public float rotationSpeed = 10f;
    public float step = 5f;

    public float maxForce = 10000f;
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
            StartPosition = rect.transform.position;
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
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                SwitchPauseMenu();
            }
            
            Mover2();
        }
    }
    void Mover2()
    {
        if (!PauseMenuActive)
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
                if (force > maxForce)
                    force = maxForce;
            }
        
            if (Input.GetKey("s") && force > 0) 
            {
                force -= step*2;
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
        }
        Rigidbody.AddRelativeForce(0f, 0f, force);
    }

    private void SwitchPauseMenu()
    {
        if (photonView.IsMine)
        {
            CrosshairManager.enabled = PauseMenuActive;
            rect.SetActive(PauseMenuActive);
            gun.gameObject.GetComponent<GunScript>().enabled = PauseMenuActive;
            PauseMenuActive = !PauseMenuActive;
            Menu.enabled = PauseMenuActive;
            Cursor.visible = PauseMenuActive;
        }
        // GetComponent<PlayerLocal>().enabled = PauseMenuActive;
    }
    public void Exit()
    {
        Application.Quit();
    }
}
