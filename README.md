# 3dScene

This repository is part of master thesis titled : Synth2Real : 3D-Furniture Reconstruction in
Ersatz Environment (S2R:3D-FREE)

This repository is coupled with Deep Learning pipeline which can be seen in https://github.com/kartikprabhu20/3dReconstruction

3DScene is a Unity-based pipeline to create synthetic dataset.  As a sample the rooms are imported from SceneNet[1] and furnitures are imported from Pix3D[2]
The GUI of the application as shown in the figure.

<img title="GUI" alt="gui"  src="https://github.com/kartikprabhu20/3dScene/blob/main/readme/GUi.png"/>


*Data Settings:*
The users can select the catagories/classes of the furnitures seperated by commma(,).
The users can also select total images per catagory.

*Path Settings:*
The user has to feed the paths for input which include room and furniture datasets, the path to textures and the destination path.

Texture Directory format:
Texture folder<br>
|_ Furniture1<br>
|_ Furniture2<br>
&nbsp; &nbsp;&nbsp; &nbsp;|_ img1<br>
&nbsp; &nbsp;&nbsp; &nbsp;|_ img2<br>

Room Directory format:
Room folder<br>
|_ roomType1<br>
|_ roomType2<br>
&nbsp; &nbsp;&nbsp; &nbsp;|_ room1.obj<br>
&nbsp; &nbsp;&nbsp; &nbsp;|_ room2.obj<br>
|_room3.obj

Furniture Directory format:
Furniture folder<br>
|_ class1<br>
|_ class2<br>
&nbsp; &nbsp; &nbsp; &nbsp; |_ model_1_folderName<br>
&nbsp; &nbsp; &nbsp; &nbsp;&nbsp; &nbsp; &nbsp; &nbsp;|_ model.obj<br>
&nbsp; &nbsp; &nbsp; &nbsp; |_ model_2_folderName<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;|_ model.obj<br>


*Camera Settings:*
The parameters include minimum height of the camera, and minimum and maximum distance from the target model.

*Light Settings:*
The lights can be randomized with colors and intensity.

*Pipeline Settings:*
The application supports 4 modes:
- Single room
- Multi-threaded single room
- Multi-objects room
- Manual pipeline

References:

[1] John McCormac, Ankur Handa, Stefan Leutenegger, and Andrew J. Davison. SceneNet RGB-D: Can 5M Synthetic Images Beat Generic ImageNet Pre-training on Indoor Segmentation? In Proceedings of the IEEE International Conference on Computer Vision, 2017.

[2] Xingyuan Sun, Jiajun Wu, Xiuming Zhang, Zhoutong Zhang, Chengkai Zhang, Tianfan Xue, Joshua B. Tenenbaum, and William T. Freeman. Pix3D: Dataset and Methods for Single-Image 3D Shape Modeling. In Proceedings of the IEEE Computer Society Conference on Computer Vision and Pattern Recognition, 2018.
