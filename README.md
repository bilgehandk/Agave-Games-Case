# Agave Games Case

🎮 **Agave Games** is a Match-3 style puzzle game developed in Unity, featuring a drag-based linking system where players connect matching tiles by dragging between them.

🎥 Watch the gameplay in action:

[![Gameplay Video](https://img.youtube.com/vi/b-Uj_H_YbBY/0.jpg)](https://youtube.com/shorts/b-Uj_H_YbBY?feature=share)

## 🔧 Game Mechanics

* 🧩 Drag-based tile matching system (no swipe mechanics)
* ✨ Visual linking between connected tiles
* 💥 Matched tiles disappear when a valid chain is created (3+ tiles)
* 🏆 Score tracking based on matched tile values
* ⏳ Limited moves gameplay with win/lose conditions

## 📂 Folder Structure

```
Assets/
|-- Scripts/                # All game logic scripts
|   |-- Core/               # Core gameplay (Board, Tile, etc.)
|   |-- Links/              # Link visualization system (LinkManager, LinkObject, LinkVisualizer)
|   |-- Managers/           # Game state and settings managers
|   |-- Utilities/          # Helper classes (LineUtils, LineMaterial, WhiteSquareCreator)
|   |-- UI/                 # UI components and screens
|-- Prefabs/                # Prefab assets for game objects
|-- Resources/              # Runtime-loaded resources
|-- Scenes/                 # Game scenes
|-- Images/                 # Graphics and sprites
|-- Sound/                  # Audio assets
|-- Items/                  # ScriptableObject item definitions
|-- Plugins/                # Third-party plugins (DOTween)
|-- AddressableAssetsData/  # Addressable assets configuration
|-- Settings/               # Project settings
|-- ScreensPrefabs/         # UI screen prefabs
|-- TextMesh Pro/           # Text rendering assets
```

## 🌟 Key Features

* **🔗 Link Visualization**: Real-time visual feedback showing connections between tiles
* **🌀 Adaptive Links**: Links automatically orient based on direction (horizontal/vertical)
* **💡 Score Animation**: Visual score feedback with floating text
* **🎥 Tile Animations**: Smooth animations for tile interactions using DOTween

## 🛠️ Technologies

* Unity
* C#
* DOTween (for animations)
* TextMeshPro (UI text)
* Unity Addressables (asset management)
* Universal Render Pipeline

## 🚀 Setup Instructions

1. Clone the repository:

   ```bash
   git clone git@github.com:bilgehandk/Agave-Games-Case.git
   ```
2. Open the project in Unity 2021 or newer.
3. Ensure all packages are properly imported (DOTween, TextMeshPro, Addressables).
4. Build Addressables via `Window > Asset Management > Addressables > Groups > Build > New Build`.
5. Open the main scene from the `Scenes` folder and press Play.

## ⚡ Performance Considerations

* Uses optimized object pooling for link visualizations.
* Efficiently manages animations to prevent memory leaks.
* Properly cleans up resources when transitioning between scenes.

## 📝 Development Notes

This project demonstrates implementation of a drag-based matching system with visual linking between tiles, replacing the traditional swipe mechanics seen in many Match-3 games.

## 🎯 Features

* Dynamic, scalable game board and tile system
* Drag-and-drop chain creation for matching tiles
* Visual link and animation effects (DOTween)
* Score tracking and end game screens
* Easily customizable items, board size, and win conditions
* Performance-optimized asset loading with Unity Addressables

## 🤝 Contribution

Contributions are welcome! Follow these steps to contribute:

1. Fork the repository.
2. Create a new branch for your feature or fix.
3. Commit your changes with clear and descriptive messages.
4. Submit a pull request for review.

## 📜 License

This project is intended for educational and evaluation purposes only.

---

🎥 Watch the gameplay in action:

[![Gameplay Video](https://img.youtube.com/vi/b-Uj_H_YbBY/0.jpg)](https://youtube.com/shorts/b-Uj_H_YbBY?feature=share)

Start your puzzle-solving journey and contribute to improving Agave Games! If you have any questions or feedback, feel free to reach out. 🚀
