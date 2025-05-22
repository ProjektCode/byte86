using UnityEngine;

public abstract class PowerupData : ScriptableObject {
    public string PowerUpName;
    public string PowerUpDescription;
    public Sprite icon;

    [Range(5f, 30f)][SerializeField] protected float duration = 10f;

    public enum PowerupCategory {
        Offensive,
        Support
    }
    public PowerupCategory PowerupType = PowerupCategory.Offensive;

    public abstract void Activate(GameObject player);
    public abstract void Deactivate(GameObject player);

    private void OnValidate() => duration = Mathf.Round(duration * 100f) / 100f;
    
    public virtual float GetDuration() => duration;
}