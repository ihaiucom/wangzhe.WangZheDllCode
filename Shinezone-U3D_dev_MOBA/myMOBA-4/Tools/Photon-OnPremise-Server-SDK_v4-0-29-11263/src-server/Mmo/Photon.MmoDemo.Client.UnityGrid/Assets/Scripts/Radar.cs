// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Radar.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
// </summary>
// --------------------------------------------------------------------------------------------------------------------


using System.Collections.Generic;
using Photon.MmoDemo.Common;
using UnityEngine;

public class Radar : MonoBehaviour
{
    // radar! by oPless from the original javascript by PsychicParrot,
    // who in turn adapted it from a Blitz3d script found in the
    // public domain online somewhere ....

    public Texture blip;
    public Texture blibSelf;
    public Vector2 mapPosition = new Vector2(50, 250);
    public GUIStyle style = new GUIStyle(GUIStyle.none); // only used for radar

    protected internal string selfId; // ID of item to draw as "selfBlip".
    private Vector2 mapSize = new Vector2(100, 100);

    internal protected Rect worldRect; // min/max world-position values for items


    private readonly Dictionary<string, Vector> itemPositions = new Dictionary<string, Vector>();

    public void OnRadarUpdate(string itemId, ItemType itemType, Vector position)
    {
        itemId += itemType;
        if (!this.itemPositions.ContainsKey(itemId))
        {
            this.itemPositions.Add(itemId, position);
            return;
        }

        this.itemPositions[itemId] = position;
    }

    public void OnGUI()
    {
        DrawRadar();
    }


    public void DrawRadar()
    {
        if (this.itemPositions.Count == 0)
        {
            return;
        }

        float scaleX = this.mapSize.x/this.worldRect.width;
        float scaleY = this.mapSize.y/this.worldRect.height;

        Vector3 north = new Vector3(0, 0, 1);
        Vector3 direction = new Vector3(Camera.main.transform.forward.x, 0, Camera.main.transform.forward.z);
        Vector3 whichWay = Vector3.Cross(Camera.main.transform.forward, north - direction);
        float angle = Vector3.Angle(north, direction);
        if (whichWay.y < 0)
        {
            angle *= -1;
        }


        Vector2 center = new Vector2(this.mapPosition.x + (this.mapSize.x/2), this.mapPosition.y + (this.mapSize.y/2));
        Matrix4x4 oldMatrix = GUI.matrix;
        GUIUtility.RotateAroundPivot(angle, center);

        GUI.BeginGroup(new Rect(this.mapPosition.x, this.mapPosition.y, this.mapSize.x, this.mapSize.y));

        foreach (KeyValuePair<string, Vector> pair in this.itemPositions)
        {
            float x = pair.Value.X;
            float y = this.worldRect.height - pair.Value.Y; // the radar has it's 0,0 coordinate top-left. so Y coordinates have to be turned around
            x *= scaleX;
            y *= scaleY;

            GUI.DrawTexture(new Rect(x - 1, y - 1, 2, 2), this.blip);
        }

        float myX = 0;
        float myY = 0;
        if (this.itemPositions.ContainsKey(this.selfId))
        {
            myX = this.itemPositions[this.selfId].X;
            myY = this.worldRect.height - this.itemPositions[this.selfId].Y; // the radar has it's 0,0 coordinate top-left. Y coordinates have to be turned around

            GUI.DrawTexture(new Rect((myX*scaleX) - 1, (myY*scaleY) - 1, 3, 3), this.blibSelf);
        }
        const int LabelSize = 20;

        this.style.alignment = TextAnchor.UpperCenter;
        GUI.Label(new Rect((this.mapSize.x/2f) - (LabelSize/2f), 0, LabelSize, LabelSize), "N", this.style);
        this.style.alignment = TextAnchor.LowerCenter;
        GUI.Label(new Rect((this.mapSize.x/2f) - (LabelSize/2f), this.mapSize.y - LabelSize, LabelSize, LabelSize), "S", this.style);
        this.style.alignment = TextAnchor.MiddleLeft;
        GUI.Label(new Rect(0, (this.mapSize.y/2f) - (LabelSize/2f), LabelSize, LabelSize), "W", this.style);
        this.style.alignment = TextAnchor.MiddleRight;
        GUI.Label(new Rect(this.mapSize.x - LabelSize, (this.mapSize.y/2f) - (LabelSize/2f), LabelSize, LabelSize), "E", this.style);

        GUI.EndGroup();

        GUI.matrix = oldMatrix;
        this.style.alignment = TextAnchor.MiddleLeft;
        GUI.Label(new Rect(this.mapPosition.x, this.mapPosition.y + this.mapSize.y + 5, 100, 20), string.Format("Radar items: {0}", this.itemPositions.Count), this.style);
    }
}