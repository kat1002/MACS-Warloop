# Space Shooter — Task Tracker

## Setup
- [x] Add physics layers (Player=8, Enemy=9, PlayerBullet=10, EnemyBullet=11, PowerUp=12)
- [x] Configure 2D collision matrix

## Scripts (`Assets/Scripts/`) ✅
- [x] `Utilities/IPoolable.cs`
- [x] `Utilities/ScreenBounds.cs`
- [x] `Core/GameManager.cs` (singleton, scoring, lives — publishes via EventManager)
- [x] `Core/EventManager.cs` (central event bus, static events, ClearAll on destroy)
- [x] `Core/PoolManager.cs` (singleton, generic object pool)
- [x] `Audio/AudioManager.cs`
- [x] `Player/PlayerController.cs`
- [x] `Player/PlayerShooter.cs`
- [x] `Player/PlayerHealth.cs`
- [x] `Player/SkinManager.cs`
- [x] `Bullets/Bullet.cs`
- [x] `Enemies/EnemyHealth.cs`
- [x] `Enemies/EnemyController.cs`
- [x] `Enemies/WaveManager.cs` (8 formations + 3 difficulty tiers)
- [x] `PowerUp/PowerUp.cs`
- [x] `VFX/EnemyDeathVFX.cs`
- [x] `VFX/PlayerDeathVFX.cs`
- [x] `UI/UIManager.cs`
- [x] `UI/FloatingText.cs`
- [x] `InputSystem_Actions.cs` (generated)

## Map / Background (`Assets/Scripts/Map/`)
- [x] `ChunkData.cs` — ScriptableObject: BiomeType enum, int[120] tile grid (10×12)
- [x] `MapChunk.cs` — holds 10×12 SpriteRenderer grid, `Refresh(ChunkData)` stamps sprites
- [x] `ChunkManager.cs` — singleton, loads all ChunkData from Resources/Chunks/, pool of 3 chunks, scrolls + recycles
- [ ] `Assets/Editor/ChunkEditor.cs` — EditorWindow with visual 10×12 paint grid, biome palette, save ChunkData asset
- [ ] Pre-author 15 ChunkData assets (5 Grassland, 5 Desert, 5 Ruins) using ChunkEditor
- [ ] `Assets/Resources/Chunks/` folder with all 15 assets

## Scene (`Assets/Scenes/SampleScene.unity`)
- [ ] Orthographic camera (size=5)
- [ ] Global Light 2D
- [ ] Background tiles + BackgroundScroller
- [ ] GameManager GameObject
- [ ] EnemySpawner GameObject
- [ ] Player placed in scene

## Prefabs (`Assets/Prefabs/`)
- [ ] `Player.prefab`
- [ ] `Enemy1.prefab`
- [ ] `PlayerBullet.prefab`
- [ ] `PlayerBulletPowered.prefab`
- [ ] `EnemyBullet.prefab`
- [ ] `PowerUp1.prefab`

## High Score
- [ ] Save/load high score via `PlayerPrefs` in `GameManager`
- [ ] Show current score vs high score on Game Over screen
- [ ] Show high score on main menu

## UI (Canvas)
- [ ] Lives panel (3 icons)
- [ ] Score text (TextMeshPro)
- [ ] Multiplier text (TextMeshPro)
- [ ] Game Over panel (title + final score + restart button)

## Skin Selection
- Sprites: Blue=ship_0000, Red=ship_0001, Green=ship_0002, Yellow=ship_0003
- Enemies: 3 groups from ship_0012–ship_0023
  - Small  (ship_0020–0023): 100 HP, 25 dam, speed 3.0, scale 0.8x
  - Mid    (ship_0016–0019): 100 HP, 25 dam, speed 2.0, scale 1.0x
  - Large  (ship_0012–0015): 100 HP, 50 dam, speed 1.2, scale 1.4x
- [ ] `UI/SkinSelector.cs` — shows 4 color options, handles selection
- [ ] Skin selection panel (4 ship buttons with color labels, large preview)
- [ ] Save/load selected skin via PlayerPrefs (`"SelectedSkin"`, default=0)
- [ ] Player loads saved skin on game start via `Resources.Load`
- [ ] EnemySpawner randomly picks from ship_0012–ship_0023 per spawn

## Fonts
- [ ] Download Press Start 2P (titles, score, GAME OVER)
- [ ] Download Pixel Operator (HUD labels, small text)
- [ ] Import both into Unity, generate TMP font assets

## Main Menu & Settings (`Assets/Scenes/MainMenu.unity`)
- [ ] Main menu scene (Play, Settings, Quit buttons)
- [ ] Settings panel (master / SFX / music volume sliders, persisted via PlayerPrefs)
- [ ] Transition: Play → GameScene, Settings toggles panel

## Audio (`Assets/Scripts/Audio/`)
- [ ] `AudioManager.cs` (singleton, music + SFX channels, reads PlayerPrefs volumes)
- [ ] Hook SFX calls: player shoot, player hit, enemy hit, enemy death, powerup collect, UI clicks
- [ ] Hook music: menu music loop, gameplay music loop

## Enemy Waves & Formations (`Assets/Scripts/Enemies/`)
- [ ] `WaveManager.cs` — tracks elapsed time, bumps difficulty tier every 30s, picks random formation from tier pool
- [ ] Difficulty tiers:
  - Easy (0–30s): Small only → Line, Random Scatter
  - Medium (30–60s): Small + Mid → V-Shape, Inverted V, Flanking
  - Hard (60s+): Mid + Large → Arrow, Diamond, Column
- [ ] Formation: Line (horizontal row)
- [ ] Formation: V-Shape (converging V)
- [ ] Formation: Inverted V (wide spread)
- [ ] Formation: Diamond (compact, center-firing)
- [ ] Formation: Column (staggered vertical lane)
- [ ] Formation: Flanking (two side clusters)
- [ ] Formation: Random Scatter (chaos)
- [ ] Formation: Arrow (dense tip pointing at player)

## Game Juice

### Score Popup
- [ ] `FloatingText.cs` — TMP text, float up + fade out via DOTween (~1s)
- [ ] Color coded: white=+10 survival, yellow=+25 small, orange=+50 mid, red=+100 large
- [ ] Shows final multiplied value at spawn position
- [ ] Pooled via PoolManager
- [ ] `FloatingText.prefab`

### Low Effort (DOTween)
- [ ] Screen shake on player hit (0.2s, DOTween camera shake)
- [ ] Player recovery knockback tween (scale/position + flash)
- [ ] Score text punch tween (1.0 → 1.3 → 1.0 on every score add)
- [ ] Multiplier upgrade bounce (scale slam on 1x→2x→4x→5x)
- [ ] Lives icon shake + fade on life loss

### Medium Effort (Particles / Trail Renderer)
- [ ] Powerup collection ring flash (circle sprite scale+fade, player turns gold)
- [ ] Enemy death particle burst (color matches enemy sprite)
- [ ] Bullet Trail Renderer (player bullets 2-3 frames, powered bullets brighter)
- [ ] Enemy warp spawn (scale 0→1, ease OutBack)

### High Effort
- [ ] High multiplier vignette (URP post-processing at 4x/5x + text pulse)
- [ ] Player death fragment effect (3-4 spinning sprite shards fly out + fade)

## Wiring & Verification
- [ ] Assign prefab references (EnemySpawner, UIManager, PlayerShooter)
- [ ] Play-mode test: movement, shooting, enemy spawn
- [ ] Play-mode test: damage, recovery flash, lives count
- [ ] Play-mode test: score + multiplier logic
- [ ] Play-mode test: powerup spawn + collection
- [ ] Play-mode test: game over screen + restart
