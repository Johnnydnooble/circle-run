using System;
using UnityEditor;
using UnityEngine;

namespace Voodoo.Sauce.Internal.Editor
{
    [CustomEditor(typeof(TinySettings))]
    public class TinySettingsEditor : UnityEditor.Editor
    {

        private  TinySettings Settings
        {
            get { return target as TinySettings; }
        }

        [MenuItem("VoodooPackages/TinySauce/Edit Settings")]
        private static void EditSettings()
        {
            
            var settings = Resources.Load<TinySettings>("TinySauce/Settings");
            if (settings == null)
            {
                settings = CreateInstance<TinySettings>();
                //create tinySauce folders if it not exists
                if(!AssetDatabase.IsValidFolder("Assets/Resources"))
                    AssetDatabase.CreateFolder("Assets", "Resources");
                
                if(!AssetDatabase.IsValidFolder("Assets/Resources/TinySettings"))
                    AssetDatabase.CreateFolder("Assets/Resources", "TinySauce");
                //create TinySettings file
                AssetDatabase.CreateAsset(settings, "Assets/Resources/TinySauce/Settings.asset");
                settings = Resources.Load<TinySettings>("TinySauce/Settings");
            }

            Selection.activeObject = settings;
        }
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUILayout.Space(15);
            
            if (GUILayout.Button(Environment.NewLine + "Check and Sync Settings" + Environment.NewLine))
            {
                
                #if UNITY_ANDROID
                    CheckAndUpdateSdkSettings( Settings, BuildTarget.Android);
                #else
                    CheckAndUpdateSdkSettings( Settings, BuildTarget.iOS);
                #endif
            }
        }
        
        public static void CheckAndUpdateSdkSettings(TinySettings settings, BuildTarget target)
        {

            if (target == BuildTarget.iOS)
            {
                TinyBuildHelper.CheckGameAnalyticsSettings(settings.gameAnalyticsIosGameKey, settings.gameAnalyticsIosSecretKey,
                    RuntimePlatform.IPhonePlayer);
                TinyBuildHelper.CheckFacebookSettings(settings.facebookAppId, false);
            }
            else if (target == BuildTarget.Android)
            {
                TinyBuildHelper.CheckGameAnalyticsSettings(settings.gameAnalyticsAndroidGameKey, settings.gameAnalyticsAndroidSecretKey,
                    RuntimePlatform.Android);
                TinyBuildHelper.CheckFacebookSettings(settings.facebookAppId, true);
            }

        }
        
       
    }
}