using UnityEngine;

public class EnemyStats : Entity {
    // Now has access to health, speed, TakeDamage, Die, etc.

    [Header("Enemy Stats")]
    public int MinHealth = 3;

    public float MinSpeed = 1.5f;

    public Bar healthBar;

    protected override void Awake() {
        // Optional: Randomize stats
        Health = Random.Range(MinHealth, Health);
        moveSpeed = Random.Range(MinSpeed, moveSpeed);
        currentHealth = Health;
        healthBar.MaxValue = currentHealth;

        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        Debug.Log(gameObject.name + ": " + currentHealth + " | " + moveSpeed);
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q)) TakeDamage(1);
    }

    protected override void OnDamageTaken() {
        base.OnDamageTaken();
        // Add enemy-specific reactions like flashing red or playing a sound
    }

    public override void Die() {

        base.Die();
        
    }

    public override void TakeDamage(int amount) {
        currentHealth -= amount;
        healthBar.Change(-amount);
        OnDamageTaken();

        if (currentHealth <= 0f) {
            Die();
            healthBar.Change(0);
        }
    }
}
