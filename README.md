# Integration of ToLua with Ellan Jiang's GameFramework

## GameFramework

- URL: [https://github.com/EllanJiang/GameFramework](https://github.com/EllanJiang/GameFramework) 

- Commit: dc1ea7ccced00c834437a3f347b5dd72549bf36f

- Modifications:
  - Add necessary game objects and components in GameFramework.unity.
  - Change config files.

## ToLua

- URL: [https://github.com/topameng/tolua](https://github.com/topameng/tolua)

- Commit: ea55728d57473bcf1a36e818f9e81bb1be0a0f25

- Modifications:
  - Exclude LuaJit for now.
  - Remove the examples.
  - Copy Unity 5.x meta files to the right locations.
  - Change path constants in CustomSettings.cs and LuaConst.cs.
  - Add label 'LuaScript' for lua files so that they could be built into assetbundles.
