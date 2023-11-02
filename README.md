# FEAther ![Icon](./RhinoCommon/pkg/dist/icon.svg)

A Rhino3D plugin sponsored by [Nano3DTech](https://nano3dtech.com/).

# Table of contents

1. [Installation](#installation)
   1. [Trial](#trial)
   1. [Commercial](#commercial)
1. [How to use](#how-to-use-it)
   1. [Video guide](#video-guide)
1. [Known issues](#known-issues)
1. [What it actually does](#what-it-actually-does)
   1. [Optimize 3D print process](#optimize-3d-print-process)
   1. [Optimize design process](#optimize-design-process)

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

# What it actually does

This plugin has two commands helping with two aspects:

1. `FeatherPrintable` command to optimize 3D print process.
   * Making sure your 3D print will be done correctly.
1. `FeatherLighten` command to optimize design process.
   * Making sure your 3D model has maximum strength and minimum weight.
   * This command is not available yet. It's being developed.

## Optimize 3D print process

A plugin command, i.e. `FeatherPrintable`, helps you optimize 3D print workflow by FEA, finite element analysis. It means you can analyze your 3D model layer-by-layer almost the way it's 3D printed slice-by-slice.

At each layer, you will see a graph showing Von Mises stress throught the 3D model. You would be able to compare them with the ultimate stress of the resin material. This way, you can precisely estimate whether your 3D print process will collapse or not.

## Optimize design process

Another plugin command, i.e. `FeatherLighten`, helps you optimize the 3D model design. You can strengthen the model while decreasing its weight. The finite element analysis - FEA - along with topology optimization are used.

An optimized 3D model will be generated. Some elements of the original 3D model are removed without affecting the required strength.
