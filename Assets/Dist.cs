using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.UI;

public class Dist : MonoBehaviour
{
    public Text Text;

    public RayScript ray;

    void Update()
    {
        Text.text = ray.raycastHit.distance.ToString();
    }
}
