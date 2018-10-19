// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FramesPerSecond.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   The fps.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;

using UnityEngine;

/// <summary>
/// The fps.
/// </summary>
public class FramesPerSecond : MonoBehaviour
{
    // Attach this to a GUIText to make a frames/second indicator.
    // It calculates frames/second over each UpdateInterval,
    // so the display does not keep changing wildly.
    // It is also fairly accurate at very low FPS counts (<10).
    // We do this not by simply counting frames per interval, but
    // by accumulating FPS for each frame. This way we end up with
    // correct overall FPS even if the interval renders something like
    // 5.5 frames.

    /// <summary>
    /// The update interval.
    /// </summary>
    public readonly float UpdateInterval = 0.5f;

    /// <summary>
    /// The accum.
    /// </summary>
    private float accum; // FPS accumulated over the interval

    /// <summary>
    /// The frames.
    /// </summary>
    private int frames; // Frames drawn over the interval

    /// <summary>
    /// The timeleft.
    /// </summary>
    private float timeleft; // Left time for current interval

    /// <summary>
    /// The start.
    /// </summary>
    public void Start()
    {
        if (!this.GetComponent<GUIText>())
        {
            Debug.Log("UtilityFramesPerSecond needs a GUIText component!");
            this.enabled = false;
            return;
        }

        this.timeleft = this.UpdateInterval;
    }

    /// <summary>
    /// The update.
    /// </summary>
    public void Update()
    {
        this.timeleft -= Time.deltaTime;
        this.accum += Time.timeScale / Time.deltaTime;
        ++this.frames;

        // Interval ended - update GUI text and start new interval
        if (this.timeleft <= 0.0)
        {
            // display two fractional digits (f2 format)
            float fps = this.accum / this.frames;
            string format = String.Format("{0:F2} FPS", fps);
            this.GetComponent<GUIText>().text = format;

            if (fps < 10)
            {
                this.GetComponent<GUIText>().material.color = Color.red;
            }
            else if (fps < 30)
            {
                this.GetComponent<GUIText>().material.color = Color.yellow;
            }
            else
            {
                this.GetComponent<GUIText>().material.color = Color.green;
            }

            // DebugConsole.Log(format,level);
            this.timeleft = this.UpdateInterval;
            this.accum = 0.0f;
            this.frames = 0;
        }
    }
}
