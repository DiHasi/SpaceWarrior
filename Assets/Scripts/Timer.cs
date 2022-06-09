using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviourPunCallbacks, IPunObservable
{
    public float defaultTimerTime;
    public float timer;

    public TMP_Text textTime;

    void Awake()
    {
        timer = defaultTimerTime;
    }
    private void FixedUpdate()
    {
        // if (!PhotonNetwork.IsMasterClient)
        // {
        //     return;
        // }
        timer -= Time.fixedDeltaTime;

        var t = (int) (timer % 60);
        var f = t > 10 ? ($"{t}") : ($"0{t}");
        textTime.text = $"{(int) (timer / 60)}:{f}";
    }

    // Start is called before the first frame update


    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting && PhotonNetwork.IsMasterClient)
        {
            Debug.Log("2114");
            stream.SendNext(timer);
        }
        else if (stream.IsReading)
        {
            Debug.Log("523twgerger");
            timer = (float) stream.ReceiveNext();
        }
    }
}
