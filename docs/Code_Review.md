# byte86 Code Review
_As of 2026-05-26_  
_Static review only. No Play Mode, build, profiler, or runtime validation was performed._

## Scope
- Reviewed: `Assets/_Scripts/**/*.cs`
- Prior review source checked: repo-root `Code_Review.md`
- Intended destination for this updated review: `docs/Code_Review.md`

## Severity legend
- **Critical** = likely gameplay-breaking or state-corrupting
- **High** = frequent correctness/lifecycle bug or strong regression risk
- **Medium** = maintainability/perf issue or bug requiring specific setup
- **Low** = cleanup, consistency, or defensive hardening

---

## Review summary
- Confirmed several prior findings, but two were overstated in the old review: the scaled-time shield/powerup timers and the `PlayerStats.OnDamageTaken()` method hiding are real design/code smells, but not the strongest confirmed breakages from static review.
- The most serious newly confirmed bug is in `Assets/_Scripts/PlayerStats.cs`: healing duration is measured with `Time.deltaTime` but advanced only once per tick, so healing can last far longer than configured.
- The powerup system has two major lifecycle bugs: refreshing an active powerup often refreshes only the handler coroutine, not the effect's real internal timer/state; and static powerup flags can survive player death/restart because deactivation is coroutine-driven on the destroyed player.
- `Assets/_Scripts/EnemySpawner.cs` still mutates shared bullet prefab state for enemy damage scaling, which can permanently inflate damage across waves/restarts.
- `Assets/_Scripts/PlayerShooting.cs` still fails to inject `PlayerStats` into every scattershot bullet, and `Assets/_Scripts/PlayerBullet.cs` still assumes `playerStats` is non-null in the vampiric path.
- Wave/restart flow needs attention: `WaveManager.ResetWaves()` does not reset difficulty scaling, and stopping only `WaveManager` coroutines may leave `EnemySpawner.SpawnWave()` running during restart.
- Performance issues remain: per-frame scene scans, repeated `Camera.main` lookups, and instantiate/destroy-heavy bullets/background tiles are all scaling risks.
- The codebase also violates the project's "New Input System only" rule in multiple scripts by using legacy `Input.*` APIs.

---

## Prior review validation

| Prior finding | Status | Notes |
|---|---|---|
| Shield/powerup timers use scaled time | **Partially confirmed / design-dependent** | `WaitForSeconds` is used, but "infinite shield" is overstated from static review alone. The stronger confirmed issue is that powerup refresh logic does not refresh some internal effect timers. |
| Enemy bullet damage scaling mutates prefab | **Confirmed** | Still a high-risk shared-state bug. |
| Scattershot bullets miss `PlayerStats` injection | **Confirmed** | Still present and still user-visible. |
| `PlayerBullet` null-ref risk in vampiric path | **Confirmed** | Still reachable because of scattershot injection bug. |
| `PlayerStats.OnDamageTaken()` hides base method | **Confirmed but lower severity** | This is a polymorphism/maintainability risk. Current direct player damage path still calls the hidden method from `PlayerStats.TakeDamage()`. |
| `PlayerController.Awake()` depends on `PlayerStats.Awake()` | **Confirmed** | Unity component awake order is not guaranteed. |
| Null-safety issues in `Entity` | **Confirmed** | Still multiple unsafe assumptions. |
| Heal bar full-heal boundary issue | **Confirmed** | Logic is still suspect. |
| `Projectile` assumes `SpriteRenderer` exists | **Confirmed** | Still unsafe. |
| Enemy/player lookup null risks | **Confirmed** | Still present. |
| Wave waiting uses per-frame scene scan | **Confirmed** | Still present. |
| Restart cleanup uses scene-wide tag scans | **Confirmed** | Still present. |
| Frequent instantiate/destroy hotspots | **Confirmed** | Still present. |
| Mutable runtime state in ScriptableObjects | **Confirmed and more severe than originally stated** | This now includes restart/state-leak bugs, not just architecture smell. |
| Powerup UI slot bookkeeping can desync | **Confirmed** | Still present. |
| `Camera.main` in update loops | **Confirmed** | Still present. |

---

## Critical

### CR-01 — Healing duration is effectively much longer than configured
- **Files:** `Assets/_Scripts/PlayerStats.cs:181-191`
- **Confidence:** Confirmed (static)
- **What happens:** `HealOverTime()` advances `elapsedTime` by `Time.deltaTime`, but then waits `healTickDelay` seconds between iterations. With a `TickDelay` around 1 second, the timer advances by ~0.016 per second instead of by 1 second, so the effect can last ~60x longer than intended.
- **Impact:** Healing powerups can remain active for dramatically longer than their configured duration.
- **Fix:** Track elapsed time using `healTickDelay`, `Time.time`, or `Time.unscaledTime` consistently.

### CR-02 — Refreshing an active powerup often refreshes only the handler coroutine, not the actual effect
- **Files:** `PlayerPowerupHandler.cs`, `ShieldPowerUp.cs`, `HealingPowerUp.cs`, `ShieldHandler.cs`, `PlayerStats.cs`
- **Confidence:** Confirmed (static)
- **What happens:** When an already-active powerup is picked up again, `PlayerPowerupHandler.ApplyPowerup()` stops and recreates only its own removal coroutine. It does not call `Activate()` again.
- **Impact:** UI can show a refreshed powerup while the real effect expires on the original schedule.
- **Fix:** Centralize duration ownership, or add explicit refresh behavior per powerup/effect.

### CR-03 — Static powerup state can leak across death/restart
- **Files:** offensive powerup ScriptableObjects, `PlayerPowerupHandler.cs`, `GameManager.cs`
- **Confidence:** Confirmed (static)
- **What happens:** Several powerups store active state in static fields. Deactivation depends on coroutines running on the player object. Destroying/restarting can stop those coroutines before deactivation.
- **Impact:** New players can inherit old offensive powerup state after restart/death.
- **Fix:** Remove runtime state from ScriptableObjects/statics. Keep active powerup state on the player/session runtime object. At minimum, force cleanup on player death/restart.

### CR-04 — Enemy wave difficulty mutates shared bullet prefab state
- **Files:** `Assets/_Scripts/EnemySpawner.cs:74-76`
- **Confidence:** Confirmed (static)
- **What happens:** `shooter.bulletPrefab.GetComponent<EnemyBullet>().damage *= multiplier;` modifies the prefab/component reference, not just bullets fired by this enemy instance.
- **Impact:** Damage can stack or drift across waves, enemies, and restarts.
- **Fix:** Apply damage scaling to projectile instances, or store a per-enemy damage multiplier and pass it to bullets on spawn.

### CR-05 — Scattershot bullets still miss stat injection, which can break vampiric/crit behavior
- **Files:** `Assets/_Scripts/PlayerShooting.cs`, `Assets/_Scripts/PlayerBullet.cs`
- **Confidence:** Confirmed (static)
- **What happens:** In scattershot mode, multiple bullets are instantiated but `GetStats(stats)` is only called on the last created bullet reference.
- **Impact:** Earlier bullets can run with `playerStats == null`.
- **Fix:** Inject stats per instantiated bullet inside the loop. Avoid a shared `bullet` field for multi-spawn logic.

## High

### H-01 — Wave restart does not fully reset wave state
- **Files:** `WaveManager.cs`, `GameManager.cs`
- **Issue:** `ResetWaves()` resets wave index/count but not `difficultyMultiplier`. Stopping only `WaveManager` coroutines may not stop an active `EnemySpawner.SpawnWave()` coroutine.
- **Fix:** Reset `difficultyMultiplier = 1f`; track and stop active spawn coroutine from one owner.

### H-02 — `PlayerController.Awake()` relies on undefined component awake order
- **Files:** `PlayerController.cs`, `PlayerStats.cs`
- **Issue:** `PlayerController.Awake()` accesses `stats.rb`, but `rb` is assigned in `PlayerStats.Awake()`.
- **Fix:** Initialize required references locally, move Rigidbody setup to `Start()`, or enforce script execution order.

### H-03 — Boss attack coroutine appears able to continue firing during death sequence
- **Files:** `BossController.cs`, `EnemyStats.cs`
- **Issue:** `AttackLoop()` runs forever and has no stop condition tied to enemy death.
- **Fix:** Stop attack coroutines on death/disable or gate shooting on alive state.

### H-04 — Legacy input APIs violate the project's New Input System rule
- **Files:** `PlayerController.cs`, `PlayerShooting.cs`, `GameManager.cs`, `DevConsole.cs`
- **Issue:** Uses `Input.GetAxisRaw`, `Input.GetKey`, and `Input.GetKeyDown`.
- **Fix:** Migrate to Input Actions / `PlayerInput`-driven input.

### H-05 — Enemy lookup assumes player always exists
- **Files:** `EnemyMovement.cs`
- **Issue:** `GameObject.FindGameObjectWithTag("Player").transform` dereferences without a null check.
- **Fix:** Null-check before accessing `.transform`.

## Medium

### M-01 — `PlayerStats.OnDamageTaken()` hides a virtual method instead of overriding it
- **Files:** `PlayerStats.cs`, `Entity.cs`
- **Fix:** Make base method `protected virtual`; use `protected override` in `PlayerStats`.

### M-02 — Powerup UI slot bookkeeping can desync from actual active list
- **Files:** `PlayerPowerupHandler.cs`
- **Fix:** Rebuild category UI from the current list after every add/remove/refresh.

### M-03 — `Entity` still assumes required optional references exist
- **Files:** `Entity.cs`
- **Fix:** Add guards and/or enforce requirements with `[RequireComponent]`, prefab validation, and editor-time checks.

### M-04 — Enemy death assumes `PowerupDropper` exists
- **Files:** `EnemyStats.cs`
- **Fix:** Null-check or require the component consistently.

### M-05 — `Projectile` assumes `SpriteRenderer` exists
- **Files:** `Projectile.cs`
- **Fix:** Add `[RequireComponent(typeof(SpriteRenderer))]` or guard/null-log.

### M-06 — Enemy health bar scaling may be racing `Bar.Start()`
- **Files:** `EnemySpawner.cs`, `Bar.cs`
- **Fix:** Give `Bar` an explicit initialization API, or defer bar refresh until after the bar is ready.

### M-07 — Runtime scene scans are used as control flow
- **Files:** `WaveManager.cs`, `GameManager.cs`, `DevConsole.cs`, `Chaser.cs`
- **Fix:** Prefer event-driven registration, counters, or cached references.

### M-08 — Frequent instantiate/destroy of bullets and background tiles is a scaling hotspot
- **Files:** `PlayerShooting.cs`, `EnemyShooting.cs`, `Projectile.cs`, `Background.cs`, `BossController.cs`
- **Fix:** Add object pooling, especially for projectiles.

## Low

### L-01 — `Camera.main` is still used in hot or repeated code paths
- **Files:** `EnemyShooting.cs`, `Background.cs`
- **Fix:** Cache camera references.

### L-02 — Field exposure is broader than necessary across the codebase
- **Files:** multiple
- **Fix:** Prefer `[SerializeField] private` plus read-only properties where needed.

### L-03 — Logging is noisy in gameplay code
- **Files:** multiple
- **Fix:** Reduce repeated debug logs or wrap them in debug-only guards.

---

## Suggested fix order
1. Fix healing duration math in `PlayerStats`.
2. Redesign powerup timing/state ownership so refresh, expiry, death, and restart all behave consistently.
3. Remove static active powerup state from ScriptableObjects.
4. Fix enemy bullet damage scaling so it does not mutate prefab/shared state.
5. Fix scattershot stat injection and add null guards in `PlayerBullet`.
6. Fix wave reset ownership (`difficultyMultiplier`, active spawn coroutine shutdown).
7. Address awake-order and null-safety hazards.
8. Migrate gameplay input to the New Input System.
9. Replace scene scans and high-churn instantiate/destroy paths with counters/caching/pooling.

---

## Static-review verification checklist
1. Healing duration matches configured value.
2. Duplicate powerup pickup refreshes real effect state and UI together.
3. Death/restart clears active powerups and static state.
4. Scattershot + vampiric/critical bullets behave consistently without null refs.
5. Enemy bullet damage scales without compounding across waves/restarts.
6. Restart during an active wave does not leave overlapping spawn routines.
7. Boss stops shooting at the intended death cutoff.
8. Enemy health bars initialize correctly on scaled enemies.
