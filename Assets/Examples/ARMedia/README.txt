---------------------------------------------------------------------------------------------------------------------------------
ARMedia SDK Unity Plugin v2.1.0. (Examples)- Copyright 2017 - Inglobe Technologies S.r.l.
---------------------------------------------------------------------------------------------------------------------------------

SUMMARY:

 - INTRODUCTION: description of the provided examples
 - USAGE: high level instructions to use the SDK examples
 - BUILD INSTRUCTIONS: requirements and build instructions for iOS and Android examples
 - SUPPORT & HELP: how to get support for the SDK

---------------------------------------------------------------------------------------------------------------------------------

INTRODUCTION:

In this package you will find the following examples:

- Planar Example: image recognition and tracking - an abstract image is tracked and augmented with reference axes
- Maquette Example: object recognition and tracking - in this example a wireframe box is displayed in such a way that 
  it completely contains a maquette.
- Church Example: object recognition and tracking - in this example the facade of a church is augmented with a X-Ray 
  view of a couple of pillars.
- Box Example: object recognition, tracking and interations with the real object (shadows casting and collisions) - in this 
  example a box is tracked, you can tap the device’s screen to throw colored balls onto the ‘real’ box that will receive 
  shadows and will also interact with virtual balls that will bounce on the box’s surface in a realistic manner
- Location Example: location tracking - three locations are used to display simple geometry at each on them
- Motion Example: device motion tracking - reference axes are displayed and tranformed to match the device's orientation
- Multiple Example: multiple object recognition and tracking - in this example models are attached to two different objects which
  can be recognized and tracked at the same time.
 
---------------------------------------------------------------------------------------------------------------------------------

USAGE:

In order to try any of the provided examples just load the corresponding scene, build the app either for iOS or Android
and install it on a real device (see below for build instructions details). 

For the planar tracking example you can aim your device toward the provided image (you can print it or just open it on your 
computer).

For any of the examples demonstrating the object tracking features you should aim the device towards the real object but you
can also open any of the images available in the corresponding Streaming/Assets/ARMedia/ example folder. NOTE: in order to 
get better results you should aim the device toward the real object of course, aiming it to a photograph is just a simplified
way to try the tracker but due to the nature of “object tracking” (as opposed to planar tracking), the augmentation will be
distorted when you move away from the sample image you are looking at, so when you use the provided sample images you should 
try to have the whole image covering the entire device’s screen.

For the location example you should simulate the device's location inside your developing environment or just modify the 
location data in the scene with more suitable coordinates.

For the motion example try to modify both the 'useInitialOrientation' and 'trackCameraMode' parameters to see how they affect
the tracking method.

---------------------------------------------------------------------------------------------------------------------------------

BUILD INSTRUCTIONS:

In order to build any of the provided examples, first of all you must set the app bundle identifier to 
"com.inglobetechnologies.ARMediaSDKUnityExample" and be sure that the same information is used in the AndroidManifest.xml file
if you are building for Android platform.

Finally, before exporting the app, be sure that the Application Key of the scene's ARMediaSDK object is set 
to: 7601C0D351ECE83780736A09EE1F1133A127BC74

If everything is set correctly you can export and deploy the example to your device. NOTE: Review the general instructions 
available in the README.TXT file available in the Scripts/ARMedia folder before proceeding.

---------------------------------------------------------------------------------------------------------------------------------

SUPPORT & HELP

Please visit http://dev.inglobetechnologies.com/helpdesk to get support for the SDK or to ask any question related to the 
examples.
