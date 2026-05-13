# Space Shooter — Appota Test

A vertical 2D space shooter built in Unity. Survive endless enemy waves, rack up score multipliers, collect power-ups, and chase your high score.

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

---

## Table of Contents

- [Project Checklist](#project-checklist)
- [Gameplay Overview](#gameplay-overview)
- [Tech Stack](#tech-stack)
- [Architecture](#architecture)
  - [Core Systems](#core-systems)
  - [Event Bus](#event-bus)
  - [Object Pool](#object-pool)
  - [Wave System](#wave-system)
  - [Skin System](#skin-system)
- [Folder Structure](#folder-structure)
- [Scenes](#scenes)
- [Prefabs](#prefabs)
- [Shaders](#shaders)
- [Game Mechanics](#game-mechanics)
  - [Player](#player)
  - [Enemies](#enemies)
  - [Scoring & Multiplier](#scoring--multiplier)
  - [Power-ups](#power-ups)
- [Data: ScriptableObjects](#data-scriptableobjects)

---

## Gameplay Overview

An infinite vertical shooter where enemy formations drop from the top of the screen in escalating difficulty. The player controls a spaceship that moves freely within the screen bounds, fires bullets, and must survive as long as possible. Score is earned by defeating enemies and surviving over time, with a combo multiplier that rewards consistent play.

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

## Architecture

The project follows a **singleton manager + static event bus** architecture. Game systems are loosely coupled — they publish and subscribe through `EventManager` rather than holding direct references to each other.

### Core Systems

| Class | Type | Responsibility |
|---|---|---|
| `GameManager` | Singleton MonoBehaviour | Score, lives, multiplier, pause, restart, game-over state |
| `AudioManager` | Singleton MonoBehaviour (DontDestroyOnLoad) | Music playback, SFX, volume settings via PlayerPrefs |
| `PoolManager` | Singleton MonoBehaviour | Pre-warmed generic object pool for all runtime-spawned objects |
| `WaveManager` | Singleton MonoBehaviour | Loads WaveData assets, ticks difficulty, spawns formations |
| `UIManager` | Singleton MonoBehaviour | HUD updates, game-over panel, floating score text, flash panel |
| `SkinManager` | Singleton MonoBehaviour (DontDestroyOnLoad) | Loads SkinData assets, persists selection, applies sprites |
| `EventManager` | Static class | Decoupled event bus — all cross-system communication |
| `ScreenBounds` | Static utility class | Calculates and exposes world-space screen edges from camera |

### Event Bus

`EventManager` is a static class containing `System.Action` events. Systems publish with `Emit*` helpers and subscribe/unsubscribe in `OnEnable`/`OnDisable` (or `Awake`/`OnDestroy` for DontDestroyOnLoad singletons).

```
OnScoreChanged(int score, int multiplier)
OnMultiplierChanged(int multiplier)
OnFloatingScore(int amount, Vector3 worldPos)
OnLivesChanged(int lives)
OnPlayerDamaged()
OnGameOver(int score, int highScore)
OnGameRestart()
OnGameQuit()
OnEnemyDefeated(int points, Vector3 worldPos)
OnWaveTierChanged(WaveManager.Difficulty tier)
OnPowerUpCollected()
OnPauseChanged(bool isPaused)
OnPlayerSkinChanged(int index)
```

`GameManager.OnDestroy` calls `EventManager.ClearAll()` to null all delegates on scene unload — preventing stale listener leaks across scene loads.

### Object Pool

`PoolManager` is a generic GameObject pool. Any prefab can be pre-warmed via the `PoolConfig[]` array in the Inspector. At runtime, call:

```csharp
var go  = PoolManager.Instance.Get(prefab);           // get by prefab reference
var cmp = PoolManager.Instance.Get<T>(prefab);        // get and return typed component
PoolManager.Instance.Return(gameObject);              // return to pool
```

Objects implementing `IPoolable` receive `OnGetFromPool()` and `OnReturnToPool()` callbacks automatically. The pool uses a `prefabLookup` dictionary so any GameObject can be returned without knowing its source prefab. Objects with no pool entry are destroyed normally.

**Pooled objects:** Player bullets, powered bullets, enemy bullets, enemies (all 3 tiers), power-ups, floating score text.

### Wave System

Waves are driven by **ScriptableObject data assets** loaded at runtime from `Assets/Resources/Waves/`. There are 15 wave definitions across 3 difficulty tiers.

```
WaveData
  ├── difficulty     : Easy | Medium | Hard
  ├── fullWidth      : int  (used to clamp center spawn X)
  └── entries[]
        ├── position : Vector2 (relative offset from formation center)
        └── type     : Small | Mid | Large
```

`WaveManager` escalates difficulty over time:
- **0–30s** → Easy
- **30–60s** → Medium
- **60s+** → Hard

Each `waveInterval` (default 4s), a random wave matching the current difficulty is selected. Spawn overlap is checked per-enemy with `Physics2D.OverlapCircle` using size-appropriate radii, and `Physics2D.SyncTransforms()` is called after each spawn so same-frame checks are accurate.

**Wave catalogue:**

| Waves | Difficulty | Formations |
|---|---|---|
| 1–6 | Easy | Flat line, V-shape, inverted-V, single column, flanking pairs, diamond |
| 7–11 | Medium | Mid-led V, mid flanks with trail, diamond mid tip/tail, alternating, mid vanguard |
| 12–15 | Hard | Large center escort, arrow, large flanks, heavy column |

### Skin System

`SkinData` ScriptableObjects live in `Assets/Resources/Skins/`. Each skin has three sprite levels (normal, power-1, power-2) plus a preview sprite for the UI.

```
SkinData
  ├── skinName    : string
  ├── level0      : Sprite  (base)
  ├── level1      : Sprite  (power-up tier 1)
  └── level2      : Sprite  (power-up tier 2)
```

`SkinManager` (DontDestroyOnLoad, lives in the MainMenu scene) persists the selected skin index to `PlayerPrefs`. `SkinManager.ApplySprite(sr, level)` is called by `PlayerController` on spawn and by `PlayerShooter` when power-up state changes.

**Available skins:** Alpha, Bravo, Charlie, Delta.

---

## Folder Structure

```
Assets/
├── Fonts/                        # Custom font assets
├── Materials/                    # Shader Graph materials
│   ├── HitShader.shadergraph     # White-flash hit effect (float _Flash)
│   ├── ShadowGraph.shadergraph   # Offset shadow for player and enemies
│   ├── ScrollingBG.shadergraph   # Scrolling gameplay background
│   └── ScrollingMenu.shadergraph # Scrolling main menu background
├── Plugins/                      # Third-party plugins (DOTween)
├── Prefabs/
│   ├── Player.prefab
│   ├── SkinManager.prefab        # Must be placed in MainMenu scene
│   ├── Enemy_Small.prefab
│   ├── Enemy_Mid.prefab
│   ├── Enemy_Large.prefab
│   ├── PlayerBullet.prefab
│   ├── PlayerBulletPowered.prefab
│   ├── EnemyBullet.prefab
│   ├── PowerUp1.prefab
│   ├── FloatingText.prefab
│   ├── ExplosionVFX.prefab
│   ├── CollectBurstVFX.prefab
│   └── UI/
│       └── SoundRow.prefab
├── Resources/                    # Runtime-loaded assets (Resources.Load)
│   ├── Maps/                     # Background sprites (random selection)
│   ├── Skins/                    # SkinData ScriptableObjects
│   │   ├── Skin_Alpha.asset
│   │   ├── Skin_Bravo.asset
│   │   ├── Skin_Charlie.asset
│   │   └── Skin_Delta.asset
│   ├── Waves/                    # WaveData ScriptableObjects (15 waves)
│   │   ├── Wave 1.asset  …  Wave 15.asset
│   ├── Cursor Pixel Pack/        # Cursor art assets
│   └── Pixel UI/                 # UI sprite sheets
├── Scenes/
│   ├── MainMenu.unity            # Build index 0
│   └── Game.unity                # Build index 1
├── Scripts/
│   ├── Audio/
│   │   └── AudioManager.cs
│   ├── Bullets/
│   │   └── Bullet.cs
│   ├── Core/
│   │   ├── EventManager.cs
│   │   ├── GameManager.cs
│   │   ├── MapController.cs
│   │   └── PoolManager.cs
│   ├── Enemies/
│   │   ├── EnemyController.cs
│   │   ├── EnemyHealth.cs
│   │   ├── WaveData.cs
│   │   └── WaveManager.cs
│   ├── Player/
│   │   ├── PlayerController.cs
│   │   ├── PlayerHealth.cs
│   │   ├── PlayerShooter.cs
│   │   ├── SkinData.cs
│   │   └── SkinManager.cs
│   ├── PowerUp/
│   │   └── PowerUp.cs
│   ├── UI/
│   │   ├── FloatingText.cs
│   │   ├── MainMenuController.cs
│   │   ├── SettingsPanel.cs
│   │   ├── SkinSelectPanel.cs
│   │   └── UIManager.cs
│   ├── Utilities/
│   │   ├── IPoolable.cs
│   │   └── ScreenBounds.cs
│   └── VFX/
│       ├── BulletTrail.cs
│       ├── EnemyDeathVFX.cs
│       ├── PlayerDeathVFX.cs
│       ├── ShadowSync.cs
│       └── VignetteController.cs
├── Settings/                     # URP renderer and pipeline assets
└── game-design-document.md
```

---

## Scenes

### MainMenu (build index 0)
- `AudioManager` — persists to Game scene via DontDestroyOnLoad
- `SkinManager` (**must be here**) — persists to Game scene via DontDestroyOnLoad
- `MainMenuController` — Play / Skins / Settings buttons, entrance animation
- `SkinSelectPanel` — skin carousel with dot indicators
- `SettingsPanel` — master / music / SFX sliders

### Game (build index 1)
- `GameManager` — orchestrates all gameplay state
- `WaveManager` — spawns enemy formations
- `PoolManager` — pre-warmed pool for bullets, enemies, text
- `UIManager` — HUD (score, lives, multiplier, timer), game-over panel
- `VignetteController` — URP vignette intensity reacts to score multiplier
- `MapController` — picks a random background sprite on start

---

## Prefabs

| Prefab | Key Components |
|---|---|
| `Player` | `PlayerController`, `PlayerShooter`, `PlayerHealth`, `PlayerDeathVFX`, `ShadowSync` (on Shadow child) |
| `SkinManager` | `SkinManager` — standalone, placed in MainMenu scene |
| `Enemy_Small/Mid/Large` | `EnemyController`, `EnemyHealth`, `EnemyDeathVFX`, `ShadowSync` (on Shadow child) |
| `PlayerBullet` | `Bullet` (isEnemyBullet=false), `BulletTrail` |
| `PlayerBulletPowered` | `Bullet` (larger, more damage), `BulletTrail` |
| `EnemyBullet` | `Bullet` (isEnemyBullet=true) |
| `PowerUp1` | `PowerUp` — scrolls down, 7% drop chance on enemy death |
| `FloatingText` | `FloatingText` — pooled, color-coded by amount |
| `ExplosionVFX` | Particle system — spawned by `EnemyDeathVFX` |
| `CollectBurstVFX` | Particle system — spawned by `PowerUp` on collect |

---

## Shaders

All shaders are authored in Shader Graph.

| Shader | Usage |
|---|---|
| `HitShader` | Applied to Player and all enemies via `hitMatInstance`. Float property `_Flash` (0–1) drives a lerp to solid white, creating the Cuphead-style hit flash. Controlled via DOTween in `PlayerHealth` and `EnemyHealth`. |
| `ShadowGraph` | Renders a desaturated, offset duplicate sprite. Used on Shadow child objects under Player and all enemy prefabs. `ShadowSync` copies the parent SpriteRenderer's sprite each `LateUpdate` so it stays in sync with skin/power-up changes. |
| `ScrollingBG` | Seamlessly scrolling background texture for the Game scene. |
| `ScrollingMenu` | Scrolling background variant for the Main Menu scene. |

---

## Game Mechanics

### Player

- Moves in all four directions with WASD or gamepad stick; clamped to screen bounds.
- **Banking animation:** `PlayerController` rotates the sprite on the Y axis (fake 3D squish) when moving left/right via DOTween.
- **3 lives.** On hit, enters `Recovery` state for 3 seconds: invincible, collider disabled, sprite loops a white-flash sequence. Final hit triggers `DeathSequence`.
- Fires at 4 bullets/second. Hold fire key to auto-fire.
- **Entrance animation:** slides in from below screen with scale + move tween on spawn.

### Enemies

Three tiers, differentiated by size, HP, and formation role:

| Type | HP | Collision Damage | Drop Size |
|---|---|---|---|
| Small | 100 | 25 | ~0.5 units |
| Mid | 100 | 25 | ~0.7 units |
| Large | 100 | 25 | ~1.0 units |

- All enemies scroll downward. They fire a bullet straight down every `fireInterval` seconds, but only when fully visible on screen (prevents off-screen sniping).
- Firing uses a downward raycast against the player layer — enemies only fire if the player is in their line of sight.
- On death: `EnemyDeathVFX` spawns an explosion, `EnemyHealth` emits `OnEnemyDefeated`, and has a **7% chance** to drop a power-up.
- On pool return: all DOTween tweens are killed and the hit material is restored.

### Scoring & Multiplier

| Event | Points |
|---|---|
| Enemy defeated | 25 × multiplier |
| Survive 5 seconds | 10 × multiplier |

Multiplier is based on time since last hit:

| No-damage streak | Multiplier |
|---|---|
| < 5s | ×1 |
| 5s | ×2 |
| 10s | ×4 |
| 30s | ×5 |

Taking any damage resets the streak and multiplier to ×1 instantly.

The `VignetteController` visually reinforces the multiplier — the URP vignette deepens in colour and intensity as the multiplier climbs.

High score is persisted to `PlayerPrefs` and displayed on the main menu.

### Power-ups

- Drop from enemies at 7% chance on death; scroll downward and despawn off-screen.
- Collection is blocked if the player is in `Recovery` (invincible) state.
- On collect: `PlayerShooter.ActivatePowerup()` increments `powerLevel` (max 2). `SkinManager` updates the player sprite to the corresponding skin level.
- Power-up lasts 10 seconds. In the last 3 seconds, the sprite flickers as a warning.
- Collecting a second power-up while one is active restacks the timer.

---

## Data: ScriptableObjects

Two data types are authored as ScriptableObjects and loaded at runtime via `Resources.LoadAll`.

**SkinData** (`Assets/Resources/Skins/`)
```
Create → SpaceShooter/SkinData
Fields: skinName, level0 sprite, level1 sprite, level2 sprite
```

**WaveData** (`Assets/Resources/Waves/`)
```
Create → SpaceShooter/WaveData
Fields: difficulty (Easy/Medium/Hard), fullWidth (int), entries[] { position, type }
```
Position is a local offset from the formation center. `fullWidth` is used to clamp the random center X so the formation never spawns partially off-screen.

