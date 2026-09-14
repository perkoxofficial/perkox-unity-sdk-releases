# Perkox Offerwall Unity SDK

[![Unity Version](https://img.shields.io/badge/unity-2020.3%2B-blue.svg)](https://unity.com/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)
[![Platforms](https://img.shields.io/badge/platform-Android%20%7C%20iOS%20%7C%20Editor-lightgrey.svg)]()

The official **Perkox Offerwall SDK for Unity** enables game developers to integrate high-converting, rewarded offerwalls into mobile games for Android and iOS.

---

## 🌟 Key Features

* **Single Cross-Platform C# API**: Simple, clean static methods for Unity game developers.
* **Native Android & iOS Power**: Powered by the official Perkox Android SDK (Kotlin) and iOS SDK (Swift/XCFramework).
* **Unity Editor Simulation (Mock Mode)**: Test your game loop, UI, and reward distribution directly inside the Unity Editor without platform errors or crashes.
* **EDM4U & CocoaPods Support**: Seamless automated dependency resolution via Google's External Dependency Manager for Unity.
* **Anti-Fraud & 35+ Standard Signals**: Built-in verification and attribution tracking for maximum publisher revenue protection.

---

## 📦 Installation

### Method 1: Unity Package Manager (UPM via Git URL) - Recommended

1. In Unity, open **Window** > **Package Manager**.
2. Click the **`+`** icon in the top-left corner and select **Add package from git URL...**
3. Enter:
   ```text
   https://github.com/perkoxofficial/perkox-unity-sdk-releases.git
   ```
   *(Or target a specific release tag: `https://github.com/perkoxofficial/perkox-unity-sdk-releases.git#v2.0.0`)*
4. Click **Add**. Unity will automatically download and import the SDK into your project.

### Method 2: `.unitypackage`

1. Download the latest `Perkox-Unity-SDK-v2.0.0.unitypackage` from [Releases](https://github.com/perkoxofficial/perkox-unity-sdk-releases/releases).
2. Drag and drop the `.unitypackage` into your open Unity project (or go to **Assets** > **Import Package** > **Custom Package...**).
3. Click **Import**.

---

## 🚀 Quick Start Guide

### 1. Initialize the SDK

Call `PerkoxSDK.Initialize()` early in your game lifecycle (for example, in your game manager's `Awake()` or `Start()`):

```csharp
using UnityEngine;
using Perkox;
using Perkox.Models;

public class GameManager : MonoBehaviour
{
    [Header("Perkox Credentials")]
    [SerializeField] private string androidAppId = "YOUR_ANDROID_APP_ID";
    [SerializeField] private string androidSdkKey = "YOUR_ANDROID_SDK_KEY";

    [SerializeField] private string iosAppId = "YOUR_IOS_APP_ID";
    [SerializeField] private string iosSdkKey = "YOUR_IOS_SDK_KEY";

    private void Awake()
    {
#if UNITY_IOS
        string appId = iosAppId;
        string sdkKey = iosSdkKey;
#else
        string appId = androidAppId;
        string sdkKey = androidSdkKey;
#endif

        // Initialize with default or guest player ID (optional)
        PerkoxSDK.Initialize(appId, sdkKey, "player_guest_123", beta: false);

        // Register event listeners
        PerkoxSDK.OnOfferwallOpened += HandleOfferwallOpened;
        PerkoxSDK.OnOfferwallClosed += HandleOfferwallClosed;
        PerkoxSDK.OnRewardReceived += HandleRewardReceived;
        PerkoxSDK.OnOfferwallError += HandleOfferwallError;
    }

    private void OnDestroy()
    {
        // Unregister event listeners
        PerkoxSDK.OnOfferwallOpened -= HandleOfferwallOpened;
        PerkoxSDK.OnOfferwallClosed -= HandleOfferwallClosed;
        PerkoxSDK.OnRewardReceived -= HandleRewardReceived;
        PerkoxSDK.OnOfferwallError -= HandleOfferwallError;
    }

    // ... Event callbacks
    private void HandleOfferwallOpened()
    {
        Debug.Log("[Game] Offerwall opened. Pausing audio / game timer.");
    }

    private void HandleOfferwallClosed()
    {
        Debug.Log("[Game] Offerwall closed. Resuming game.");
    }

    private void HandleRewardReceived(PerkoxReward reward)
    {
        // Access raw or typed reward fields
        double payout = reward.GetDouble("payout", 0);
        string currency = reward.GetString("currency", "Coins");

        Debug.Log($"[Game] User earned reward: {payout} {currency}!");
        // Add coins/gems to player balance
    }

    private void HandleOfferwallError(string error)
    {
        Debug.LogError($"[Game] Offerwall Error: {error}");
    }
}
```

### 2. Set User / Player ID

When a user logs in or their authenticated ID changes, update the SDK:

```csharp
PerkoxSDK.SetUserId("player_user_98765");
```

> [!IMPORTANT]
> `playerId` must NOT be empty. Always provide a unique user identifier so rewards are correctly attributed to the player's account.

### 3. Display the Offerwall

Call `PerkoxSDK.ShowOfferwall()` when the player taps a "Free Coins" or "Earn Rewards" button:

```csharp
public void OnOfferwallButtonClicked()
{
    PerkoxSDK.ShowOfferwall();
}
```

---

## 🔒 Critical: Package ID Matching Requirements

The Perkox Offerwall backend validates incoming traffic strictly by `app_id`, `sdk_key`, and `package_id`.

| Platform | Setting in Unity | Requirement |
| :--- | :--- | :--- |
| **Android** | **Player Settings** > **Identification** > **Package Name** | **MUST** match the Android Package ID registered in your [Perkox Dashboard](https://pub.perkox.com). |
| **iOS** | **Player Settings** > **Identification** > **Bundle Identifier** | **MUST** match the iOS Bundle Identifier registered in your [Perkox Dashboard](https://pub.perkox.com). |

> [!WARNING]
> If the Package ID does not match the dashboard app entry, the offerwall will return `Invalid package_id for this offerwall` and show zero offers.

---

## 📱 Platform Configuration

### Android Setup
* **Minimum API Level**: Android 5.0 (API Level 21) or higher.
* **Target API Level**: Android 14 (API Level 34) or latest Google Play requirement.
* **Dependencies**: If using EDM4U, run **Assets** > **External Dependency Manager** > **Android Resolver** > **Resolve** to automatically download native dependencies.

### iOS Setup
* **Target SDK**: iOS 12.0 or higher.
* Unity's build output generates an Xcode project with Swift 5.0 and `-ObjC` linker flags preconfigured by `PerkoxPostProcessBuild.cs`.
* Run `pod install` in the generated Xcode project directory if using CocoaPods.

---

## 💻 Unity Editor Simulation (Mock Mode)

When you hit **Play** inside the Unity Editor:
1. `PerkoxSDK.Initialize()` logs your parameters safely to the Unity Console.
2. Calling `PerkoxSDK.ShowOfferwall()` triggers a simulated lifecycle:
   - Dispatches `OnOfferwallOpened`
   - Dispatches a test `OnRewardReceived` callback (100 Coins)
   - Dispatches `OnOfferwallClosed`
3. No Android JNI or iOS native linking exceptions will occur.

---

## 📄 License

This SDK is distributed under the [MIT License](LICENSE).
For support and dashboard configuration, visit [pub.perkox.com](https://pub.perkox.com) or email `support@perkox.com`.
