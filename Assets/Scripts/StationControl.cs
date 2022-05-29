using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationControl : MonoBehaviour
{
    public List<GameObject> Objects;
    public GameObject Shpere1; 
    public GameObject Shpere2;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setRigitBodyTrue()
    {
        Objects.ForEach(o => o.GetComponent<Rigidbody>().isKinematic = false);
    }
}
