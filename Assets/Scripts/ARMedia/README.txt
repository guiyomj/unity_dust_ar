---------------------------------------------------------------------------------------------------------------------------------
ARMedia SDK Unity Plugin v2.1.0. - Copyright 2017 - Inglobe Technologies S.r.l.
---------------------------------------------------------------------------------------------------------------------------------

SUMMARY:

 - INTRODUCTION: description of the package content
 - USAGE: high level instructions to use the SDK
 - BUILD INSTRUCTIONS: requirements and build instructions for iOS and Android
 - SUPPORT & HELP: how to get support for the SDK

---------------------------------------------------------------------------------------------------------------------------------

INTRODUCTION:

The ARMedia SDK comes with a set of trackers that will allow you to recognise and track real-life objects (car engines, buildings,
toys, ...), images (posters, blue prints, drawings, ...), locations on Earth or the mobile devices orientation in space. The main 
components of the SDK are the trackers and related targets.

Here follows a brief description of the content of the SDK/Plugin package:

Scripts/ARMedia: this folder contains the main script (ARMediaSDK.cs) required to use the ARMedia SDK in your application, in
order to use any of the provided trackers you are supposed to create an empty GameObject and attach the ARMedia SDK script
to it or you can conveniently just use the provided prefab (see below for details).

Scripts/ARMedia/Trackers: this folder contains all the trackers that can be used in your apps, specifically:

- ARMediaObjectTracker: used to recognise and track generic objects (car engine, buildings, toys, ...)
- ARMediaPlanarTracker: used to recognise and track images (posters, blue prints, magazines, books, ...)
- ARMediaLocationTracker: used to track the user's position on Earth and display geolocated content around him/her
- ARMediaMotionTracker: used to track the mobile device's orientation and display content around the user

Scripts/ARMedia/Targets: this folder contains all the targets types that can be used by any of the above trackers, specifically:

- ARMediaObjectTarget: used to define the object being tracked by the ARMediaObjectTracker
- ARMediaPlanarTracker: used to define the images being tracked by the ARMediaPlanarTracker
- ARMediaLocationTracker: used to define the locations (in terms of GPS coordinates) being tracked by the ARMediaLocationTracker

Note that the above targets are used 'mainly' as visual hint in the Editor to help you place the 'augmented' content relative to 
the tracked targets, the real tracking data is specified elsewhere (see below). You will find a description of relevant data into
each of the above components.

Scripts/ARMedia/Utilities: this folder contains some utilities methods used by other scripts in the SDK.

Resources/ARMedia: this folder contains all resources and prefabs required by the plugin, usually you are not supposed to modify 
anything in this folder. The most important things to get from this folder are the prefabs related to the SDK, trackers and their 
corresponding targets (see below for a description on how to assemble them).

Plugins/Android & Plugins/iOS: these folders contain both Android and iOS plugin's binaries.

StreamingAssets/ARMedia: this folder is supposed to contain the tracking data related to either the object or planar targets you
will use in your app, feel free to organize its content as you prefer but be sure to have all targets' related files into the 
same folder (you can refer to the examples accompanying the SDK in order to have an idea of how to organize your targets). 
The trargets' related files are usually made of a configuration file (XML) a set of images (keyframes) and eventually a pointcloud
(PLY) or a cache file, you typically get this data from the ARMedia SDK Developer Portal (http://dev.inglobetechnologies.com).

---------------------------------------------------------------------------------------------------------------------------------

USAGE:

In order to create an app that uses any of the available trackers you first of all have to:

- set the bundle identifier of your app (in Build Settings -> Player Settings...)
- and, on the ARMedia SDK Developer Portal (http://dev.inglobetechnologies.com), enter the chosen bundle identifier and 
  receive a unique Application Key
  
Then, for each scene where you want to use the SDK with:

- you instantiate the ARMediaSDK prefab and set the Application Key obtained in the previous step
- you instantiate one or more type of trackers you want to use by choosing among any of the available trackers prefabs
- you add the instantiated trackers to the ARMediaSDK object through the 'Trackers Objects' array
- you add (and configure) one or more targets to the scene and add them to the instantiated trackers

In order to see how each tracker (and related targets) is configured please refer to the accompanying examples and to the 
descriptions provided in each script file.

---------------------------------------------------------------------------------------------------------------------------------

BUILD INSTRUCTIONS:

Be sure to use the very latest version of Unity3D (5.4.1f1 and later).

Set a valid bundle identifier for your app get the corresponding Application Key from the ARMedia SDK Developer Portal 
(http://dev.inglobetechnologies.com).

Then choose "OpenGL ES 2.0" as Graphics API, you can do this by opening the "Player Settings..." and in the "Other Settings" 
section pick the correct option (OpenGLES2) from the "Graphics API" dropdown list (Note that OpenGLES2 should be the only entry 
in the list, if others entries are there just remove them). If you miss to do this you may not see the video camera feed.

iOS Specific:

Supported version of Xcode is 7.1 and later, minimum iOS supported version is 7.0. In order to build the app with Xcode, export
the project, open it in Xcode and then you must add the following framework to the project:

- AssetsLibrary.framework

Be sure to add the "Privacy - Camera Usage Description" key to the generated Info.plist file and to disable the Bitcode build 
option in the generated Xcode project.

When you modify something in Unity3D, take care to use the "Append" option to export (actually update) the corresponding Xcode 
project otherwise you will need to repeat the above steps.

To build for 64bit architectures be sure to set the "Scripting Backend to "IL2CPP", you do this by opening the "Player Settings..." 
and locating the "Other Settings" section. Eventually choose "Universal" for the "Architecture" setting.

Android Specific:

Minimum Android supported version is 4.1 (Jelly Bean).

Refer to the "ARMediaSDK_AndroidManifest.xml" file to create a valid manifest for your app (if you do not have to use any custom
feature you can just rename it to "AndroidManifest.xml" and use as is otherwise merge its content with the one you are already 
using).

---------------------------------------------------------------------------------------------------------------------------------

SUPPORT & HELP

Please visit http://dev.inglobetechnologies.com/helpdesk to get support for the SDK.
