using UnityEditor;
using System.Linq;
using UnityEngine;

namespace CocoDoogy.Editor
{
    public static class BuildAutomator
    {
        private const string KeystorePath = "./Assets/NotShared/user.keystore";
        private const string KeystorePass = "qwer1234!@#$";
        private const string AliasName = "CocoDoogy";
        private const string AliasPass = "qwer1234!@#$";

        [MenuItem("Build/Android")]
        public static void BuildForAndroid()
        {
            Debug.Log("[CI] Android 빌드 시작");

            // 1) Android 플랫폼 전환
            EditorUserBuildSettings.SwitchActiveBuildTarget(
                BuildTargetGroup.Android,
                BuildTarget.Android
            );

            // 2) Keystore 적용
            ApplyAndroidKeystoreSettings();

            // 3) 출력 경로 설정
            string outputPath = "Builds/CocoDoogy.apk";
            Debug.Log($"[CI] 출력 경로: {outputPath}");

            BuildPlayerOptions buildPlayerOptions = new()
            {
                scenes = GetScenesFromBuildSettings(),
                locationPathName = outputPath,
                target = BuildTarget.Android,
                options = BuildOptions.None,
            };

            // 4) 빌드 실행
            var report = BuildPipeline.BuildPlayer(buildPlayerOptions);

            Debug.Log($"[CI] Android 빌드 종료: {report.summary.result}");

            if (report.summary.result != UnityEditor.Build.Reporting.BuildResult.Succeeded)
            {
                Debug.LogError("[CI] Android 빌드 실패!");
                throw new System.Exception("Android Build Failed.");
            }
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
            Debug.Log("[CI] Keystore 설정 적용");

            PlayerSettings.Android.useCustomKeystore = true;
            PlayerSettings.Android.keystoreName = KeystorePath;
            PlayerSettings.Android.keystorePass = KeystorePass;
            PlayerSettings.Android.keyaliasName = AliasName;
            PlayerSettings.Android.keyaliasPass = AliasPass;
        }
    }
}
