using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CamState : MonoBehaviour
{
    public CameraController camCon;
    public abstract void activate();
    public abstract void rotateCam();

}
