# FEAther ![Icon](./RhinoCommon/pkg/dist/icon.svg)

A Rhino3D plugin sponsored by [Nano3DTech](https://nano3dtech.com/).

# Table of contents

1. [Installation](#installation)
   1. [Trial](#trial)
   1. [Commercial](#commercial)
1. [How to use](#how-to-use-it)
   1. [Video guide](#video-guide)
1. [Known issues](#known-issues)
1. [Why this plugin](#why-this-plugin)
1. [Commands](#commands)
   1. [`FeatherPrintable` command](#featherprintable-command)
       1. [3D print process](#3d-print-process)
       1. [3D print process analysis](#3d-print-process-analysis)
       1. [Verification by Abaqus](#verification-by-abaqus)
   1. [`FeatherCastable` command](#feathercastable-command)
   1. [`FeatherUsable` command](#featherusable-command)

# Installation

## Trial

If you like to try it, simply visit the plugin [page](https://www.food4rhino.com/en/app/feather). Then log in and push the `install` button in front of latest released version. Or alternatively, do these:

1. Use `_PackageManager` command on Rhino3D commandline.
1. Search for plugin name i.e. `Feather`.
1. The plugin will be listed through the package manager UI.
1. Click the install button.

![Install trial by `_PackageManager` command](RhinoCommon/doc/install-package-manager.svg "Install trial via `_PackageManager` command")

## Commercial

If you like to purchase it, simply do these:

1. Visit the online shop [here](https://www.patreon.com/Megidd/shop).
1. Download the compressed file.
1. Extract the compressed file.
1. Use Rhino3D `PlugInManager` command to lunch the corresponding UI.
1. Click on `Install...` button.
1. Select the `RHP` file inside the extracted folder to install it.

![Install commercial by plugin manager](RhinoCommon/doc/install-plugin-manager.svg "Install commercial by plugin manager")

# How to use it

Once the plugin is installed, its commands can be accessed by typing the plugin name on command line. Start typing `Feather` on Rhino3D command line to auto complete the plugin commands.

![Access commands](RhinoCommon/doc/commands.svg "How to access plugin commands")

## Video guide

The following video demonstrates how to use the plugin commands after installation.

[![Usage guide video](http://img.youtube.com/vi/_UDrNsUkYzo/0.jpg)](https://youtu.be/_UDrNsUkYzo "Usage guide video")

# Known issues

It's still work-in-progress. There are known issues like this: https://github.com/Megidd/Feather/issues/2

# Why this plugin?

The digital designers use 3D software like Rhino. But, they don’t have any engineering knowledge to predict if their designs would be acceptable regarding:

1. Manufacturing.
   1. 3D print process for prototyping.
   1. Casting process for mass production.
1. Consumption.
   1. Usage process under environment loads.

The manufacturing process consists of prototyping and mass production. It requires engineering knowledge to figure out if a digital 3D design would be acceptable. Moreover, predicting the suitability for consumption requires engineering analysis. The 3D designers are digital artists. They are not equipped with the required engineering knowledge to predict the vulnerability of their digital designs. This plugin tries to bridge this gap by providing a convenient tool for digital artists.

# Commands

This plugin offers multiple commands helping with multiple aspects of manufacturing of 3D designs:

1. `FeatherPrintable` command to optimize 3D print process.
   * Making sure your 3D print will be done correctly.
1. `FeatherCastable` command to optimize casting process.
   * Making sure your casting will be done correctly[^1].
1. `FeatherUsable` command to optimize usage process.
   * Making sure your 3D model has maximum strength and minimum weight[^1].

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

## `FeatherCastable` command

This command helps you optimize the 3D model for the casting process[^1].

## `FeatherUsable` command

This command helps you optimize the usability of 3D model[^1]. You can strengthen the model while decreasing its weight. The finite element analysis - FEA - along with topology optimization are the technologies under the hood.

An optimized 3D model will be generated. Some elements of the original 3D model are removed without affecting the required strength.

![Topology optimization]( RhinoCommon/doc/BESO.png "Topology optimization")

When its development is finished, it will provide an alternative for what Frustum does. The figure below is taken from Frustum.

![How Frustum optimizes 3D models]( RhinoCommon/doc/Frustum.webp " How Frustum optimizes 3D models")

[^1]: This command is not available yet. It's being developed.
