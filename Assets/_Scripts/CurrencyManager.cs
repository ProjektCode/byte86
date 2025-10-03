using UnityEngine;

public class CurrencyManager : MonoBehaviour {
    public static CurrencyManager Instance { get; private set; }

    public int Currency { get; private set; }

    void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void AddCurrency(int amount) {
        Currency += amount;
        GameManager.Instance.UpdateCurrencyUI(Currency); // optional
    }

    public bool SpendCurrency(int amount) {
        if (Currency >= amount) {
            Currency -= amount;
            GameManager.Instance.UpdateCurrencyUI(Currency); // optional
            return true;
        }
        return false;
    }

    public int GetCurrency() {
        return Currency;
    }  
}
