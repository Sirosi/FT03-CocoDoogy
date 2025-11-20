using CocoDoogy.Core;
using FMOD.Studio;
using FMODUnity;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CocoDoogy.Audio
{
    public class BgmManager : Singleton<BgmManager>
    {
        //모든 Bgm류의 사운드를 여기에서 관리합니다.
        private static bool HasInstance => Instance != null;
        
        [Header("BGMList")]
        public List<BgmReference> bgmList = new();

        //런타임에서 빠른 검색을 위해서 만든 딕셔너리
        private Dictionary<BgmType, EventInstance> bgmDictionary = new();
        
        //현재 재생중인 Bgm
        private BgmType currentBgmType = BgmType.None;
        
        protected override void Awake()
        {
            base.Awake();
            
            if (Instance != this) return;
            DontDestroyOnLoad(gameObject);
            
            //인스턴스 생성
            Init();
            
            PlayBgm(BgmType.LobbyBgm);
        }

        private static void Init()
        {
            if (!Instance)
            {
                Debug.LogWarning("InitializeDictionary : 인스턴스가 존재하지 않습니다!");
                return;
            }

            foreach (var bgmType in Instance.bgmList)
            {
                EventInstance bgmInstance = RuntimeManager.CreateInstance(bgmType.eventReference);
                Instance.bgmDictionary.Add(bgmType.type, bgmInstance);
            }
            
            //SceneManager의 이벤트를 구독해서 실행 (메서드는 따로 만들어야함)
            SceneManager.sceneLoaded += (scene, mode) =>
            {
                if (scene.name == "Lobby")
                {
                    PlayBgm(BgmType.LobbyBgm);
                }
            };
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            foreach (var bgmType in bgmDictionary)
            {
                bgmType.Value.release();
            }
        }
        
        //1. BgmType에 맞는 Bgm만 튼다.
        //2. 나머지는 다 끈다? 
        
        /// <summary>
        /// BgmType에 맞는 Bgm만 틉니다.
        /// </summary>
        /// <param name="bgmType"></param>
        public static void PlayBgm(BgmType bgmType)
        {
            if (!Instance)
            {
                Debug.LogWarning("ToggleBgm : 인스턴스가 존재하지 않습니다!");
                return;
            }

            if (Instance.currentBgmType == bgmType)
            {
                return;
            }
            
            MuteBgm();
            
            if (Instance.bgmDictionary.TryGetValue(bgmType, out EventInstance eventInstance))
            {
                eventInstance.start();
                Instance.currentBgmType = bgmType;
            }
        }
        
        /// <summary>
        /// 모든 배경음악을 중단합니다.
        /// </summary>
        /// <param name="fadeOut">true면 fadeout 아니면 false면 즉시종료</param>
        public static void MuteBgm(bool fadeOut = true)
        {
            if (!Instance) return;
            
            foreach (var BgmType in Instance.bgmDictionary)
            {
                if (fadeOut)
                {
                    BgmType.Value.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                }
                else
                {
                    BgmType.Value.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                }
            }
        }
    }
}
