using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class Timer : MonoBehaviourPunCallbacks, IPunObservable
{
    public float defaultTimerTime;
    public float startTime = 20;
    public float timer;

    public TMP_Text textTime;

    public TMP_Text winner;
    public Canvas WinnerCanvas;
    public Canvas KillCounterCanvas;
    public KillCounter KillCounter;

    void Awake()
    {
        timer = defaultTimerTime + startTime;
    }

    public bool flagA = true;
    public bool flagB = true;
    private void FixedUpdate()
    {
        if (GameObject.Find("GameManager").GetComponent<GameManagerTeamFight>().isStart)
        {
            if (timer > 0)
            {
                timer -= Time.fixedDeltaTime;

                if (timer > defaultTimerTime)
                {
                    var t = (int) ((timer - defaultTimerTime) % 60);
                    var f = t >= 10 ? ($"{t}") : ($"0{t}");
                    textTime.text = $"{(int) ((timer - defaultTimerTime) / 60)}:{f}";
                    // textTime.color = Color.cyan;
                }
                else if (flagB)
                {
                    // textTime.color = Color.white;
                    FindObjectsOfType<Player>().ToList().ForEach(p => p.photonView.RPC("RPC_Respawn", RpcTarget.AllBuffered));
                    flagB = false;
                }
                else
                {
                    var t = (int) (timer % 60);
                    var f = t >= 10 ? ($"{t}") : ($"0{t}");
                    textTime.text = $"{(int) (timer / 60)}:{f}";
                }
            }
            else
            {
                if (KillCounter.killsCountTeam1 > KillCounter.killsCountTeam2)
                {
                    winner.color = new Color(0, 0.9f, 1f);
                    winner.text = "WINNER\nBLUE";
                }
                else if (KillCounter.killsCountTeam1 < KillCounter.killsCountTeam2)
                {
                    winner.color = new Color(1, 0.5f, 0f);
                    winner.text = "WINNER\nORANGE";
                }
                else
                {
                    winner.text = "DRAW";
                }

                if (photonView.IsMine)
                {
                    if (flagA)
                    {
                        FindObjectsOfType<Player>().ToList().ForEach(p => p.photonView.RPC("RPC_Finish", RpcTarget.All, winner.text,
                            winner.color.r, winner.color.g, winner.color.b));
                        flagA = !flagA;
                    }
                                   
                }
            }

        }
    }


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(timer);
        }
        else if (stream.IsReading)
        {
            timer = (float) stream.ReceiveNext();
        }
    }
}
