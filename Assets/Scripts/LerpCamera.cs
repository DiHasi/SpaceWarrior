using UnityEngine;
using System.Collections;

public class LerpCamera: MonoBehaviour {
    
    public Transform target;

    public Vector3 distFromObject;
    


    void  LateUpdate ()
    {
        transform.LookAt(target);
    }
}