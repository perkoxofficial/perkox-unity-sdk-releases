package com.perkox.unity;

import android.app.Activity;
import android.util.Log;
import com.unity3d.player.UnityPlayer;
import com.perkoxofferwall.sdk.PerkoxOfferwall;
import com.perkoxofferwall.sdk.Offerwall;
import org.json.JSONObject;
import java.util.HashMap;
import java.util.Map;
import kotlin.Unit;
import kotlin.jvm.functions.Function0;
import kotlin.jvm.functions.Function1;

/**
 * PerkoxUnityBridge
 * Android Java bridge for Perkox Unity SDK.
 */
public class PerkoxUnityBridge {
    private static final String TAG = "PerkoxUnity";
    private static final String RECEIVER_OBJECT = "PerkoxCallbackReceiver";

    private static String activeAppId = "";
    private static String activeSdkKey = "";
    private static String activePlayerId = "";
    private static boolean activeBeta = false;

    public static void initSDK(String appId, String sdkKey, String playerId, boolean beta) {
        activeAppId = appId != null ? appId.trim() : "";
        activeSdkKey = sdkKey != null ? sdkKey.trim() : "";
        activePlayerId = playerId != null ? playerId.trim() : "";
        activeBeta = beta;
        Log.d(TAG, "Initialized PerkoxUnityBridge (appId=" + activeAppId + ", beta=" + activeBeta + ")");
    }

    public static void setUserId(String playerId) {
        activePlayerId = playerId != null ? playerId.trim() : "";
        Log.d(TAG, "Updated Player ID to: " + activePlayerId);
    }

    public static void showOfferwall(final Activity activity, String appId, String sdkKey, String playerId, final boolean beta) {
        if (activity == null) {
            Log.e(TAG, "Cannot show offerwall: Activity is null");
            UnityPlayer.UnitySendMessage(RECEIVER_OBJECT, "OnOfferwallError", "Activity is null");
            return;
        }

        final String app = (appId != null && !appId.isEmpty()) ? appId.trim() : activeAppId;
        final String key = (sdkKey != null && !sdkKey.isEmpty()) ? sdkKey.trim() : activeSdkKey;
        final String user = (playerId != null && !playerId.isEmpty()) ? playerId.trim() : activePlayerId;

        if (app.isEmpty() || key.isEmpty()) {
            String err = "Cannot show offerwall: appId and sdkKey must not be empty";
            Log.e(TAG, err);
            UnityPlayer.UnitySendMessage(RECEIVER_OBJECT, "OnOfferwallError", err);
            return;
        }

        activity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                try {
                    Offerwall offerwall = PerkoxOfferwall.INSTANCE.create(app, key, user);

                    offerwall.setOnReward(new Function1<Map<String, ?>, Unit>() {
                        @Override
                        public Unit invoke(Map<String, ?> rewardMap) {
                            try {
                                JSONObject json = new JSONObject(rewardMap != null ? rewardMap : new HashMap<String, Object>());
                                UnityPlayer.UnitySendMessage(RECEIVER_OBJECT, "OnRewardReceivedInternal", json.toString());
                            } catch (Exception ex) {
                                Log.e(TAG, "Error serializing reward callback JSON: " + ex.getMessage());
                                UnityPlayer.UnitySendMessage(RECEIVER_OBJECT, "OnRewardReceivedInternal", "{}");
                            }
                            return Unit.INSTANCE;
                        }
                    });

                    offerwall.setOnClose(new Function0<Unit>() {
                        @Override
                        public Unit invoke() {
                            UnityPlayer.UnitySendMessage(RECEIVER_OBJECT, "OnOfferwallClosedInternal", "");
                            return Unit.INSTANCE;
                        }
                    });

                    offerwall.launch(activity, beta);
                    UnityPlayer.UnitySendMessage(RECEIVER_OBJECT, "OnOfferwallOpenedInternal", "");
                } catch (Throwable t) {
                    String msg = t.getMessage() != null ? t.getMessage() : "Unknown error launching offerwall";
                    Log.e(TAG, "Exception launching offerwall: " + msg, t);
                    UnityPlayer.UnitySendMessage(RECEIVER_OBJECT, "OnOfferwallError", msg);
                }
            }
        });
    }
}
