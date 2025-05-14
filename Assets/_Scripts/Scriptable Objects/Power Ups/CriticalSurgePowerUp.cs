using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Critical Surge", menuName = "Byte86/Powerups/CriticalSurgePowerUp")]
public class CriticalSurgePowerUp : PowerupData {

    [Range(1.5f, 2f)][SerializeField] private float multiplier = 1.5f;
    [Range(0.25f, 1f)][SerializeField] private float chance = 0.45f;

    public static CriticalSurgePowerUp Instance { get; private set;}

    private void OnEnable() {
        Instance = this;
    }

    public static bool Active { get; private set; }
    public override void Activate(GameObject player) {
        if (Active) return;
        Active = true;
    }

    public override void Deactivate(GameObject player) {
        Active = false;
    }

    public static float GetMultiplier => Instance.multiplier;
    public static float GetChance => Instance.chance;

}
