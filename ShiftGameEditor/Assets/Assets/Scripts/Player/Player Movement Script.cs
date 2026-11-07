using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementScript : MonoBehaviour
{
    public float MovementSpeed;
    private Rigidbody2D PlayerBody;
    protected Vector2 InputValue;


    void Awake()
    {
        PlayerBody = gameObject.GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        PlayerBody.linearVelocity = InputValue * MovementSpeed;
    }

    void OnMove(InputValue value)
    {
        InputValue = value.Get<Vector2>();
    }
}
