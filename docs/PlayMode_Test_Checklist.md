# Play Mode Test Checklist

## Powerups
- [ ] Shield activates, expires, and refreshes correctly
- [ ] Healing duration matches configured value
- [ ] Healing stops cleanly on expiry and death
- [ ] Offensive powerups do not persist after restart
- [ ] Powerup UI matches actual active effects

## Combat
- [ ] Scattershot applies all bullet effects consistently
- [ ] Vampiric bullets never null-ref
- [ ] Enemy wave damage scales correctly without compounding
- [ ] Boss stops attacking on death

## Wave Flow
- [ ] Restart during active spawn does not leave overlapping waves
- [ ] Difficulty resets after restart
- [ ] Wave completion does not hitch from scene scans at small/large enemy counts
