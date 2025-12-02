using UnityEditor;

using System.Linq;

namespace CocoDoogy.Editor
{
    /// <summary>
    /// CI용 클래스
    /// </summary>
    public static class BuildAutomator
    {
        [MenuItem("Build/Android")]
        public static void BuildForAndroid()
        {
            BuildPlayerOptions buildPlayerOptions = new()
            {
                scenes = GetScenesFromBuildSettings(),
                locationPathName = "./Builds/CocoDoogy.apk",
                target = BuildTarget.Android,
                options = BuildOptions.Development,
            };

            BuildPipeline.BuildPlayer(buildPlayerOptions);
        }

        
        
        [MenuItem("Build/Windows")]
        public static void BuildForWindows()
        {
            BuildPlayerOptions buildPlayerOptions = new()
            {
                scenes = GetScenesFromBuildSettings(),
                locationPathName = "./Builds/CocoDoogy.exe",
                target = BuildTarget.StandaloneWindows64,
                options = BuildOptions.Development,
            };

            BuildPipeline.BuildPlayer(buildPlayerOptions);
        }
        
        private static string[] GetScenesFromBuildSettings()
        {
            return EditorBuildSettings.scenes
                .Where(scene => scene.enabled)
                .Select(scene => scene.path)
                .ToArray();
        }
    }
}