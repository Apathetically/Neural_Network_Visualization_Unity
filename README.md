# Neural Network Visualization Unity

a simple demo for neural network visualization by unity.

This is a demo written half a year ago, which displays the output of the middle feature layer of the neural network with unity. To a certain extent, it reveals the neural network training process and the learned information. Now it only supports small networks. If you want to support more networks, you can use a plane in unity and then paste the feature maps in the form of textures, which can save the amount of rendering calculations.

# Demo Video
https://www.bilibili.com/video/BV16U4y1h7wN?p=2

# Required Environment

Unity 2019.4.9f1

vscode, python and pytorch

GPU

# Test Method
1. git clone the file
2. open the CNNV use Unity
3. open the CIFAR10 with vscode (python and pytorch enviroment was needed)
4. for Unity, run it and switch viewport to Scene
5. for vscode, activate the pytorch-enviroment and run python ./main.py

like this

<img width="600" height="350" src="https://user-images.githubusercontent.com/37832985/139447708-35ede03e-ac7d-48d0-869e-8f81a2ad5e93.png"/>
