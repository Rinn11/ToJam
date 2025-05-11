using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
{
    public PlayerMove PlayerMove; 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 moveValue = InputSystem.actions.FindAction("Move").ReadValue<Vector2>();
        PlayerMove.ProcessInputs(moveValue.x, moveValue.y);
    }
}
