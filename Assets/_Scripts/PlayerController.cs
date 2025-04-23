using System;
using UnityEngine;

public class PlayerController : PlayerStats {

    private Vector2 input;
    private Camera mainCamera;

    [NonSerialized]
    public bool isDead = false;

    void Start() {
        mainCamera = Camera.main;

        // Optional: Set Rigidbody to Kinematic (recommended for player control)
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
    }

    void Update() {
        if(isDead) return;
        // Handle input and animation in Update
        input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        animator.SetBool("isBoosting", input != Vector2.zero);
    }

    void FixedUpdate() {
        if(isDead) return;

        // Move the player using Rigidbody2D
        Vector2 newPos = rb.position + moveSpeed * Time.fixedDeltaTime * input;

        // Clamp the new position to screen bounds
        Vector2 clampedPos = ClampToScreen(newPos);

        // Apply the movement
        rb.MovePosition(clampedPos);
    }

    Vector2 ClampToScreen(Vector2 targetPos) {
        Vector3 viewPos = mainCamera.WorldToViewportPoint(targetPos);

        // Clamp to screen bounds (X: full width, Y: bottom half)
        viewPos.x = Mathf.Clamp(viewPos.x, 0f, 1f);
        viewPos.y = Mathf.Clamp(viewPos.y, 0f, 0.5f);

        Vector3 worldPos = mainCamera.ViewportToWorldPoint(viewPos);
        return new Vector2(worldPos.x, worldPos.y);
    }

    protected override void Die() {
        isDead = true;
        base.Die();
    }

}
