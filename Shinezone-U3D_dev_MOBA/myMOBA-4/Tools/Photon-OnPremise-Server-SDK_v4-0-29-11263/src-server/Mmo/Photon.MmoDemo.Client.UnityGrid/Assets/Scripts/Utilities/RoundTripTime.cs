// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RoundTripTime.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   The rtt.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Photon.MmoDemo.Client;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(RunBehaviour))]
public class RoundTripTime : MonoBehaviour
{
    private Game game;
    public Text UiRttItem;


    // initializes this component with a Game
    public void Start()
    {
        if (this.UiRttItem == null)
        {
            enabled = false;
            return;
        }

        RunBehaviour rb = GetComponent<RunBehaviour>();
        if (rb == null || rb.Game == null)
        {
            Debug.Log("'Game' component not found. Not showing a Roundtrip Time.");
            enabled = false;
        }

        this.game = rb.Game;
    }

    // called by unity each frame. will update the RTT display (if any)
    public void Update()
    {
        if (this.game != null)
        {
            this.UiRttItem.text = string.Format("{0} RTT   {1} VAR", this.game.Peer.RoundTripTime, this.game.Peer.RoundTripTimeVariance);
        }
    }
}