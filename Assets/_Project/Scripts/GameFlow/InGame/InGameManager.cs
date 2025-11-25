using CocoDoogy.Core;
using CocoDoogy.Data;
using CocoDoogy.GameFlow.InGame.Command;
using CocoDoogy.GameFlow.InGame.Phase;
using CocoDoogy.GameFlow.InGame.Phase.Passage;
using CocoDoogy.Test;
using CocoDoogy.Tile;
using CocoDoogy.Utility;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace CocoDoogy.GameFlow.InGame
{
    public class InGameManager : Singleton<InGameManager>
    {
        public static event Action<int> OnActionPointChanged = null;
        public static event Action<int> OnRefillCountChanged = null;


        /// <summary>
        /// 현재 인게임이 정상적인(= 플레이 가능) 상태인지 체크
        /// </summary>
        public static bool IsValid
        {
            get
            {
                if (!Instance) return false;
                if (!PlayerHandler.Instance) return false;


                return true;
            }
        }

        /// <summary>
        /// 마지막으로 소모된 ActionPoints
        /// </summary>
        public static int LastConsumeActionPoints { get; private set; } = 0;
        /// <summary>
        /// Map 시작 후, 소모된 ActionPoints
        /// </summary>
        public static int ConsumedActionPoints
        {
            get;
            private set;
        }

        /// <summary>
        /// 남은 RefillPoints
        /// </summary>
        public static int RefillPoints
        {
            get => Instance?.refillPoints ?? 0;
            private set
            {
                if (!IsValid) return;

                Instance.refillPoints = value;
                OnRefillCountChanged?.Invoke(Instance.refillPoints);
            }
        }
        /// <summary>
        /// Refill전까지 남은 ActionPoints
        /// </summary>
        public static int ActionPoints
        {
            get => Instance?.actionPoints ?? 0;
            private set
            {
                if (!IsValid) return;

                Instance.actionPoints = value;
                OnActionPointChanged?.Invoke(Instance.actionPoints);
            }
        }

        public static List<PassageBase> Passages { get; } = new();
        /// <summary>
        /// InGame에서 사용되는 StageData
        /// </summary>
        public static StageData Stage
        {
            get => stageData;
            set
            {
                stageData = value;
                MapData = stageData ? stageData.mapData.text : null;
            }
        }
        /// <summary>
        /// InGame에서 사용되는 MapData
        /// </summary>
        public static string MapData { get; private set; } = null;

        /// <summary>
        /// 플레이 하고 있는 맵의 최대 행동력
        /// </summary>
        public static int CurrentMapMaxActionPoints { get; private set; } = 0;

        private static StageData stageData = null;


        private Camera mainCamera = null;
        private bool touched = false;

        private int refillPoints = 0;
        private int actionPoints = 0;

        private readonly IPhase[] turnPhases =
        {
            new ClearCheckPhase(),
            new PreGravityButtonPhase(),
            new TornadoCheckPhase(),
            new SlideCheckPhase(),
            new PassageCheckPhase(),
            new CrateMovePhase(),
            new CrateProcessPhase(),
            new TriggerCheckPhase(),
            new RegenCheckPhase(),
            new DeckCheckPhase(),
            new LockCheckPhase(),
            new ActionPointCheckPhase(),
        };


        void Start()
        {
            mainCamera = Camera.main;

            DrawMap(MapData);
        }

        void Update()
        {
            if (IsPointerOverUI()) return;
            // TODO: 리팩토링 필요
            if (TouchSystem.TouchCount > 0)
            {
                if (touched) return;
                touched = true;

                Ray ray = mainCamera.ScreenPointToRay(TouchSystem.TouchAverage);
                if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, LayerMask.GetMask("Tile")))
                {
                    // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
                    HexTile selectedTile = hit.collider.GetComponentInParent<HexTile>();
                    if (!selectedTile) return;

                    HexDirection? direction = PlayerHandler.GridPos.GetRelativeDirection(selectedTile.GridPos);
                    if (!direction.HasValue) return;

                    HexTile playerTile = HexTile.GetTile(PlayerHandler.GridPos);
                    if (!playerTile.CanMove(direction.Value)) return;

                    CommandManager.Move(direction.Value);
                }
            }
            else
            {
                touched = false;
            }
        }


        /// <summary>
        /// 맵 그리기 및 캐릭터 배치
        /// </summary>
        /// <param name="mapJson"></param>
        public static void DrawMap(string mapJson)
        {
            // TODO:
            //  1. DrawMap을 다른 곳으로 옮기는 게 좋아 보임
            //  2. Map 그리는 클래스와 게임 진행 클리스를 분리해야 함
            if (!IsValid) return;

            Instance.Clear();
            CommandManager.Clear();

            if (mapJson is null)
            {
                // MapData가 없이 InGame에 들어가면, Test데이터 생성
                mapJson = Resources.Load<TextAsset>($"MapData/Test").text;
            }
            MapSaveLoader.Apply(mapJson);

            RefillPoints = HexTileMap.RefillPoint;
            ActionPoints = HexTileMap.ActionPoint;
            CurrentMapMaxActionPoints = HexTileMap.ActionPoint;
            CommandManager.Deploy(HexTileMap.StartPos, HexDirection.NorthEast);
            CommandManager.Weather(HexTileMap.DefaultWeather);

            foreach (var weather in HexTileMap.Weathers)
            {
                Passages.Add(new WeatherPassage(weather.Key, weather.Value));
            }

            Camera.main.transform.position = new Vector3(HexTileMap.StartPos.x, 5, HexTileMap.StartPos.y - 3);

            ProcessPhase();
        }

        private void Clear()
        {
            OutlineForTest.Clear();
            Passages.Clear();
            LastConsumeActionPoints = 0;
            ConsumedActionPoints = 0;
            RefillPoints = 0;
            ActionPoints = 0;
        }

        /// <summary>
        /// 초기화 동작
        /// </summary>
        public static void RefillActionPoint()
        {
            RefillPoints--;
            ActionPoints = HexTileMap.ActionPoint;
        }
        /// <summary>
        /// 역초기화 동작
        /// </summary>
        public static void ClearActionPoint()
        {
            RefillPoints++;
            ActionPoints = 0;
        }

        public static void RegenActionPoint(int regen, bool containConsume = true)
        {
            if (containConsume)
            {
                ConsumedActionPoints -= regen;
            }
            ActionPoints += regen;
        }
        public static void ConsumeActionPoint(int consume, bool containConsume = true)
        {
            if (containConsume)
            {
                LastConsumeActionPoints = consume;
                ConsumedActionPoints += consume;
            }
            ActionPoints -= consume;
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public static void ProcessPhase()
        {
            if (!IsValid) return;

            foreach (var phase in Instance.turnPhases)
            {
                if (!phase.OnPhase()) break;
            }
            // TODO: 추후 삭제 필요
            OutlineForTest.Draw();
        }

        /// <summary>
        /// UI위로 클릭하면 게임에 영향을 주지 않게 하는 메서드
        /// </summary>
        /// <returns></returns>
        private bool IsPointerOverUI()
        {
            if (EventSystem.current.IsPointerOverGameObject()) // 에디터
                return true;

            if (Touchscreen.current != null) // 빌드
            {
                foreach (var touch in Touchscreen.current.touches)
                {
                    if (touch.isInProgress &&
                        EventSystem.current.IsPointerOverGameObject(touch.touchId.ReadValue()))
                        return true;
                }
            }

            return false;
        }
    }
}