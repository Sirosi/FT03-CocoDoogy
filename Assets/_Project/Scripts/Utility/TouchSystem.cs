using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

using TouchPhase = UnityEngine.InputSystem.TouchPhase;

namespace CocoDoogy.Utility
{
    public static class TouchSystem
    {
        public enum InputType
        {
            None,
            Mouse,
            Touch
        }


        /// <summary>
        /// 현재 Input 모드
        /// </summary>
        public static InputType CurrentInputType { get; private set; } = InputType.None;


        public static Vector2 TouchAverage
        {
            get
            {
                switch(CurrentInputType)
                {
                    case InputType.Mouse:
                        return Mouse.current.position.ReadValue();
                    case InputType.Touch:
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
                    default:
                        return Vector2.zero;
                }
            }
        }
        public static float DistanceAverage
        {
            get
            {
                if(CurrentInputType == InputType.Touch)
                {
                    Vector2 center = TouchAverage;
                    float average = 0f;
                    int count = 0;
                    foreach (var touch in Touchscreen.current.touches)
                    {
                        if (touch.phase.value is TouchPhase.Began or TouchPhase.Moved)
                        {
                            average += Vector2.Distance(center, touch.position.value);
                            count++;
                        }
                    }
                    return average / count;
                }
                
                return 0f;
            }
        }

        public static int TouchCount
        {
            get
            {
                switch(CurrentInputType)
                {
                    case InputType.Mouse:
                        return Mathf.RoundToInt(Mouse.current.leftButton.value);
                    case InputType.Touch:
                        int count = 0;
                        foreach (var touch in Touchscreen.current.touches)
                        {
                            if (touch.phase.value is TouchPhase.Began or TouchPhase.Moved)
                            {
                                count++;
                            }
                        }
                        return count;
                    default:
                        return 0;
                }
            }
        }


        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitRuntime()
        {
            InputSystem.onEvent += OnInputEvent;
        }

        private static void OnInputEvent(InputEventPtr ptr, InputDevice device)
        {
            if(!ptr.IsA<StateEvent>() && !ptr.IsA<DeltaStateEvent>()) return;


            if(device is Mouse)
            {
                CurrentInputType = InputType.Mouse;
            }
            else if(device is Touchscreen)
            {
                CurrentInputType = InputType.Touch;
            }
        }
    }
}