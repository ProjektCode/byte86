using UnityEngine;

[CreateAssetMenu(fileName = "Power Core Sync", menuName = "Byte86/Powerups/PowerCoreSyncPowerUp")]
public class PowerCoreSyncPowerUp : PowerupData {
    [Range(1f, 2f)][SerializeField] private float multiplier = 1.25f;

    public static bool Active { get; private set; }

    public static PowerCoreSyncPowerUp Instance { get; private set; }

    private void OnEnable() {
        Instance = this;
    }
    public override void Activate(GameObject player) {
        Active = true;
    }

    public override void Deactivate(GameObject player) {
        Active = false;
    }

    private void OnValidate() => multiplier = Mathf.Round(multiplier * 100f) / 100f;
    public static float GetMultiplier => Instance.multiplier;
}
