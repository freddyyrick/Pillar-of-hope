using System.Collections;
using UnityEngine;

public class BossHealthPickUp : MonoBehaviour
{
    public int healthRestoreAmount = 25;
    public float respawnTime = 5f; // time in seconds before it reappears

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Damageable damageable = collision.gameObject.GetComponent<Damageable>();
        if (damageable != null && damageable.IsAlive)
        {
            if (damageable.Health < damageable.MaxHealth)
            {
                int missingHealth = damageable.MaxHealth - damageable.Health;
                int actualHeal = Mathf.Min(healthRestoreAmount, missingHealth);

                damageable.Heal(actualHeal);  

                // Disable pickup instead of destroying
                gameObject.SetActive(false);

                // Start respawn coroutine
                StartCoroutine(RespawnPickup());
            }
            else
            {
                Debug.Log("Health already full — potion not picked up.");
            }
        }
    }

    private IEnumerator RespawnPickup()
    {
        yield return new WaitForSeconds(respawnTime);
        gameObject.SetActive(true); // Reactivate pickup
    }
}
