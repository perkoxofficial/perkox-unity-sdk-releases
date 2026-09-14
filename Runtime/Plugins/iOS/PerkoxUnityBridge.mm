#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>

#if defined(__cplusplus)
extern "C" {
#endif

// Unity extern function
void UnitySendMessage(const char* obj, const char* method, const char* msg);

void _Perkox_InitSDK(const char* appId, const char* sdkKey, const char* playerId, bool beta) {
    NSString* appStr = appId != NULL ? [NSString stringWithUTF8String:appId] : @"";
    NSString* keyStr = sdkKey != NULL ? [NSString stringWithUTF8String:sdkKey] : @"";
    NSString* userStr = playerId != NULL ? [NSString stringWithUTF8String:playerId] : @"";

    Class bridgeClass = NSClassFromString(@"PerkoxUnityBridge");
    if (bridgeClass == nil) {
        // Swift classes in frameworks might be namespaced
        bridgeClass = NSClassFromString(@"Perkox.PerkoxUnityBridge");
    }

    if (bridgeClass != nil && [bridgeClass respondsToSelector:@selector(initSDK:sdkKey:playerId:beta:)]) {
        [bridgeClass performSelector:@selector(initSDK:sdkKey:playerId:beta:)
                          withObject:appStr
                          withObject:keyStr];
    }
}

void _Perkox_SetUserId(const char* playerId) {
    NSString* userStr = playerId != NULL ? [NSString stringWithUTF8String:playerId] : @"";

    Class bridgeClass = NSClassFromString(@"PerkoxUnityBridge");
    if (bridgeClass == nil) {
        bridgeClass = NSClassFromString(@"Perkox.PerkoxUnityBridge");
    }

    if (bridgeClass != nil && [bridgeClass respondsToSelector:@selector(setUserId:)]) {
        [bridgeClass performSelector:@selector(setUserId:) withObject:userStr];
    }
}

void _Perkox_ShowOfferwall(const char* appId, const char* sdkKey, const char* playerId, bool beta) {
    NSString* appStr = appId != NULL ? [NSString stringWithUTF8String:appId] : @"";
    NSString* keyStr = sdkKey != NULL ? [NSString stringWithUTF8String:sdkKey] : @"";
    NSString* userStr = playerId != NULL ? [NSString stringWithUTF8String:playerId] : @"";

    Class bridgeClass = NSClassFromString(@"PerkoxUnityBridge");
    if (bridgeClass == nil) {
        bridgeClass = NSClassFromString(@"Perkox.PerkoxUnityBridge");
    }

    if (bridgeClass != nil) {
        SEL selector = @selector(showOfferwall:sdkKey:playerId:beta:);
        NSMethodSignature* signature = [bridgeClass methodSignatureForSelector:selector];
        if (signature != nil) {
            NSInvocation* invocation = [NSInvocation invocationWithMethodSignature:signature];
            [invocation setTarget:bridgeClass];
            [invocation setSelector:selector];
            [invocation setArgument:&appStr atIndex:2];
            [invocation setArgument:&keyStr atIndex:3];
            [invocation setArgument:&userStr atIndex:4];
            [invocation setArgument:&beta atIndex:5];
            [invocation invoke];
            return;
        }
    }

    // Fallback: Notify Unity if bridge class was not loaded
    UnitySendMessage("PerkoxCallbackReceiver", "OnOfferwallError", "PerkoxUnityBridge class not found in iOS build");
}

#if defined(__cplusplus)
}
#endif
