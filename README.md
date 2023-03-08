# VRKG-CollaborativeExploration
A Virtual Reality multi-user application for Meta Quest Pro to collaboratively import, discuss and interact with a fully 3D representation of a Knowledge Graph

The project is divided in two parts: 
* UnityProject: The VR Application that imports and visualizes the knowledge graphs
* WebApp: The source code of the web application that allow users to populate the public repository with SPARQL query results, that can then be imported by the Virtual Reality Application

## How to use

* Download Unity 2021.3.2f1, including the Android module
* Import the project located at the "UnityProject" folder of this repository
* Attach a Meta Quest to the pc via USB
* Open the Build Settings window
* Switch the build target to Android
* Pick the Meta Quest among the list of devices
* Click "Build and Run"
* Once the process has completed, you can find the application installed on the headset among the "unknown sources" section

## Notes

This repository is an attachment to the Demo submission titled: **VRKG-CollaborativeExploration - Thematic Data-driven Discussions via Knowledge Graphs on the Road of the Social Metaverse** for the ESWC 2023 Conference from the Dipartimento di Informatica - Università di Salerno - Italy

Differences with the APK:
* The "Clouds" graphics profile is not available in this repository because it contains a paid Unity Asset

Made with Unity 2021.3.2f1

Imported packages/assets:
* Mixed Reality Toolkit 2.8 [https://learn.microsoft.com/en-us/windows/mixed-reality/mrtk-unity/mrtk2/?view=mrtkunity-2022-05]
* Oculus XR Plugin 3.0.2 [https://docs.unity3d.com/Packages/com.unity.xr.oculus@3.0/manual/index.html]
* Photon Unity Networking 2.39 [https://doc.photonengine.com/pun/current/getting-started/pun-intro]
* Unity Particle Pack [https://assetstore.unity.com/packages/essentials/tutorial-projects/unity-particle-pack-127325]
* Starfield Skybox [https://assetstore.unity.com/packages/2d/textures-materials/sky/starfield-skybox-92717]
