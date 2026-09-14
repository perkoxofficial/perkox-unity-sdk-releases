namespace Perkox.Internal
{
    /// <summary>
    /// Internal interface implemented by platform-specific bridge classes (Android, iOS, Editor).
    /// </summary>
    internal interface IPerkoxPlatform
    {
        void InitSDK(string appId, string sdkKey, string playerId, bool beta);
        void SetUserId(string playerId);
        void ShowOfferwall(string appId, string sdkKey, string playerId, bool beta);
    }
}
