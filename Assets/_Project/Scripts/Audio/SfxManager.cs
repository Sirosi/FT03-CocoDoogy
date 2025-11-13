using CocoDoogy.Core;
using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

namespace CocoDoogy.Audio
{
    public class SfxManager : Singleton<SfxManager>
    {
        //모든 Sfx류의 사운드를 여기에서 관리합니다.
        private static bool HasInstance => Instance != null;
        
        //인스펙터에서 추가하지만 enum도 추가해야합니다! 인스펙터 창에서 Reset Sfx List누르면 초기화
        [Header("SfxList")]
        public List<SfxReference> sfxList = new ();
        
        //런타임에서 빠른 검색 위해서 만든 딕셔너리
        private Dictionary<SfxType, EventInstance> sfxDictionary = new ();
        
        //효과음 재생간 브금 감소를 위한 변수들 (Ducking)
        public EventReference duckingSnapShot;
        private EventInstance duckingInstance;
        
        protected override void Awake()
        {
            base.Awake();
            if (Instance != this) return;
            
            DontDestroyOnLoad(gameObject);
            
            InitializeDictionary();
            duckingInstance = RuntimeManager.CreateInstance(duckingSnapShot);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            foreach (var sfxType in sfxDictionary)
            {
                sfxType.Value.release();
            }
        }
        
        /// <summary>
        /// 정해진 효과음을 재생합니다.
        /// </summary>
        /// <param name="sfxType"></param>
        public static void PlaySfx(SfxType sfxType)
        {
            if (!HasInstance)
            {
                Debug.LogWarning("PlaySfx : 인스턴스가 존재하지 않습니다!");
                return;
            }
            
            if (Instance.sfxDictionary.TryGetValue(sfxType, out EventInstance eventInstance))
            {
                eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                eventInstance.start();
            }
        }
        
        private static void InitializeDictionary()
        {
            if (!HasInstance)
            {
                Debug.LogWarning("InitializeDictionary : 인스턴스가 존재하지 않습니다!");
                return;
            }
            
            foreach (var sfxType in Instance.sfxList)
            {
                EventInstance sfxInstance = RuntimeManager.CreateInstance(sfxType.eventReference);
                Instance.sfxDictionary.Add(sfxType.type, sfxInstance);
                
                //출력 테스트 라인 with BBUX
                /*sfxDictionary[sfxType.type].getDescription(out var eventDescription);
                eventDescription.getPath(out var path);
                eventDescription.getID(out var id);
                Debug.Log(path + " " + id);*/
            }
        }

        public static void ToggleLoopSound(SfxType sfxType)
        {
            if (!HasInstance)
            {
                Debug.LogWarning("ToggleLoopSound : 인스턴스가 존재하지 않습니다!");
                return;
            }
            
            if (sfxType != SfxType.Loop_Detecting && sfxType != SfxType.Loop_ShakeUmbrella)
            {
                return;
            }
            
            Instance.sfxDictionary[sfxType].getPlaybackState(out PLAYBACK_STATE state);
            
            if (state == PLAYBACK_STATE.STOPPED)
            {
                Instance.sfxDictionary[sfxType].start();
            }
            else
            {
                Instance.sfxDictionary[sfxType].stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            }
        }

        public static void InitDetectingLevelParameter(int level)
        {
            if (!HasInstance)
            {
                Debug.LogWarning("InitDetectingLevelParameter : 인스턴스가 존재하지 않습니다!");
                return;
            }
            
            level = Mathf.Clamp(level, 1, 3);

            if (Instance.sfxDictionary.TryGetValue(SfxType.Loop_Detecting, out EventInstance eventInstance))
            {
                Instance.sfxDictionary[SfxType.Loop_Detecting].setParameterByName("Level", level);
            }
        }
        
        //Ducking이란? 특정 상황에서 BGM을 줄여서 몰입도를 늘리는 기능입니다.
        //Ducking이 필요하면 이 메서드를 PlaySfx 메서드를 불러오기 전에 같이 불러오고, 종료시 StopDucking 불러오면 됩니다.
        public static void PlayDucking()
        {
            if (!HasInstance)
            {
                Debug.LogWarning("PlayDucking : 인스턴스가 존재하지 않습니다!");
                return;
            }
            
            Instance.duckingInstance.start();
        }

        public static void StopDucking()
        {
            if (!HasInstance)
            {
                Debug.LogWarning("StopDucking : 인스턴스가 존재하지 않습니다!");
                return;
            }
            
            Instance.duckingInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }

        #region  SfxList Reset
        //sfxList를 Reset 버튼 누름으로써 빠르게 초기화 하는 기능
        [ContextMenu("Reset Sfx List")]
        private void ResetSfxList()
        {
            sfxList.Clear();
            int successCount = 0;
            int failCount = 0;
            var allEvents = EventManager.Events;
            
            foreach (SfxType sfxType in System.Enum.GetValues(typeof(SfxType)))
            {
                if (sfxType == SfxType.None) continue;
                string enumName = sfxType.ToString();
                
                string[] parts = enumName.Split('_');
                if (parts.Length != 2)
                {
                    Debug.LogError($"SfxType {enumName}은 적합한 형식 아님! 이름 바꾸쇼");
                    failCount++;
                    continue;
                }
                
                string category = parts[0];
                string soundName = parts[1];
                
                //분리한 이름 기반으로 FMOD 이벤트 경로 생성
                string eventPath = $"event:/SFX/{category}/{soundName}";
                EventReference eventRef = EventReference.Find(eventPath);
                
                sfxList.Add(new SfxReference
                {
                    type = sfxType,
                    eventReference = eventRef
                });
                
                if (eventRef.IsNull)
                {
                    Debug.LogWarning($"FMOD 이벤트 없음: {eventPath}");
                    failCount++;
                }
                else
                {
                    Debug.Log($"매칭 성공: {sfxType} -> {eventPath}");
                    successCount++;
                }
            }
            
            Debug.Log($"=== 자동 매칭 완료 ===\n 성공: {successCount}개 | 실패: {failCount}개");
        }
        #endregion
    }
}
