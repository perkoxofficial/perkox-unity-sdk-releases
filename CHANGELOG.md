# Changelog

All notable changes to the Perkox Unity SDK will be documented in this file.

## [1.0.0] - 2026-09-11

### Added
- Initial release of the official **Perkox Offerwall SDK for Unity**.
- Full cross-platform support for **Android** (Kotlin SDK v2.0.8 via JNI) and **iOS** (Swift SDK XCFramework via native bridge).
- Public C# API:
  - `PerkoxSDK.Initialize(appId, sdkKey, playerId, beta)`
  - `PerkoxSDK.ShowOfferwall(playerId, beta)`
  - `PerkoxSDK.SetUserId(playerId)`
  - C# events: `OnOfferwallOpened`, `OnOfferwallClosed`, `OnRewardReceived`, `OnOfferwallError`
- **Unity Editor Mock Mode**: Safe gameplay testing inside Unity Editor with simulated reward and closure events without crashing.
- **EDM4U Support**: Automated native dependency resolution for Android Gradle (JitPack) and iOS CocoaPods.
- **Offline Binaries**: Bundled fallback `.aar` and `.xcframework` for offline builds.
- Automated Xcode build configuration via `PostProcessBuild` for Swift 5.0 and `-ObjC` linker flags.
