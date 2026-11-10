using UnityEngine;

public class ProjectileLogicScript : MonoBehaviour
{
    public float ProjeciteSpeed;

    void FixedUpdate()
    {
        transform.position -= transform.up * ProjeciteSpeed;
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        Destroy(gameObject);
    }
}
