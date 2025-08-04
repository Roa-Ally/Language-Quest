# Language Quest

A narrative-driven 2D adventure game that explores the power of language, memory, and cultural preservation through the lens of a young protagonist's journey into a mystical forest.

## Game Overview

The game follows a young woman who discovers a weathered book in her late grandmother's cabin. This book contains fragments of an ancient story written in both Spanish and an older, nearly forgotten language. The story tells of Chullachaqui, a forest guardian spirit with one twisted foot, and a forest where words hold magical power.

## The Story

The protagonist's childhood forest is dying, not from natural causes, but from forgetting. The world tree, buried deep in the ancient wood, is withering as the forest's language fades from memory. The player must journey through forgotten paths, recover lost story fragments, and relive the tale of Chullachaqui to restore what was broken.

## Core Gameplay Features

### Narrative Exploration

- Players explore four distinct "Art Maps" (scenes) filled with story fragments and dialogue

### Bilingual Experience

- All text can be switched between Spanish and English using a language toggle button

### Interactive Storytelling

Three main interaction types:

- **Dialogue System**: Conversations with forest spirits and characters  
- **Fragment Collection**: Discovering and collecting story pieces scattered throughout the forest  
- **Retelling Puzzles**: Drag-and-drop puzzles where players reconstruct story phrases in the correct order

### Thought Bubbles

- Player's internal monologue appears as thought bubbles, providing context and reflection

### Environmental Storytelling

- Visual elements and animations that respond to story progression

## Technical Features

- Unity 2D with custom sprite animations  
- Scene-based progression from Main Menu to Intro to Art Maps 1–4  
- Modular dialogue system with branching conversations  
- Language switching for all UI elements (dialogue, fragments, puzzles)  
- Puzzle completion triggers that unlock environmental changes  
- Smooth scene transitions with fade effects

## Unique Elements

- **Cultural Preservation Theme**: Explores how language and stories connect us to the earth  
- **Bilingual Accessibility**: Seamless switching between Spanish and English  
- **Environmental Consequences**: The forest's health is tied to story completion  
- **Mystical Realism**: Blends realistic forest settings with supernatural elements

## Educational Value

The game serves as both entertainment and a meditation on:

- The importance of preserving cultural stories  
- The relationship between language and identity  
- Environmental stewardship through narrative  
- Bilingual literacy and cultural appreciation

## Game Structure

Language Quest is essentially a narrative puzzle game that uses the power of storytelling to explore themes of cultural memory, environmental consciousness, and the magical connection between words and the natural world.

## Development

Built in Unity with C# scripting, featuring:

- Custom dialogue and fragment systems  
- Bilingual text management  
- Interactive puzzle mechanics  
- Scene management and transitions  
- Player movement and animation systems

## Controls

- **Movement**: Arrow keys or WASD  
- **Interact**: Enter or mouse click  
- **Skip Intro**: Space bar  
- **Language Toggle**: UI button to switch between Spanish and English  
- **Puzzle Interaction**: Drag and drop interface for story reconstruction

## Directory Structure

### Core Game Files
Language-Quest/
├── Assets/                          # Main Unity assets directory
│   ├── Art/                        # Visual assets
│   │   ├── Animations/             # Character and object animations
│   │   ├── Background/             # Scene background images
│   │   ├── MC/                     # Main character sprites
│   │   ├── TileMap/                # Tile-based map assets
│   │   └── Tree/                   # Environmental assets
│   ├── Audio/                      # Sound and music files
│   │   └── Music/                  # Background music and SFX
│   ├── Dialogues/                  # Story content
│   │   ├── Act 1/                  # First act dialogue files
│   │   ├── Act 2/                  # Second act dialogue files
│   │   ├── Act 3/                  # Third act dialogue files
│   │   └── Act 4/                  # Fourth act dialogue files
│   ├── Prefabs/                    # Reusable game objects
│   ├── Resources/                  # Runtime-loaded assets
│   ├── Scenes/                     # Unity scene files
│   │   ├── MainMenu.unity          # Main menu scene
│   │   ├── Intro scene.unity       # Introduction scene
│   │   ├── Art Map 1.unity         # First forest zone
│   │   ├── Art Map 2.unity         # Second forest zone
│   │   ├── Art Map 3.unity         # Third forest zone
│   │   └── Art Map 4.unity         # Final forest zone
│   └── Scripts/                    # C# source code
│       ├── Background/             # Background management scripts
│       ├── DialogueSystem/         # Dialogue and conversation scripts
│       ├── Player/                 # Player movement and animation
│       ├── WordGameScripts/        # Puzzle and game mechanics
│       └── Music/                  # Audio management scripts
├── Library/                        # Unity library files (auto-generated)
├── Logs/                           # Unity log files
├── Packages/                       # Unity package dependencies
├── ProjectSettings/                # Unity project configuration
├── Temp/                           # Temporary Unity files
└── UserSettings/                   # User-specific Unity settings


### Source Code Files

#### Core Game Systems

- `SimpleLanguageButton.cs` - Bilingual language switching system  
- `InventoryManager.cs` - Fragment collection and inventory management  
- `SceneTransitionManager.cs` - Scene loading and transition effects  
- `PlayerMovement.cs` - Player character movement controls  
- `PlayerAnimations.cs` - Character animation management

#### Dialogue System

- `DialogueManager.cs` - Main dialogue system controller  
- `DialogueTrigger.cs` - Dialogue activation triggers  
- `FragmentDisplayManager.cs` - Story fragment display system  
- `RetellingPuzzleManager.cs` - Puzzle mechanics and validation

#### Audio & Visual

- `MusicManager.cs` - Background music and audio management  
- `BackgroundController.cs` - Environmental background changes  
- `SpriteSwap.cs` - Visual asset switching

#### UI & Interaction

- `ThoughtBubbleManager.cs` - Internal monologue system  
- `PuzzleGameManager.cs` - Vocabulary puzzle mechanics  
- `ButtonTest.cs` - UI interaction testing

### Deployable Files

The game can be built into executable files using Unity's Build Settings:

- **Target Platform**: PC, Mac & Linux Standalone  
- **Build Location**: `Builds/` directory (not included in repository)  
- **Required Files**: All `Assets`, `Scenes`, and `Scripts` directories

## Installation Guide

### Prerequisites

- Unity Hub v3.12.1 or later  
- Unity Editor version 2022.3.12f1 LTS  
- Git (for cloning repository)

### Minimum System Requirements

- 2 GB RAM  
- 1 GB free disk space  
- Windows 10/11, macOS 10.15+, or Linux

### Step-by-Step Installation

1. **Install Unity Hub**  
   - Download Unity Hub from unity.com  
   - Run the installer and follow the setup wizard  
   - Open Unity Hub and sign in with your Unity account  

2. **Install Unity Editor**  
   - In Unity Hub, go to the "Installs" tab  
   - Click "Install Editor"  
   - Select version 2022.3.12f1 from the "Archive" tab  
   - Ensure "2D Game Support" is checked  
   - Click "Install" and wait for completion  

3. **Clone the Repository**  
   - git clone https://github.com/Roa-Ally/Language-Quest.git

4. **Open Project in Unity**  
   - Open Unity Hub  
   - Go to the "Projects" tab  
   - Click "Open"  
   - Navigate to and select the `Language-Quest` folder  
   - Wait for Unity to import all assets  

5. **Install Dependencies**  
   - In Unity, go to `Window > Package Manager`  
   - Ensure the following packages are installed:  
     - TextMeshPro  
     - Unity UI  
     - 2D Sprite  
     - Audio  

6. **Configure Project Settings**  
   - Go to `Edit > Project Settings`  
   - In "Player" settings, ensure:  
     - **Company Name**: Your team name  
     - **Product Name**: Language Quest  
   - In "Audio" settings, verify audio sample rate is set to 48000 Hz  

7. **Test the Installation**  
   - In the Project window, navigate to `Assets > Scenes`  
   - Double-click `MainMenu.unity` to open the main menu scene  
   - Click the Play button in Unity Editor  
   - Verify the game launches and all systems work correctly  

## Configuration Files

No additional configuration files need to be modified. The project is self-contained and ready to run after installation.

## User Manual

### Main Menu

- **Start Button**: Begins the game and loads the intro scene  
- **Credits Button**: Displays team member information  
- **Quit Button**: Exits the application

### In-Game Controls

- **Movement**: Arrow keys or A/D keys to move left/right  
- **Inventory**: Press E to open/close inventory panel  
- **Dialogue**: Spacebar or left mouse click to advance  
- **Pause**: Esc key to open pause menu  
- **Language Toggle**: Click the language button to switch between Spanish/English

### Game Systems

#### Fragment Collection

- Explore the environment to find glowing story fragments  
- Each fragment contains bilingual text (Spanish/English)  
- Collected fragments are stored in your inventory  
- Use navigation arrows in inventory to browse fragments

#### Dialogue System

- Interact with characters and objects to start conversations  
- Some dialogues offer multiple choice responses  
- Use the language toggle to switch between Spanish and English  
- Dialogue choices may affect story progression

#### Retelling Puzzles

- Drag and drop words to reconstruct story phrases  
- Puzzles require Spanish vocabulary from collected fragments  
- Receive immediate feedback on correct/incorrect answers  
- Complete all puzzles to advance the story

#### Inventory Management

- Press E to access your inventory  
- View fragment collection progress  
- Track puzzle completion status  
- Navigate through collected story pieces

### Game Progression

- **Intro Scene**: Learn about the story and setting  
- **Act 1 (Green Forest)**: Begin fragment collection and meet forest spirits  
- **Act 2 (Blue Forest)**: Continue story progression and solve puzzles  
- **Act 3 (Purple Forest)**: Face more complex challenges  
- **Act 4 (Final Scene)**: Complete the final retelling and restore the forest

## Troubleshooting

- **Game won't start**: Ensure Unity Editor version 2022.3.12f1 is installed  
- **Missing assets**: Re-import the project in Unity (`Assets > Reimport All`)  
- **Audio issues**: Check system audio settings and Unity Audio preferences  
- **Performance issues**: Close other applications to free up system resources

## Development Notes

- Built with Unity 2022.3.12f1 LTS  
- Uses C# for all scripting  
- Modular architecture allows for easy expansion  
- All assets are original or properly licensed  
- Version controlled with Git

## Team Information

- **Project Duration**: 12 weeks  
- **Team Size**: 4 members  
- **Development Tools**: Unity, C#, Clip Studio Paint, GitHub  
- **Target Platform**: PC Standalone (Windows/Mac/Linux)  

For additional support or questions, please refer to the project documentation or contact the development team.
