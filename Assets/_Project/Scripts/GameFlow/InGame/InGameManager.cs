using CocoDoogy.Core;
using CocoDoogy.GameFlow.InGame.Command;
using CocoDoogy.GameFlow.InGame.Phase;
using CocoDoogy.Tile;
using CocoDoogy.Utility;
using System;
using UnityEngine;

namespace CocoDoogy.GameFlow.InGame
{
    public class InGameManager: Singleton<InGameManager>
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

        public static int ConsumedActionPoints
        {
            get;
            private set;
        }

        public static int RefillCount
        {
            get => Instance?.refillCount ?? 0;
            private set
            {
                if (!IsValid) return;

                Instance.refillCount = value;
                OnRefillCountChanged?.Invoke(Instance.refillCount);
            }
        }
        public static int ActionPoint
        {
            get => Instance?.actionPoint ?? 0;
            private set
            {
                if (!IsValid) return;

                Instance.actionPoint = value;
                OnActionPointChanged?.Invoke(Instance.actionPoint);
            }
        }
        /// <summary>
        /// InGame에서 사용할 MapData
        /// </summary>
        public static string MapData { get; set; } = null;


        private Camera mainCamera = null;
        private bool touched = false;

        private int refillCount = 0;
        private int actionPoint = 0;

        private readonly IPhase[] turnPhases =
        {
            new ClearCheckPhase(),
            new TornadoCheckPhase(),
            new SlideCheckPhase(),
            new WeatherCheckPhase(),
            new OutlineDrawPhase(),
            new TriggerCheckPhase(),
            new ActionPointCheckPhase(),
            new LockCheckPhase(),
        };


        void Start()
        {
            mainCamera = Camera.main;

            DrawMap(MapData);
        }

        void Update()
        {
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

                    HexTile playerTile = HexTile.GetTile(PlayerHandler.GridPos);
                    for (int i = 0; i < 6; i++)
                    {
                        HexDirection direction = (HexDirection)i;

                        if (selectedTile.GridPos == playerTile.GridPos.GetDirectionPos(direction))
                        {
                            if (playerTile.CanMove(direction))
                            {
                                CommandManager.Move(direction);
                            }
                            break;
                        }
                    }
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
            if (!IsValid) return;
            
            Instance.Clear();
            CommandManager.Clear();

            if (mapJson is null)
            {
                // MapData가 없이 InGame에 들어가면, Test데이터 생성
                mapJson = Resources.Load<TextAsset>($"MapData/Test").text;
            }
            MapSaveLoader.Apply(mapJson);

            CommandManager.Deploy(HexTileMap.StartPos, HexDirection.NorthEast);
        }

        private void Clear()
        {
            RefillCount = 0;
            ActionPoint = 0;
        }

        /// <summary>
        /// 초기화 동작
        /// </summary>
        public static void RefillActionPoint()
        {
            RefillCount++;
            ActionPoint = HexTileMap.ActionPoint;
        }
        /// <summary>
        /// 역초기화 동작
        /// </summary>
        public static void ClearActionPoint()
        {
            RefillCount--;
            ActionPoint = 0;
        }

        public static void RegenActionPoint(int regen)
        {
            ConsumedActionPoints -= regen;
            ActionPoint += regen;
        }
        public static void ConsumeActionPoint(int consume)
        {
            ConsumedActionPoints += consume;
            ActionPoint -= consume;
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public static void ProcessPhase()
        {
            if (!IsValid) return;

            foreach (var phase in Instance.turnPhases)
            {
                if (!phase.OnPhase()) break;
            }
        }
    }
}