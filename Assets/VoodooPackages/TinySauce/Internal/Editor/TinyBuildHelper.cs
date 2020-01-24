using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Facebook.Unity.Editor;
using Facebook.Unity.Settings;
using GameAnalyticsSDK;
using Google;
using UnityEditor;
using UnityEngine;
namespace Voodoo.Sauce.Internal.Editor
{
    internal static class TinyBuildHelper
    {
        private const float MinIosVersion = 9.0f;
        private const string IgnoreCheck = "ignore";

        internal static void UpdateIosProjectSetting()
        {
            //set iOS CPU Architecture to Universal 
            PlayerSettings.SetArchitecture(BuildTargetGroup.iOS, 2);
            // set iOS Min Version
            float iosMinVersion;
            var changeMinVersion = true;
            if (float.TryParse(PlayerSettings.iOS.targetOSVersionString, out iosMinVersion))
            {
                if (iosMinVersion >= MinIosVersion)
                {
                    changeMinVersion = false;  
                }
                       
            }
            if (changeMinVersion)
            {
                PlayerSettings.iOS.targetOSVersionString = MinIosVersion.ToString(CultureInfo.InvariantCulture);
            }
        }
        
        internal static void UpdateAndroidProjectSetting()
        {
            // Set Android ARM64/ARMv7 Architecture   
            PlayerSettings.SetScriptingBackend(EditorUserBuildSettings.selectedBuildTargetGroup, ScriptingImplementation.IL2CPP);
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64 | AndroidArchitecture.ARMv7;

            // Set Android min version
            if (PlayerSettings.Android.minSdkVersion < AndroidSdkVersions.AndroidApiLevel19)
            {
                PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel19;
            }
        }
        
        internal static void UpdateIosPodsSetting()
        {
            IOSResolver.PodfileGenerationEnabled = true;
            IOSResolver.PodToolExecutionViaShellEnabled = true;
            IOSResolver.AutoPodToolInstallInEditorEnabled = true;
            IOSResolver.UseProjectSettings = true;
            IOSResolver.CocoapodsIntegrationMethodPref = IOSResolver.CocoapodsIntegrationMethod.Project;
        }
        internal static void ResolveAndroidDependencies()
        {
            GooglePlayServices.PlayServicesResolver.Resolve(null, true);
        }

        internal static void UpdateGradleProperties(string pathToBuildProject)
        {
            var androidBuildPath = Path.Combine(pathToBuildProject, Application.productName);
            //Mode Build
            if (!Directory.Exists(androidBuildPath)) 
                return;

            //Mode Export
            var sw = new StreamWriter(Path.Combine(androidBuildPath, "gradle.properties"));
            sw.WriteLine("org.gradle.jvmargs=-Xmx4608M");
            sw.WriteLine("android.enableR8 = false");
            sw.WriteLine("android.useAndroidX=true");
            sw.WriteLine("android.enableJetifier = true");
            sw.Close();
            
        }
        
        
        internal  static void PrepareAndroidGradleAndManifest()
        {
            const string voodooAndroidPath = "VoodooPackages/TinySauce/Internal/Editor/Android";
            
            var sourceManifestPath = Path.Combine(Application.dataPath, string.Format("{0}/AndroidManifest.xml", voodooAndroidPath));
            var sourceGradlePath = Path.Combine(Application.dataPath, string.Format("{0}/mainTemplate.gradle", voodooAndroidPath));
            
            if (!Directory.Exists(Path.Combine(Application.dataPath, "Plugins")))
                Directory.CreateDirectory(Path.Combine(Application.dataPath, "Plugins"));
            if (!Directory.Exists(Path.Combine(Application.dataPath, "Plugins/Android")))
                Directory.CreateDirectory(Path.Combine(Application.dataPath, "Plugins/Android"));

            var destManifestPath = Path.Combine(Application.dataPath, "Plugins/Android/AndroidManifest.xml");
            var destGradlePath = Path.Combine(Application.dataPath, "Plugins/Android/mainTemplate.gradle");
            
            if (File.Exists(destManifestPath)) File.Delete(destManifestPath);
            if (File.Exists(destGradlePath)) File.Delete(destGradlePath);
            
            File.Copy(sourceManifestPath, destManifestPath);
            File.Copy(sourceGradlePath, destGradlePath);

        }
        
        internal  static void UpdateAndroidManifest(Dictionary<string, string> keysToReplace)
        {
            var destManifestPath = Path.Combine(Application.dataPath, "Plugins/Android/AndroidManifest.xml");
            var content = File.ReadAllText( destManifestPath);
          
            foreach (var keys in keysToReplace)
            {
                content = content.Replace(keys.Key, keys.Value);
            }
           
            File.WriteAllText(destManifestPath, content);
            
        }
        
        internal static void CheckFacebookSettings(string facebookId, bool buildTargetAndroid )
        {
            if (string.IsNullOrEmpty(facebookId))
            {
                Debug.LogError("Missing Facebook App Id");
            }
            else if(facebookId.ToLower() != IgnoreCheck)
            {
                FacebookSettings.AppIds = new List<string> {facebookId};
                FacebookSettings.AppLabels = new List<string> {Application.productName};
                EditorUtility.SetDirty(FacebookSettings.Instance);
                if (buildTargetAndroid && ManifestMod.CheckManifest())
                {
                    ManifestMod.GenerateManifest();
                }
            }
        }

        internal static void CheckGameAnalyticsSettings(string gameKey, string secretKey , RuntimePlatform platform)
        {
            if (string.IsNullOrEmpty(gameKey)||string.IsNullOrEmpty(secretKey))
            {
                Debug.LogError("Missing GameAnalytics keys");
            }
            else if(gameKey.ToLower() != IgnoreCheck && secretKey.ToLower() != IgnoreCheck)
            {
                if (!GameAnalytics.SettingsGA.Platforms.Contains(platform))
                {
                    GameAnalytics.SettingsGA.AddPlatform(platform);
                }

                var platformIndex = GameAnalytics.SettingsGA.Platforms.IndexOf(platform);
                GameAnalytics.SettingsGA.UpdateGameKey(platformIndex, gameKey);
                GameAnalytics.SettingsGA.UpdateSecretKey(platformIndex, secretKey);
                GameAnalytics.SettingsGA.Build[platformIndex] = Application.version;
                GameAnalytics.SettingsGA.InfoLogBuild = true;
                GameAnalytics.SettingsGA.InfoLogEditor = true;
            }

        }
        
    }
}