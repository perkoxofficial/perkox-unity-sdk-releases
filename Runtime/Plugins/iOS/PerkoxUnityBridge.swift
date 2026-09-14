import Foundation
import UIKit

#if canImport(PerkoxOfferwall)
import PerkoxOfferwall
#endif

// Forward declaration of UnitySendMessage from Unity runtime
@_silgen_name("UnitySendMessage")
func UnitySendMessage(_ obj: UnsafePointer<CChar>?, _ method: UnsafePointer<CChar>?, _ msg: UnsafePointer<CChar>?)

@objc public class PerkoxUnityBridge: NSObject {
    private static var activeAppId: String = ""
    private static var activeSdkKey: String = ""
    private static var activePlayerId: String = ""
    private static var activeBeta: Bool = false

    private static let receiverObject = "PerkoxCallbackReceiver"

    private static func sendToUnity(method: String, message: String) {
        receiverObject.withCString { objPtr in
            method.withCString { methodPtr in
                message.withCString { msgPtr in
                    UnitySendMessage(objPtr, methodPtr, msgPtr)
                }
            }
        }
    }

    @objc public static func initSDK(_ appId: String, sdkKey: String, playerId: String, beta: Bool) {
        activeAppId = appId.trimmingCharacters(in: .whitespacesAndNewlines)
        activeSdkKey = sdkKey.trimmingCharacters(in: .whitespacesAndNewlines)
        activePlayerId = playerId.trimmingCharacters(in: .whitespacesAndNewlines)
        activeBeta = beta
    }

    @objc public static func setUserId(_ playerId: String) {
        activePlayerId = playerId.trimmingCharacters(in: .whitespacesAndNewlines)
    }

    @objc public static func showOfferwall(_ appId: String, sdkKey: String, playerId: String, beta: Bool) {
        DispatchQueue.main.async {
            guard let topVC = getTopViewController() else {
                sendToUnity(method: "OnOfferwallError", message: "Unable to find top UIViewController to present offerwall")
                return
            }

            #if canImport(PerkoxOfferwall)
            let app = !appId.isEmpty ? appId.trimmingCharacters(in: .whitespacesAndNewlines) : activeAppId
            let key = !sdkKey.isEmpty ? sdkKey.trimmingCharacters(in: .whitespacesAndNewlines) : activeSdkKey
            let user = !playerId.isEmpty ? playerId.trimmingCharacters(in: .whitespacesAndNewlines) : activePlayerId

            if app.isEmpty || key.isEmpty {
                sendToUnity(method: "OnOfferwallError", message: "Cannot show offerwall: appId and sdkKey must not be empty")
                return
            }

            let offerwall = PerkoxOfferwall.create(appId: app, sdkKey: key, playerId: user)

            offerwall.onReward = { reward in
                do {
                    var sanitized: [String: Any] = [:]
                    for (k, v) in reward {
                        if let val = v {
                            sanitized[k] = val
                        }
                    }
                    let jsonData = try JSONSerialization.data(withJSONObject: sanitized, options: [])
                    if let jsonString = String(data: jsonData, encoding: .utf8) {
                        sendToUnity(method: "OnRewardReceivedInternal", message: jsonString)
                    }
                } catch {
                    sendToUnity(method: "OnRewardReceivedInternal", message: "{}")
                }
            }

            offerwall.onClose = {
                sendToUnity(method: "OnOfferwallClosedInternal", message: "")
            }

            offerwall.launch(viewController: topVC, beta: beta)
            sendToUnity(method: "OnOfferwallOpenedInternal", message: "")
            #else
            sendToUnity(method: "OnOfferwallError", message: "PerkoxOfferwall framework is not linked in Unity iOS build")
            #endif
        }
    }

    private static func getTopViewController(from rootVC: UIViewController? = nil) -> UIViewController? {
        let root = rootVC ?? {
            if #available(iOS 13.0, *) {
                return UIApplication.shared.connectedScenes
                    .compactMap { $0 as? UIWindowScene }
                    .flatMap { $0.windows }
                    .first(where: { $0.isKeyWindow })?.rootViewController
            } else {
                return UIApplication.shared.keyWindow?.rootViewController
            }
        }()

        if let nav = root as? UINavigationController {
            return getTopViewController(from: nav.visibleViewController)
        }
        if let tab = root as? UITabBarController {
            return getTopViewController(from: tab.selectedViewController)
        }
        if let presented = root?.presentedViewController {
            return getTopViewController(from: presented)
        }
        return root
    }
}
