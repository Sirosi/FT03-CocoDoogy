using UnityEditor;
using System.Linq;
using UnityEngine;

namespace CocoDoogy.Editor
{
    /// <summary>
    /// CI(Jenkins)용 빌드 자동화 클래스
    /// </summary>
    public static class BuildAutomator
    {
        private const string KeystorePath = "./Assets/NotShared/user.keystore";
        private const string KeystorePass = "qwer1234!@#$";
        private const string AliasName = "CocoDoogy";
        private const string AliasPass = "qwer1234!@#$";

        private const string ArgName_BuildVersion = "buildVersion";

        [MenuItem("Build/Android")]
        public static void BuildForAndroid()
        {
            ApplyAndroidKeystoreSettings();

            // CLI 인자에서 버전 가져오기
            string cliVersion = GetCommandLineArgument(ArgName_BuildVersion);

            if (!string.IsNullOrEmpty(cliVersion))
            {
                PlayerSettings.bundleVersion = cliVersion;
            }

            // 버전 로그 출력 — Jenkins가 이걸 읽어서 ZIP 파일명에 사용
            Debug.Log("[CI_VERSION]" + PlayerSettings.bundleVersion);

            BuildPlayerOptions opts = new()
            {
                scenes = GetScenesFromBuildSettings(),
                locationPathName = "./Builds/CocoDoogy.apk",
                target = BuildTarget.Android,
                options = BuildOptions.None
            };

            BuildPipeline.BuildPlayer(opts);
        }

        private static string[] GetScenesFromBuildSettings()
        {
            return EditorBuildSettings.scenes
                .Where(s => s.enabled)
                .Select(s => s.path)
                .ToArray();
        }

        private static string GetCommandLineArgument(string name)
        {
            var args = System.Environment.GetCommandLineArgs();
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "-" + name && i + 1 < args.Length)
                    return args[i + 1];
            }
            return null;
        }

        private static void ApplyAndroidKeystoreSettings()
        {
            PlayerSettings.Android.useCustomKeystore = true;
            PlayerSettings.Android.keystoreName = KeystorePath;
            PlayerSettings.Android.keystorePass = KeystorePass;
            PlayerSettings.Android.keyaliasName = AliasName;
            PlayerSettings.Android.keyaliasPass = AliasPass;
        }
    }
}
