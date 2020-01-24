using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace Voodoo.Sauce.Internal.Editor
{
    public static class VoodooPostBuild
    {
        [PostProcessBuild]
        private static void AddGradleProperties(BuildTarget buildTarget, string pathToBuildProject)
        {
            if (buildTarget == BuildTarget.Android)
            {
                TinyBuildHelper.UpdateGradleProperties(pathToBuildProject);
            }
           
        }

    }
}