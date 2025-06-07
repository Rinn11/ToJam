/*
 * Passes input from the controller to the PlayerMove script
 */

// TODO: Originally was used because Cop AI used the same movement model (PlayerMove) that the player used.
// Since the cop is now another player, this may not be needed anymore.

using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
{
    public PlayerSwapEventSender swapSender;

    public MonoBehaviour movementScript;    // So we can drag anything in
    private IMovementModel movementModel;   // But we still need a class to run methods

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] string _horizontalAxis;
    [SerializeField] string _verticalAxis;

    [SerializeField] string _horizontalAxisOpposite;
    [SerializeField] string _verticalAxisOpposite;

    private bool locked = false;

    void Start()
    {
        movementModel = movementScript as IMovementModel;
    }

    private void OnEnable()
    {
        if (swapSender != null)
            swapSender.OnBoolEvent += recievePlayerSwap;
    }

    private void OnDisable()
    {
        if (swapSender != null)
            swapSender.OnBoolEvent -= recievePlayerSwap;
    }

    public void SetLocked(bool newLocked)
    {
        locked = newLocked;
    }


    public void recievePlayerSwap(bool isPlayer1Driving)
    {
        // Swap the horizontal and vertical axes
        string tempHorizontal = _horizontalAxis;
        string tempVertical = _verticalAxis;

        _horizontalAxis = _horizontalAxisOpposite;
        _verticalAxis = _verticalAxisOpposite;

        _horizontalAxisOpposite = tempHorizontal;
        _verticalAxisOpposite = tempVertical;
        Debug.Log($"Swapped controls: {_horizontalAxis}, {_verticalAxis} <-> {_horizontalAxisOpposite}, {_verticalAxisOpposite}");
    }

    

    // Update is called once per frame
    void Update()
    {
        Vector2 moveValue = new Vector2(Input.GetAxis(_horizontalAxis), Input.GetAxis(_verticalAxis));
        if (locked)
        {
            moveValue = Vector2.zero;
        }
        movementModel.ProcessInputs(moveValue.x, moveValue.y);
    }
    
}