# Agave Games Case

A Match-3 style puzzle game developed in Unity, featuring a drag-based linking system where players connect matching tiles by dragging between them.

## Game Mechanics
- Drag-based tile matching system (no swipe mechanics)
- Visual linking between connected tiles
- Matched tiles disappear when a valid chain is created (3+ tiles)
- Score tracking based on matched tile values
- Limited moves gameplay with win/lose conditions

## Folder Structure
- `Assets/Scripts/` — All game logic scripts
  - `Core/` — Core gameplay (Board, Tile, etc.)
  - `Links/` — Link visualization system (LinkManager, LinkObject, LinkVisualizer)
  - `Managers/` — Game state and settings managers
  - `Utilities/` — Helper classes (LineUtils, LineMaterial, WhiteSquareCreator)
  - `UI/` — UI components and screens
- `Assets/Prefabs/` — Prefab assets for game objects
- `Assets/Resources/` — Runtime-loaded resources
- `Assets/Scenes/` — Game scenes
- `Assets/Images/` — Graphics and sprites
- `Assets/Sound/` — Audio assets
- `Assets/Items/` — ScriptableObject item definitions
- `Assets/Plugins/` — Third-party plugins (DOTween)
- `Assets/AddressableAssetsData/` — Addressable assets configuration
- `Assets/Settings/` — Project settings
- `Assets/ScreensPrefabs/` — UI screen prefabs
- `Assets/TextMesh Pro/` — Text rendering assets

## Key Features
- **Link Visualization**: Real-time visual feedback showing connections between tiles
- **Adaptive Links**: Links automatically orient based on direction (horizontal/vertical)
- **Score Animation**: Visual score feedback with floating text
- **Tile Animations**: Smooth animations for tile interactions using DOTween

## Technologies
- Unity
- C#
- DOTween (for animations)
- TextMeshPro (UI text)
- Unity Addressables (asset management)
- Universal Render Pipeline

## Setup Instructions
1. Open the project in Unity 2021 or newer
2. Ensure all packages are properly imported (DOTween, TextMeshPro, Addressables)
3. Build Addressables via `Window > Asset Management > Addressables > Groups > Build > New Build`
4. Open the main scene and press Play

## Performance Considerations
- Uses optimized object pooling for link visualizations
- Efficiently manages animations to prevent memory leaks
- Properly cleans up resources when transitioning between scenes

## Development Notes
This project demonstrates implementation of a drag-based matching system with visual linking between tiles, replacing the traditional swipe mechanics seen in many Match-3 games.

## Features
- Dynamic, scalable game board and tile system
- Drag-and-drop chain creation for matching tiles
- Visual link and animation effects (DOTween)
- Score tracking and end game screens
- Easily customizable items, board size, and win conditions
- Performance-optimized asset loading with Unity Addressables

## Contribution
Fork the repository and submit a pull request for improvements or bug fixes.

## License
This project is for educational and evaluation purposes only.
