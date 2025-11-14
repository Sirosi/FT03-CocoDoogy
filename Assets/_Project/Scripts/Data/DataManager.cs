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
        
        /// <summary>
        /// Firebase Store의 Document 내부의 필드에 변화가 생기면 발생하는 이벤트. public Doc 하위 필드 변경시 발생
        /// </summary>
        public event Action OnPublicUserDataLoaded;
        /// <summary>
        /// Firebase Store의 Document 내부의 필드에 변화가 생기면 발생하는 이벤트. private Doc 하위 필드 변경시 발생
        /// </summary>
        public event Action OnPrivateUserDataLoaded;

        /// <summary>
        /// Firebase Store의 Document 내부의 필드에 변화를 감지하는 리스너. public Doc 하위 필드 변경시 발생
        /// </summary>
        private ListenerRegistration publicListener;
        /// <summary>
        /// Firebase Store의 Document 내부의 필드에 변화를 감지하는 리스너. private Doc 하위 필드 변경시 발생
        /// </summary>
        private ListenerRegistration privateListener;
        
        private PrivateUserData lastPrivateData;
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
            
            var listener = docRef.Listen(snapshot =>
            {
                try
                {
                    if (!snapshot.Exists || snapshot.Metadata.HasPendingWrites) return;

                    if (UserData == null)
                    {
                        UserData = new UserData();
                        UserData.SetUid(userId);
                    }

                    if (isPublic)
                    {
                        var newPublicData = snapshot.ConvertTo<PublicUserData>();
                        if (UserData.PublicUserData != null && UserData.PublicUserData.Equals(newPublicData)) return;

                        UserData.PublicUserData = newPublicData;
                        Debug.Log("PublicUserData 업데이트됨");
                        OnPublicUserDataLoaded?.Invoke();
                    }
                    else
                    {
                        var newPrivateData = snapshot.ConvertTo<PrivateUserData>();
                        
                        if (lastPrivateData != null && lastPrivateData.Equals(newPrivateData)) return;

                        lastPrivateData = newPrivateData;
                        UserData.PrivateUserData = newPrivateData;
                        Debug.Log("PrivateUserData 업데이트됨");
                        OnPrivateUserDataLoaded?.Invoke();
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"실시간 데이터 동기화 중 오류 발생: {e.Message}");
                }
            });
            
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