using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace Voodoo.Sauce.Internal.Editor
{
    public class TinyPreBuild : IPreprocessBuildWithReport
    {
        public int callbackOrder
        {
            get { return 0; }
        }

       public void OnPreprocessBuild(BuildReport report)
        {
            
            // check settings
            var settings = Resources.Load<TinySettings>("TinySauce/Settings");
            if (settings == null)
            {
                Debug.LogError("TinySettings can't be found");
                return;
            }
            var target = report.summary.platform;
            
            TinySettingsEditor.CheckAndUpdateSdkSettings(settings,target);
            if (target == BuildTarget.Android)
            {
                var manifestkeys = new Dictionary<string, string>
                {
                    { "[FB_APP_ID]", settings.facebookAppId }
                };
                TinyBuildHelper.PrepareAndroidGradleAndManifest();
                TinyBuildHelper.UpdateAndroidManifest(manifestkeys);
                TinyBuildHelper.UpdateAndroidProjectSetting();
                TinyBuildHelper.ResolveAndroidDependencies();

            }
            else if (target == BuildTarget.iOS)
            {
                TinyBuildHelper.UpdateIosPodsSetting();
                TinyBuildHelper.UpdateIosProjectSetting();
            }
        }
       
    }
}
