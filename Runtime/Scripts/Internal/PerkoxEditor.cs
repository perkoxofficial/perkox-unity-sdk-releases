using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Perkox.Models;

namespace Perkox.Internal
{
    /// <summary>
    /// Mock implementation of PerkoxPlatform for testing inside the Unity Editor.
    /// Prevents DllNotFoundException and JNI crashes during desktop testing.
    /// </summary>
    internal class PerkoxEditor : IPerkoxPlatform
    {
        private string _appId = "";
        private string _sdkKey = "";
        private string _playerId = "";
        private bool _beta = false;

        public void InitSDK(string appId, string sdkKey, string playerId, bool beta)
        {
            _appId = appId;
            _sdkKey = sdkKey;
            _playerId = playerId;
            _beta = beta;

            Debug.Log($"<color=#4CAF50>[Perkox SDK (Editor Mock)]</color> Initialized with AppId: '{_appId}', SdkKey: '{_sdkKey}', PlayerId: '{_playerId}', Beta: {_beta}");
        }

        public void SetUserId(string playerId)
        {
            _playerId = playerId;
            Debug.Log($"<color=#4CAF50>[Perkox SDK (Editor Mock)]</color> Player ID updated to: '{_playerId}'");
        }

        public void ShowOfferwall(string appId, string sdkKey, string playerId, bool beta)
        {
            string effectiveAppId = string.IsNullOrEmpty(appId) ? _appId : appId;
            string effectiveSdkKey = string.IsNullOrEmpty(sdkKey) ? _sdkKey : sdkKey;
            string effectivePlayerId = string.IsNullOrEmpty(playerId) ? _playerId : playerId;

            Debug.Log($"<color=#4CAF50>[Perkox SDK (Editor Mock)]</color> ShowOfferwall requested for AppId: '{effectiveAppId}', PlayerId: '{effectivePlayerId}'.");

            if (string.IsNullOrEmpty(effectiveAppId) || string.IsNullOrEmpty(effectiveSdkKey))
            {
                Debug.LogError("[Perkox SDK (Editor Mock)] Error: Cannot show offerwall without valid appId and sdkKey.");
                return;
            }

            // Trigger mock callbacks via coroutine to simulate user flow
            var receiver = PerkoxCallbackReceiver.Instance;
            if (receiver != null)
            {
                receiver.StartCoroutine(SimulateOfferwallFlow(receiver));
            }
        }

        private IEnumerator SimulateOfferwallFlow(PerkoxCallbackReceiver receiver)
        {
            yield return new WaitForSeconds(0.2f);
            receiver.OnOfferwallOpenedInternal("mock_open");
            Debug.Log("<color=#4CAF50>[Perkox SDK (Editor Mock)]</color> Offerwall opened event dispatched.");

            yield return new WaitForSeconds(1.5f);
            // Simulate a mock reward
            string mockRewardJson = "{\"payout\":100,\"currency\":\"Coins\",\"transaction_id\":\"mock_tx_12345\"}";
            receiver.OnRewardReceivedInternal(mockRewardJson);
            Debug.Log("<color=#4CAF50>[Perkox SDK (Editor Mock)]</color> Reward event dispatched (100 Coins).");

            yield return new WaitForSeconds(0.5f);
            receiver.OnOfferwallClosedInternal("mock_close");
            Debug.Log("<color=#4CAF50>[Perkox SDK (Editor Mock)]</color> Offerwall closed event dispatched.");
        }
    }
}
