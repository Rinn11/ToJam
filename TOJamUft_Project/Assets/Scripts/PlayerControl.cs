/*
 * Passes input from the controller to the PlayerMove script
 */

// TODO: Originally was used because Cop AI used the same movement model (PlayerMove) that the player used.
// Since the cop is now another player, this may not be needed anymore.

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