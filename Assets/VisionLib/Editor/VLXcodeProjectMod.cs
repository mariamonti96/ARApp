#if UNITY_IOS || UNITY_STANDALONE_OSX

using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.iOS.Xcode;
using UnityEditor.Callbacks;
using System.Collections;

/// <summary>
/// VL xcode project mod, modifies the Xcode project for iOS builds to include
/// the following changes:
/// 1. Add frameworks: Accelerate, Metal and GLKit
/// 2. Disable Bitcode option
/// 3. Add Libraries: libxml2, libz
/// </summary>
public class VLXcodeProjectMod
{
    [PostProcessBuildAttribute (0)]
    public static void OnPostprocessBuild(BuildTarget buildTarget, string path)
    {
        if (buildTarget == BuildTarget.iOS)
        {
            string projPath = path + "/Unity-iPhone.xcodeproj/project.pbxproj";

            PBXProject proj = new PBXProject();
            proj.ReadFromFile(projPath);
            string target = proj.TargetGuidByName("Unity-iPhone");

            // Add Frameworks
            proj.AddFrameworkToProject(target, "Accelerate.framework", false);
            proj.AddFrameworkToProject(target, "Metal.framework", false);
            proj.AddFrameworkToProject(target, "GLKit.framework", false);
            proj.AddFrameworkToProject(target, "ARKit.framework", true);

            // Disable use of Bitcode
            proj.SetBuildProperty(target, "ENABLE_BITCODE", "NO");

            // Only generate 64bit variants, so set Valid Archs to only "arm64"
            //proj.SetBuildProperty(target, "ARCHS", "arm64");
            //proj.SetBuildProperty(target, "VALID_ARCHS", "arm64");
            proj.UpdateBuildProperty(target, "ARCHS",
                new string[]{},new string[]{"armv7","armv7s","armv7"});
            proj.UpdateBuildProperty(target, "VALID_ARCHS",
                new string[]{"arm64"}, new string[]{"armv7","armv7s"});

            // Compile libz and xml2 along with project
            proj.UpdateBuildProperty(target, "OTHER_LDFLAGS",
                new string[]{"-lz","-lxml2"}, new string[]{});

            proj.WriteToFile (projPath);

            // Since iOS 10 it's necessary to add a reason for accessing the
            // camera to Info.plist. Newer version of Unity allow to set the
            // usage description inside the editor. For older Versions of
            // Unity, we add a default value automatically.

            // Get plist
            string plistPath = path + "/Info.plist";
            PlistDocument plist = new PlistDocument();
            plist.ReadFromString(File.ReadAllText(plistPath));

            // Get root
            PlistElementDict rootDict = plist.root;

            // Set usage description, if not set already
            string cameraUsageDescriptionKey =
                "Privacy - Camera Usage Description";
            string cameraUsageDescriptionValue =
                "Augmented Reality";
            PlistElementString cameraUsageDescriptionEl =
                (PlistElementString)rootDict[cameraUsageDescriptionKey];
            if (cameraUsageDescriptionEl == null)
            {
                rootDict.SetString(cameraUsageDescriptionKey,
                    cameraUsageDescriptionValue);
                File.WriteAllText(plistPath, plist.WriteToString());
            }
            else if (String.IsNullOrEmpty(cameraUsageDescriptionEl.value))
            {
                cameraUsageDescriptionEl.value = cameraUsageDescriptionValue;
                File.WriteAllText(plistPath, plist.WriteToString());
            }
        }
    }
}

#endif // UNITY_IOS || UNITY_STANDALONE_OSX
