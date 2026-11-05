using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementScript : MonoBehaviour
{
    public float PlayerSpeed;
    private Vector2 InputValue;
    private Rigidbody2D PlayerBody;

    void Awake()
    {
        PlayerBody = gameObject.GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        PlayerBody.linearVelocity = InputValue * PlayerSpeed;
    }

    void OnMove(InputValue value)
    {
        InputValue = value.Get<Vector2>();
    }
}
