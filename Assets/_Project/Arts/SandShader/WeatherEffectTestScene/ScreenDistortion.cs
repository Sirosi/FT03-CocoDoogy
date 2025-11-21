using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class SimpleHeatEffect : MonoBehaviour
{
    public Volume volume;
    private LensDistortion lensDistortion;
    
    public float waveSpeed = 2f;
    public float waveIntensity = 0.3f;
    
    void Start()
    {
        if (volume.profile.TryGet(out lensDistortion))
        {
            lensDistortion.active = true;
        }
    }
    
    void Update()
    {
        if (lensDistortion != null)
        {
            // 사인파로 일렁임 효과
            float wave = Mathf.Sin(Time.time * waveSpeed) * waveIntensity;
            lensDistortion.intensity.value = wave;
        }
    }
    
    // 이벤트 발생 시 호출
    public void TriggerEffect()
    {
        StartCoroutine(WaveEffect());
    }
    
    System.Collections.IEnumerator WaveEffect()
    {
        float duration = 3f;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            float wave = Mathf.Sin(elapsed * waveSpeed * 2f) * waveIntensity;
            lensDistortion.intensity.value = wave;
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        lensDistortion.intensity.value = 0f;
    }
}
