using UnityEngine;
using UnityEngine.UI;

public class MoveCar : MonoBehaviour
{
    public Color DebugColor;

    private float _translationInput;
    private float _rotationInput;

    public float translateScale = 30f;
    public float rotateScale = 10f;

    private Transform _transform;
    private Rigidbody _rigidbody;

    public Text SpeedText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _transform = GetComponent<Transform>();
    }

    // Update is called once per frame
    public void ProcessInput(float translation, float rotation)
    {
        _translationInput = translation;
        _rotationInput = rotation;
    }

    private void FixedUpdate()
    {
        _rigidbody.AddForce(_transform.forward * _translationInput * translateScale, ForceMode.Force);

        _rigidbody.AddTorque(_transform.up * _rotationInput * rotateScale, ForceMode.Force);

        SpeedText.text = "Speed: " + _rigidbody.linearVelocity.magnitude + "m/s";

        Debug.DrawRay(_transform.position, _transform.forward * 10, DebugColor);

        Debug.DrawRay(_transform.position, _rigidbody.linearVelocity, DebugColor);
    }
}
