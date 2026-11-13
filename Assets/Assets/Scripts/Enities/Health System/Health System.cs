using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    // The current and maximun amount of health points this object has.
    public int CurrentHealthPoints;
    public int MaximumHealthPoints;

    // The length of time the object is invincible to damage after getting hit.
    public float InvincibilityDuration;
    private float InvincibilityTimer;

    void Start()
    {
        // Setting InvincibilityTimer to InvincibilityDuration to prevent the object from being briefly invincible at the start of a scene.
        InvincibilityTimer = InvincibilityDuration;
    }

    void FixedUpdate()
    {
        // Ticks the InvincibilityTimer. Used to determine if the object is currently invincible.
        InvincibilityTimer += Time.fixedDeltaTime;
    }

    // Heal the object by healvalue. Cannot Exceed MaximunHealthPoints
    public void Heal(int healvalue)
    {
        CurrentHealthPoints += healvalue;

        if (CurrentHealthPoints > MaximumHealthPoints)
        {
            CurrentHealthPoints = MaximumHealthPoints;
        }

        Debug.Log("+" + healvalue + " HP. Current HP = " + CurrentHealthPoints);
    }

    // Damages the object by damagevalue if they aren't currently invincible. If the health points of the object reaches 0 or below, the object's death function is called.
    public void Hurt(int damagevalue, Vector2 damagesourceposition, bool continuouscontact)
    {
        // Checks if the object is invincible before dealing damage. If they are, then no damage is applied.
        if (InvincibilityTimer > InvincibilityDuration)
        {
            CurrentHealthPoints += damagevalue;
            Debug.Log(damagevalue + " HP. Current HP = " + CurrentHealthPoints);

            // Sets the object to be invincibility for a period of time determined by InvincibilityDuration.
            InvincibilityTimer = 0;

            // Checks if the object is at 0 or below health points and, if so, if it is the player or a enemy, as to know which death function to call. If the object isn't the player or a enemy, no function is called.
            if (CurrentHealthPoints <= 0)
            {
                if (gameObject.tag == "Player")
                {
                    PlayerInputSystem PlayerScript = gameObject.GetComponent<PlayerInputSystem>();
                    if (PlayerScript.Alive == true)
                    {
                        Debug.Log("Player is Dead");
                        gameObject.GetComponent<Collider2D>().excludeLayers = LayerMask.GetMask("Player", "Ignore Raycast", "Default");

                        if (continuouscontact == false)
                        {
                            PlayerScript.DeathTargetVector = damagesourceposition;
                            PlayerScript.DeathTargetVector = PlayerScript.DeathTargetVector - new Vector2(transform.position.x, transform.position.y);
                        }
                        else
                        {
                            PlayerScript.DeathTargetVector = Vector2.zero;
                        }

                        PlayerScript.Dying();
                    }
                }
                else if (gameObject.tag == "Enemy")
                {
                    Debug.Log("Enemy is Dead");
                    GuardBaseBehaviourSystem GuardBaseBehaviourSystemScript = gameObject.GetComponent<GuardBaseBehaviourSystem>();
                    GuardBaseBehaviourSystemScript.DeathTargetPosition = damagesourceposition;
                    GuardBaseBehaviourSystemScript.CurrentEnemyState = GuardBaseBehaviourSystem.EnemyState.Dying;
                }
                else
                {
                    Debug.Log("How");
                }
            }
        }
    }
}
