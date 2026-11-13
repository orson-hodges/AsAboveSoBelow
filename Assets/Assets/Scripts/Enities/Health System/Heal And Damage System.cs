using System.Reflection.PortableExecutable;
using UnityEngine;

public class HealAndDamageSystem : MonoBehaviour
{
    // The change in health points caused by this object.
    public int HealthPointChange;
    
    // If the object can continuous change health points of objects it touches. If not, then the change in health points is a one time event.
    public bool ContinuousContact;

    void OnTriggerStay2D(Collider2D collider)
    {
        // Get a reference to the Health System Script of the hit object
        HealthSystem HealthSystemScript = collider.gameObject.GetComponent<HealthSystem>();

        // If the hit object doesn't have the Health System Script, the function ends to avoid errors.
        if (HealthSystemScript == null)
        {
            return;
        }

        // Checks if it should call the heal or hurt function of the referenced Health System Script, depending on if HealthPointChange is positive or not. HealthPointChange being 0 results in no function being called.
        if (HealthPointChange > 0)
        {
            HealthSystemScript.Heal(HealthPointChange);
        }
        else if (HealthPointChange < 0)
        {
            HealthSystemScript.Hurt(HealthPointChange, transform.position, ContinuousContact);
        }
        else if (HealthPointChange == 0)
        {
            Debug.Log("¯|_(ツ)_|¯");
        }

        // Checks if CotinousContact is true. If not, then the object destroys itself.
        if (ContinuousContact == false)
        {
            Destroy(gameObject);
        }
    }
}
