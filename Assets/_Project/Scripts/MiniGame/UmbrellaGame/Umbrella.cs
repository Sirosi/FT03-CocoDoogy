using CocoDoogy.Audio;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace CocoDoogy.MiniGame.UmbrellaGame
{
    public class Umbrella : CanMoveImage
    {
        private Vector2 lastDelta;
        private float lastChangeTime;
        private int needSwipeCount = 5;

        [SerializeField] private float shakeThreshold = 1.5f; // 감지 임계값
        [SerializeField] private float minIntervalShake = 0.3f;
        private float timeSinceLastShake = 0f;

        bool isDry = false;

        private Sprite drySprite;

        private UmbrellaSwipeMiniGame parent;

        private void Start()
        {
            if (Accelerometer.current != null)
            {
                InputSystem.EnableDevice(Accelerometer.current);
                Debug.Log("Accelerometer enabled");
            }
            MiniGameParticleManager.Instance.ParticleWatering(transform);
        }

        public void Init(UmbrellaSwipeMiniGame umbrellaSwipeMiniGame)
        {
            parent = umbrellaSwipeMiniGame;
        }

        private void Update()
        {
            DetectShake();
        }

        /// <summary>
        /// UmbrellaSwipeMiniGame으로부터 젖은 우산이미지와 마른 우산이미지를 받음
        /// </summary>
        /// <param name="wetsprites"></param>
        /// <param name="drysprite"></param>
        public void SetSprites(Sprite wetsprites, Sprite drysprite)
        {
            image.sprite = wetsprites;
            drySprite = drysprite;
        }

        /// <summary>
        /// 마른이미지와 상태로 교체하면서 클리어 조건검사
        /// </summary>
        /// <param name="drysprite"></param>
        public void SetDry(Sprite drysprite)
        {
            isDry = true;
            image.sprite = drysprite;
            image.raycastTarget = false;
            parent.clearcount.Remove(this);
            parent.CheckClear();
        }


        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            Handheld.Vibrate();
        }

        public override void OnDrag(PointerEventData eventData)
        {
            base.OnDrag(eventData);
            Vector2 delta = eventData.delta; // 프레임 간 이동량
            if (Mathf.Sign(delta.x) != Mathf.Sign(lastDelta.x))
            {
                print(needSwipeCount);
                if (Time.time - lastChangeTime < 0.15f && Mathf.Abs(delta.x) > 10f && needSwipeCount >= 0)
                {
                    needSwipeCount--;

                    SfxManager.PlaySfx(SfxType.Minigame_ShakeUmbrella);
                }
                lastChangeTime = Time.time;
                if (needSwipeCount <= 0 && isDry == false)
                {
                    SetDry(drySprite);
                    SfxManager.PlaySfx(SfxType.UI_SuccessMission);
                }
            }

            lastDelta = delta;
        }


        #region 모바일 환경
        /// <summary>
        /// 모바일기기 흔들기 감지(테스트 완)
        /// </summary>
        private void DetectShake()
        {
            if (isDry) return;

            timeSinceLastShake += Time.deltaTime;

            if (Accelerometer.current == null) return;

            Vector3 accel = Accelerometer.current.acceleration.ReadValue();
            Debug.Log($"Accel: {accel.magnitude:F2}");

            if (timeSinceLastShake < minIntervalShake) return;

            if (accel.magnitude >= shakeThreshold)
            {
                timeSinceLastShake = 0f;
                Handheld.Vibrate();
                SfxManager.PlaySfx(SfxType.Minigame_ShakeUmbrella);
                needSwipeCount--;
                if (needSwipeCount <= 0 && !isDry)
                {
                    isDry = true;
                    SetDry(drySprite);
                    SfxManager.PlaySfx(SfxType.UI_SuccessMission);
                }
            }

        }
        #endregion
    }
}