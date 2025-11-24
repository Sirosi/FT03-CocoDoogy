using Lean.Pool;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CocoDoogy.UI.Popup
{
    // TODO : 아이템 정보나 타일의 정보를 보여주기위한 InfoDialog
    // 1. StageSelectUI에서 아이템 버튼 클릭 시
    //      - 상점으로 가는 버튼
    //      - 아이템 이미지
    //      - 아이템 설명
    // 2. StageSelectUI에서 타일 버튼 클릭 시
    //      - 버튼은 없어도 되고
    //      - 타일 이미지
    //      - 타일 설명
    // 3. InGame에서 아이템 버튼 클릭 시
    //      - 아이템 사용 버튼, 취소 버튼(취소 버튼의 경우 Popup 메세지 이외의 다른 부분을 클릭하면 닫히게도 됨)
    //      - 아이템 이미지
    //      - 아이템 설명
    //      - 만약 아이템이 없는 상태에서 클릭한다면 아이템 구매버튼이 나오게
    
    public class InfoDialog : MonoBehaviour
    {
        private static InfoDialog prefab = null;
        private static Transform canvas = null;
        
        [Header("Texts")]
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI subtitleText;
        [SerializeField] private TextMeshProUGUI infoText;
        
        [Header("Panels")]
        [SerializeField] private Button backGround;
        
        [Header("Buttons")]
        [SerializeField] private Button purchaseButton;
        [SerializeField] private Button cancelButton;
        [SerializeField] private Button useButton;
        
        // TODO: purchase 버튼만 설정이 구매하기, 상점가기 2개가 있어서 텍스트 변경 용도로 남겨둠
        [Header("Buttons Text")]
        [SerializeField] private TextMeshProUGUI purchaseButtonText;

        [Header("Info Image")] 
        [SerializeField] private Image icon;
        
        private Action<CallbackType> callback = null;
        private bool hasInit = false;
        
        
        private void Init()
        {
            hasInit = true;
            purchaseButton.onClick.AddListener(OnConfirmOrYesClick);
            useButton.onClick.AddListener(OnConfirmOrYesClick);
            cancelButton.onClick.AddListener(OnNoClick);
            backGround.onClick.AddListener(OnNoClick);
        }
        
        private void Release() => LeanPool.Despawn(this);
        
        private void OnConfirmOrYesClick()
        {
            callback?.Invoke(CallbackType.Yes);
            Release();
        }
        private void OnNoClick()
        {
            callback?.Invoke(CallbackType.No);
            Release();
        }

        public static void ShowInfo(string title, string subtitle, string info, Sprite icon, DialogMode type, Action<CallbackType> callback, string buttonText = "구매하기")
        {
            InfoDialog infoDialog = Create();
            
            infoDialog.titleText.text = title;
            infoDialog.subtitleText.text = subtitle;
            infoDialog.infoText.text = info;
            
            infoDialog.icon.sprite = icon;
            
            infoDialog.purchaseButton.gameObject.SetActive(type == DialogMode.Confirm);
            infoDialog.cancelButton.gameObject.SetActive(type == DialogMode.YesNo);
            infoDialog.useButton.gameObject.SetActive(type == DialogMode.YesNo);

            if (type == DialogMode.Confirm)
            {
                infoDialog.purchaseButtonText.text = buttonText;
            }
            infoDialog.callback = callback;
        }


        private static InfoDialog Create()
        {
            if (!canvas)
            {
                canvas = GameObject.Find("PopupCanvas").transform;
            }
            
            InfoDialog result = LeanPool.Spawn(prefab, canvas);
            if (!result.hasInit)
            {
                result.Init();
            }
            result.callback = null;

            return result;
        }
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitializeRuntime()
        {
            prefab = Resources.Load<InfoDialog>("UI/InfoDialog");
        }
    }
}