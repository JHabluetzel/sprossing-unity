using System;
using UnityEngine;

[Serializable]
public class SavedObject
{
    public string prefabName;
    public float[] position;
    public int layer;

    public SavedObject(string fullName, Vector3 position, int layer)
    {
        prefabName = fullName.Split('(')[0];
        this.position = new float[2];
        this.position[0] = position.x;
        this.position[1] = position.y;

        this.layer = layer;
    }
}
