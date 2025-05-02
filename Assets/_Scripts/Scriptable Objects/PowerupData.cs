using UnityEngine;

[CreateAssetMenu(fileName = "PowerupData", menuName = "Byte86/PowerupData")]
public abstract class PowerupData : ScriptableObject {
    public string powerupName;
    public Sprite icon;

    [Range(10f, 30f)]
    public float duration = 10f;

    public abstract void Activate(GameObject player);
    public abstract void Deactivate(GameObject player);

    private void OnValidate() => duration = Mathf.Round(duration * 100f) / 100f;
}
