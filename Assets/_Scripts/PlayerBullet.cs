using System.Collections;
using UnityEngine;

public class PlayerBullet : Projectile {

    private float orgDamage = 10;
    private float explosiveRadius;

    private Coroutine flashRoutine;

    private bool isBoosted = false;
    private bool isPiercing = false;
    private bool isExplosive = false;
    private bool isVampiric = false;

    private LayerMask enemyLayer;

    private PlayerStats playerStats;

    protected override string TargetTag => "Enemy";

    protected override void Awake() {
        orgDamage = damage;
        base.Awake();
    }

    protected override void Start() {
        base.Start();
        if (DamageBoostPowerUp.BoostedActive) {
            SetBoosted(true);
        }
        if(VampiricRoundsPowerUp.VampiricRoundsActive){
            SetVampiric(true);
        }
    }

    public void SetVampiric(bool vampiric) {
        isVampiric = vampiric;
    }

    public void SetBoosted(bool boosted) {
        isBoosted = boosted;

        if (isBoosted && flashRoutine == null) {
            flashRoutine = StartCoroutine(FlashRed());
        }
    }

    public void SetPiercing(bool piercing) {
        isPiercing = piercing;
    }

    public void SetExplosive(bool value, float radius = 0f, LayerMask layer = default){
        isExplosive = value;
        explosiveRadius = radius;
        enemyLayer = layer;
    }

    // Override OnTriggerEnter2D from Projectile (copy base version but modify it slightly)
    protected override void OnTriggerEnter2D(Collider2D col) {
        if (col.CompareTag("Bullet")) {
            Destroy(col.gameObject);
            Destroy(gameObject);
            return;
        }

        if (!col.CompareTag(TargetTag)) return;

        Entity entity = col.GetComponent<Entity>();
        if (entity != null) entity.TakeDamage(damage);

        if(isVampiric) playerStats.Heal(damage / 2);

        if (isExplosive) Explode();

        if(isPiercing) return;

        Destroy(gameObject);
    }

    private void Explode() {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosiveRadius, enemyLayer);
        foreach (Collider2D hit in hits) {
            if (hit.TryGetComponent(out Entity e)) {

                if(isVampiric) playerStats.Heal(damage / 2);

                e.TakeDamage(damage);
            }
        }

        // Add explosion VFX or SFX here
        FeedBackManager.Instance.CameraShake(0.5f,0.1f, 50);
    }

    private IEnumerator FlashRed() {
        float duration = 0.2f;
        Color flashColor = Color.red;

        while (isBoosted) {
            float t = 0f;
            while (t < 1f) {
                t += Time.deltaTime / duration;
                sr.color = Color.Lerp(orgColor, flashColor, t);
                yield return null;
            }

            t = 0f;
            while (t < 1f) {
                t += Time.deltaTime / duration;
                sr.color = Color.Lerp(flashColor, orgColor, t);
                yield return null;
            }
        }

        sr.color = orgColor;
    }

    public void GetStats(PlayerStats stats) {
        playerStats = stats;

        if(playerStats == null){
            Debug.Log("Player Stats was not found");
        }
    }

    public float ReturnOrgDamage() => orgDamage;

    private void OnDrawGizmosSelected() {
        if (isExplosive) {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, explosiveRadius);
        }
    }

    
}
