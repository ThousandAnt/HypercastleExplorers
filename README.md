# Hypercastle SDK

## What is it?

The Hypercastle Explorers SDK allows for the creation of alternative visualizations of the Terraforms by Mathcastles NFT project in the Unity game engine.

## Unity

In order to run the code in the SDK you need to first install the Unity game engine version 2021.3.0f1 which can be downloaded directly from the [Unity Download Archive](https://unity3d.com/get-unity/download/archive). Later versions of Unity should be compatible but are not currently tested or supported.

## Getting Terraforms Data

The SDK includes code which allows you to query the Terraforms contract running on Ethereum and get back available data. Scraping all 9000+ tokens takes some time so we've provided a convenience snapshot zip file for you to download. It's in a Unity package format so you should be able to simply drag it into the Assets folder of your Unity project and use it.

## Current State

The SDK now includes functionality for querying the contract functions by incorporating [Nethereum](https://github.com/Nethereum/Nethereum), visualizing Terraforms, and parsing SVG data from the downloaded Terraforms for display in Unity. Our team at Thousand Ant is working on creating both 2D and 3D visualizations of this data to demo the capabilities of the SDK which we aim to ship in July of 2022.

## Configurations

In order to query the Ethereum contract and receive up to date data your project must have it's own Infura key saved in a secrets.json file in your Unity project.

* Add in a `secrets.json` to `Assets/SteamingAssets` with your infura key (See the example below)
* `Assembly Version Validate` is turned off due to Nethereum DLL dependencies.

```
{
    "InfuraProjectId": "your_project_key",
    "EtherscanKey": "your_api_key"
}
```

## Sign up for Infura
1. Sign up for Infura here: [https://infura.io/](https://infura.io/)
2. Click "Create New Project"
    - Select Ethereum under the Product Dropdown
    - Name your project
3. Copy the Project ID from the Keys section of the Project page.

## Hypercastle Explorers Tokens and Discord

The SDK is being created alongside it's own NFT art collection which can be found [on OpenSea](https://opensea.io/collection/hypercastle-explorers)

Owners of Hypercastle Explorers NFTs get preview access to new versions of the SDK and access to a private channel in our [Discord server](https://discord.gg/hef6hNUPHH).

## Follow our progress!

We are releasing regular devlog videos on YouTube which explain the SDK and our progress so far. Watch them [here](https://www.youtube.com/playlist?list=PLZ28Hz6y9j_ivXg36fPKJwNcZaR7H1fir)
