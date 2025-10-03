using UnityEngine;

public static class UpgradeManager {
    public static PlayerUpgradeData LoadUpgrades() {
        return new PlayerUpgradeData {
            HealthLevel = PlayerPrefs.GetInt("HealthLevel", 0),
            DamageLevel = PlayerPrefs.GetInt("DamageLevel", 0)
        };
    }

    public static void SaveUpgrades(PlayerUpgradeData data) {
        PlayerPrefs.SetInt("HealthLevel", data.HealthLevel);
        PlayerPrefs.SetInt("DamageLevel", data.DamageLevel);
        PlayerPrefs.Save();
    }
}
