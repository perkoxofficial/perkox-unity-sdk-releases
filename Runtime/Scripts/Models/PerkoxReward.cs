using System;
using System.Collections.Generic;

namespace Perkox.Models
{
    /// <summary>
    /// Represents reward information dispatched when a user completes an offerwall task.
    /// </summary>
    [Serializable]
    public class PerkoxReward
    {
        /// <summary>
        /// Raw dictionary of parameters received from the native SDK callback.
        /// </summary>
        public Dictionary<string, object> RawData { get; private set; }

        public PerkoxReward(Dictionary<string, object> rawData)
        {
            RawData = rawData ?? new Dictionary<string, object>();
        }

        /// <summary>
        /// Helper to retrieve a string value from the reward payload.
        /// </summary>
        public string GetString(string key, string defaultValue = "")
        {
            if (RawData != null && RawData.TryGetValue(key, out var val) && val != null)
            {
                return val.ToString();
            }
            return defaultValue;
        }

        /// <summary>
        /// Helper to retrieve a double/numeric value from the reward payload.
        /// </summary>
        public double GetDouble(string key, double defaultValue = 0.0)
        {
            if (RawData != null && RawData.TryGetValue(key, out var val) && val != null)
            {
                if (double.TryParse(val.ToString(), out var result))
                {
                    return result;
                }
            }
            return defaultValue;
        }

        public override string ToString()
        {
            return $"PerkoxReward(Data: {RawData?.Count ?? 0} items)";
        }
    }
}
