using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace CocoDoogy
{
    public class WaveDistortionController : MonoBehaviour
    {
        [Header("Fade In/Out")]
        [SerializeField] private float fadeInDuration = 1f;
        [SerializeField] private float fadeOutDuration = 1f;
        
        [Header("Target Parameters")]
        [SerializeField] private float targetWaveAmplitude = 0.05f;
        [SerializeField] private float targetSecondWaveAmp = 0.02f;
        [SerializeField] private float targetVerticalStretch = 0.1f;
        //여기에 Tint도 추가?
        [SerializeField] private GameObject parentObject;
        
        private Material materialInstance;
        private bool isDeactivating = false;
        private Sequence currentSequence;
        
        private static readonly int WaveAmplitudeID = Shader.PropertyToID("_WaveAmplitude");
        private static readonly int SecondWaveAmpID = Shader.PropertyToID("_SecondWaveAmp");
        private static readonly int VerticalStretchID = Shader.PropertyToID("_VerticalStretch");
        
        private void Awake()
        {
            RawImage rawImage = GetComponent<RawImage>();
            if (rawImage != null)
            {
                materialInstance = rawImage.material;
            }
            else
            {
                Debug.LogError("WaveDistortionController : ");
            }
            
        }
        
        private void OnEnable()
        {
            if (materialInstance == null) return;
            
            isDeactivating = false;

            ResetAllParameters();

            FadeInSequence();
        }

        public void Deactivate()
        {
            if (isDeactivating || materialInstance == null) return;
            
            isDeactivating = true;
            
            if (currentSequence != null && currentSequence.IsActive()) currentSequence.Kill();
            
            FadeOutSequence();
            
            currentSequence.OnComplete(() => parentObject.SetActive(false));
        }
        
        //private void OnDisable() 은 강제호출 대비용. 일단 독립적인 형태라서 당장은 필요없음.
        
        private void OnDestroy()
        {
            //메모리 해제해주기
            if (currentSequence != null) currentSequence.Kill();
            if (materialInstance != null)
            {
                if (Application.isPlaying)
                    Destroy(materialInstance);
                else
                    DestroyImmediate(materialInstance);
            }
        }
        
        #region Dotweening
        private void FadeInSequence()
        {
            currentSequence = DOTween.Sequence();
            
            currentSequence.Join(DOTween.To(() => 0f, x => materialInstance.SetFloat(WaveAmplitudeID, x), 
                targetWaveAmplitude, fadeInDuration));
            currentSequence.Join(DOTween.To(() => 0f, x => materialInstance.SetFloat(SecondWaveAmpID, x), 
                targetSecondWaveAmp, fadeInDuration));
            currentSequence.Join(DOTween.To(() => 0f, x => materialInstance.SetFloat(VerticalStretchID, x), 
                targetVerticalStretch, fadeInDuration));
            
            currentSequence.SetEase(Ease.OutQuad);
        }

        private void FadeOutSequence()
        {
            currentSequence = DOTween.Sequence();
            currentSequence.Join(DOTween.To(() => materialInstance.GetFloat(WaveAmplitudeID), 
                x => materialInstance.SetFloat(WaveAmplitudeID, x), 0f, fadeOutDuration));
            currentSequence.Join(DOTween.To(() => materialInstance.GetFloat(SecondWaveAmpID), 
                x => materialInstance.SetFloat(SecondWaveAmpID, x), 0f, fadeOutDuration));
            currentSequence.Join(DOTween.To(() => materialInstance.GetFloat(VerticalStretchID), 
                x => materialInstance.SetFloat(VerticalStretchID, x), 0f, fadeOutDuration));
            
            currentSequence.SetEase(Ease.InQuad);
        }
        
        private void ResetAllParameters()
        {
            if (materialInstance == null) return;
            
            materialInstance.SetFloat(WaveAmplitudeID, 0f);
            materialInstance.SetFloat(SecondWaveAmpID, 0f);
            materialInstance.SetFloat(VerticalStretchID, 0f);
        }
        #endregion
    }
}
