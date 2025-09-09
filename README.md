# 🎮 Freedom Fighter: For My Brother

[![Unity](https://img.shields.io/badge/Unity-2022%20LTS-blue.svg)](https://unity.com/)
[![Platform](https://img.shields.io/badge/Platform-Windows-lightgrey.svg)](https://www.microsoft.com/en-us/windows)
[![Genre](https://img.shields.io/badge/Genre-2D%20Beat--em--up-red.svg)](https://en.wikipedia.org/wiki/Beat_%27em_up)
[![Status](https://img.shields.io/badge/Status-In%20Development-yellow.svg)](https://github.com/yourusername/freedom-fighter)

> A powerful 2D side-scrolling beat-'em-up that tells a story of resistance, family, and justice through classic arcade-style combat and deeply personal narrative.

## 📖 About the Game

**Freedom Fighter: For My Brother** is a 2D side-scrolling beat-'em-up game that combines classic arcade-style combat mechanics with a deeply personal narrative exploring themes of oppression, loss, and the fight for freedom. The game follows a protagonist whose peaceful life is shattered when settlers attack his village, leading to a tragic loss that transforms his motivation from simple defense to a burning desire for justice.

### 🎯 Current Status: Early Development

This repository contains the **early development version** of Freedom Fighter, which includes:

- ✅ Unity project setup with proper folder structure
- ✅ Main character creation and setup
- ✅ Enemy character variants (unarmed and armed with baseball bat)
- ✅ Character art import (.psb files) and integration
- ✅ Character rigging and bone setup for all characters
- ✅ Character animations for all characters:
  - FreedomFighter (Playable): Walking, Punching, Kicking
  - Settler1 (Enemy): Walking, Bat_swing (Attack)
  - Settler2 (Enemy): Walking, Punching (Attack)
- ✅ Level 1 environment setup (background, platforms, and level structure)
- ✅ Combat system (basic melee implemented: punch/kick with timed damage)
- ✅ Enemy AI and behavior (patrol/chase/attack for Settler1 bat and Settler2 punch)
- 🚧 Collectable health items and enemy placement (next priority)
- ✅ UI (HUD basics: player health text, enemy counter Killed: X/Total)
- 🚧 Audio system (in development)

## 🎮 Game Features

### ⚔️ Combat System

- **Melee Combat**: Punch and kick attacks with different damage and range
- **Enemy Melee**: Settler1 (bat) and Settler2 (punch) use timed hitboxes via animation events
- **Health System**: Player and enemy health; player implements `IDamageable`
- **Damage System**: Enemy hitboxes damage the player on contact windows

### 🎨 Visual Design

- **Art Style**: Cartoonish, symbolic, and emotionally expressive
- **Animation**: Smooth 2D skeletal animation using Unity's 2D Animation system
- **Environment**: Side-scrolling levels with parallax backgrounds
- **Character Design**: Hand-drawn, distinctive character designs

### 🎬 Story & Cutscenes

- **Intro Cutscene**: Establishes the peaceful life and the attack on the village
- **Outro Cutscene**: Reveals the military arrival and the tragic loss of the brother
- **Skip Functionality**: Players can skip cutscenes if desired
- **Emotional Narrative**: A deeply personal story of brotherly love and loss

### 🎵 Audio Design

- **Sound Effects**: Punch impacts, enemy reactions, environmental audio
- **Music**: Atmospheric background music that enhances the emotional tone
- **Voice Acting**: Text-based dialogue with emotional weight

## 🛠️ Technical Features

### 🎮 Controls

- **Movement**: Left/Right movement with smooth acceleration
- **Combat**: Punch (J key) and Kick (K key) with timing-based mechanics
- **Pause**: Escape key for pause menu access

### 🎯 Game Systems

- **Input System**: Unity's new Input System for modern control handling
- **Physics**: 2D physics with Rigidbody2D and Collider2D
- **Camera**: Smooth following camera with level boundaries using Cinemachine
- **UI System**: Canvas-based UI with health bars and menus

### 🎨 Development Tools

- **2D Animation**: Unity's 2D Animation system with bone rigging ✅
- **PSB Workflow**: Photoshop Big files for character animation ✅
- **TextMeshPro**: 3D world-space enemy health text; HUD text ✅
- **Timeline**: For cutscene creation and management (planned)
- **ScriptableObjects**: For enemy stats and game configuration (planned)

## 🚀 Getting Started

### Prerequisites

- **Unity 2022.3 LTS** or later
- **Windows 10/11** (target platform)
- **Git** (for version control)

### Installation

1. **Clone the repository**

   ```bash
   git clone https://github.com/Amer-Abuyaqob/Freedom-Fighter-for-my-brother
   cd Freedom-Fighter-for-my-brother
   ```

2. **Open in Unity**

   - Launch Unity Hub
   - Click "Add" and select the project folder
   - Open the project with Unity 2022.3 LTS

3. **Build and Run**
   - Go to `File > Build Settings`
   - Select `Windows` platform
   - Click `Build and Run`

### 🎮 How to Play (When Complete)

1. **Main Menu**: Click "Start" to begin the game
2. **Intro Cutscene**: Watch the story unfold (or skip with Space)
3. **Level 1**: Fight through waves of settlers
   - **A/D** or **Arrow Keys**: Move left/right
   - **J**: Punch attack
   - **K**: Kick attack
   - **Esc**: Pause menu
4. **Outro Cutscene**: Experience the emotional conclusion

### 🚧 Current Development Status

**Note**: The game is currently in early development. The main character and enemies are created and rigged, with complete animation sets implemented for all characters. Level 1 environment is now complete with backgrounds, main platform, and level structure. The next priorities are adding collectable health items and pre-spawned enemies to the level, followed by combat systems, AI, and gameplay mechanics.

## 🎯 Development Roadmap

### ✅ Completed (Early Development)

- [x] Unity project setup and folder structure
- [x] Main character creation and setup
- [x] Enemy character variants (unarmed and armed with baseball bat)
- [x] Character art import (.psb files) and integration
- [x] Character rigging and bone setup for all characters
- [x] Character animations for all characters:
  - [x] FreedomFighter (Playable): Walking, Punching, Kicking
  - [x] Settler1 (Enemy): Walking, Bat_swing (Attack)
  - [x] Settler2 (Enemy): Walking, Punching (Attack)
- [x] Level 1 environment setup (tilemap, platforms, parallax backgrounds, level bounds)

### 🚧 In Progress

- [ ] Combat system implementation (punch/kick mechanics)
- [ ] Enemy AI and behavior systems
- [ ] Additional character animations (idle, die)
- [ ] Collectable health items placement in Level 1
- [ ] Pre-spawned enemy placement in Level 1
- [ ] UI and menu systems
- [ ] Audio system implementation
- [ ] Cutscene system with Timeline
- [ ] Build configuration for Windows

### 🔮 Future Plans (Post-Level 1)

- **Additional Levels**: More complex environments and enemy types
- **Boss Battles**: Epic confrontations with major antagonists
- **Character Development**: Deeper exploration of the main character's journey
- **Enhanced Combat**: More attack types, special moves, and combo systems
- **Environmental Storytelling**: Levels that tell the story through design
- **Multiplayer**: Cooperative gameplay modes
- **Mobile Port**: Android and iOS versions

## 🤝 Contributing

We welcome contributions to Freedom Fighter! Here's how you can help:

### 🎨 Art & Animation

- Character sprites and animations
- Environment art and tilesets
- Visual effects and UI elements
- Cutscene storyboards

### 🎵 Audio

- Sound effects and music composition
- Voice acting and dialogue
- Audio mixing and implementation

### 💻 Programming

- Bug fixes and performance improvements
- New gameplay features and systems
- UI/UX enhancements
- Platform-specific optimizations

### 📝 Documentation

- Code documentation and comments
- Game design documentation
- Tutorial and guide creation

### How to Contribute

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 🎨 Art Direction

### Visual Style

- **Cartoonish Aesthetic**: Accessible and emotionally expressive
- **Symbolic Elements**: Visual metaphors that enhance the narrative
- **Color Palette**: Thoughtful use of color to convey mood and emotion
- **Character Design**: Distinctive, memorable character designs

### Animation Philosophy

- **Emotional Expression**: Animations that convey character feelings
- **Combat Clarity**: Clear, readable attack animations
- **Environmental Storytelling**: Background elements that support the narrative

## 🎵 Audio Vision

### Music

- **Atmospheric**: Music that enhances the emotional tone
- **Dynamic**: Audio that responds to gameplay situations
- **Memorable**: Themes that players will remember

### Sound Design

- **Impact**: Satisfying combat sound effects
- **Ambience**: Environmental audio that brings the world to life
- **Feedback**: Audio cues that help players understand game state

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

- **Unity Technologies** for the amazing game engine
- **The Game Development Community** for inspiration and support
- **All Contributors** who help make this project possible

## 📞 Contact

- **Project Lead**: [Amer Abuyaqob]
- **Email**: [amer.abuyaqob@gmail.com]

## 🌟 Support the Project

If you enjoy Freedom Fighter and want to support its development:

- ⭐ **Star this repository** to show your support
- 🐛 **Report bugs** and issues
- 💡 **Suggest features** and improvements
- 🔄 **Share with others** who might enjoy the game
- 💖 **Consider contributing** to the development

---

_This project represents a personal journey of storytelling through interactive media, combining classic game design with contemporary themes and emotional depth. Thank you for being part of this journey._

## 🎮 Screenshots

_Screenshots will be added as the project develops_

## 🎥 Trailer

_Gameplay trailer will be added when available_

---

**Made with ❤️ and Unity**
