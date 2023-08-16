# Diff Details

Date : 2023-08-15 20:50:54

Directory /Users/teddysiker/Desktop/Coding/GitHub/herbicide/Herbicide

Total : 103 files,  23099 codes, 667 comments, 4467 blanks, all 28233 lines

[Summary](results.md) / [Details](details.md) / [Diff Summary](diff.md) / Diff Details

## Files
| filename | language | code | comment | blank | total |
| :--- | :--- | ---: | ---: | ---: | ---: |
| [Assembly-CSharp.csproj](/Assembly-CSharp.csproj) | XML | 6 | 0 | 0 | 6 |
| [Assets/Levels/level0.json](/Assets/Levels/level0.json) | JSON | 220 | 0 | 0 | 220 |
| [Assets/Levels/level_easy.json](/Assets/Levels/level_easy.json) | JSON | 44 | 0 | 0 | 44 |
| [Assets/Levels/level_hard.json](/Assets/Levels/level_hard.json) | JSON | 148 | 0 | 0 | 148 |
| [Assets/Levels/level_medium.json](/Assets/Levels/level_medium.json) | JSON | 0 | 0 | 1 | 1 |
| [Assets/Scripts/Board/AnimatedTile.cs](/Assets/Scripts/Board/AnimatedTile.cs) | C# | -31 | -20 | -8 | -59 |
| [Assets/Scripts/Board/Flooring.cs](/Assets/Scripts/Board/Flooring.cs) | C# | -196 | -163 | -54 | -413 |
| [Assets/Scripts/Board/GrassTile.cs](/Assets/Scripts/Board/GrassTile.cs) | C# | -35 | -41 | -11 | -87 |
| [Assets/Scripts/Board/ShoreTile.cs](/Assets/Scripts/Board/ShoreTile.cs) | C# | -20 | -24 | -10 | -54 |
| [Assets/Scripts/Board/SoilFlooring.cs](/Assets/Scripts/Board/SoilFlooring.cs) | C# | -30 | -15 | -5 | -50 |
| [Assets/Scripts/Board/Tile.cs](/Assets/Scripts/Board/Tile.cs) | C# | -300 | -268 | -95 | -663 |
| [Assets/Scripts/Board/TileGrid.cs](/Assets/Scripts/Board/TileGrid.cs) | C# | -586 | -377 | -134 | -1,097 |
| [Assets/Scripts/Board/WaterTile.cs](/Assets/Scripts/Board/WaterTile.cs) | C# | -8 | -9 | -3 | -20 |
| [Assets/Scripts/Controllers/CameraController.cs](/Assets/Scripts/Controllers/CameraController.cs) | C# | -29 | -27 | -8 | -64 |
| [Assets/Scripts/Controllers/CanvasController.cs](/Assets/Scripts/Controllers/CanvasController.cs) | C# | -84 | -67 | -23 | -174 |
| [Assets/Scripts/Controllers/DefenderController.cs](/Assets/Scripts/Controllers/DefenderController.cs) | C# | -1 | 0 | 0 | -1 |
| [Assets/Scripts/Controllers/EnemyController.cs](/Assets/Scripts/Controllers/EnemyController.cs) | C# | 7 | 1 | 2 | 10 |
| [Assets/Scripts/Controllers/EnemyManager.cs](/Assets/Scripts/Controllers/EnemyManager.cs) | C# | 74 | 57 | 24 | 155 |
| [Assets/Scripts/Controllers/InventoryController.cs](/Assets/Scripts/Controllers/InventoryController.cs) | C# | 93 | 73 | 25 | 191 |
| [Assets/Scripts/Controllers/JSONController.cs](/Assets/Scripts/Controllers/JSONController.cs) | C# | 7 | 9 | 3 | 19 |
| [Assets/Scripts/Controllers/LevelController.cs](/Assets/Scripts/Controllers/LevelController.cs) | C# | -16 | 21 | -1 | 4 |
| [Assets/Scripts/Controllers/MovingEnemyController.cs](/Assets/Scripts/Controllers/MovingEnemyController.cs) | C# | -8 | -8 | -3 | -19 |
| [Assets/Scripts/Controllers/SoundController.cs](/Assets/Scripts/Controllers/SoundController.cs) | C# | 38 | 28 | 10 | 76 |
| [Assets/Scripts/Controllers/TileGrid.cs](/Assets/Scripts/Controllers/TileGrid.cs) | C# | 522 | 361 | 124 | 1,007 |
| [Assets/Scripts/Enemies/Enemy.cs](/Assets/Scripts/Enemies/Enemy.cs) | C# | -296 | -270 | -86 | -652 |
| [Assets/Scripts/Enemies/EnemyAnimation.cs](/Assets/Scripts/Enemies/EnemyAnimation.cs) | C# | -154 | -119 | -36 | -309 |
| [Assets/Scripts/Enemies/EnemyManager.cs](/Assets/Scripts/Enemies/EnemyManager.cs) | C# | -74 | -59 | -20 | -153 |
| [Assets/Scripts/Enemies/Kudzu.cs](/Assets/Scripts/Enemies/Kudzu.cs) | C# | -117 | -83 | -30 | -230 |
| [Assets/Scripts/Enemies/MovingEnemy.cs](/Assets/Scripts/Enemies/MovingEnemy.cs) | C# | -45 | -29 | -12 | -86 |
| [Assets/Scripts/Enums/Direction.cs](/Assets/Scripts/Enums/Direction.cs) | C# | -7 | -4 | -2 | -13 |
| [Assets/Scripts/Enums/GameState.cs](/Assets/Scripts/Enums/GameState.cs) | C# | -8 | -3 | -1 | -12 |
| [Assets/Scripts/Factories/EdgeFactory.cs](/Assets/Scripts/Factories/EdgeFactory.cs) | C# | 30 | 30 | 10 | 70 |
| [Assets/Scripts/Factories/TileFactory.cs](/Assets/Scripts/Factories/TileFactory.cs) | C# | 32 | 32 | 10 | 74 |
| [Assets/Scripts/Inventory/InventoryController.cs](/Assets/Scripts/Inventory/InventoryController.cs) | C# | -93 | -73 | -25 | -191 |
| [Assets/Scripts/Inventory/InventorySlot.cs](/Assets/Scripts/Inventory/InventorySlot.cs) | C# | -69 | -67 | -22 | -158 |
| [Assets/Scripts/LevelJSON/level_easy.json](/Assets/Scripts/LevelJSON/level_easy.json) | JSON | -22 | 0 | 0 | -22 |
| [Assets/Scripts/LevelJSON/level_hard.json](/Assets/Scripts/LevelJSON/level_hard.json) | JSON | -148 | 0 | 0 | -148 |
| [Assets/Scripts/LevelJSON/level_medium.json](/Assets/Scripts/LevelJSON/level_medium.json) | JSON | 0 | 0 | -1 | -1 |
| [Assets/Scripts/Loading/LayerData.cs](/Assets/Scripts/Loading/LayerData.cs) | C# | 74 | 73 | 20 | 167 |
| [Assets/Scripts/Loading/ObjectData.cs](/Assets/Scripts/Loading/ObjectData.cs) | C# | 66 | 61 | 19 | 146 |
| [Assets/Scripts/Loading/PropertiesData.cs](/Assets/Scripts/Loading/PropertiesData.cs) | C# | 19 | 18 | 5 | 42 |
| [Assets/Scripts/Loading/TiledData.cs](/Assets/Scripts/Loading/TiledData.cs) | C# | 78 | 62 | 17 | 157 |
| [Assets/Scripts/Loading/TilesetData.cs](/Assets/Scripts/Loading/TilesetData.cs) | C# | 27 | 26 | 7 | 60 |
| [Assets/Scripts/Models/AnimatedTile.cs](/Assets/Scripts/Models/AnimatedTile.cs) | C# | 31 | 20 | 8 | 59 |
| [Assets/Scripts/Models/BasicTree.cs](/Assets/Scripts/Models/BasicTree.cs) | C# | 9 | 12 | 4 | 25 |
| [Assets/Scripts/Models/Currency.cs](/Assets/Scripts/Models/Currency.cs) | C# | 39 | 38 | 13 | 90 |
| [Assets/Scripts/Models/Currency/Currency.cs](/Assets/Scripts/Models/Currency/Currency.cs) | C# | -39 | -38 | -13 | -90 |
| [Assets/Scripts/Models/Currency/SeedToken.cs](/Assets/Scripts/Models/Currency/SeedToken.cs) | C# | -7 | -6 | -2 | -15 |
| [Assets/Scripts/Models/Defender.cs](/Assets/Scripts/Models/Defender.cs) | C# | 193 | 215 | 65 | 473 |
| [Assets/Scripts/Models/Direction.cs](/Assets/Scripts/Models/Direction.cs) | C# | 7 | 4 | 2 | 13 |
| [Assets/Scripts/Models/EdgeTile.cs](/Assets/Scripts/Models/EdgeTile.cs) | C# | 11 | 11 | 3 | 25 |
| [Assets/Scripts/Models/Enemy.cs](/Assets/Scripts/Models/Enemy.cs) | C# | 354 | 288 | 91 | 733 |
| [Assets/Scripts/Models/EnemyAnimation.cs](/Assets/Scripts/Models/EnemyAnimation.cs) | C# | 154 | 119 | 36 | 309 |
| [Assets/Scripts/Models/Flooring.cs](/Assets/Scripts/Models/Flooring.cs) | C# | 200 | 166 | 55 | 421 |
| [Assets/Scripts/Models/GameState.cs](/Assets/Scripts/Models/GameState.cs) | C# | 8 | 3 | 1 | 12 |
| [Assets/Scripts/Models/GrassTile.cs](/Assets/Scripts/Models/GrassTile.cs) | C# | 25 | 28 | 8 | 61 |
| [Assets/Scripts/Models/InventorySlot.cs](/Assets/Scripts/Models/InventorySlot.cs) | C# | 69 | 67 | 22 | 158 |
| [Assets/Scripts/Models/Kudzu.cs](/Assets/Scripts/Models/Kudzu.cs) | C# | 117 | 83 | 31 | 231 |
| [Assets/Scripts/Models/Loading/EnemyData.cs](/Assets/Scripts/Models/Loading/EnemyData.cs) | C# | -27 | -26 | -10 | -63 |
| [Assets/Scripts/Models/Loading/LevelData.cs](/Assets/Scripts/Models/Loading/LevelData.cs) | C# | -13 | -10 | -3 | -26 |
| [Assets/Scripts/Models/MovingEnemy.cs](/Assets/Scripts/Models/MovingEnemy.cs) | C# | 45 | 29 | 12 | 86 |
| [Assets/Scripts/Models/PlaceableObject.cs](/Assets/Scripts/Models/PlaceableObject.cs) | C# | 76 | 96 | 25 | 197 |
| [Assets/Scripts/Models/Placeables/Defense/Defenders/Defender.cs](/Assets/Scripts/Models/Placeables/Defense/Defenders/Defender.cs) | C# | -193 | -215 | -65 | -473 |
| [Assets/Scripts/Models/Placeables/Defense/Defenders/Squirrel.cs](/Assets/Scripts/Models/Placeables/Defense/Defenders/Squirrel.cs) | C# | -31 | -33 | -14 | -78 |
| [Assets/Scripts/Models/Placeables/PlaceableObject.cs](/Assets/Scripts/Models/Placeables/PlaceableObject.cs) | C# | -76 | -96 | -25 | -197 |
| [Assets/Scripts/Models/Placeables/Trees/BasicTree.cs](/Assets/Scripts/Models/Placeables/Trees/BasicTree.cs) | C# | -9 | -12 | -4 | -25 |
| [Assets/Scripts/Models/Placeables/Trees/Tree.cs](/Assets/Scripts/Models/Placeables/Trees/Tree.cs) | C# | -228 | -218 | -63 | -509 |
| [Assets/Scripts/Models/SeedToken.cs](/Assets/Scripts/Models/SeedToken.cs) | C# | 7 | 6 | 2 | 15 |
| [Assets/Scripts/Models/ShoreTile.cs](/Assets/Scripts/Models/ShoreTile.cs) | C# | 7 | 7 | 2 | 16 |
| [Assets/Scripts/Models/SoilFlooring.cs](/Assets/Scripts/Models/SoilFlooring.cs) | C# | 30 | 15 | 5 | 50 |
| [Assets/Scripts/Models/Sound.cs](/Assets/Scripts/Models/Sound.cs) | C# | 25 | 27 | 7 | 59 |
| [Assets/Scripts/Models/Squirrel.cs](/Assets/Scripts/Models/Squirrel.cs) | C# | 31 | 33 | 14 | 78 |
| [Assets/Scripts/Models/Tile.cs](/Assets/Scripts/Models/Tile.cs) | C# | 300 | 268 | 95 | 663 |
| [Assets/Scripts/Models/Tree.cs](/Assets/Scripts/Models/Tree.cs) | C# | 228 | 218 | 63 | 509 |
| [Assets/Scripts/Models/WaterTile.cs](/Assets/Scripts/Models/WaterTile.cs) | C# | 8 | 9 | 3 | 20 |
| [Assets/Scripts/Sound/Sound.cs](/Assets/Scripts/Sound/Sound.cs) | C# | -25 | -27 | -7 | -59 |
| [Assets/Scripts/Sound/SoundController.cs](/Assets/Scripts/Sound/SoundController.cs) | C# | -38 | -28 | -10 | -76 |
| [Assets/Scripts/View/CameraController.cs](/Assets/Scripts/View/CameraController.cs) | C# | 36 | 33 | 9 | 78 |
| [Assets/Scripts/View/CanvasController.cs](/Assets/Scripts/View/CanvasController.cs) | C# | 84 | 67 | 23 | 174 |
| [Builds/HerbicideGameplayPrototype.app/Contents/MonoBleedingEdge/etc/mono/2.0/DefaultWsdlHelpGenerator.aspx](/Builds/HerbicideGameplayPrototype.app/Contents/MonoBleedingEdge/etc/mono/2.0/DefaultWsdlHelpGenerator.aspx) | HTML | 1,594 | 28 | 280 | 1,902 |
| [Builds/HerbicideGameplayPrototype.app/Contents/MonoBleedingEdge/etc/mono/4.0/DefaultWsdlHelpGenerator.aspx](/Builds/HerbicideGameplayPrototype.app/Contents/MonoBleedingEdge/etc/mono/4.0/DefaultWsdlHelpGenerator.aspx) | HTML | 1,594 | 28 | 280 | 1,902 |
| [Builds/HerbicideGameplayPrototype.app/Contents/MonoBleedingEdge/etc/mono/4.5/DefaultWsdlHelpGenerator.aspx](/Builds/HerbicideGameplayPrototype.app/Contents/MonoBleedingEdge/etc/mono/4.5/DefaultWsdlHelpGenerator.aspx) | HTML | 1,594 | 28 | 280 | 1,902 |
| [Builds/HerbicideGameplayPrototype.app/Contents/MonoBleedingEdge/etc/mono/browscap.ini](/Builds/HerbicideGameplayPrototype.app/Contents/MonoBleedingEdge/etc/mono/browscap.ini) | Ini | 13,255 | 274 | 3,451 | 16,980 |
| [Builds/HerbicideGameplayPrototype.app/Contents/MonoBleedingEdge/etc/mono/mconfig/config.xml](/Builds/HerbicideGameplayPrototype.app/Contents/MonoBleedingEdge/etc/mono/mconfig/config.xml) | XML | 525 | 30 | 62 | 617 |
| [Builds/HerbicideGameplayPrototype.app/Contents/Resources/Data/RuntimeInitializeOnLoads.json](/Builds/HerbicideGameplayPrototype.app/Contents/Resources/Data/RuntimeInitializeOnLoads.json) | JSON | 1 | 0 | 1 | 2 |
| [Builds/HerbicideGameplayPrototype.app/Contents/Resources/Data/ScriptingAssemblies.json](/Builds/HerbicideGameplayPrototype.app/Contents/Resources/Data/ScriptingAssemblies.json) | JSON | 1 | 0 | 0 | 1 |
| [Builds/HerbicideGameplayPrototype.app/Contents/Resources/Data/StreamingAssets/UnityServicesProjectConfiguration.json](/Builds/HerbicideGameplayPrototype.app/Contents/Resources/Data/StreamingAssets/UnityServicesProjectConfiguration.json) | JSON | 1 | 0 | 0 | 1 |
| [Logs/AssetImportWorker0-prev.log](/Logs/AssetImportWorker0-prev.log) | ApexLog | -6,036 | 0 | -118 | -6,154 |
| [Logs/AssetImportWorker0.log](/Logs/AssetImportWorker0.log) | ApexLog | 7,496 | 0 | 127 | 7,623 |
| [Logs/AssetImportWorker1-prev.log](/Logs/AssetImportWorker1-prev.log) | ApexLog | -5,922 | 0 | -118 | -6,040 |
| [Logs/AssetImportWorker1.log](/Logs/AssetImportWorker1.log) | ApexLog | 7,346 | 0 | 127 | 7,473 |
| [Logs/AssetImportWorker2-prev.log](/Logs/AssetImportWorker2-prev.log) | ApexLog | -5,003 | 0 | -87 | -5,090 |
| [Logs/AssetImportWorker2.log](/Logs/AssetImportWorker2.log) | ApexLog | 5,507 | 0 | 95 | 5,602 |
| [Logs/AssetImportWorker3-prev.log](/Logs/AssetImportWorker3-prev.log) | ApexLog | -4,961 | 0 | -87 | -5,048 |
| [Logs/AssetImportWorker3.log](/Logs/AssetImportWorker3.log) | ApexLog | 5,435 | 0 | 95 | 5,530 |
| [Logs/shadercompiler-AssetImportWorker0.log](/Logs/shadercompiler-AssetImportWorker0.log) | ApexLog | 2 | 0 | 2 | 4 |
| [Tiled/Objects/propertytypes.xml](/Tiled/Objects/propertytypes.xml) | XML | 2 | 0 | 1 | 3 |
| [Tiled/Tilemaps/level0.tmx](/Tiled/Tilemaps/level0.tmx) | XML | 121 | 0 | 1 | 122 |
| [Tiled/Tilesets/Enemies.tsx](/Tiled/Tilesets/Enemies.tsx) | TypeScript JSX | 10 | 0 | 1 | 11 |
| [Tiled/Tilesets/Grass.tsx](/Tiled/Tilesets/Grass.tsx) | TypeScript JSX | 4 | 0 | 1 | 5 |
| [Tiled/Tilesets/Shore.tsx](/Tiled/Tilesets/Shore.tsx) | TypeScript JSX | 9 | 0 | 1 | 10 |
| [Tiled/Tilesets/Soil.tsx](/Tiled/Tilesets/Soil.tsx) | TypeScript JSX | 4 | 0 | 1 | 5 |
| [Tiled/Tilesets/Water.tsx](/Tiled/Tilesets/Water.tsx) | TypeScript JSX | 4 | 0 | 1 | 5 |

[Summary](results.md) / [Details](details.md) / [Diff Summary](diff.md) / Diff Details