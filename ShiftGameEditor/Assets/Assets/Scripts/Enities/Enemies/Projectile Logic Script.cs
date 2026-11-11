using UnityEngine;

public class ProjectileLogicScript : MonoBehaviour
{
    // The speed at which the projectile moves.
    public float ProjectileSpeed;

    void FixedUpdate()
    {
        // Moves the projectile forwards at a speed determined by ProjectileSpeed.
        transform.position -= transform.up * ProjectileSpeed * Time.fixedDeltaTime;
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        // Destroys the projectile on collision with the player or the environment.
        Destroy(gameObject);
    }
}
