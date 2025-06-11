using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "LanePath")]
public class LanePath : ScriptableObject
{
    public List<Vector3> points = new();
}
