using UnityEngine;
using UnityEngine.UI;

public class MovePlayer : MonoBehaviour
{
    public MoveCar MoveCar;

    public Text VInputText;
    public Text HInputText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        float translationInput = Input.GetAxis("Vertical");
        float rotationInput = Input.GetAxis("Horizontal");

        VInputText.text = "V: " + translationInput;
        HInputText.text = "H: " + rotationInput;
        MoveCar.ProcessInput(translationInput, rotationInput);
    }
}
