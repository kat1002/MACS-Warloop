# MACS: WARLOOP — Architecture & Technical Docs

---

## Table of Contents

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
