using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class RayScript : MonoBehaviourPunCallbacks
{
    public RectTransform RectTransform;
    // public Transform gun;
    public Camera Camera;
    public LayerMask _layerMask;
    public float maxDistance;

    public List<Transform> guns = new List<Transform>();

    private Vector3 target;
    private Vector3 startPosition;

    public Transform Ship;

    public RaycastHit raycastHit;
    private void Start()
    {
        // startPosition = gun.transform.position;
    }

    void LateUpdate()
    {
        if (photonView.IsMine)
        {
            RaySolver();
        }
        
        foreach (var gun in guns)
        {
            Debug.DrawLine(gun.position, raycastHit.point, Color.magenta);
        }
        // Debug.DrawLine(gun.position, raycastHit.point, Color.magenta);
    }

    private void RaySolver()
    {
        Ray ray = Camera.ScreenPointToRay(RectTransform.position);
        // print(RectTransform.position + " " + ray.direction);
        Debug.DrawRay(ray.origin, ray.direction * 1000, Color.yellow);
        
        if (Physics.Raycast(ray, out raycastHit, maxDistance = Single.PositiveInfinity, ~_layerMask))
        {
            target = raycastHit.point;
            foreach (var gun in guns)
            {
                gun.LookAt(target);
            }
            // gun.LookAt(target);
        }
        else
        {
            var b = Ship.rotation;
            // gun.transform.position = Ship.position;
            foreach (var gun in guns)
            {
                gun.transform.rotation = Ship.rotation * Quaternion.Euler(15f, 0f, 0f);
            }
            // gun.transform.rotation = Ship.rotation * Quaternion.Euler(15f, 0f, 0f);
        }
    }
}
