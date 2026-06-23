# Fix Roadmap

## 1. Stabilize gameplay state
- Fix healing duration math.
- Fix powerup refresh behavior.
- Fix restart cleanup.
- Remove/reset static powerup state.

## 2. Stabilize combat correctness
- Fix scattershot stat injection.
- Fix enemy bullet damage scaling.
- Stop boss attacks on death.

## 3. Harden architecture
- Remove ScriptableObject runtime state.
- Add null guards / component requirements.
- Reduce public field surface.
- Migrate gameplay input to the New Input System.

## 4. Improve performance
- Replace scene scans with counters or events.
- Cache repeated lookups.
- Add projectile pooling.
