using System;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementScript : MonoBehaviour
{
    public float MovementSpeed;
    [NonSerialized] public Rigidbody2D PlayerBody;
    protected Vector2 InputDirection;

    public float RangedAttackFireRate;
    private float RangedAttackCooldownTimer;

    public GameObject PlayerProjectilePrefab;
    [NonSerialized] public GameObject PlayerCamera;

    [NonSerialized] public bool Alive;
    [NonSerialized] public Vector2 DeathTargetVector;

    public GameObject DeathScene;

    public InputActionAsset InputMap;
    private InputAction RangedAttackAction;
    private InputAction ShiftAction;
    private InputAction LookIntoShiftAction;

    void Start()
    {
        PlayerBody = gameObject.GetComponent<Rigidbody2D>();
        PlayerCamera = GameObject.FindWithTag("MainCamera");
        DeathScene.SetActive(false);

        RangedAttackAction = InputMap.FindAction("Ranged Attack");
        ShiftAction = InputMap.FindAction("Shift");
        LookIntoShiftAction = InputMap.FindAction("Look Into Shift");

        RangedAttackCooldownTimer = RangedAttackFireRate;

        Alive = true;
    }

    void FixedUpdate()
    {
        if (Alive == true)
        {
            PlayerBody.linearVelocity = InputDirection * MovementSpeed;
        }
        else if (Alive == false)
        {
            PlayerBody.linearVelocity = -DeathTargetVector * 3;
        }
    }

    void Update()
    {
        if (Alive == true)
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

        if (Alive == false)
        {
            Time.timeScale = Mathf.Clamp(Time.timeScale - Time.unscaledDeltaTime/2, 0, 1);
        }
    }

    public void Dying()
    {
        DeathScene.SetActive(true);
    }

    void OnMove(InputValue inputvalue)
    {
        if (Alive == true)
        {
            // Keep track of the player's input direction.
            InputDirection = inputvalue.Get<Vector2>();
        }
    }

    void OnMeleeAttack()
    {
        
    }
}
