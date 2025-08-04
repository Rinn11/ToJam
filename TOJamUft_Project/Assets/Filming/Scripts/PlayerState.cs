using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class PlayerState : MonoBehaviour
{
    public PlayerMovement pMove;
    public CameraController camCon;
    public abstract void activate();
    public abstract void playerInput();
    public abstract void movePlayer();
}
