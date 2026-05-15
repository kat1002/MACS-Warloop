![MACS: WARLOOP](docs/banner.png)

# MACS: WARLOOP — Appota Test

*A fast-paced vertical 2D space shooter built in Unity.*

**[⬇ Download Latest Release](https://github.com/kat1002/MACS-Warloop/releases/latest)**

---

## Story

*"War is no longer fought on the ground. It lives in the simulation."*

Year 322X. The world is split — the powerful float above the clouds while the streets below drown in chaos. Your only escape is the MACS program. Strap in, pilot. The loop never ends.

Survive endless waves of enemy fighters. Every second alive earns points. Every kill earns more. Go on a no-damage streak and watch your multiplier climb — 2x, 4x, all the way to 5x.

**One hit. And it all comes crashing down.**

---

## How to Play

### Controls

| Action | Keyboard | Gamepad |
|---|---|---|
| Move | WASD / Arrow Keys | Left Stick / D-Pad |
| Fire | Hold Space / Left Click | Hold A / Cross |
| Pause | ESC | Start |

### Rules

- You have **3 lives**. Taking a hit triggers a 3-second invincibility window — use it.
- Survive to earn **+10 pts every 5 seconds**. Defeat enemies for **+25 pts each**.
- Stay untouched and your **score multiplier climbs** — one hit resets it to ×1.
- Collect **power-ups** dropped by enemies (7% drop chance) to fire larger, harder-hitting bullets.

### Score Multiplier

| No-damage streak | Multiplier |
|---|---|
| < 5 seconds | ×1 |
| 5 seconds | ×2 |
| 10 seconds | ×4 |
| 30 seconds | ×5 |

---

## Tech Stack

| Tool | Purpose |
|---|---|
| Unity 2022+ (URP) | Engine and render pipeline |
| Unity Input System | Player input (keyboard, gamepad) |
| DOTween | All runtime animation and tweening |
| TextMeshPro | UI text rendering |
| Shader Graph | Custom hit-flash, shadow, and scrolling background shaders |

---

## Architecture & Technical Docs

For a full breakdown of system design, folder structure, scenes, prefabs, shaders, and game mechanics, see [ARCHITECTURE.md](ARCHITECTURE.md).

---

## Project Checklist

### Core Systems
- [x] Static event bus (`EventManager`) — decouples all game systems
- [x] Generic object pool (`PoolManager`) with `IPoolable` callbacks and pre-warm support
- [x] `ScreenBounds` static utility — world-space screen edges derived from camera
- [x] `GameManager` — score, lives, multiplier, pause, restart, game-over state
- [x] `AudioManager` — DontDestroyOnLoad, per-scene music, SFX, volume via PlayerPrefs

### Player
- [x] Movement in all four directions, clamped to screen bounds
- [x] Banking animation — fake 3D Y-axis tilt on horizontal movement (DOTween)
- [x] Entrance animation — slides in from below screen with scale tween on spawn
- [x] Auto-fire at 4 bullets/second
- [x] 3 lives system
- [x] Recovery state — 3-second invincibility with looping hit-flash after taking damage
- [x] Death sequence — fragment VFX then deactivate

### Enemies
- [x] 3 enemy tiers: Small, Mid, Large
- [x] Downward scrolling movement; auto-return to pool when off-screen
- [x] Line-of-sight raycast firing — enemies only shoot when fully visible and player is below them
- [x] Hit-flash shader effect on taking damage
- [x] 7% power-up drop chance on death
- [x] Explosion VFX on death
- [x] Shadow child object synced to parent sprite via `ShadowSync`

### Wave System
- [x] `WaveData` ScriptableObject — difficulty, formation width, array of typed enemy entries
- [x] `WaveManager` — loads all wave assets from `Resources/Waves` at runtime
- [x] Three difficulty tiers with automatic escalation over time (Easy → Medium → Hard)
- [x] 15 hand-authored wave formations (6 Easy, 5 Medium, 4 Hard)
- [x] Overlap detection per spawn using size-appropriate radii + `Physics2D.SyncTransforms`
- [x] Formation center randomised within screen bounds using `fullWidth`

### Scoring & Progression
- [x] Enemy kill points (25 per kill)
- [x] Survival bonus (10 points every 5 seconds)
- [x] Score multiplier system — ×1 / ×2 / ×4 / ×5 based on no-damage streak
- [x] Multiplier resets instantly on taking damage
- [x] High score persisted to `PlayerPrefs`
- [x] Floating score text — pooled, colour-coded by amount
- [x] URP vignette intensity reacts to current multiplier (`VignetteController`)

### Power-ups
- [x] Power-up scrolls down and despawns off-screen (pooled)
- [x] Collection blocked during player recovery (invincibility window)
- [x] Two power levels — larger bullet sprite and increased damage
- [x] Flicker warning in final 3 seconds before expiry
- [x] Stacks timer if collected while already active

### Skin System
- [x] `SkinData` ScriptableObject — name, 3 sprite levels (base, power-1, power-2), preview
- [x] `SkinManager` — DontDestroyOnLoad, loads from `Resources/Skins`, persists selection
- [x] 4 skins: Alpha, Bravo, Charlie, Delta
- [x] Skin carousel UI (`SkinSelectPanel`) with dot indicator and scale animations

### Audio
- [x] Separate music tracks for Main Menu and Game scene
- [x] Music stops on quit before returning to Main Menu (prevents overlap)
- [x] SFX for: shoot, player hit, enemy hit, enemy death, power-up collect, game over, UI click
- [x] Master / Music / SFX volume sliders, values persisted to `PlayerPrefs`

### UI & HUD
- [x] Live score display with punch-scale animation on change
- [x] Score multiplier label
- [x] Survival timer (MM:SS)
- [x] Life icons with shake-and-hide animation on loss
- [x] Game over panel — final score, high score, restart and quit buttons
- [x] Pause via settings panel (DOTween-animated open/close)
- [x] Screen flash on power-up collect
- [x] Main menu entrance animation (title scale-in, buttons fade-in)

### Visual Effects
- [x] `HitShader` (Shader Graph) — `_Flash` float drives lerp to solid white
- [x] `ShadowGraph` (Shader Graph) — offset drop shadow for player and enemies
- [x] `ShadowSync` — LateUpdate copies parent sprite to shadow child every frame
- [x] `ScrollingBG` and `ScrollingMenu` Shader Graph scrolling backgrounds
- [x] `BulletTrail` — pool-safe TrailRenderer with one-frame delay to avoid position artefacts
- [x] Camera shake on player hit
- [x] Player punch-position tween on hit
- [x] Player death fragment VFX (spinning, fading sprite shards)
- [x] Enemy explosion VFX on death
- [x] Collect burst VFX on power-up pickup

### Scenes & Structure
- [x] Main Menu scene (build index 0) — skin select, settings, high score display
- [x] Game scene (build index 1) — full gameplay loop
- [x] Random background map selection at runtime (`MapController`)
- [x] `ScriptableObject`-driven data for waves and skins (no hardcoded formations)
