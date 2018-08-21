= Industrial Park - Preview =
Please read all of this before attempting to do anything with Industrial Park. File is a button.

Keyboard view controls:

- W, A, S, D: move view forward, left, backward, right
- Shift + (W, S): move view up, down
- Ctrl + (W, A, S, D): rotate view up, left, down, right
- Q, E: decrease interval, increase interval
- R: reset view
- Z: toggle mouse mode*
- F1: display view config. Here you can view and edit a few general setting for the view, such as draw distance and field of view.

Mouse controls:

- Mouse wheel to move forward/backward
- Right click and drag to pan
- Middle click and drag to rotate view

*Mouse mode: similar to a first person camera. The view rotates automatically as you move the mouse. Use the keyboard to move around.


= Options =
- View Config (F1): displays the View Config box.
- No culling (C): toggles backface culling
- Wireframe (F): toggles wireframe mode
- Background color: allows you to choose a new background color for the view
- Level Model: toggles display of JSP level models.
- Object Models: toggles display of some assets which contain XYZ coordinates.

= View Config (F1) =
Press F1 to display the view config window. Here you can set the view's position, rotation and a few other settings.

= Archive Editor =
The archive editor is the main way to edit HIP/HOP archives in Industrial Park. You can open any amount of Archive Editors you want to, and each will have one HIP or HOP file.
If you wish to edit both HIP and HOP files for a level, for example, you can open two Archive Editors each with one of the files.
Also, opening a third one with boot.HIP will allow you to view the objects whose models are contained there (such as spatula, underwear and shiny objects).

* File: yes, File is a button and although some people don't realize it, you have to click it before doing anything.
** Open: choose a HIP/HOP file to open in this Archive Editor.
** Save: saves the currently open file and overwrites it.
** Save As: allows you to pick a new destination to save the file.
** Export Textures: exports all RWTX assets to a folder.
** Export knowlife's TXT: exports some assets to a text format. I don't think you really have a reason to use this as it's a function I made specifically to help knowlife4 with BFBB HD.
** Close: closes this Archive Editor and unloads the HIP/HOP file. This doesn't save the file and doesn't check if there are unsaved changes.
Closing the Archive Editor through the X button will not close it, only hide it.

* Layer: assets in HIP/HOP archives are organized into layers. Each layer has a list of assets and if you're adding new assets you should add them to the appropriate layer.
** Layer Box: this will allow you to pick a layer and view its assets.
** Layer Type: this will allow you to see and edit a layer's type. I don't recommend changing this.
** Add: this will add a new layer to the archive.
** Remove: this will delete the selected layer from the archive along with all its assets.

* Assets: each asset is an individual in-game object with a type and function.
** Show by type: this will allow you to see in the list only assets of a specific type, or all of them.
** Add: shows the new asset dialog, which allows you to import raw data for a new asset.
** Copy: duplicates an asset. The new asset is identical to the previous one, except for the Asset ID, which will be incremented by one. The internal asset ID of the asset's data, if present, will not be changed.
** Remove: deletes the selected asset from the archive.
** View: will try to show you where the selected asset is. Some assets don't have a position in the world and thus this button will do nothing.
** Export raw: allows you to export an asset's raw data to a file.
** Edit: allows you to edit an existing asset's ID, name and other information, and also replace the raw data with a new file.