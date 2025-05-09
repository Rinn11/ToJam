using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class MovePB : MonoBehaviour
{
    private float _translationInput;
    private float _rotationInput;
    private Vector3 _userRot;
    private bool _userJumped;

    private const float _translateScale = 30f;
    private const float _rotateScale = 10f;

    private Transform _transform;
    private Rigidbody _rigidbody;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _transform = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        _translationInput = Input.GetAxis("Vertical");
        _rotationInput = Input.GetAxis("Horizontal");
        _userJumped = Input.GetButton("Jump");
    }

    private void FixedUpdate()
    {
        _rigidbody.AddForce(_transform.forward * _translationInput * _translateScale, ForceMode.Force);

        _rigidbody.AddTorque(_transform.up * _rotationInput * _rotateScale, ForceMode.Force);

        Debug.DrawRay(_transform.position, _transform.forward * 10, Color.green);

        Debug.DrawRay(_transform.position, _rigidbody.linearVelocity, Color.green);

        if (_userJumped)
        {
            _rigidbody.AddForce(Vector3.up, ForceMode.VelocityChange);
            _userJumped = false;
        }
    }
}
