using UnityEngine;

public class Attack : MonoBehaviour
{
    public int attackDamage = 10;
    public Vector2 knockback = Vector2.zero;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Damageable dmg = collision.GetComponent<Damageable>();

        if (dmg != null)
        {
            // dmg.Hit() handles invincibility, damage, and knockback
            dmg.Hit(attackDamage, knockback);
        }
    }
}