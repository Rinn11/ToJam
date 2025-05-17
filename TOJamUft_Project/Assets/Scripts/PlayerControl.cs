using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
{
    public PlayerMove PlayerMove;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] string _horizontalAxis;
    [SerializeField] string _verticalAxis;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 moveValue = new Vector2(Input.GetAxis(_horizontalAxis), Input.GetAxis(_verticalAxis));
        PlayerMove.ProcessInputs(moveValue.x, moveValue.y);
    }
}