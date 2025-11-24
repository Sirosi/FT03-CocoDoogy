using CocoDoogy.Audio;
using CocoDoogy.Tile;
using System.Collections;
using UnityEngine;

namespace CocoDoogy.GameFlow.InGame
{
    public partial class PlayerHandler
    {
        private Coroutine _footstepCoroutine;
        
        [Header("Footstep Settings")]
        [Tooltip("발소리 간격 (초)")]
        [SerializeField] private float footstepInterval = 0.3f;

        [Tooltip("타일의 레이어")]
        [SerializeField] private LayerMask tileLayerMask;
        
        public void StartFootstepSound()
        {
            if (_footstepCoroutine != null)
            {
                StopCoroutine(_footstepCoroutine);
            }
            Debug.Log("<color=green><b>[Footstep]</b> 코루틴을 시작합니다.</color>");
            _footstepCoroutine = StartCoroutine(PlayFootstepCoroutine());
        }
        
        public void StopFootstepSound()
        {
            if (_footstepCoroutine != null)
            {
                Debug.Log("<color=red><b>[Footstep]</b> 코루틴을 중지합니다!</color>");
                StopCoroutine(_footstepCoroutine);
                _footstepCoroutine = null;
            }
        }
        
        //발소리 코루틴
        private IEnumerator PlayFootstepCoroutine()
        {
            while (true)
            {
                if (Physics.Raycast(transform.position + Vector3.up * 0.1f,
                        Vector3.down,
                        out RaycastHit hit,
                        1.5f,
                        tileLayerMask))
                {
                    HexTile currentTile = hit.collider.GetComponentInParent<HexTile>();
                    if (currentTile != null && currentTile.CurrentData.stepSfx != SfxType.None)
                    {
                        SfxManager.PlaySfx(currentTile.CurrentData.stepSfx);
                    }
                }
                yield return new WaitForSeconds(footstepInterval);
            }
        }
    }
}
