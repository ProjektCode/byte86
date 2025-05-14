using UnityEngine;

[CreateAssetMenu(fileName = "Damage Boost", menuName = "Byte86/Powerups/Damage Boost")]
public class DamageBoostPowerUp : PowerupData {
    [Range(1f, 2f)][SerializeField] private float multiplier = 1.25f;

    public static bool Active { get; private set; }

    public static DamageBoostPowerUp Instance { get; private set;}

    private void OnEnable() {
        Instance = this;
    }

    public override void Activate(GameObject player) {
        if (Active) return;
        Active = true;
    }

    public override void Deactivate(GameObject player) {
        if(!Active) return;
        Active = false;
    }

    private void OnValidate() => multiplier = Mathf.Round(multiplier * 100f) / 100f;
    public static float GetMultiplier => Instance.multiplier;
}


