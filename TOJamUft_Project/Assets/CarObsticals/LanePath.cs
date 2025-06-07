// Assets/Scripts/Traffic/LanePath.cs
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewLanePath", menuName = "Traffic/Lane Path")]
public class LanePath : ScriptableObject
{
    public List<Vector3> points = new();

    // List of potential next lanes
    //public List<LanePath> nextLanes = new();
    public object gameObject;
}
