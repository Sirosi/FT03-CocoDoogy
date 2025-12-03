using CocoDoogy.GameFlow.InGame;
using CocoDoogy.GameFlow.InGame.Weather;
using CocoDoogy.Tile;
using CocoDoogy.Utility;
using System;
using System.Collections;
using UnityEngine;

namespace CocoDoogy.EmoteBillboard
{
    /// <summary>
    /// 감정 시스템 핸들러
    /// 기획서의 우선순위와 트리거 조건을 처리
    /// </summary>
    public class EmotionSystemHandler : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("이모티콘을 표시할 EmoteBillboard 컴포넌트")]
        [SerializeField] private EmoteBillboard emoteBillboard;

        [Header("Settings")]
        [Tooltip("지루 체크 간격 (초)")]
        [SerializeField] private float boredomCheckInterval = 0.5f;
        [Tooltip("지루 트리거 시간 (초)")]
        [SerializeField] private float boredomThreshold = 10f;

        // 현재 표시 중인 감정 (중첩 방지)
        private Emotion currentEmotion = Emotion.None;
        private bool isShowingEmotion = false;

        // 지루 감지용
        private float lastActionTime = 0f;
        private Coroutine boredomCheckCoroutine = null;
        private int previousTouchCount = 0;

        // 날씨 변경 추적용
        private WeatherType previousWeather = WeatherType.Sunny;

        // 행동력 변경 추적용
        private int previousActionPoints = 0;
        private bool isActionPointRecovered = false;

        public static event Action OnActionPointRecovered = null; // 행동력 회복 시 발생하는 이벤트 (만족 감정 트리거용)
        public static event Action OnGameFailed = null; // 행동력 회복 이벤트 발생 (외부에서 호출)

        /// <summary>
        /// 게임 실패 이벤트 발생 (외부에서 호출)
        /// </summary>
        public static void TriggerGameFailed()
        {
            OnGameFailed?.Invoke();
        }

        public static void TriggerActionPointRecovered()
        {
            OnActionPointRecovered?.Invoke();
        }

        private void OnEnable()
        {
            // InGame 씬 초기화 이벤트 구독
            InGameManager.OnMapDrawn += OnMapInitialized;

            // 날씨 변경 이벤트 구독
            WeatherManager.OnWeatherChanged += OnWeatherChanged;

            // 행동력 변경 이벤트 구독
            InGameManager.OnActionPointChanged += OnActionPointChanged;

            // 플레이어 이벤트 구독
            PlayerHandler.OnEvent += OnPlayerEvent;

            // 행동력 회복 이벤트 구독 (만족 감정)
            OnActionPointRecovered += OnSatisfactionTriggered;

            // 게임 실패 이벤트 구독 (슬픔 감정)
            OnGameFailed += OnGameFailedTriggered;
        }

        private void OnDisable()
        {
            InGameManager.OnMapDrawn -= OnMapInitialized;
            WeatherManager.OnWeatherChanged -= OnWeatherChanged;
            InGameManager.OnActionPointChanged -= OnActionPointChanged;
            PlayerHandler.OnEvent -= OnPlayerEvent;
            OnActionPointRecovered -= OnSatisfactionTriggered;
            OnGameFailed -= OnGameFailedTriggered;

            if (boredomCheckCoroutine != null)
            {
                StopCoroutine(boredomCheckCoroutine);
                boredomCheckCoroutine = null;
            }
        }

        /// <summary>
        /// 맵 초기화 완료 시 호출 (InGame 씬 시작 시)
        /// </summary>
        private void OnMapInitialized(Data.StageData stageData)
        {
            // 초기값 설정
            if (WeatherManager.Instance)
                previousWeather = WeatherManager.NowWeather;

            if (InGameManager.Instance)
                previousActionPoints = InGameManager.ActionPoints;

            // 마지막 행동 시간 초기화
            lastActionTime = Time.time;

            // 지루 체크 코루틴 시작 (이미 실행 중이면 재시작)
            if (boredomCheckCoroutine != null)
            {
                StopCoroutine(boredomCheckCoroutine);
            }
            boredomCheckCoroutine = StartCoroutine(CheckBoredomCoroutine());
        }

        #region 이벤트 핸들러

        /// <summary>
        /// 날씨 변경 시 호출
        /// </summary>
        private void OnWeatherChanged(WeatherType newWeather)
        {
            // 설렘: 폭우/폭설/신기루 에서 맑음 으로 변경경
            if (IsSevereWeather(previousWeather) && newWeather == WeatherType.Sunny)
            {
                TryShowEmotion(Emotion.Thrill, 2); // 우선순위 2
            }
            // 불안: 맑음 또는 다른 날씨에서 폭우/폭설/신기루 로 변경
            else if (IsSevereWeather(newWeather))
            {
                TryShowEmotion(Emotion.Anxiety, 2); // 우선순위 2
            }

            previousWeather = newWeather;
        }

        /// <summary>
        /// 행동력 변경 시 호출
        /// </summary>
        private void OnActionPointChanged(int newActionPoints)
        {
            // 행동력이 증가했는지 확인 (만족 트리거)
            if (newActionPoints > previousActionPoints && isActionPointRecovered)
            {
                isActionPointRecovered = false;
                // RegenActionPoint가 호출된 직후이므로 약간의 지연을 두고 체크
                StartCoroutine(CheckSatisfactionDelayed());
            }

            previousActionPoints = newActionPoints;
        }

        /// <summary>
        /// 플레이어 이벤트 (이동 등)
        /// </summary>
        private void OnPlayerEvent(Vector2Int gridPos, PlayerEventType eventType)
        {
            // 마지막 행동 시간 업데이트
            lastActionTime = Time.time;

            // 기쁨: 도착 지점 도착
            if (gridPos == HexTileMap.EndPos)
            {
                TryShowEmotion(Emotion.Joy, 1); // 우선순위 1
            }
        }

        #endregion

        #region 만족 감지 (행동력 회복)

        /// <summary>
        /// 행동력 회복 이벤트 발생 시 호출
        /// </summary>
        private void OnSatisfactionTriggered()
        {
            isActionPointRecovered = true;
        }

        /// <summary>
        /// 만족 감지를 위한 지연 체크
        /// RegenActionPoint 호출 후 실제로 행동력이 증가했는지 확인
        /// </summary>
        private IEnumerator CheckSatisfactionDelayed()
        {
            yield return null; // 1프레임 대기

            TryShowEmotion(Emotion.Satisfaction, 3); // 우선순위 3
        }

        #endregion

        #region 슬픔 감지 (게임 실패)

        /// <summary>
        /// 게임 실패 이벤트 발생 시 호출
        /// </summary>
        private void OnGameFailedTriggered()
        {
            TryShowEmotion(Emotion.Sad, 1); // 우선순위 1
        }

        #endregion

        #region 지루 감지

        /// <summary>
        /// 지루 상태를 주기적으로 체크하는 코루틴
        /// </summary>
        private IEnumerator CheckBoredomCoroutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(boredomCheckInterval);

                if (!InGameManager.IsValid) continue;
                if (PlayerHandler.IsReplay) continue;
                if (isShowingEmotion) continue; // 감정 표시 중이면 체크 안 함

                // 입력 감지
                int currentTouchCount = TouchSystem.TouchCount;
                if (currentTouchCount != previousTouchCount && currentTouchCount > 0)
                {
                    lastActionTime = Time.time; // 입력이 있으면 시간 업데이트
                }
                previousTouchCount = currentTouchCount;

                float timeSinceLastAction = Time.time - lastActionTime;

                if (timeSinceLastAction >= boredomThreshold)
                {
                    TryShowEmotion(Emotion.Boredom, 4); // 우선순위 4
                    lastActionTime = Time.time; // 트리거 후 시간 리셋 (중복 방지)
                }
            }
        }

        #endregion

        #region 감정 표시 로직

        /// <summary>
        /// 우선순위를 고려하여 감정 표시 시도
        /// </summary>
        private void TryShowEmotion(Emotion emotion, int priority)
        {
            // 현재 표시 중인 감정이 있으면 우선순위 체크
            if (isShowingEmotion)
            {
                int currentPriority = GetEmotionPriority(currentEmotion);

                // 현재 감정의 우선순위가 더 높으면 무시
                if (currentPriority < priority)
                {
                    return;
                }

                // 같은 우선순위면 무시 (중첩 방지)
                if (currentPriority == priority)
                {
                    return;
                }
            }

            // 감정 표시
            ShowEmotion(emotion);
        }

        /// <summary>
        /// 감정 표시
        /// </summary>
        private void ShowEmotion(Emotion emotion)
        {
            if (!emoteBillboard) return;
            if (!InGameManager.IsValid) return;

            currentEmotion = emotion;
            isShowingEmotion = true;

            // EmoteBillboard에 감정 전달
            emoteBillboard.ShowEmotion(emotion, OnEmotionComplete);
        }

        /// <summary>
        /// 감정 표시 완료 콜백
        /// </summary>
        private void OnEmotionComplete()
        {
            isShowingEmotion = false;
            currentEmotion = Emotion.None;
        }

        /// <summary>
        /// 감정의 우선순위 반환
        /// </summary>
        private int GetEmotionPriority(Emotion emotion)
        {
            return emotion switch
            {
                Emotion.Joy => 1,
                Emotion.Sad => 1,
                Emotion.Thrill => 2,
                Emotion.Anxiety => 2,
                Emotion.Satisfaction => 3,
                Emotion.Boredom => 4,
                _ => 999
            };
        }

        /// <summary>
        /// 심한 날씨인지 확인 (폭우, 폭설, 신기루)
        /// </summary>
        private bool IsSevereWeather(WeatherType weather)
        {
            return weather == WeatherType.Rain ||
                   weather == WeatherType.Snow ||
                   weather == WeatherType.Mirage;
        }

        #endregion
    }
}

