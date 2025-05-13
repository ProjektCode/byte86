using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DevConsole : MonoBehaviour {
    [Header("UI References")]
    public GameObject consoleUI;
    public TMP_InputField inputField;
    public TextMeshProUGUI suggestionText;

    private bool isOpen = false;
    private Dictionary<string, System.Action> commandMap;
    private List<string> allCommands = new List<string>();
    private int suggestionIndex = 0;

    void Start() {
        BuildCommandMap();
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.BackQuote)) {
            ToggleConsole();
        }

        if (!isOpen) return;

        if (Input.GetKeyDown(KeyCode.Return)) {
            SubmitCommand(inputField.text);
            inputField.text = "";
            suggestionText.text = "";
            suggestionIndex = 0;
            inputField.ActivateInputField();
        }

        if (Input.GetKeyDown(KeyCode.Tab)) {
            AutoFillCommand();
        }

        UpdateSuggestionText(inputField.text);
    }

    void ToggleConsole() {
        isOpen = !isOpen;
        consoleUI.SetActive(isOpen);

        if (isOpen) {
            inputField.text = "";
            suggestionText.text = "";
            inputField.ActivateInputField();
            Time.timeScale = 0f;
        } else {
            Time.timeScale = 1f;
        }
    }

    void SubmitCommand(string command) {
        if (commandMap.TryGetValue(command.ToLower(), out var action)) {
            action?.Invoke();
            Debug.Log($"[DevConsole] Executed command: {command}");
        } else {
            Debug.LogWarning($"[DevConsole] Unknown command: {command}");
        }
    }

    void BuildCommandMap() {
        commandMap = new Dictionary<string, System.Action>();
        allCommands = new List<string>();

        var powerups = Resources.LoadAll<PowerupData>("Powerups");
        if (powerups == null || powerups.Length == 0) {
            Debug.LogWarning("[DevConsole] No Powerups found in Resources/Powerups.");
            return;
        }

        foreach (var powerup in powerups) {
            if (string.IsNullOrWhiteSpace(powerup.PowerUpName)) continue;

            string cmd = $"powerup.{powerup.PowerUpName.ToLower()}";
            commandMap[cmd] = () => {
                var player = GameObject.FindWithTag("Player");
                if (player != null) {
                    var handler = player.GetComponent<PlayerPowerupHandler>();
                    if (handler != null) {
                        handler.ApplyPowerup(powerup);
                        Debug.Log($"[DevConsole] Applied powerup: {powerup.PowerUpName}");
                    } else {
                        Debug.LogWarning("[DevConsole] Player missing PlayerPowerupHandler component.");
                    }
                } else {
                    Debug.LogWarning("[DevConsole] Player not found (tagged 'Player').");
                }
            };

            allCommands.Add(cmd);
            Debug.Log($"[DevConsole] Command registered: {cmd}");
        }

        allCommands.Sort();
    }

    void AutoFillCommand() {
        string typed = inputField.text.ToLower();

        List<string> matches = allCommands.FindAll(cmd => cmd.StartsWith(typed));
        if (matches.Count == 0) return;

        inputField.text = matches[suggestionIndex % matches.Count];
        inputField.MoveTextEnd(false);
        suggestionIndex++;
    }

    void UpdateSuggestionText(string typed) {
        typed = typed.ToLower();

        // Only show suggestions if input starts with "powerup." and has more after it
        if (!typed.StartsWith("powerup.") || typed.Length <= "powerup.".Length) {
            suggestionText.text = "";
            return;
        }

        List<string> matches = allCommands.FindAll(cmd => cmd.StartsWith(typed));

        if (matches.Count == 0) {
            suggestionText.text = "";
            return;
        }

        suggestionText.text = string.Join("\n", matches.GetRange(0, Mathf.Min(5, matches.Count)));
    }

}