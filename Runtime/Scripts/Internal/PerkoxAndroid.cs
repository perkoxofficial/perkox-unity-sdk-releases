using System;
using UnityEngine;

namespace Perkox.Internal
{
    /// <summary>
    /// Android platform bridge using JNI and AndroidJavaClass.
    /// Bridges to com.perkox.unity.PerkoxUnityBridge.
    /// </summary>
    internal class PerkoxAndroid : IPerkoxPlatform
    {
        private const string BRIDGE_CLASS_NAME = "com.perkox.unity.PerkoxUnityBridge";
        private const string UNITY_PLAYER_CLASS_NAME = "com.unity3d.player.UnityPlayer";

        public void InitSDK(string appId, string sdkKey, string playerId, bool beta)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            try
            {
                using (var bridgeClass = new AndroidJavaClass(BRIDGE_CLASS_NAME))
                {
                    bridgeClass.CallStatic("initSDK", appId, sdkKey, playerId, beta);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[Perkox SDK] Android JNI InitSDK failed: {ex.Message}");
            }
#endif
        }

        public void SetUserId(string playerId)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            try
            {
                using (var bridgeClass = new AndroidJavaClass(BRIDGE_CLASS_NAME))
                {
                    bridgeClass.CallStatic("setUserId", playerId);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[Perkox SDK] Android JNI SetUserId failed: {ex.Message}");
            }
#endif
        }

        public void ShowOfferwall(string appId, string sdkKey, string playerId, bool beta)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            try
            {
                using (var unityPlayer = new AndroidJavaClass(UNITY_PLAYER_CLASS_NAME))
                using (var currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
                using (var bridgeClass = new AndroidJavaClass(BRIDGE_CLASS_NAME))
                {
                    bridgeClass.CallStatic("showOfferwall", currentActivity, appId, sdkKey, playerId, beta);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[Perkox SDK] Android JNI ShowOfferwall failed: {ex.Message}");
            }
#endif
        }
    }
}
