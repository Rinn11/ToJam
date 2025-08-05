using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PFreeCamState : PlayerState
{

    public override void activate()
    {
        // Unlock Camera
        camCon.stateIndex = 1;

        // Make so that rb does not cause unecessary interactions in free cam
        pMove.rb.linearDamping = pMove.groundDrag;
        pMove.rb.detectCollisions = false;
        pMove.rb.useGravity = false;

        // Also reset the parent back to null to ensure there is no unintended behavior...
        Transform playerTransf = pMove.gameObject.transform;
        if (playerTransf.parent != null) playerTransf.SetParent(null);
    }

    public override void playerInput()
    {
        //keyboard inputs
        pMove.horizontalInput = Input.GetAxisRaw("Horizontal");
        pMove.verticalInput = Input.GetAxisRaw("Vertical");

        // Fly up is space
        if (Input.GetKey(pMove.jumpKey))
        {
            // We use time.deltatime because it could be held down constantly...
            pMove.rb.AddForce(pMove.transform.up * pMove.flyForce * Time.deltaTime, ForceMode.Force);
        }
        else if (Input.GetKey(pMove.ctrlKey))
        {
            // Fly down is LCtrl
            pMove.rb.AddForce(-pMove.transform.up * pMove.flyForce * Time.deltaTime, ForceMode.Force);
        }

        // Add speed
        if (Input.GetKeyDown(pMove.addSpeedKey))
        {
            pMove.moveSpeed += 5f;
            // Debug.Log("Speed: " + pMove.moveSpeed.ToString("F2"));
        }
        // Subtract speed
        else if (Input.GetKeyDown(pMove.subSpeedKey))   
        {
            pMove.moveSpeed -= 5f;
            if (pMove.moveSpeed < 0f) pMove.moveSpeed = 0f; // Prevent negative speed
            // Debug.Log("Speed: " + pMove.moveSpeed.ToString("F2"));
        }
    }

    public override void movePlayer()
    {
        // Perform a movement calculation but with the camera's forward transform instead of orientation...
        pMove.moveDirection = pMove.cam.forward * pMove.verticalInput + pMove.orientation.right * pMove.horizontalInput;
        pMove.rb.AddForce(pMove.moveDirection.normalized * pMove.moveSpeed * 10f, ForceMode.Force);
    }


}
