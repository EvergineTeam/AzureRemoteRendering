# Evergine.AzureRemoteRendering Addon

[![Build Status](https://waveengineteam.visualstudio.com/Evergine/_apis/build/status/Add-ons/AzureRemoteRendering/ARR%20CI?branchName=master)](https://waveengineteam.visualstudio.com/Evergine/_build/latest?definitionId=123&branchName=master)
[![Nuget](https://img.shields.io/nuget/v/Evergine.AzureRemoteRendering?logo=nuget)](https://www.nuget.org/packages/Evergine.AzureRemoteRendering)

This repository contains the add-on to use [Azure Remote Rendering](https://azure.microsoft.com/es-es/services/remote-rendering) service in Evergine.

It provides a set of components and all necessary native dependencies that ease the development of an application using this product.

## Sample

There is a sample app at `Samples/AzureRemoteRendering_Demo`. Before running it, you need to configure it with your Azure Remote Rendering instance. Go to `AzureRemoteRendering_Demo/MainScene.cs`, line 33 and enter your credentials.

## How to contribute

It is recommended to use the sample to develop the add-on, linking it to the source code. To do so, comment the lines 5-6 of the file _Samples\AzureRemoteRendering_Demo\AzureRemoteRendering_Demo.weproj_, and comment the line 20 and uncomment the lines 25-27 from _Samples\AzureRemoteRendering_Demo\AzureRemoteRendering_Demo\AzureRemoteRendering_Demo.csproj_
