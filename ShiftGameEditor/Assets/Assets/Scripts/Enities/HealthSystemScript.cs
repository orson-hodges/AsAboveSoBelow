using Unity.VisualScripting;
using UnityEngine;

public class HealthSystemScript : MonoBehaviour
{
    public int CurrentHealthPoints;
    public int MaximumHealthPoints;

    public void Heal(int healvalue)
    {
        CurrentHealthPoints += healvalue;

        if (CurrentHealthPoints > MaximumHealthPoints)
        {
            CurrentHealthPoints = MaximumHealthPoints;
        }

        Debug.Log("+" + healvalue + " HP. Current HP = " + CurrentHealthPoints);
    }

    public void Hurt(int damagevalue)
    {
        CurrentHealthPoints += damagevalue;
        Debug.Log(damagevalue + " HP. Current HP = " + CurrentHealthPoints);

        if (CurrentHealthPoints <= 0)
        {
            if (gameObject.tag == "Player")
            {
                Debug.Log("Player is Dead");
                PlayerMovementScript PlayerScript = gameObject.GetComponent<PlayerMovementScript>();
                PlayerScript.Die();
            }
            else if (gameObject.tag == "Enemy")
            {
                Debug.Log("Enemy is Dead");
                NormalEnemyBehaviourScript NormalEnemyScript = gameObject.GetComponent<NormalEnemyBehaviourScript>();
                NormalEnemyScript.CurrentState = NormalEnemyBehaviourScript.State.Dying;
            }
            else
            {
                Debug.Log("How");
            }
        }
    }
}
