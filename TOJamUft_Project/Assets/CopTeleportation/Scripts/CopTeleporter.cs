using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;

public class CopTeleporter : MonoBehaviour
{
    public float speed = 0.25f;     // The speed of the cursor when being moved with the joystick

    public GameObject CopCar;       // The cop car that is teleported

    public GameObject SelectorImage;  // The selector image, which is shown/hidden as needed
    public GameObject CopMapImage;    // The cop teleportation map image, which is shown/hidden as needed

    private bool state = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha6)) {
            // When the key is pressed
            state = !state;
            if (state) {
                SelectorImage.SetActive(true);
                CopMapImage.SetActive(true);
            }
            else {
                // Find closest marked car
                GameObject closestCar = null;
                float minDist = Mathf.Infinity;

                GameObject[] markedCars = GameObject.FindGameObjectsWithTag("MarkedCar");
                foreach (GameObject car in markedCars) {
                    Vector3 diff = car.transform.position - transform.position;
                    float dist = diff.sqrMagnitude;

                    if (dist < minDist) {
                        minDist = dist;
                        closestCar = car;
                    }
                }

                if (closestCar != null) {
                    // "Stop" both entities in place
                    Rigidbody rb1 = CopCar.GetComponent<Rigidbody>();
                    rb1.linearVelocity = Vector3.zero;
                    rb1.angularVelocity = Vector3.zero;

                    Rigidbody rb2 = closestCar.GetComponent<Rigidbody>();
                    rb2.linearVelocity = Vector3.zero;
                    rb2.angularVelocity = Vector3.zero;

                    // Swap positions and angles
                    Vector3 TempPos = CopCar.transform.position;
                    CopCar.transform.position = closestCar.transform.position;
                    closestCar.transform.position = TempPos;

                    Quaternion tempRot = CopCar.transform.rotation;
                    CopCar.transform.rotation = closestCar.transform.rotation;
                    closestCar.transform.rotation = tempRot;
                }
                SelectorImage.SetActive(false);
                CopMapImage.SetActive(false);
            }
        }

        // Main loop
        if (state) {
            Vector3 delta = new Vector3(Input.GetAxis("Horizontal1"), 0f, Input.GetAxis("Vertical1"));
            transform.position += delta * speed;   
        }
    }
}
