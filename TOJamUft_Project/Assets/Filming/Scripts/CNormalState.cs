using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CNormalState : CamState
{
    public override void activate()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public override void rotateCam()
    {
        //Mouse input 
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * camCon.sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * camCon.sensY;

        camCon.yRotation += mouseX;
        camCon.xRotation -= mouseY;

        //Limits rotation of camera in the x axis
        camCon.xRotation = Mathf.Clamp(camCon.xRotation, -90f, 90f);

        //Movement of character with the camera
        camCon.transform.rotation = Quaternion.Euler(camCon.xRotation, camCon.yRotation, 0);
        camCon.orientation.rotation = Quaternion.Euler(0, camCon.yRotation, 0);
    }
}
