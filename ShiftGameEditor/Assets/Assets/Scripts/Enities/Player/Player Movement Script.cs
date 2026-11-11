using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementScript : MonoBehaviour
{
    public float MovementSpeed;
    private Rigidbody2D PlayerBody;
    protected Vector2 InputDirection;

    public InputActionAsset InputMap;
    private InputAction RangedAttackAction;
    private InputAction ShiftAction;
    private InputAction LookIntoShiftAction;

    void Start()
    {
        PlayerBody = gameObject.GetComponent<Rigidbody2D>();

        RangedAttackAction = InputMap.FindAction("Ranged Attack");
        ShiftAction = InputMap.FindAction("Shift");
        LookIntoShiftAction = InputMap.FindAction("Look Into Shift");
    }

    void FixedUpdate()
    {
        PlayerBody.linearVelocity = InputDirection * MovementSpeed;
    }

    void Update()
    {
        if (RangedAttackAction.IsPressed())
        {
            Debug.Log("Ranged Attack");
        }

        if (ShiftAction.IsPressed())
        {
            Debug.Log("Shift");
        }

        if (LookIntoShiftAction.IsPressed())
        {
            Debug.Log("Look Into Shift");
        }
    }

    public void Die()
    {
        Debug.Log("omg you are so dead rn :O");
    }

    void OnMove(InputValue inputvalue)
    {
        // Keep track of the player's input direction.
        InputDirection = inputvalue.Get<Vector2>();
    }

    void OnMeleeAttack()
    {
        
    }
}
