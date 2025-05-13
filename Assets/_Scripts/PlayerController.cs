using System;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    private Vector2 input;
    private Camera mainCamera;

    private PlayerStats stats;

    [NonSerialized]
    public bool isDead = false;

    void Awake() {
        stats = GetComponent<PlayerStats>();
        // Optional: Set Rigidbody to Kinematic (recommended for player control)
        stats.rb.bodyType = RigidbodyType2D.Kinematic;
        stats.rb.gravityScale = 0f;
    }

    void Start() {
        mainCamera = Camera.main;
    }

    void Update() {
        if(isDead) return;
        // Handle input and animation in Update
        input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        stats.animator.SetBool("isBoosting", input != Vector2.zero);
    }

    void FixedUpdate() {

        // Move the player using Rigidbody2D
        Vector2 newPos = stats.rb.position + stats.moveSpeed * Time.fixedDeltaTime * input;

        // Clamp the new position to screen bounds
        Vector2 clampedPos = ClampToScreen(newPos);

        // Apply the movement
        stats.rb.MovePosition(clampedPos);
    }

    Vector2 ClampToScreen(Vector2 targetPos) {
        Vector3 viewPos = mainCamera.WorldToViewportPoint(targetPos);

        // Clamp to screen bounds (X: full width, Y: bottom half)
        viewPos.x = Mathf.Clamp(viewPos.x, 0f, 1f);
        viewPos.y = Mathf.Clamp(viewPos.y, 0f, 0.7f);

        Vector3 worldPos = mainCamera.ViewportToWorldPoint(viewPos);
        return (Vector2)worldPos;
    }

}
