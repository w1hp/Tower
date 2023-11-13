using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("References")]
    public Transform trans;
    public Transform projectileSeekPoint;

    [Header("Stats")]
    public float maxHealth;
    public float healthGainPerLevel;
    [HideInInspector] public float health;
    [HideInInspector] public bool alive = true;

    public void TakeDamage(float amount)
    {
        if (amount > 0)
        {
            health = Mathf.Max(health - amount, 0);
            if (health == 0)
                Die();
        }
    }
    public void Die()
    {
        if (alive)
        {
            alive = false;
            Destroy(gameObject);
        }
    }
    public void Leak()
    {
        GameManager.remainingLives -= 1;
        Destroy(gameObject);
    }
    protected virtual void Start()
    {
        maxHealth = maxHealth + (healthGainPerLevel * (GameManager.level - 1));
        health = maxHealth;
    }
}
