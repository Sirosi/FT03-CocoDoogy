using CocoDoogy.Audio;
using Coffee.UIExtensions;
using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace CocoDoogy.UI
{
    public class CompleteScore : MonoBehaviour
    {
        [SerializeField] private Star leftStar;
        [SerializeField] private Star centerStar;
        [SerializeField] private Star rightStar;
        [SerializeField] private UIParticle starParticle;
        [SerializeField] private UIParticle _3starParticle;
        
        private RectTransform rectTransform;
        private Star[] stars;
        
        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            stars = new[] { leftStar, centerStar, rightStar };
        }

        /// <summary>
        /// 생성된 UIParticle 한번에 제거 아니면 TryGetStar의 Callback으로 넣어도 되고
        /// </summary>
        private void OnDisable()
        {
            // transform.childCount를 활용하거나 ToArray로 복사
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                Transform child = transform.GetChild(i);
                UIParticle particle = child.GetComponent<UIParticle>();
                if (particle != null)
                {
                    Destroy(particle.gameObject);
                }
            }
        }

        /// <summary>
        /// score를 매개변수로 받아 switch로 분기
        /// //TODO: 사운드 추가작업 남음
        /// </summary>
        /// <param name="isDefeat"></param>
        /// <param name="score"></param>
        public void GetStageClearResult(bool isDefeat, int score)
        {
            Debug.Log($"GetStageClearResult : score = {score}");
            if (isDefeat)
            {
                PlayDefeat();
            }
            else
            {
                Sequence seq = DOTween.Sequence();
                seq.AppendInterval(0.1f)
                    .AppendCallback(() =>
                    {
                        PlayStarRecursive(0, score);
                    });
            }
        }

        /// <summary>
        /// 별을 반복해서 최대 3번 변경하는 메서드. 재귀함수
        /// </summary>
        /// <param name="index"></param>
        /// <param name="max"></param>
        private void PlayStarRecursive(int index, int max)
        {
            SfxManager.PlayDucking();
            if (index >= max)
            {
                if (max == 3)
                {
                    Instantiate(_3starParticle, rectTransform);

                    stars[0].TryGetStar(null);
                    stars[1].TryGetStar(null);
                    stars[2].TryGetStar(null);
                    
                    SfxManager.PlaySfx(SfxType.UI_SuccessStage);
                    SfxManager.StopDucking();
                }
                return;
            }

            Star star = stars[index];
            
            Instantiate(starParticle, star.StarPos(), Quaternion.identity, rectTransform);
            
            PlayStarSfx(index);
            
            star.TryGetStar(() =>
            {
                PlayStarRecursive(index + 1, max); // 재귀 함수 부분
            });
        }

        private void PlayDefeat()
        {
            SfxManager.PlaySfx(SfxType.UI_FailStage);
        }

        private void PlayStarSfx(int index)
        {
            switch (index)
            {
                case 0:
                    SfxManager.PlaySfx(SfxType.UI_ClearStar1);
                    break;
                case 1:
                    SfxManager.PlaySfx(SfxType.UI_ClearStar2);
                    break;
                case 2:
                    SfxManager.PlaySfx(SfxType.UI_ClearStar3);
                    break;
            }
        }
    }
}