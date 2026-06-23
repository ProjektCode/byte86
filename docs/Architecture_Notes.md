# Architecture Notes

## Runtime state ownership
- Player-only runtime state belongs on player components, not ScriptableObjects.
- ScriptableObjects should hold config, not active effect state.
- Wave spawning should have a single owner for start/stop/reset.

## Powerup model
- Source of truth for duration: choose one owner before fixing refresh bugs.
- Source of truth for active list: `PlayerPowerupHandler` or a dedicated runtime powerup service.
- Refresh behavior: duplicate pickup should refresh both UI and real effect state.
- Restart/death cleanup behavior: active effects must be explicitly cleared before/during player teardown.

## Performance model
- Prefer counters/events over scene scans.
- Prefer cached references over repeated lookups.
- Pool projectiles before raising bullet counts further.
