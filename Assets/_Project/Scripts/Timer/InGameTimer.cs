using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;

namespace CocoDoogy.Timer
{
    public class InGameTimer : MonoBehaviour
    {
        public static float CurrentTime { get; private set; }

        private static bool isPaused = false;

        public TextMeshProUGUI timerText;


        
        
        void Update()
        {
            if (isPaused) return;
            
            CurrentTime += Time.deltaTime;
            UpdateTimerUI();
        }
        
        public static void ToggleTimer() => isPaused = !isPaused;
        void UpdateTimerUI()
        {
            int minutes = (int)(CurrentTime / 60);
            int seconds = (int)(CurrentTime % 60);
            timerText.text = $"{minutes:00}:{seconds:00}";
        }
    }
}
