using UnityEditor;

using System.Linq;

namespace CocoDoogy.Editor
{
    /// <summary>
    /// CI용 클래스
    /// </summary> 
    public static class BuildAutomator
    {
        private const string KeystorePath = "./Assets/NotShared/user.keystore"; // 네 경로로 수정
        private const string KeystorePass = "qwer1234!@#$";   // 네 keystore 비번
        private const string AliasName = "CocoDoogy";      // 네 alias 이름
        private const string AliasPass = "qwer1234!@#$";   // alias 비번


        [MenuItem("Build/Windows")]
        public static void BuildForWindows()
        {
            ApplyAndroidKeystoreSettings();
            BuildPlayerOptions buildPlayerOptions = new()
            {
                scenes = GetScenesFromBuildSettings(),
                locationPathName = "./Builds/CocoDoogy.exe",
                target = BuildTarget.StandaloneWindows64,
                options = BuildOptions.Development,
            };

            BuildPipeline.BuildPlayer(buildPlayerOptions);
        }

        [MenuItem("Build/Android")]
        public static void BuildForAndroid()
        {
            ApplyAndroidKeystoreSettings();
            BuildPlayerOptions buildPlayerOptions = new()
            {
                scenes = GetScenesFromBuildSettings(),
                locationPathName = "./Builds/CocoDoogy.apk",
                target = BuildTarget.Android,
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

        private static void ApplyAndroidKeystoreSettings()
        {
            PlayerSettings.Android.keystoreName = KeystorePath;
            PlayerSettings.Android.keystorePass = KeystorePass;
            PlayerSettings.Android.keyaliasName = AliasName;
            PlayerSettings.Android.keyaliasPass = AliasPass;
        }
    }
}