using System;
using System.Collections.Generic;
using UnityEngine;
using Perkox.Models;

namespace Perkox.Internal
{
    /// <summary>
    /// Hidden persistent GameObject that receives callbacks from native Android & iOS code
    /// via UnitySendMessage and triggers PerkoxSDK C# events.
    /// </summary>
    [AddComponentMenu("")]
    public class PerkoxCallbackReceiver : MonoBehaviour
    {
        private const string GAME_OBJECT_NAME = "PerkoxCallbackReceiver";
        private static PerkoxCallbackReceiver _instance;

        public static PerkoxCallbackReceiver Instance
        {
            get
            {
                if (_instance == null)
                {
                    var existing = GameObject.Find(GAME_OBJECT_NAME);
                    if (existing != null)
                    {
                        _instance = existing.GetComponent<PerkoxCallbackReceiver>();
                    }

                    if (_instance == null)
                    {
                        var go = new GameObject(GAME_OBJECT_NAME);
                        _instance = go.AddComponent<PerkoxCallbackReceiver>();
                        DontDestroyOnLoad(go);
                    }
                }
                return _instance;
            }
        }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// Invoked by native Android/iOS when the Offerwall opens.
        /// </summary>
        public void OnOfferwallOpenedInternal(string message)
        {
            PerkoxSDK.DispatchOfferwallOpened();
        }

        /// <summary>
        /// Invoked by native Android/iOS when the Offerwall is closed.
        /// </summary>
        public void OnOfferwallClosedInternal(string message)
        {
            PerkoxSDK.DispatchOfferwallClosed();
        }

        /// <summary>
        /// Invoked by native Android/iOS when a reward is received (JSON string).
        /// </summary>
        public void OnRewardReceivedInternal(string jsonString)
        {
            try
            {
                var dict = MiniJsonParser.Deserialize(jsonString);
                var reward = new PerkoxReward(dict);
                PerkoxSDK.DispatchRewardReceived(reward);
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[Perkox] Failed to parse reward callback JSON: {ex.Message}");
                PerkoxSDK.DispatchRewardReceived(new PerkoxReward(new Dictionary<string, object>()));
            }
        }

        /// <summary>
        /// Invoked by native Android/iOS when an error occurs during launch.
        /// </summary>
        public void OnOfferwallError(string errorMessage)
        {
            Debug.LogError($"[Perkox] Native Offerwall Error: {errorMessage}");
            PerkoxSDK.DispatchOfferwallError(errorMessage);
        }
    }
}
