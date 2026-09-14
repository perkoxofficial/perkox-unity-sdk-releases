using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#endif

namespace Perkox.Editor
{
    /// <summary>
    /// PostProcessBuild hook to configure Xcode project settings for Perkox iOS SDK.
    /// Ensures Swift compatibility and linker flags are properly set.
    /// </summary>
    public static class PerkoxPostProcessBuild
    {
        [PostProcessBuild(999)]
        public static void OnPostProcessBuild(BuildTarget target, string pathToBuiltProject)
        {
#if UNITY_IOS
            if (target != BuildTarget.iOS)
                return;

            string projPath = PBXProject.GetPBXProjectPath(pathToBuiltProject);
            var proj = new PBXProject();
            proj.ReadFromFile(projPath);

            string targetGuid = proj.GetUnityMainTargetGuid();
            string frameworkTargetGuid = proj.GetUnityFrameworkTargetGuid();

            // Set Swift version to 5.0
            proj.SetBuildProperty(targetGuid, "SWIFT_VERSION", "5.0");
            proj.SetBuildProperty(frameworkTargetGuid, "SWIFT_VERSION", "5.0");

            // Embed Swift Standard Libraries
            proj.SetBuildProperty(targetGuid, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "YES");

            // Add -ObjC linker flag
            proj.AddBuildProperty(targetGuid, "OTHER_LDFLAGS", "-ObjC");
            proj.AddBuildProperty(frameworkTargetGuid, "OTHER_LDFLAGS", "-ObjC");

            // Write back to file
            proj.WriteToFile(projPath);
            Debug.Log("[Perkox Build] Xcode project configured with Swift 5.0 and -ObjC linker flags.");
#endif
        }
    }
}
