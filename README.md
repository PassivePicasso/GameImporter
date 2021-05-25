# Unity Game Importer for ThunderKit

This tool utilizes a modified version of [uTinyRipper](https://github.com/mafaca/UtinyRipper) to import game assets that continue to target game assemblies.

This allows you to easily setup a project for learning about the tools you're working with when making mods for games with modding support.

to install this package add the following to your ProjectRoot/Packages/manifest.json
``` "com.passivepicasso.unitygameimporter":"https://github.com/PassivePicasso/GameImporter.git", ```

you will see a large number of warnings when installing this project, I'll likely be simply removing most of the TODO items for the purposes of this project.

This tool requires ThunderKit is installed in the project, go to https://github.com/PassivePicasso/ThunderKit to get started with ThunderKit in Unity.

Configure what you want to export from the game from the ThunderKit Settings window.

Make sure you Locate the Game using ThunderKit and have the games assemblies in your Packages folder before importing assets or the import will fail

Use the Main Menu/Tools/Game Asset Importer to begin importing assets from the configured game.
