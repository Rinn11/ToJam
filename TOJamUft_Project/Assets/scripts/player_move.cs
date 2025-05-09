using UnityEngine;

public class player_move : MonoBehaviour
{
    private AlcoholManager _alcoholManager;
    private float _playerInput;
    private float _rotationInput;
    private Vector3 _userRot;

    private const float _inputScale = 0.5f;
    
    private const float _maxVelocity = 10f;

    private Transform _transform;
    private Rigidbody _rigidbody;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _transform = GetComponent<Transform>();

        GameObject alcoholGameObject = GameObject.Find("Alcohol");
        if (alcoholGameObject != null)
        {
            _alcoholManager = alcoholGameObject.GetComponent<AlcoholManager>();
        }
        else
        {
            Debug.LogError("GameObject 'Alcohol' not found!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        _playerInput = Input.GetAxis("Vertical");
        _rotationInput = Input.GetAxis("Horizontal");
        // _userJumped = Input.GetButton("Jump");
    }

    private void FixedUpdate()
    {
        _userRot = _transform.rotation.eulerAngles;
        _userRot += new Vector3(0, _rotationInput, 0);
        _transform.rotation = Quaternion.Euler(_userRot);
        _rigidbody.linearVelocity = Vector3.ClampMagnitude(
            _rigidbody.linearVelocity + _transform.forward *_playerInput * _inputScale,
            _maxVelocity * _alcoholManager.GetAlcoholMultiplier());
    }
}

