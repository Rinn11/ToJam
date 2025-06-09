using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LanePath : ScriptableObject
{
    public List<Vector3> points;

    [Header("Optional: Connected Lanes")]
    public List<LanePath> nextLanes;
}
