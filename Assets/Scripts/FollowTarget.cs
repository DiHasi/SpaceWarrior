using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    public Vector3 dist;
    public Transform target;

    // Update is called once per frame
    void Update()
    {
        
        // _transform.position = target.position + dist;
        // Quaternion quaternion = target.rotation;
        transform.position = target.position;

        transform.rotation = target.rotation;
        // transform.RotateAround(target.position, Vector3.up, target.rotation.eulerAngles.magnitude);
    }
}
