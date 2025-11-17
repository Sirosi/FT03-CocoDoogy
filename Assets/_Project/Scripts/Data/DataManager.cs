using CocoDoogy.Core;
using CocoDoogy.Network;
using CocoDoogy.Tile;
using CocoDoogy.Tile.Piece;
using Firebase.Firestore;
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CocoDoogy.Data
{
    public partial class DataManager : Singleton<DataManager>
    {
        //TODO : 아이템 데이터를 넣어서 여기서 itemId를 가져와 DB와 비교한 다음 ShopItem을 생성하도록
        [SerializeField] private ItemData[] itemData;
        [SerializeField] private ItemData[] cashData;
        [SerializeField] private ItemData[] stampData;

        public UserData UserData { get; private set; }

        //데이터가 변경되면 불러오는 이벤트 입니다. UI에서 구독하면 자동으로 업데이트가 가능합니다.
        public event Action OnUserDataLoaded;

        private ListenerRegistration publicListener;
        private ListenerRegistration privateListener;

        private bool isPublicDataLoaded = false;
        private bool isPrivateDataLoaded = false;


#if UNITY_EDITOR
        void Reset()
        {
            string[] guids = AssetDatabase.FindAssets("t:HexTileData");
            tileData = guids
                .Select(guid => AssetDatabase.LoadAssetAtPath<HexTileData>(AssetDatabase.GUIDToAssetPath(guid)))
                .ToArray();
            guids = AssetDatabase.FindAssets("t:PieceData");
            pieceData = guids
                .Select(guid => AssetDatabase.LoadAssetAtPath<PieceData>(AssetDatabase.GUIDToAssetPath(guid)))
                .ToArray();
        }
#endif


        protected override void Awake()
        {
            base.Awake();
            if (Instance == this)
            {
                InitTileData();
                DontDestroyOnLoad(gameObject);
            }
        }


        /// <summary>
        /// 게임 실행 시, DontDestroy해야 하는 모든 Manager 스크립트를 갖고 있는<br/>
        /// CoreManager 생성 메소드
        /// </summary>
        [RuntimeInitializeOnLoadMethod]
        private static void InitializeRuntime()
        {
            Instantiate(Resources.Load<GameObject>("CoreManager")).name = "CoreManager";
        }


        //실시간 리스너 구독
        public void StartListeningForUserData(string userId)
        {
            StopListening();

            publicListener = CreateListener(userId, "public", "profile", true);
            privateListener = CreateListener(userId, "private", "data", false);
        }

        private ListenerRegistration CreateListener(string userId, string collection, string document, bool isPublic)
        {
            var docRef = FirebaseManager.Instance.Firestore
                .Collection("users").Document(userId)
                .Collection(collection).Document(document);

            if (isPublic)
            {
                publicListener?.Stop();
            }
            else
            {
                privateListener?.Stop();
            }

            var listener = docRef.Listen(snapshot =>
            {
                try
                {
                    if (snapshot.Metadata.HasPendingWrites)
                        return;

                    if (!snapshot.Exists) return;

                    if (UserData == null)
                    {
                        UserData = new UserData();
                        UserData.SetUid(userId);
                    }

                    if (isPublic)
                    {
                        UserData.PublicUserData = snapshot.ConvertTo<PublicUserData>();
                        Debug.Log("PublicUserData");
                        isPublicDataLoaded = true;
                    }
                    else
                    {
                        UserData.PrivateUserData = snapshot.ConvertTo<PrivateUserData>();
                        Debug.Log("PrivateUserData");
                        isPrivateDataLoaded = true;
                    }

                    if (isPrivateDataLoaded && isPublicDataLoaded)
                    {
                        OnUserDataLoaded?.Invoke();
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"실시간 데이터 동기화 중 오류 발생: {e.Message}");
                }
            });

            if (isPublic)
            {
                publicListener = listener;
            }
            else
            {
                privateListener = listener;
            }

            return listener;
        }

        private void StopListening()
        {
            publicListener?.Stop();
            publicListener = null;
            privateListener?.Stop();
            privateListener = null;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            StopListening();
        }
    }
}