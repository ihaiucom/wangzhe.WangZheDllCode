// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ItemBehaviour.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   The item behaviour.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using Photon.MmoDemo.Client;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public class ItemBehaviour : MonoBehaviour
{
    public Text actorText;
    public GameObject actorView;
    public GameObject actorViewExit;

    private Item item;


    private float lastMoveUpdateTime;

    private Vector3 lastMoveUpdate;


    public void Destroy()
    {
        Destroy(this);  // childs and components get destroyed, too
    }


    public void Initialize(Game mmoGame, Item actorItem, string name, Radar worldRadar)
    {
        this.item = actorItem;
        this.name = name;

        transform.position = new Vector3(this.item.Position.X, transform.position.y, this.item.Position.Y) * RunBehaviour.WorldToUnityFactor;

        ShowActor(false);
    }


    /// <summary>
    /// Updates to item logic once per frame (called by Unity).
    /// </summary>
    public void Update()
    {
        if (this.item == null || !this.item.IsUpToDate)
        {
            ShowActor(false);
            return;
        }

        // you could update the radar more often by using available info about items close-by:
        // this.radar.OnRadarUpdate(this.item.Id, this.item.Type, this.item.Position);


        byte[] colorBytes = BitConverter.GetBytes(this.item.Color);
        SetActorColor(new Color((float) colorBytes[2]/byte.MaxValue, (float) colorBytes[1]/byte.MaxValue, (float) colorBytes[0]/byte.MaxValue));


        Vector3 newPos = new Vector3(this.item.Position.X, transform.position.y, this.item.Position.Y)*RunBehaviour.WorldToUnityFactor;
        if (newPos != this.lastMoveUpdate)
        {
            this.lastMoveUpdate = newPos;
            this.lastMoveUpdateTime = Time.time;
        }

        // move smoothly
        float lerpT = (Time.time - this.lastMoveUpdateTime)/0.05f;
        bool moveAbsolute = ShowActor(true);

        if (moveAbsolute)
        {
            //// Debug.Log("move absolute: " + newPos);
            transform.position = newPos;
        }
        else if (newPos != transform.position)
        {
            //// Debug.Log("move lerp: " + newPos);
            transform.position = Vector3.Lerp(transform.position, newPos, lerpT);
        }

        // view distance
        if (this.item.ViewDistanceEnter.X > 0 && this.item.ViewDistanceEnter.Y > 0)
        {
            this.actorView.transform.localScale = new Vector3(this.item.ViewDistanceEnter.X, this.item.ViewDistanceEnter.Y, this.item.ViewDistanceEnter.Z) * RunBehaviour.WorldToUnityFactor;
            this.actorView.transform.localScale *= 2; // ViewDistanceEnter is +/- units from the item's position. So we have to double the quad-scale.
        }
        else
        {
            this.actorView.transform.localScale *= 0;
        }
        // exit view distance
        if (this.item.ViewDistanceExit.X > 0 && this.item.ViewDistanceExit.Y > 0)
        {
            this.actorViewExit.transform.localScale = new Vector3(this.item.ViewDistanceExit.X, this.item.ViewDistanceExit.Y, this.item.ViewDistanceExit.Z) * RunBehaviour.WorldToUnityFactor;
            this.actorViewExit.transform.localScale *= 2; // ViewDistanceExit is +/- units from the item's position. So we have to double the quad-scale.
        }
        else
        {
            this.actorViewExit.transform.localScale *= 0;
        }


        // update text
        actorText.text = this.item.Text;
        if (this.item.IsMine)
        {
            actorText.text = string.Format("{0}\n{1:0.0}:{2:0.0}", this.item.Text, this.item.Position.X, this.item.Position.Y);
        }
        else
        {
            actorText.text = string.Format("{0}", this.item.Text);
        }
    }


    private void SetActorColor(Color actorColor)
    {
        var renderers = GetComponentsInChildren<Renderer>();
        foreach (var render in renderers)
        {
            render.material.color = actorColor;
        }
        this.actorText.color = actorColor;

        actorColor.a = 0.10f;
        this.actorView.GetComponent<Renderer>().material.color = actorColor;
        actorColor.a = 0.05f;
        this.actorViewExit.GetComponent<Renderer>().material.color = actorColor;
    }


    private bool ShowActor(bool show)
    {
        var renderers = GetComponentsInChildren<Renderer>();
        if (renderers[0].enabled != show)
        {
            foreach (var render in renderers)
            {
                render.enabled = show;
            }
            return true;
        }
        this.actorText.enabled = show;

        return false;
    }
}