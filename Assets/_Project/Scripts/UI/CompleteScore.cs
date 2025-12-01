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

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        [ContextMenu("GetStar1")]
        public void GetStar1()
        {
            GetStar(1);
        }
        [ContextMenu("GetStar2")]
        public void GetStar2()
        {
            GetStar(2);
        }
        [ContextMenu("GetStar3")]
        public void GetStar3()
        {
            GetStar(3);
        }
        
         /// <summary>
         /// score를 매개변수로 받아 switch로 분기
         /// //TODO: 사운드 추가작업 남음
         /// </summary>
         /// <param name="score"></param>
         public void GetStar(int  score)
         {
             Sequence seq = DOTween.Sequence();
             switch (score) 
             {
                 case 1:
                     seq.AppendInterval(0.1f)
                         .AppendCallback(() =>
                         {
                             Instantiate(starParticle, leftStar.StarPos(), Quaternion.identity,  rectTransform);
                             //TODO: 사운드 추가
                             //SfxManager.PlaySfx(SfxType.UI_ClearStar1);
                             leftStar.TryGetStar(null);
                         });
                     break;
                 case 2:
                     seq.AppendInterval(0.1f)
                         .AppendCallback(() =>
                         {
                             Instantiate(starParticle, leftStar.StarPos(), Quaternion.identity,  rectTransform);
                             leftStar.TryGetStar(()=>
                             {
                                 Instantiate(starParticle, centerStar.StarPos(), Quaternion.identity,  rectTransform);
                                 centerStar.TryGetStar(null);
                             });
                         });
                     
                     break;
                 case 3:
                     seq.AppendInterval(0.1f)
                         .AppendCallback(() =>
                         {
                             Instantiate(starParticle, leftStar.StarPos(), Quaternion.identity,  rectTransform);
                             leftStar.TryGetStar(()=>
                             {
                                 Instantiate(starParticle, centerStar.StarPos(), Quaternion.identity,  rectTransform);
                                 centerStar.TryGetStar(()=>
                                 {
                                     Instantiate(starParticle, rightStar.StarPos(), Quaternion.identity,  rectTransform);
                                     rightStar.TryGetStar(()=>
                                     {
                                         Instantiate(_3starParticle, rectTransform);
                                         leftStar.TryGetStar(null);
                                         centerStar.TryGetStar(null);
                                         rightStar.TryGetStar(null);
                                     });
                                 });
                             });
                         });
                     
                     break;
             }
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
    }
}
