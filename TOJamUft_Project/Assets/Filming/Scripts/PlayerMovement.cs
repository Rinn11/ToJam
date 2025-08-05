using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;

    public float groundDrag;

    public float jumpForce;
    public float jumpTimer;
    public float airMultiplier;
    public float flyForce;
    public Transform cam;

    [HideInInspector]
    public bool readyToJump;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;

    [Header("Keybinds")]
    public KeyCode jumpKey;
    public KeyCode ctrlKey;
    public KeyCode addSpeedKey;
    public KeyCode subSpeedKey;

    public Transform orientation;

    [HideInInspector]
    public float horizontalInput;
    [HideInInspector]
    public float verticalInput;

    [HideInInspector]
    public Vector3 moveDirection;
    [HideInInspector]
    public Rigidbody rb;

    [Header("State")]
    public int stateIndex = 1;
    public List<PlayerState> pStateList = new List<PlayerState>();
    protected PlayerState pState;
    public Vector3 spawnPoint;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        readyToJump = true;

        // activate the state immediately.
        pState = pStateList[stateIndex];
        pState.activate();

        //sets spawnpoint
        spawnPoint = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        pState.playerInput();
        // speedLimit();

        pState = pStateList[stateIndex];
        pState.activate();
    }
    public void Respawn(){
        this.gameObject.SetActive(false);
        if (rb == null) rb = GetComponent<Rigidbody>();
        rb.linearVelocity = new Vector3(0f, 0f, 0f);
        transform.position = spawnPoint;
        this.gameObject.SetActive(true);
    }
    private void FixedUpdate()
    {
        pState.movePlayer();

        // Debug.Log("Speed: " + moveSpeed.ToString("F2"));
    }
    public void FreeCam(){
        stateIndex = 2;
    }
    public void Movement(){
        Respawn();
        stateIndex = 1;
    }
    public void Locked(){
        stateIndex = 0;
    }
    private void speedLimit()
    {
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
        }
    }
}
