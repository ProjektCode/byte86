using UnityEngine;

[System.Serializable]
public class PlayerUpgradeData {

    public int HealthLevel = 0;
    public int DamageLevel = 0;
    public int MaxLevel = 10;

    public int GetHealthBonus() => HealthLevel * 10;
    public float GetDamageMultiplier() => 1f + DamageLevel * 0.25f;

}
