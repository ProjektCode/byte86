# Unity Code Review — Bugs & Suggestions

Scope: `Assets/_Scripts/` (C#). This is a static review (no runtime profiling).

## Critical Bugs (likely to break gameplay)

### 1) Shield can become infinite (unkillable) due to scaled-time timers
- Files:
  - `Assets/_Scripts/ShieldHandler.cs:38-41` (`WaitForSeconds(duration)`)
  - `Assets/_Scripts/PlayerPowerupHandler.cs:53-60` (`WaitForSeconds(duration)`)
  - `Assets/_Scripts/DevConsole.cs:51-54` (`Time.timeScale = 0` when console open)
  - `Assets/_Scripts/GameManager.cs:80` (`Time.timeScale = 0` on game over)
- What happens:
  - Both shield expiry and powerup expiry depend on **scaled time**. If the game is paused (`Time.timeScale == 0`) while shield is active, the timers stop and `shieldActive` can stay `true`, making the player permanently invulnerable until something force-disables it.
- Fix (pick one, but be consistent):
  1) Use **unscaled time** for shield/powerup durations (e.g., `WaitForSecondsRealtime`) so effects expire even while paused.
  2) Keep scaled time, but add an explicit “pause/game over cleanup” path that force-disables shield and deactivates all timed powerups when pausing.
  3) Add a safety net: on player `OnDisable/OnDestroy`, force `shieldActive = false` (prevents state leak across respawns).

### 2) Enemy bullet damage scaling mutates prefab reference
- File: `Assets/_Scripts/EnemySpawner.cs:74-77`
- Issue: `shooter.bulletPrefab.GetComponent<EnemyBullet>().damage *= multiplier;`
- Impact: Scales the prefab’s component state (shared reference). Depending on how that prefab is shared and on Unity reload settings, damage can accumulate or become inconsistent.
- Fix: Apply scaling to the instantiated bullet instance, or store a per-enemy/per-wave damage multiplier and apply it on spawn.

### 3) Scattershot bullets often don’t receive PlayerStats
- File: `Assets/_Scripts/PlayerShooting.cs:54-67`
- Issue: In scattershot mode, multiple bullets are instantiated, but `GetStats(stats)` is only called on the last assigned `bullet` after the loop.
- Impact: Many bullets have `playerStats == null`, leading to incorrect vampiric/critical behavior and possible null refs.
- Fix: Call `GetStats(stats)` for each instantiated bullet (inside the loop) and avoid using a shared `bullet` field for coroutine spawns.

### 4) PlayerBullet null ref risk (vampiric path)
- File: `Assets/_Scripts/PlayerBullet.cs:73-77`
- Issue: `playerStats.GetCurrentHealth()` used without ensuring `playerStats != null`.
- Impact: Rare crashes if a bullet wasn’t initialized properly (see Scattershot issue).
- Fix: Guard `playerStats` before use; ensure stats always injected during spawn.

### 5) PlayerStats damage feedback not reliably invoked (method hiding)
- File: `Assets/_Scripts/PlayerStats.cs:50-55`
- Issue: `new public void OnDamageTaken()` hides base method instead of overriding.
- Impact: Polymorphism breaks; base calls may bypass player-specific feedback like camera shake.
- Fix: Keep `Entity.OnDamageTaken` as `protected virtual`, and implement `protected override` in `PlayerStats`.

### 6) Awake order hazards between PlayerController and PlayerStats
- File: `Assets/_Scripts/PlayerController.cs:15-19`
- Issue: Uses `stats.rb` in `PlayerController.Awake`, but `stats.rb` is assigned in `PlayerStats.Awake`.
- Impact: Intermittent null refs depending on component Awake order.
- Fix: Initialize Rigidbody/Animator references in one place, move PlayerController rb usage to `Start`, or enforce script execution order.

## High Priority Bugs / Correctness

### 7) Entity base class: null-safety issues
- File: `Assets/_Scripts/Entity.cs:61-68`, `Assets/_Scripts/Entity.cs:76-77`
- Issues:
  - `animator.SetBool("isDead", true)` can null-ref if animator missing.
  - `healthBar.Change(amount)` is called without checking `healthBar != null`.
- Fix: Add guards or enforce requirements via `[RequireComponent]` / validation.

### 8) Heal bar update likely incorrect at full heal boundary
- File: `Assets/_Scripts/Entity.cs:74-77`
- Issue: `if(currentHealth != MaxHealth) healthBar.Change(amount);` can skip updating UI on the last heal tick that reaches full.
- Fix: Compute actual healed amount and apply it consistently; always null-check `healthBar`.

### 9) Projectile assumes SpriteRenderer exists
- File: `Assets/_Scripts/Projectile.cs:13-15`
- Issue: Can null-ref if a projectile prefab is missing a `SpriteRenderer`.
- Fix: Guard `sr` or require it via `[RequireComponent(typeof(SpriteRenderer))]`.

### 10) Enemy/player lookups can crash if tags/components missing
- Files:
  - `Assets/_Scripts/EnemyMovement.cs:23-25` (`FindGameObjectWithTag("Player").transform` without null check)
  - `Assets/_Scripts/Chaser.cs:69-75` (`other.GetComponent<PlayerStats>()` without null check)
- Fix: Check for null and fail gracefully.

## Performance Issues (scaling/frame-time)

### 11) Per-frame scene tag scan during wave waiting
- File: `Assets/_Scripts/WaveManager.cs:34`
- Issue: `WaitUntil(() => GameObject.FindGameObjectsWithTag("Enemy").Length == 0)` allocates/scans every frame.
- Fix: Maintain an enemy counter (increment on spawn, decrement on enemy death) or use events.

### 12) Restart cleanup uses FindGameObjectsWithTag
- File: `Assets/_Scripts/GameManager.cs:92-100`
- Issue: Full-scene tag scans for enemies and bullets.
- Fix: Track spawned entities/bullets in lists, or pool projectiles.

### 13) Frequent instantiate/destroy hotspots (pooling candidate)
- Files:
  - `Assets/_Scripts/PlayerShooting.cs:53-76`
  - `Assets/_Scripts/EnemyShooting.cs:34-47`
  - `Assets/_Scripts/Projectile.cs:18-19`, `41-42`
  - `Assets/_Scripts/Background.cs:47-53`
- Fix: Use object pooling (`UnityEngine.Pool.ObjectPool<T>`) for bullets/tiles.

## Maintainability / Architecture Suggestions

### 14) Avoid runtime state in ScriptableObjects (global/static state)
- Files:
  - `Assets/_Scripts/Scriptable Objects/Power Ups/DamageBoostPowerUp.cs:7-27`
  - `Assets/_Scripts/Scriptable Objects/Power Ups/CriticalSurgePowerUp.cs:9-23`
  - `Assets/_Scripts/Scriptable Objects/Power Ups/PowerCoreSyncPowerUp.cs:7-24`
  - `Assets/_Scripts/Scriptable Objects/Power Ups/RapidFirePowerup.cs:9-21`
  - `Assets/_Scripts/Scriptable Objects/Power Ups/JuggernautPowerup.cs:8-47`
- Issue: Static/instance mutable fields on ScriptableObjects can leak across runs/players and behave oddly with domain reload settings.
- Suggestion: Treat ScriptableObjects as immutable config; keep active state on the player (component) with timers.

### 15) Powerup UI slot bookkeeping can desync
- File: `Assets/_Scripts/PlayerPowerupHandler.cs:27-35`, `53-61`
- Issue: Removing “oldest” clears slot 0 but doesn’t shift slot 1 down; stored `slotIndex` can become stale after list mutations.
- Suggestion: Derive UI slots from current list each time (render function) or maintain stable slot assignment.

### 16) Cache `Camera.main` usage in Update loops
- Files:
  - `Assets/_Scripts/EnemyShooting.cs:51`
  - `Assets/_Scripts/Background.cs:40`
- Suggestion: Cache camera reference once in `Awake/Start`.

## Quick Win Checklist (recommended fix order)
1. Fix scattershot stat injection (`PlayerShooting` + `PlayerBullet` null guards).
2. Fix shield duration semantics (realtime vs scaled time) and add a safety net force-disable.
3. Fix prefab mutation damage scaling (`EnemySpawner`).
4. Fix method hiding / damage feedback polymorphism (`Entity`/`PlayerStats`).
5. Remove per-frame tag scans in wave loop (`WaveManager`) and consider pooling.
