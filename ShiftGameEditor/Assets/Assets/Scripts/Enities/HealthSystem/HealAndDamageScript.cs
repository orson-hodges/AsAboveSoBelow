using UnityEngine;

public class HealAndDamageScript : MonoBehaviour
{
    public int HealthPointChange;

    void OnTriggerEnter2D(Collider2D collider)
    {
        HealthSystemScript HealthScript = collider.gameObject.GetComponent<HealthSystemScript>();

        if (HealthScript == null)
        {
            return;
        }

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
