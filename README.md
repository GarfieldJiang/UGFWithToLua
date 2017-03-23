# Integration of ToLua with Ellan Jiang's GameFramework

## GameFramework

- URL: [GameFramework](https://github.com/GameFramework/GameFramework) 

- Commit: dc1ea7ccced00c834437a3f347b5dd72549bf36f

- Modifications:
  - Add necessary game objects and components in GameFramework.unity.
    - Tick procedures on `Procedure Component` but still select `Procedure Launch` with which to run the project.
    - Select `Package` mode on `Resource Component` so it won't check for resource update/download.
  - Git ignore AssetBundleBuilder.xml since its output directory field depends on your own folder structure.

## ToLua

- URL: [Tolua](https://github.com/topameng/tolua)

- Commit: ea55728d57473bcf1a36e818f9e81bb1be0a0f25

- Modifications:
  - Exclude LuaJit for now.
  - Remove the examples.
  - Copy Unity 5.x meta files to the right locations.
  - Change path constants in CustomSettings.cs and LuaConst.cs. (See comments like `// UGF`)
  - Add label 'LuaScript' for lua files so that they could be built into assetbundles. (Could be done by the editor script `AssetLabelEnsurer.cs`.

## Structure

- Core functionality: `Assets/GameFrameworkExtensions/Lua/Scripts`
  - `LuaComponent` extends `GameFrameworkComponent` and provides features of loading and running Lua scripts.
  - `CustomLuaLoader` hacks ToLua's file reading mechanism so that asset bundles built by Game Framework will work.
- Usage example:
  - `Assets/Scripts/ProcedureXXX.cs`
  - `Assets/LuaScripts/XXX.lua`
- Editor helper scripts: `Assets/Scripts/Editor`

## How to play the example

- Download this repo and open it in Unity (5.3 or newer).
- Create your own AssetBundleBuilder.xml file according to Game Framework examples, especially the output directory for asset bundles.
- When assets are imported and source files are compiled, there will be a `Game` menu showing in the Unity editor. Select it and click `Build AssetBundles` to build lua scripts into asset bundles.
- Copy all `Package` assetbundles of the platform on which you're going to run the project into `Assets/StreamingAssets/`.
- Run the game in the editor or build and run the game on any platform.

## Notes

- If you tick `Editor resource mode` on the `Base Component` of Game Framework, `CustomLuaLoader` reads the Lua files on the disk. Otherwise, the project, when launched, first loads the asset bundles that contain Lua scripts so that `CustomLuaLoader` gets the script contents later from the loaded (and hence cached) texts.

- Currently `.lua` files will be recoginized by Unity as default assets, rather than text assets, so that you cannot load them directly from some asset bundle. So `AssetBundleBuilder.cs` add `.bytes` extension to the file names before building asset bundles and revert them afterwards.
