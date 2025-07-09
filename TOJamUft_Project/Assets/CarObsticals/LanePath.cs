using System.Collections.Generic;
using UnityEngine;
using static LaneGenerator;

[CreateAssetMenu(menuName = "LanePath")]
public class LanePath : ScriptableObject
{
    public List<Vector3> points = new();
    public List<LanePath> nextLanes = new();
}
