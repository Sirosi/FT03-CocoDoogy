using UnityEngine;
using UnityEngine.UI; // RawImage 사용을 위해 추가

[RequireComponent(typeof(RawImage))] // MeshRenderer 대신 RawImage 요구
public class SandEffectController : MonoBehaviour
{
    // Inspector에서 셰이더 속성 이름과 duration 설정 가능
    [SerializeField] private string progressPropertyName = "_Progress"; 
    
    public float duration = 2f;
    
    private RawImage rawImage;
    private Material materialInstance; // RawImage에 적용될 머터리얼 인스턴스
    private float progress = 0.8f;
    
    void Awake()
    {
        // RawImage 컴포넌트 가져오기
        rawImage = GetComponent<RawImage>();
        
        // RawImage.material에 접근하면 해당 RawImage만을 위한 
        // 머터리얼 인스턴스가 생성되어 다른 UI 요소에 영향을 주지 않습니다.
        if (rawImage != null)
        {
            materialInstance = rawImage.material;
        }
    }

    private void OnEnable()
    {
        // 초기 진행률 설정
        progress = 0.8f;
        
        if (materialInstance != null)
        {
            // 시작 시점에 머터리얼에 진행률 적용
            materialInstance.SetFloat(progressPropertyName, progress);
        }
    }

    void Update()
    {
        // progress가 0보다 크고 머터리얼 인스턴스가 유효할 때만 업데이트
        if (progress > 0f && materialInstance != null)
        {
            // 시간 경과에 따른 진행률 감소
            progress -= Time.deltaTime / duration;
            progress = Mathf.Max(0f, progress);
            
            // 머터리얼에 _Progress 값 적용
            materialInstance.SetFloat(progressPropertyName, progress);
        }
    }
}