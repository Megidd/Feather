# FEAther ![Icon](./RhinoCommon/pkg/dist/icon.svg)

A Rhino3D plugin sponsored by [Nano3DTech](https://nano3dtech.com/).

# Installation

## Trial

If you like to try it, simply visit the [plugin page](https://www.food4rhino.com/en/app/feather). Then log in and push the `install` button in front of latest released version. Or alternatively, do these:

1. Use `PackageManager` command on Rhino3D commandline.
1. Search for plugin name i.e. `Feather`.
1. The plugin will be listed through the package manager UI.
1. Click the install button.

![Install trial by `PackageManager` command](RhinoCommon/doc/install-package-manager.svg "Install trial via `PackageManager` command")

## Commercial

If you like to purchase it, simply do these:

1. Visit the online shop [here](https://www.patreon.com/Megidd/shop).
1. Download the compressed file.
1. Extract the compressed file.
1. Use Rhino3D `PlugInManager` command to launch the corresponding UI.
1. Click on `Install...` button.
1. Select the `RHP` file inside the extracted folder to install it.

![Install commercial by plugin manager](RhinoCommon/doc/install-plugin-manager.svg "Install commercial by plugin manager")

# How to use it

Once the plugin is installed, its command can be accessed by typing the plugin name on command line. Start typing `Feather` on Rhino3D command line to auto complete the plugin command.

![Access commands](RhinoCommon/doc/commands.svg "How to access plugin command")

## Video guide

The following video demonstrates how to use the plugin command after installation.

[![Usage guide video](http://img.youtube.com/vi/_UDrNsUkYzo/0.jpg)](https://youtu.be/_UDrNsUkYzo "Usage guide video")

# Known issues

It's still work-in-progress. There are known issues like this: https://github.com/Megidd/Feather/issues/2

# Why this plugin?

3D print is simulated by this plugin. The additive manufacturing simulations can help with anticipating whether 3D designs are printable or not. If the stresses are too high, the 3D print would collapse.

# Commands

This plugin offers a `FeatherPrintable` command to simulate the 3D print process. Making sure your 3D print will be done correctly.

## `FeatherPrintable` command

This command helps you with the 3D print workflow by FEA, finite element analysis. It means you can analyze your 3D model layer-by-layer almost the way it's 3D printed slice-by-slice.

### 3D print process

The 3D print process is usually done by a print floor moving up from a resin tank. The 3D model is solidified layer-by-layer and is created upside-down.

![3D print process]( RhinoCommon/doc/3D-print-process.svg "3D print process")

### 3D print process analysis

By `FeatherPrintable` command, at each layer, you will see a graph showing the Von Mises stress throughout the 3D model. You would be able to compare the stress with the ultimate stress of the resin material. This way, you can precisely estimate whether your 3D print process will collapse or not. For example, figure below indicates the analysis for a specific layer.

![3D print process analysis]( RhinoCommon/doc/3D-print-process-analysis.svg "3D print process analysis")

The comparison of analysis for different layers of a sample 3D model is displayed below. The figure shows the 3D model being printed along the `+Z` axis. The only force acting upon the 3D model is gravity in `+Z` direction. It should be noted that 3D print process is usually done by a print floor moving up layer-by-layer while creating the 3D model in an upside-down fasion. The very first layer of 3D model is touching the print floor. So, the first layer is a restrained boundary condition.

![Analysis at different layers]( RhinoCommon/doc/3d-print-process-analysis-compare.svg "Analysis at different layers")

### Verification by Abaqus

The verification is done by Abaqus software. For a single layer, FEAther result is compared with Abaqus one. They are matching. The left side is the FEAther result for a single layer and the right side is the Abaqus one.

![Verification by Abaqus]( RhinoCommon/doc/Abaqus-verification.svg "Verification by Abaqus")
