using UnityEngine;
using UnityEngine.UI;

public class MoveCop : MonoBehaviour
{
    public GameObject Target;

    private const float _translateScale = 30f;
    private const float _rotateScale = 10f;

    private Transform _transform;
    private Rigidbody _rigidbody;

    public Text distanceText;
    public Text angleText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _transform = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void FixedUpdate()
    {
        Vector3 Diff = Target.transform.position - this.transform.position;

        distanceText.text = "Dist Error: " + Mathf.Round(Diff.magnitude * 100) / 100;
        angleText.text = "Ang Error: " + Mathf.Round(Diff.magnitude * 100) / 100;

        Vector3 _direction = Diff.normalized;
        Quaternion _angle = Quaternion.LookRotation(_direction);

        Debug.DrawRay(_transform.position, _transform.forward * 10, Color.blue);

        Debug.DrawRay(_transform.position, _rigidbody.linearVelocity, Color.blue);

        Debug.DrawRay(this.transform.position, Diff, Color.orange);

        //_rigidbody.AddForce(Diff * 5, ForceMode.Force);
        //_rigidbody.AddForce(_transform.forward * _translationInput * _translateScale, ForceMode.Force);

        //_rigidbody.AddForce(_transform.forward * 100);
        //_rigidbody.AddTorque(_transform.up * -_angle.eulerAngles.y * 10, ForceMode.Force);
    }
}
