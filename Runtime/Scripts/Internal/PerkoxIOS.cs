using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Perkox.Internal
{
    /// <summary>
    /// iOS platform bridge using C-style P/Invoke ([DllImport("__Internal")]).
    /// Bridges to PerkoxUnityBridge.mm.
    /// </summary>
    internal class PerkoxIOS : IPerkoxPlatform
    {
#if UNITY_IOS && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void _Perkox_InitSDK(string appId, string sdkKey, string playerId, bool beta);

        [DllImport("__Internal")]
        private static extern void _Perkox_SetUserId(string playerId);

        [DllImport("__Internal")]
        private static extern void _Perkox_ShowOfferwall(string appId, string sdkKey, string playerId, bool beta);
#endif

        public void InitSDK(string appId, string sdkKey, string playerId, bool beta)
        {
#if UNITY_IOS && !UNITY_EDITOR
            try
            {
                _Perkox_InitSDK(appId ?? "", sdkKey ?? "", playerId ?? "", beta);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[Perkox SDK] iOS InitSDK failed: {ex.Message}");
            }
#endif
        }

        public void SetUserId(string playerId)
        {
#if UNITY_IOS && !UNITY_EDITOR
            try
            {
                _Perkox_SetUserId(playerId ?? "");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[Perkox SDK] iOS SetUserId failed: {ex.Message}");
            }
#endif
        }

        public void ShowOfferwall(string appId, string sdkKey, string playerId, bool beta)
        {
#if UNITY_IOS && !UNITY_EDITOR
            try
            {
                _Perkox_ShowOfferwall(appId ?? "", sdkKey ?? "", playerId ?? "", beta);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[Perkox SDK] iOS ShowOfferwall failed: {ex.Message}");
            }
#endif
        }
    }
}
