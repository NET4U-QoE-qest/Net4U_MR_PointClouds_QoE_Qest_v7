Project Name: QoE\_qest\_MR\_Pointclouds
Company: NET4U
Version: 1.7.0

Description:
This Unity project provides a Mixed Reality interface for visualizing and evaluating point clouds in an immersive environment (e.g. Meta Quest Pro).

Main Features:

* Dynamic loading of point clouds from external storage (e.g. internal memory on Android)
* Quality selector (q1, q2, ... q5)
* Distance placement options (1m, 2m, 3m)
* Interactive QoE rating panel with 10-level slider
* Data saving to CSV for evaluation logging

Workflow:

1. Place point clouds in the following folder on the Android device:
   `/sdcard/Android/data/com.NET4U.QoE_qest_MR_Pointclouds/files/PointClouds/{name}/q{quality}/PointClouds/*.ply`
2. Launch the app in VR or desktop mode.
3. Use the dropdown UI to:

   * Select point cloud name
   * Select quality
   * Choose display distance
4. Press "Play" to load and animate the point cloud.
5. Enable the QoE panel via toggle.
6. Submit ratings. They will be saved locally as:
   `valutazioni_qoe.csv` in `Application.persistentDataPath`

Important Scripts:

* `UIManager.cs`: Handles UI and dropdown interactions
* `PointCloudsLoader.cs`: Manages loading from folders
* `PointCloudController.cs`: Instantiates and animates clouds
* `QoESliderManager.cs`: Controls the rating interface

File Exclusion (handled by .gitignore):

* Large raw data: \*.ply, \*.bin
* Build artifacts: \*.apk, \*.exe, \*.zip, Builds/, Library/
* Temporary/demo scenes and metadata

Build Info:

* Tested for Meta Quest Pro (OpenXR backend)
* Scene configured for both desktop and VR play modes

Contact:
Project maintained by NET4U\_QoE\_Qest (GitHub: [NET4U-QoE-qest](https://github.com/NET4U-QoE-qest))

