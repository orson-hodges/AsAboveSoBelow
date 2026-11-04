using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementScript : MonoBehaviour
{
    public float Speed;
    private Vector2 InputValue;
    private GameObject Player;
    private Vector3 PlayerPosition;

    void Awake()
    {
        Player = gameObject;
        Debug.Log(Player);
    }

    void Update()
    {
        PlayerPosition = InputValue; 
        Player.transform.position += PlayerPosition * Speed * Time.deltaTime;
    }

    void OnMove(InputValue value)
    {
        InputValue = value.Get<Vector2>();
        Debug.Log("Moved. Input: " + InputValue); 
    }
}
