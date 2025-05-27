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
    
    public PlayerMove PlayerMove;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] string _horizontalAxis;
    [SerializeField] string _verticalAxis;

    [SerializeField] string _horizontalAxisOpposite;
    [SerializeField] string _verticalAxisOpposite;

    void Start()
    {
        
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
        PlayerMove.ProcessInputs(moveValue.x, moveValue.y);
    }
    
}