# Scripts

Two assemblies. `Domain` has no Unity references; `Game` is Unity code that uses it.
Namespaces follow the assembly and feature (`PocketFish.Domain.Fishing`), not the subfolder.

## Domain (pure C#, unit tested)
| Folder | Holds |
|---|---|
| `Catch/Fish` | `FishSpecies`, `Rarity` |
| `Catch/Quality` | `Quality`, `QualityOdds`, the weighted roll (`CatchRoller`), `CatchOutcome` |
| `Fishing/TapMinigame` | The single-tap timing minigame, its config and `TapGrade` |
| `Fishing/MultiTap` | `MultiTapMinigame`: a sequence of taps, scores, average and final grade |
| `Pond` | Pond population and time |
| `Progression` | `PlayerProgress` (coins, aquariums, upgrade levels), `CoinRewardTable` |

## Game (Unity)
| Folder | Holds |
|---|---|
| `Data` | ScriptableObjects: `FishDefinition`, `CatchTuning`, `AquariumDefinition`, `AquariumCatalog`, `EconomyTuning` |
| `Progression` | `ProgressSession`: the player's progress, shared by the Aquarium and Pond scenes |
| `Aquarium` | The lobby: loads the collection for the book and fills the tank with a set number of random fish |
| `Navigation` | `SceneNames`, `SceneButton` |
| `Pond` | Scene controller, session and views for the pond and its fish |
| `UI/Minigame` | Timer bar and timing arc |
| `UI/Reveal` | Catch reveal |
| `Persistence` | Pond save data and store |

Tests mirror `Domain` under `Tests/EditMode`.
