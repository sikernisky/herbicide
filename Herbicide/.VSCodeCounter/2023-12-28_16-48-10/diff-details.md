# Diff Details

Date : 2023-12-28 16:48:10

Directory /Users/teddysiker/Desktop/Coding/GitHub/herbicide/Herbicide

Total : 90 files,  2388 codes, 473 comments, 201 blanks, all 3062 lines

[Summary](results.md) / [Details](details.md) / [Diff Summary](diff.md) / Diff Details

## Files
| filename | language | code | comment | blank | total |
| :--- | :--- | ---: | ---: | ---: | ---: |
| [Assembly-CSharp.csproj](/Assembly-CSharp.csproj) | XML | -1 | 0 | 0 | -1 |
| [Assets/Levels/level0.json](/Assets/Levels/level0.json) | JSON | -195 | 0 | 0 | -195 |
| [Assets/Levels/level1.json](/Assets/Levels/level1.json) | JSON | 297 | 0 | 0 | 297 |
| [Assets/Scripts/Controllers/BombController.cs](/Assets/Scripts/Controllers/BombController.cs) | C# | -1 | 0 | 0 | -1 |
| [Assets/Scripts/Controllers/ButterflyController.cs](/Assets/Scripts/Controllers/ButterflyController.cs) | C# | -8 | 0 | 0 | -8 |
| [Assets/Scripts/Controllers/CollectableController.cs](/Assets/Scripts/Controllers/CollectableController.cs) | C# | 34 | 41 | 12 | 87 |
| [Assets/Scripts/Controllers/ControllerController.cs](/Assets/Scripts/Controllers/ControllerController.cs) | C# | 56 | -3 | 6 | 59 |
| [Assets/Scripts/Controllers/DefenderController.cs](/Assets/Scripts/Controllers/DefenderController.cs) | C# | 2 | 0 | 1 | 3 |
| [Assets/Scripts/Controllers/DewController.cs](/Assets/Scripts/Controllers/DewController.cs) | C# | 7 | 0 | 2 | 9 |
| [Assets/Scripts/Controllers/EconomyController.cs](/Assets/Scripts/Controllers/EconomyController.cs) | C# | 9 | 7 | 2 | 18 |
| [Assets/Scripts/Controllers/EnemyController.cs](/Assets/Scripts/Controllers/EnemyController.cs) | C# | -4 | -15 | -10 | -29 |
| [Assets/Scripts/Controllers/InputController.cs](/Assets/Scripts/Controllers/InputController.cs) | C# | 4 | 4 | 1 | 9 |
| [Assets/Scripts/Controllers/InventoryController.cs](/Assets/Scripts/Controllers/InventoryController.cs) | C# | -7 | 7 | 0 | 0 |
| [Assets/Scripts/Controllers/KudzuController.cs](/Assets/Scripts/Controllers/KudzuController.cs) | C# | 147 | 65 | 37 | 249 |
| [Assets/Scripts/Controllers/LevelController.cs](/Assets/Scripts/Controllers/LevelController.cs) | C# | -4 | -7 | 2 | -9 |
| [Assets/Scripts/Controllers/MobController.cs](/Assets/Scripts/Controllers/MobController.cs) | C# | 20 | 17 | 5 | 42 |
| [Assets/Scripts/Controllers/ModelController.cs](/Assets/Scripts/Controllers/ModelController.cs) | C# | 13 | 34 | 8 | 55 |
| [Assets/Scripts/Controllers/PlacementController.cs](/Assets/Scripts/Controllers/PlacementController.cs) | C# | -6 | 1 | -1 | -6 |
| [Assets/Scripts/Controllers/ShopBoatController.cs](/Assets/Scripts/Controllers/ShopBoatController.cs) | C# | 75 | 22 | 20 | 117 |
| [Assets/Scripts/Controllers/SquirrelController.cs](/Assets/Scripts/Controllers/SquirrelController.cs) | C# | 6 | 7 | 2 | 15 |
| [Assets/Scripts/Controllers/TreeController.cs](/Assets/Scripts/Controllers/TreeController.cs) | C# | 2 | 0 | 1 | 3 |
| [Assets/Scripts/DataStructures/ModelCounts.cs](/Assets/Scripts/DataStructures/ModelCounts.cs) | C# | 23 | 26 | 6 | 55 |
| [Assets/Scripts/DataStructures/ModelType.cs](/Assets/Scripts/DataStructures/ModelType.cs) | C# | 1 | 0 | 0 | 1 |
| [Assets/Scripts/DataStructures/PathfindingCache.cs](/Assets/Scripts/DataStructures/PathfindingCache.cs) | C# | 47 | 44 | 14 | 105 |
| [Assets/Scripts/DataStructures/ShopModel.cs](/Assets/Scripts/DataStructures/ShopModel.cs) | C# | 0 | 0 | -1 | -1 |
| [Assets/Scripts/Factories/AcornFactory.cs](/Assets/Scripts/Factories/AcornFactory.cs) | C# | 26 | 35 | 12 | 73 |
| [Assets/Scripts/Factories/BasicTreeFactory.cs](/Assets/Scripts/Factories/BasicTreeFactory.cs) | C# | 26 | 35 | 11 | 72 |
| [Assets/Scripts/Factories/BombFactory.cs](/Assets/Scripts/Factories/BombFactory.cs) | C# | 29 | 42 | 13 | 84 |
| [Assets/Scripts/Factories/BombSplatFactory.cs](/Assets/Scripts/Factories/BombSplatFactory.cs) | C# | 26 | 35 | 11 | 72 |
| [Assets/Scripts/Factories/ButterflyFactory.cs](/Assets/Scripts/Factories/ButterflyFactory.cs) | C# | 35 | 55 | 17 | 107 |
| [Assets/Scripts/Factories/CollectableFactory.cs](/Assets/Scripts/Factories/CollectableFactory.cs) | C# | -27 | -19 | -7 | -53 |
| [Assets/Scripts/Factories/DefenderFactory.cs](/Assets/Scripts/Factories/DefenderFactory.cs) | C# | -64 | -62 | -26 | -152 |
| [Assets/Scripts/Factories/DewFactory.cs](/Assets/Scripts/Factories/DewFactory.cs) | C# | 26 | 35 | 11 | 72 |
| [Assets/Scripts/Factories/EnemyFactory.cs](/Assets/Scripts/Factories/EnemyFactory.cs) | C# | -81 | -44 | -17 | -142 |
| [Assets/Scripts/Factories/FlooringFactory.cs](/Assets/Scripts/Factories/FlooringFactory.cs) | C# | 0 | 0 | -1 | -1 |
| [Assets/Scripts/Factories/HazardFactory.cs](/Assets/Scripts/Factories/HazardFactory.cs) | C# | -26 | -18 | -6 | -50 |
| [Assets/Scripts/Factories/KudzuFactory.cs](/Assets/Scripts/Factories/KudzuFactory.cs) | C# | 146 | 160 | 52 | 358 |
| [Assets/Scripts/Factories/NexusFactory.cs](/Assets/Scripts/Factories/NexusFactory.cs) | C# | 25 | 30 | 11 | 66 |
| [Assets/Scripts/Factories/NexusHoleFactory.cs](/Assets/Scripts/Factories/NexusHoleFactory.cs) | C# | 26 | 32 | 11 | 69 |
| [Assets/Scripts/Factories/ProjectileFactory.cs](/Assets/Scripts/Factories/ProjectileFactory.cs) | C# | -34 | -23 | -8 | -65 |
| [Assets/Scripts/Factories/ShopBoatFactory.cs](/Assets/Scripts/Factories/ShopBoatFactory.cs) | C# | 29 | 43 | 13 | 85 |
| [Assets/Scripts/Factories/SquirrelFactory.cs](/Assets/Scripts/Factories/SquirrelFactory.cs) | C# | 72 | 69 | 23 | 164 |
| [Assets/Scripts/Factories/StructureFactory.cs](/Assets/Scripts/Factories/StructureFactory.cs) | C# | -32 | -24 | -11 | -67 |
| [Assets/Scripts/Factories/TreeFactory.cs](/Assets/Scripts/Factories/TreeFactory.cs) | C# | -41 | -43 | -17 | -101 |
| [Assets/Scripts/Loading/ObjectData.cs](/Assets/Scripts/Loading/ObjectData.cs) | C# | 21 | 11 | 4 | 36 |
| [Assets/Scripts/Managers/EnemyManager.cs](/Assets/Scripts/Managers/EnemyManager.cs) | C# | 17 | 9 | 4 | 30 |
| [Assets/Scripts/Managers/ShopManager.cs](/Assets/Scripts/Managers/ShopManager.cs) | C# | 51 | 31 | 13 | 95 |
| [Assets/Scripts/Managers/TileGrid.cs](/Assets/Scripts/Managers/TileGrid.cs) | C# | 71 | 3 | 11 | 85 |
| [Assets/Scripts/Models/Acorn.cs](/Assets/Scripts/Models/Acorn.cs) | C# | 3 | 17 | 4 | 24 |
| [Assets/Scripts/Models/BasicTree.cs](/Assets/Scripts/Models/BasicTree.cs) | C# | 6 | 15 | 3 | 24 |
| [Assets/Scripts/Models/Bomb.cs](/Assets/Scripts/Models/Bomb.cs) | C# | 3 | 17 | 4 | 24 |
| [Assets/Scripts/Models/Butterfly.cs](/Assets/Scripts/Models/Butterfly.cs) | C# | 6 | 12 | 3 | 21 |
| [Assets/Scripts/Models/Collectable.cs](/Assets/Scripts/Models/Collectable.cs) | C# | -5 | -5 | 0 | -10 |
| [Assets/Scripts/Models/Defender.cs](/Assets/Scripts/Models/Defender.cs) | C# | -15 | -9 | -3 | -27 |
| [Assets/Scripts/Models/Dew.cs](/Assets/Scripts/Models/Dew.cs) | C# | 3 | 17 | 4 | 24 |
| [Assets/Scripts/Models/Enemy.cs](/Assets/Scripts/Models/Enemy.cs) | C# | -1 | -3 | -1 | -5 |
| [Assets/Scripts/Models/Flooring.cs](/Assets/Scripts/Models/Flooring.cs) | C# | 4 | 5 | 1 | 10 |
| [Assets/Scripts/Models/GrassTile.cs](/Assets/Scripts/Models/GrassTile.cs) | C# | 1 | 3 | 2 | 6 |
| [Assets/Scripts/Models/Hazard.cs](/Assets/Scripts/Models/Hazard.cs) | C# | -1 | -3 | -1 | -5 |
| [Assets/Scripts/Models/Kudzu.cs](/Assets/Scripts/Models/Kudzu.cs) | C# | 0 | -4 | 0 | -4 |
| [Assets/Scripts/Models/Model.cs](/Assets/Scripts/Models/Model.cs) | C# | 4 | 14 | 4 | 22 |
| [Assets/Scripts/Models/Nexus.cs](/Assets/Scripts/Models/Nexus.cs) | C# | 1 | 3 | 0 | 4 |
| [Assets/Scripts/Models/PlaceableObject.cs](/Assets/Scripts/Models/PlaceableObject.cs) | C# | -2 | -8 | -2 | -12 |
| [Assets/Scripts/Models/Projectile.cs](/Assets/Scripts/Models/Projectile.cs) | C# | -9 | -15 | -3 | -27 |
| [Assets/Scripts/Models/SeedToken.cs](/Assets/Scripts/Models/SeedToken.cs) | C# | -16 | -21 | -5 | -42 |
| [Assets/Scripts/Models/ShopBoat.cs](/Assets/Scripts/Models/ShopBoat.cs) | C# | 32 | 87 | 12 | 131 |
| [Assets/Scripts/Models/ShoreTile.cs](/Assets/Scripts/Models/ShoreTile.cs) | C# | 1 | 3 | 1 | 5 |
| [Assets/Scripts/Models/SoilFlooring.cs](/Assets/Scripts/Models/SoilFlooring.cs) | C# | 1 | 3 | 1 | 5 |
| [Assets/Scripts/Models/Squirrel.cs](/Assets/Scripts/Models/Squirrel.cs) | C# | 6 | 15 | 4 | 25 |
| [Assets/Scripts/Models/Structure.cs](/Assets/Scripts/Models/Structure.cs) | C# | -1 | -3 | -1 | -5 |
| [Assets/Scripts/Models/Tile.cs](/Assets/Scripts/Models/Tile.cs) | C# | 4 | 5 | 1 | 10 |
| [Assets/Scripts/Models/Tree.cs](/Assets/Scripts/Models/Tree.cs) | C# | -23 | -32 | -8 | -63 |
| [Assets/Scripts/Models/WaterTile.cs](/Assets/Scripts/Models/WaterTile.cs) | C# | 1 | 3 | 1 | 5 |
| [Assets/Scripts/Scriptables/CollectableScriptable.cs](/Assets/Scripts/Scriptables/CollectableScriptable.cs) | C# | -18 | -18 | -6 | -42 |
| [Assets/Scripts/Scriptables/DefenderScriptable.cs](/Assets/Scripts/Scriptables/DefenderScriptable.cs) | C# | -89 | -66 | -21 | -176 |
| [Assets/Scripts/Scriptables/EnemyScriptable.cs](/Assets/Scripts/Scriptables/EnemyScriptable.cs) | C# | -234 | -177 | -50 | -461 |
| [Assets/Scripts/Scriptables/HazardScriptable.cs](/Assets/Scripts/Scriptables/HazardScriptable.cs) | C# | -18 | -18 | -5 | -41 |
| [Assets/Scripts/Scriptables/ProjectileScriptable.cs](/Assets/Scripts/Scriptables/ProjectileScriptable.cs) | C# | -21 | -25 | -11 | -57 |
| [Assets/Scripts/Scriptables/ShopScriptable.cs](/Assets/Scripts/Scriptables/ShopScriptable.cs) | C# | 9 | 21 | 6 | 36 |
| [Assets/Scripts/Scriptables/StructureScriptable.cs](/Assets/Scripts/Scriptables/StructureScriptable.cs) | C# | -19 | -18 | -6 | -43 |
| [Assets/Scripts/View/SceneController.cs](/Assets/Scripts/View/SceneController.cs) | C# | 9 | 16 | 6 | 31 |
| [Logs/AssetImportWorker0-prev.log](/Logs/AssetImportWorker0-prev.log) | ApexLog | -3,874 | 0 | -64 | -3,938 |
| [Logs/AssetImportWorker0.log](/Logs/AssetImportWorker0.log) | ApexLog | 4,420 | 0 | 81 | 4,501 |
| [Logs/AssetImportWorker1-prev.log](/Logs/AssetImportWorker1-prev.log) | ApexLog | -3,544 | 0 | -64 | -3,608 |
| [Logs/AssetImportWorker1.log](/Logs/AssetImportWorker1.log) | ApexLog | 4,512 | 0 | 81 | 4,593 |
| [Logs/shadercompiler-AssetImportWorker0.log](/Logs/shadercompiler-AssetImportWorker0.log) | ApexLog | 2 | 0 | 1 | 3 |
| [Tiled/Tilemaps/level0.tmx](/Tiled/Tilemaps/level0.tmx) | XML | -26 | 0 | 0 | -26 |
| [Tiled/Tilemaps/level1.json](/Tiled/Tilemaps/level1.json) | JSON | 316 | 0 | 0 | 316 |
| [Tiled/Tilemaps/level1.tmx](/Tiled/Tilemaps/level1.tmx) | XML | 116 | 0 | 1 | 117 |
| [Tiled/Tilesets/Markers.tsx](/Tiled/Tilesets/Markers.tsx) | TypeScript JSX | 6 | 0 | 0 | 6 |

[Summary](results.md) / [Details](details.md) / [Diff Summary](diff.md) / Diff Details