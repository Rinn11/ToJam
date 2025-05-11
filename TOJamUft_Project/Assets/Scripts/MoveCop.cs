using UnityEngine;
using UnityEngine.UI;

public class MoveCop : MonoBehaviour
{
    public PIDController RotationPID = new PIDController(1f, 0f, 0.1f, 0.1f);

    public MoveCar MoveCar;
    public GameObject Target;

    public Text DistText;
    public Text AngText;

    public Text VInputText;
    public Text HInputText;

    public Text VIntegralText;

    public Text SpeedText;

    private Rigidbody _rigidbody;
    private Rigidbody _targetRigidbody;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _targetRigidbody = Target.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        float dt = Time.fixedDeltaTime;

        Vector3 InitDiff = Target.transform.position - transform.position;
        float distDiff = InitDiff.magnitude;

        float speed = Mathf.Max(_rigidbody.linearVelocity.magnitude, 1);

        Vector3 TarPos = Target.transform.position;
        Vector3 TarVel = _targetRigidbody.linearVelocity;
        Vector3 Intercept = TarPos + TarVel * distDiff / speed;

        Vector3 Diff = Intercept - transform.position;
        Debug.DrawRay(transform.position, Diff, Color.red);

        float angDiff = Vector3.SignedAngle(transform.forward, Diff, Vector3.up);
        float rotation = Mathf.Clamp(RotationPID.Process(angDiff, dt), -1, 1);
        float translation = 1f; // No brakes...

        VIntegralText.text = "INT: " + RotationPID._integral;
        
        DistText.text = "Dist Error: " + Diff.magnitude;
        AngText.text = "Ang Error: " + angDiff;
        VInputText.text = "V: " + translation;
        HInputText.text = "H: " + rotation;
        MoveCar.ProcessInput(translation, rotation);
    }
}
