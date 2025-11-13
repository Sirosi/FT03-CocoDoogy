using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CocoDoogy
{
    public class IntroUIManager : MonoBehaviour
    {
        [Header("Start")]
        [SerializeField] private Button backGround;
        
        [Header("Step1")]
        [SerializeField] private GameObject touchToStart;

        [Header("Step2")]
        [SerializeField] private GameObject loginPanel;
        
        [Header("Step3")]
        [SerializeField] private GameObject registerPanel;
        [SerializeField] private TMP_InputField nickname;
        [SerializeField] private Button registerButton;
        
        
        
        private void Awake()
        {
            touchToStart.SetActive(true);
            loginPanel.SetActive(false);
            registerPanel.SetActive(false);
            
            backGround.onClick.AddListener(OnBackGroundButtonClicked);
        }

        private void OnBackGroundButtonClicked()
        {
            touchToStart.SetActive(false);
            loginPanel.SetActive(true);
        }


        
        
        //모든 절차가 끝났을때
        //Loading.LoadScene("대충로비화면씬이름");
    }

    
}
