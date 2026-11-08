using UnityEngine;

public class HealAndDamageScript : MonoBehaviour
{
    public int HealthPointChange;

    void OnTriggerEnter2D(Collider2D collision)
    {
        HealthSystemScript HealthScript = collision.GetComponent<HealthSystemScript>();

        if (HealthPointChange > 0)
        {
            HealthScript.Heal(HealthPointChange);
        }
        else if (HealthPointChange < 0)
        {
            HealthScript.Hurt(HealthPointChange);
        }
        else if (HealthPointChange == 0)
        {
            Debug.Log("¯|_(ツ)_|¯");
        }

        Destroy(gameObject);
    }
}
