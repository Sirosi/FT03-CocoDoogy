using UnityEditor;

namespace CocoDoogy.Editor
{
    /// <summary>
    /// CI용 클래스
    /// </summary>
    public static class BuildAutomator
    {
        [MenuItem("Build/Windows")]
        public static void BuildForWindows()
        {
            BuildPlayerOptions buildPlayerOptions = new()
            {
                locationPathName = "./Builds/CocoDoogy.exe",
                target = BuildTarget.StandaloneWindows64,
                options = BuildOptions.Development,
            };

            BuildPipeline.BuildPlayer(buildPlayerOptions);
        }

        [MenuItem("Build/Android")]
        public static void BuildForAndroid()
        {
            BuildPlayerOptions buildPlayerOptions = new()
            {
                locationPathName = "./Builds/CocoDoogy.apk",
                target = BuildTarget.Android,
                options = BuildOptions.Development,
            };

            BuildPipeline.BuildPlayer(buildPlayerOptions);
        }
    }
}