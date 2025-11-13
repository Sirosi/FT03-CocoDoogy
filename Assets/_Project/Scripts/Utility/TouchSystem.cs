using UnityEngine;
using UnityEngine.InputSystem;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

namespace CocoDoogy.Utility
{
    public static class TouchSystem
    {
        public static Vector2 TouchAverage
        {
            get
            {
#if UNITY_EDITOR
                return Mouse.current.position.ReadValue();
#elif UNITY_ANDROID || UNITY_IOS // Mobile
                int count = 0;
                Vector2 average = Vector2.zero;
                foreach (var touch in Touchscreen.current.touches)
                {
                    if (touch.phase.value is TouchPhase.Began or TouchPhase.Moved)
                    {
                        average += touch.position.value;
                        count++;
                    }
                }
                return average / count;
#else
                return Vector2.zero;
#endif
            }
        }

        public static int TouchCount
        {
            get
            {
#if UNITY_EDITOR
                return Mathf.RoundToInt(Mouse.current.leftButton.value);
#elif UNITY_ANDROID || UNITY_IOS // Mobile
                int count = 0;
                foreach (var touch in Touchscreen.current.touches)
                {
                    if (touch.phase.value is TouchPhase.Began or TouchPhase.Moved)
                    {
                        count++;
                    }
                }
                return count;
#else
                return 0;
#endif
            }
        }
    }
}