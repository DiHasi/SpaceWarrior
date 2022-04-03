using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cccc : MonoBehaviour
{
    public RayScript RayScript;
    void LateUpdate()
    {
        transform.position = RayScript.raycastHit.point;
    }
}
