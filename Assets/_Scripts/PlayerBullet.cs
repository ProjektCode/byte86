using System.Collections;
using UnityEngine;

public class PlayerBullet : Projectile {

    private float orgDamage = 10;

    private Coroutine flashRoutine;

    private bool isBoosted = false;
    private bool isPiercing = false;
    private bool isVampiric = false;
    public bool isCritical = false;

    private PlayerStats playerStats;
    private Entity entity;

    protected override string TargetTag => "Enemy";

    protected override void Awake() {
        orgDamage = damage;

        base.Awake();
    }

    protected override void Start() {
        base.Start();

        if(DamageBoostPowerUp.Active) SetBoosted(true);
        if(VampiricRoundsPowerUp.Active) SetVampiric(true);
        if(PiercingBulletsPowerUp.Active) SetPiercing(true);
        if(CriticalSurgePowerUp.Active) SetCritical(true);
    }

    public void SetVampiric(bool vampiric) {
        isVampiric = vampiric;
    }

    public void SetCritical(bool crit) {
        isCritical = crit;
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

    // Override OnTriggerEnter2D from Projectile (copy base version but modify it slightly)
    protected override void OnTriggerEnter2D(Collider2D col) {

        if (col.CompareTag("Bullet")) {
            Destroy(col.gameObject);
            Destroy(gameObject);
            return;
        }

        if (!col.CompareTag(TargetTag)) return;

        entity = col.GetComponent<EnemyStats>();
        float finalDamage = damage;

        if(isBoosted){
             finalDamage *= DamageBoostPowerUp.GetMultiplier; //All other damage-based powerups go below this
        }

        if (isVampiric) {
            if (playerStats.GetCurrentHealth() == playerStats.MaxHealth) {
                finalDamage *= 1.5f;
            } else { playerStats.Heal(damage / 2); }
        }

        finalDamage = GetCriticalDamage(finalDamage);

        PlayerUpgradeData upgrades = UpgradeManager.LoadUpgrades();
        finalDamage *= upgrades.GetDamageMultiplier();

        if (entity != null) entity.TakeDamage(finalDamage);
        
        if (isPiercing) return;
        Destroy(gameObject);
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

        if (playerStats == null) {
            Debug.Log("[Player Bullet] Player Stats was not found");
            return;
        }
    }

    public float GetCriticalDamage(float baseDamage) {
        if (isCritical && Random.value <= CriticalSurgePowerUp.GetChance) {
            if (entity != null) entity.isCrit = true;
            float critDamage = baseDamage * CriticalSurgePowerUp.GetMultiplier;
            //Debug.Log("[Player Bullet] Critical Hit! Damage: " + critDamage);
            return critDamage;
        }
        return baseDamage;
    }
    
}
