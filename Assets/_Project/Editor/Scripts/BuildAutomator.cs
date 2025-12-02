using UnityEditor;
using System.Linq;
using UnityEngine;
using System.IO;

namespace CocoDoogy.Editor
{
    public static class BuildAutomator
    {
        private static string KeystorePass => "qwer1234!@#$";
        private static string AliasName => "CocoDoogy";
        private static string AliasPass => "qwer1234!@#$";

        // === 절대 경로로 변환된 Keystore 경로 ===
        private static string KeystoreAbsolutePath =>
            Path.Combine(Application.dataPath, "NotShared/user.keystore");

        [MenuItem("Build/Android")]
        public static void BuildForAndroid()
        {

            ApplyAndroidKeystoreSettings();

            string outputDir = "Builds";
            if (!Directory.Exists(outputDir))
                Directory.CreateDirectory(outputDir);

            string outputPath = Path.Combine(outputDir, "CocoDoogy.apk");
            Debug.Log($"[CI] 출력 경로: {Path.GetFullPath(outputPath)}");

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

            if (!File.Exists(KeystoreAbsolutePath))
                Debug.LogError("[CI] Keystore 파일을 찾을 수 없습니다: " + KeystoreAbsolutePath);

            PlayerSettings.Android.useCustomKeystore = true;
            PlayerSettings.Android.keystoreName = KeystoreAbsolutePath;   // ★ 절대경로 적용
            PlayerSettings.Android.keystorePass = KeystorePass;
            PlayerSettings.Android.keyaliasName = AliasName;
            PlayerSettings.Android.keyaliasPass = AliasPass;

            Debug.Log("[CI] 적용된 Keystore 경로: " + KeystoreAbsolutePath);
        }
    }
}
