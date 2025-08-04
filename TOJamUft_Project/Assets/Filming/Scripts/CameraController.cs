using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float sensY;
    public float sensX;

    public Transform orientation;

    [HideInInspector]
    public float xRotation;
    [HideInInspector]
    public float yRotation;

    [Header("States")]
    public int stateIndex;
    public List<CamState> cStateList = new List<CamState>();
    protected CamState cState;


    // Start is called before the first frame update
    void Start()
    {
        cState = cStateList[0];
    }

    // Update is called once per frame
    void Update()
    {
        cState = cStateList[0];
        cState.activate();
        cState.rotateCam();
    }
}
