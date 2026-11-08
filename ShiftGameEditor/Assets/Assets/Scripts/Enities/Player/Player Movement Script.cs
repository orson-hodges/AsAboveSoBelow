using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementScript : MonoBehaviour
{
    public float MovementSpeed;
    private Rigidbody2D PlayerBody;
    protected Vector2 InputDirection;


    void Awake()
    {
        PlayerBody = gameObject.GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        PlayerBody.linearVelocity = InputDirection * MovementSpeed;
    }

    public void Die()
    {
        Debug.Log("omg you are so dead rn :O");
    }

    void OnMove(InputValue inputdirectionvalue)
    {
        InputDirection = inputdirectionvalue.Get<Vector2>();
    }
}
