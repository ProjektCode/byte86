using System.Collections;
using MoreMountains.Feedbacks;
using UnityEngine;

public class PlayerStats : Entity {

    [Space]
    public MMF_Player camShaker;

    private PlayerController playerController;
    private bool isEvasionActive = false;
    private float evasionChance = 0f;

    private Material playerMat;
    private Color orgColor;

    private Coroutine HealCoroutine;
    private Coroutine flashCoroutine;

    private float healAmountPerSecond;
    private float healTimer;
    private float healTickDelay;

    public bool isHealing{ get; set; }

    protected override void Awake() {
        currentHealth = MaxHealth;
        healthBar.MaxValue = Mathf.RoundToInt(currentHealth);

        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        playerController = GetComponent<PlayerController>();
        audioSource  = GetComponent<AudioSource>();
        sr = GetComponent<SpriteRenderer>();
        playerMat = sr.material;
        orgColor = playerMat.color;

        if(sr != null) orgMat = sr.material;
    }

    new public void OnDamageTaken() {
       camShaker.PlayFeedbacks();
        base.OnDamageTaken();
        // Add enemy-specific reactions like flashing red or playing a sound
    }

    public override void Die() {
        playerController.isDead = true;
        animator.SetBool("isDead", true);
        //GameManager.Instance.GameOver();
        base.Die();
    }

    public override void TakeDamage(float amount) {

        if(isEvasionActive && Random.value < evasionChance) {
            //Add some visual effects for this
            StartColorFlash(new Color(0.6f, 0f, 1f, 1f), 0.1f, false);

            return;
        }

        ShieldHandler shield = GetComponent<ShieldHandler>();
        if (shield != null && shield.IsShieldActive()) {
            shield.ConsumeHit();

            return;
        }

        currentHealth -= amount;
        healthBar.Change(-amount);
        OnDamageTaken();

        if (currentHealth <= 0f) {
            Die();
            healthBar.Change(0);
        }
    }

    public override void OnAnimationComplete() {
        GameManager.Instance.GameOver();
        base.OnAnimationComplete();
    }

    // Call this from the powerup
    public void SetEvasionActive(bool active, float chance) {
        isEvasionActive = active;
        evasionChance = chance;
    }

    public void StartHealing(float healAmount, float duration, float TickDelay) {
        healAmountPerSecond = healAmount;
        healTimer = duration;
        healTickDelay = TickDelay;
        HealCoroutine = StartCoroutine(HealOverTime());
        isHealing = true;
        FlashHealingEffect(true);
    }

    public void StopHealing() {
        if (HealCoroutine != null) {
            StopCoroutine(HealCoroutine);
            HealCoroutine = null;
        }

        isHealing = false;

        if (flashCoroutine != null) {
            StopCoroutine(flashCoroutine);
            flashCoroutine = null;
        }

        playerMat.color = orgColor; // ensure visual reset
    }


    private void FlashHealingEffect(bool startFlashing){
        if(startFlashing){
            StartColorFlash(Color.green, healTickDelay / 2f, true);
        } else {playerMat.color = orgColor;}
    }

    private void StartColorFlash(Color flashColor, float duration, bool loop = false)
    {
        // Stop any previous flash to avoid overlapping
        if (flashCoroutine != null)
            StopCoroutine(flashCoroutine);

        flashCoroutine = StartCoroutine(ColorFlashCoroutine(flashColor, duration, loop));
    }

    private IEnumerator ColorFlashCoroutine(Color flashColor, float duration, bool loop)
    {
        do
        {
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime / duration;
                playerMat.color = Color.Lerp(orgColor, flashColor, t);
                yield return null;
            }

            t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime / duration;
                playerMat.color = Color.Lerp(flashColor, orgColor, t);
                yield return null;
            }

        } while (loop);

        playerMat.color = orgColor;
    }


    private IEnumerator HealOverTime() {
        float elapsedTime = 0f;

        while (elapsedTime < healTimer) {
            if (GetCurrentHealth() < MaxHealth) {
                float healAmount = Mathf.Min(healAmountPerSecond, MaxHealth - GetCurrentHealth());
                Heal(healAmount);
            }
            elapsedTime += Time.deltaTime;
            yield return new WaitForSeconds(healTickDelay);
        }

        StopHealing(); // Stop healing after duration
    }


}
