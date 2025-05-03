**Game Design Document: Byte86**

---

### Game Overview

**Title:** Byte86
**Genre:** 2D Vertical Shooter (Bullet Hell Elements)
**Premise:**
Set in a retro-futuristic digital world, the player pilots a combat program through waves of hostile malware and rogue AIs. Armed with upgradable weapons and temporary power-ups, players battle to survive increasingly intense waves while pushing their high score.

**Theme:** Retro-cyberpunk with a pixel-art aesthetic and synthwave vibe.
**Target Audience:** Fans of arcade-style shooters, especially players who enjoy short, high-replay-value experiences with tight controls and intense challenge.

---

### Formal Elements

**Players:**
Single-player experience.

**Objectives:**
Survive as long as possible while defeating waves of enemies, increasing score, and reaching bosses.

**Procedures:**

* Use WASD or gamepad to move.
* Aim and shoot using arrow keys, mouse, or gamepad right stick.
* Avoid enemy bullets and contact.
* Collect power-ups to temporarily enhance your abilities.

**Rules:**

* The player cannot move outside the camera bounds.
* Bullets destroy each other if they collide.
* Collisions with enemies or bullets cause damage.
* Power-ups cannot stack and expire after a duration.
* Boosted bullets only apply to player bullets.

**Resources:**

* **Health:** Lost when hit by enemies or projectiles. Healed via the Juggernaut power-up.
* **Power-ups:** Grant temporary bonuses like Rapid Fire or Scattershot.
* **Score:** Increased by defeating enemies and surviving waves.

**Conflict:**

* Increasing wave difficulty and bullet density.
* Bosses with complex projectile patterns.
* Limited space and mobility within the camera bounds.

**Boundaries:**

* The player remains confined within screen bounds.
* Off-screen enemies cannot interact with the player until visible.

**Outcome:**

* The game ends when the player’s health reaches zero.
* Outcome measured by final score and wave reached.

---

### Dramatic Elements

**Challenge:**

* Reflex-based dodging, bullet pattern reading, and enemy prioritization.

**Engagement Through Feedback:**

* Hit flashes and screen shake enhance the sense of impact.
* Death animations and color flashes communicate states like boosting or taking damage.

**Premise and Style:**

* A minimalistic narrative: survive the corrupted digital world as the last antivirus program.
* Synth-style retro-futuristic audio and visuals deliver a nostalgic feel.

---

### Level Design

**Wave-Based Arena (Vertical Scrolling)**

* **Player Area:** Confined within the camera bounds at the bottom of the screen.
* **Enemy Entry:** Enemies spawn off-screen and move downward or towards the player.
* **Obstacles:** No physical obstacles, but bullet patterns and enemy formations create environmental hazards.
* **Objective:** Survive until the end of the wave.
* **Transition:** A message appears briefly between waves.

**Boss Encounter (Example: Gigashade)**

* Appears every several waves.
* Large enemy with health bar and advanced projectile patterns.
* Has bullets that may track the player but can be shot down.

---

### Completed Features List

* Player movement constrained within screen bounds.
* Basic shooting system using player direction.
* Enemy wave system using ScriptableObjects.
* Several enemy types:

  * PulseRig: basic shooter.
  * Bulkhead-9: slow tank enemy.
  * Trackbyte Drone: fast zig-zag chaser.
  * Gigashade: boss with custom behavior.
* Bullet collision with enemies and other bullets.
* Bullet direction follows shooter’s rotation.
* Player and enemy health systems.
* Power-ups:

  * **Rapid Fire** – Increases fire rate.
  * **Scattershot** – Fires additional projectiles in a spread.
  * **Juggernaut** – Temporarily increases max HP and heals.
  * **Evasion** – Temporarily increases movement speed.
* Bullet boost system (visual flashing, enhanced behavior).
* Screen shake and white flash hit feedback using FEEL.
* Bullet trail visuals to match pixel style.
* Enemy shooting cooldown tied to visibility.
* Wave transition message UI.
* Game over condition when player health reaches zero.

---

### How to Play

* Move your ship using **WASD** or gamepad left stick.
* Aim with mouse or arrow keys.
* **Shoot** to destroy enemies.
* **Dodge** bullets and avoid enemy contact.
* **Collect power-ups** to temporarily boost your abilities.
* Survive as long as possible and rack up points.

---

### Additional Notes

* All gameplay systems are polished for tight feedback and responsiveness.
* The game is currently balanced for short bursts of action with increasing difficulty.
* Player’s bullets are visually distinguished when boosted for clarity.
* Boss bullets are interactable and readable, with potential to be destroyed.
* No stacking of power-ups to maintain clarity and balance.

---

*This design document reflects the current completed state of Byte86 as of May 2, 2025.*
