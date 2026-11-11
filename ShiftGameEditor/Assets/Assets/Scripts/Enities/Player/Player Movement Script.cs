using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementScript : MonoBehaviour
{
    public float MovementSpeed;
    private Rigidbody2D PlayerBody;
    protected Vector2 InputDirection;

    public float RangedAttackFireRate;
    private float RangedAttackCooldownTimer;

    public GameObject PlayerProjectilePrefab;

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

        RangedAttackCooldownTimer = RangedAttackFireRate;
    }

    void FixedUpdate()
    {
        PlayerBody.linearVelocity = InputDirection * MovementSpeed;
    }

    void Update()
    {
        RangedAttackCooldownTimer += Time.deltaTime;

        if (RangedAttackAction.IsPressed())
        {
            if (RangedAttackCooldownTimer > RangedAttackFireRate)
            {
                Instantiate(PlayerProjectilePrefab, transform.position, transform.rotation);
                RangedAttackCooldownTimer = 0;
            }
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
