using UnityEngine;
using UnityEngine.Rendering;
using CocoDoogy.UI;

namespace CocoDoogy.Core
{
    /// <summary>
    /// 런타임에 LightingPreset을 적용하는 매니저
    /// PageCameraSwiper의 OnPageChanged 이벤트를 구독하여 자동으로 Lighting 전환
    /// </summary>
    public class LightingManager : MonoBehaviour
    {
        public static LightingManager Instance { get; private set; }

        [Header("Lighting Presets")]
        [Tooltip("4개 페이지용 LightingPreset 배열 (인덱스 순서대로)")]
        [SerializeField] private LightingPreset[] presets;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                // PageCameraSwiper의 페이지 전환 이벤트 구독
                PageCameraSwiper.OnPageChanged += ApplyPreset;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void OnDestroy()
        {
            // 이벤트 구독 해제 (메모리 누수 방지)
            if (Instance == this)
            {
                PageCameraSwiper.OnPageChanged -= ApplyPreset;
            }
        }

        /// <summary>
        /// 지정된 페이지 인덱스의 LightingPreset을 적용
        /// </summary>
        /// <param name="pageIndex">페이지 인덱스 (0~3)</param>
        public void ApplyPreset(int pageIndex)
        {
            if (pageIndex < 0 || pageIndex >= presets.Length)
            {
                Debug.LogWarning($"[LightingManager] 페이지 인덱스 범위 초과: {pageIndex} (최대: {presets.Length - 1})");
                return;
            }

            LightingPreset preset = presets[pageIndex];
            if (preset == null)
            {
                Debug.LogWarning($"[LightingManager] 페이지 {pageIndex}에 LightingPreset이 할당되지 않음");
                return;
            }

            ApplyLightingPreset(preset);
        }

        /// <summary>
        /// LightingPreset의 모든 설정을 RenderSettings에 적용
        /// </summary>
        /// <param name="preset">적용할 LightingPreset</param>
        private void ApplyLightingPreset(LightingPreset preset)
        {
            // Environment Settings
            RenderSettings.skybox = preset.skybox;
            RenderSettings.ambientMode = preset.ambientMode;
            RenderSettings.ambientSkyColor = preset.ambientSkyColor;
            RenderSettings.ambientEquatorColor = preset.ambientEquatorColor;
            RenderSettings.ambientGroundColor = preset.ambientGroundColor;

            // Fog
            RenderSettings.fog = preset.useFog;
            RenderSettings.fogMode = preset.fogMode;
            RenderSettings.fogColor = preset.fogColor;
            RenderSettings.fogDensity = preset.fogDensity;
            RenderSettings.fogStartDistance = preset.fogStart;
            RenderSettings.fogEndDistance = preset.fogEnd;

            // Sun Light
            RenderSettings.sun = preset.sun;

            // Other Settings
            RenderSettings.subtractiveShadowColor = preset.subtractiveShadowColor;
            RenderSettings.haloStrength = preset.haloStrength;
            RenderSettings.flareStrength = preset.flareStrength;
            RenderSettings.flareFadeSpeed = preset.flareFadeSpeed;

            //             // Dynamic GI 업데이트 (실시간 전역 조명)
            // #if UNITY_EDITOR
            //             DynamicGI.UpdateEnvironment();
            // #endif

            Debug.Log($"[LightingManager] LightingPreset 적용 완료: {preset.name}");
        }
    }
}

