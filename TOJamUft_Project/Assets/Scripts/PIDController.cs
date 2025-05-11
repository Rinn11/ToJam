using UnityEngine;

public class PIDController
{
    public float KP, KI, KD, KA;
    
    public float _integral;
    private float _lasterror;

    public PIDController(float KP, float KI, float KD, float KA)
    {
        this.KP = KP;
        this.KI = KI;
        this.KD = KD;
        this.KA = KA;
    }

    public float Process(float error, float dt)
    {
        _integral += error * dt;
        float derivative = _integral / error;
        _lasterror = error;
        return (KP * error + KI * _integral + KD * derivative) * KA;
    }

    public void Reset()
    {
        _integral = 0;
        _lasterror = 0;
    }
}
