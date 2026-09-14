using System;
using UnityEngine;
using Perkox.Internal;
using Perkox.Models;

namespace Perkox
{
    /// <summary>
    /// Official Perkox Offerwall SDK for Unity.
    /// Provides cross-platform methods to initialize and display high-converting rewarded offerwalls.
    /// </summary>
    public static class PerkoxSDK
    {
        public const string SDK_VERSION = "1.0.0";

        private static string _appId = "";
        private static string _sdkKey = "";
        private static string _playerId = "";
        private static bool _beta = false;
        private static bool _isInitialized = false;

        private static readonly IPerkoxPlatform _platform;

        // --- C# Events ---
        /// <summary>
        /// Triggered when the Offerwall UI is displayed to the user.
        /// </summary>
        public static event Action OnOfferwallOpened;

        /// <summary>
        /// Triggered when the Offerwall UI is dismissed / closed by the user.
        /// </summary>
        public static event Action OnOfferwallClosed;

        /// <summary>
        /// Triggered when the user successfully completes an offerwall task and earns a reward.
        /// </summary>
        public static event Action<PerkoxReward> OnRewardReceived;

        /// <summary>
        /// Triggered if an error occurs while preparing or showing the Offerwall.
        /// </summary>
        public static event Action<string> OnOfferwallError;

        static PerkoxSDK()
        {
#if UNITY_EDITOR
            _platform = new PerkoxEditor();
#elif UNITY_ANDROID
            _platform = new PerkoxAndroid();
#elif UNITY_IOS
            _platform = new PerkoxIOS();
#else
            _platform = new PerkoxEditor();
#endif
        }

        /// <summary>
        /// Initializes the Perkox SDK with your credentials.
        /// </summary>
        /// <param name="appId">Your Perkox App ID from the dashboard.</param>
        /// <param name="sdkKey">Your Perkox SDK Key from the dashboard.</param>
        /// <param name="playerId">Unique user/player identifier (optional, can be updated later via SetUserId).</param>
        /// <param name="beta">Set to true for testing unapproved apps in sandbox/preview mode.</param>
        public static void Initialize(string appId, string sdkKey, string playerId = "", bool beta = false)
        {
            _appId = (appId ?? "").Trim();
            _sdkKey = (sdkKey ?? "").Trim();
            _playerId = (playerId ?? "").Trim();
            _beta = beta;
            _isInitialized = true;

            // Ensure native callback receiver is mounted in the scene
            var _ = PerkoxCallbackReceiver.Instance;

            _platform.InitSDK(_appId, _sdkKey, _playerId, _beta);
        }

        /// <summary>
        /// Updates the active Player ID / User ID in the SDK.
        /// Call this when a user logs in, changes accounts, or a guest user authenticates.
        /// </summary>
        /// <param name="playerId">Unique player identifier.</param>
        public static void SetUserId(string playerId)
        {
            _playerId = (playerId ?? "").Trim();
            _platform.SetUserId(_playerId);
        }

        /// <summary>
        /// Displays the Offerwall using the credentials provided during Initialize().
        /// </summary>
        /// <param name="playerId">Optional override for the player ID.</param>
        /// <param name="beta">Optional override for beta mode.</param>
        public static void ShowOfferwall(string playerId = "", bool? beta = null)
        {
            if (!_isInitialized && string.IsNullOrEmpty(_appId))
            {
                Debug.LogError("[Perkox SDK] Error: You must call PerkoxSDK.Initialize() before showing the offerwall.");
                return;
            }

            string user = !string.IsNullOrEmpty(playerId) ? playerId.Trim() : _playerId;
            bool useBeta = beta.HasValue ? beta.Value : _beta;

            if (string.IsNullOrEmpty(user))
            {
                Debug.LogWarning("[Perkox SDK] Warning: Player ID is empty. Offers may not attribute properly without a unique user identifier.");
            }

            _platform.ShowOfferwall(_appId, _sdkKey, user, useBeta);
        }

        /// <summary>
        /// Displays the Offerwall with explicit App ID and SDK Key credentials.
        /// </summary>
        public static void ShowOfferwall(string appId, string sdkKey, string playerId, bool beta = false)
        {
            _platform.ShowOfferwall(appId, sdkKey, playerId, beta);
        }

        // --- Getters ---
        public static string GetAppId() => _appId;
        public static string GetSdkKey() => _sdkKey;
        public static string GetPlayerId() => _playerId;
        public static bool IsBeta() => _beta;
        public static bool IsInitialized() => _isInitialized;

        // --- Internal Dispatchers (Called by PerkoxCallbackReceiver) ---
        internal static void DispatchOfferwallOpened()
        {
            try
            {
                OnOfferwallOpened?.Invoke();
            }
            catch (Exception ex)
            {
                Debug.LogError($"[Perkox SDK] Exception in OnOfferwallOpened handler: {ex.Message}");
            }
        }

        internal static void DispatchOfferwallClosed()
        {
            try
            {
                OnOfferwallClosed?.Invoke();
            }
            catch (Exception ex)
            {
                Debug.LogError($"[Perkox SDK] Exception in OnOfferwallClosed handler: {ex.Message}");
            }
        }

        internal static void DispatchRewardReceived(PerkoxReward reward)
        {
            try
            {
                OnRewardReceived?.Invoke(reward);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[Perkox SDK] Exception in OnRewardReceived handler: {ex.Message}");
            }
        }

        internal static void DispatchOfferwallError(string error)
        {
            try
            {
                OnOfferwallError?.Invoke(error);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[Perkox SDK] Exception in OnOfferwallError handler: {ex.Message}");
            }
        }
    }
}
