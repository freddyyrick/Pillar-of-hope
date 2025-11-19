using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickUp : MonoBehaviour
{
    public int healthRestoreAmount = 25;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Only the PLAYER can pick this up
        PlayerController player = collision.GetComponent<PlayerController>();
        if (player == null)
            return; // enemy or anything else touched it — ignore

        Damageable damageable = collision.GetComponent<Damageable>();
        if (damageable != null && damageable.IsAlive)
        {
            if (damageable.Health < damageable.MaxHealth)
            {
                int missingHealth = damageable.MaxHealth - damageable.Health;
                int actualHeal = Mathf.Min(healthRestoreAmount, missingHealth);

                damageable.Heal(actualHeal);
                Destroy(gameObject);
            }
            else
            {
                Debug.Log("Health already full — cannot pick up.");
            }
        }
    }
}
