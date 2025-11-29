using CocoDoogy.GameFlow.InGame.Command;
using UnityEngine;

namespace CocoDoogy.GameFlow.Replay
{
    public class ReplayHandler : MonoBehaviour
    {
        public static string ReplayData { get; set; }
        
        public static void Load()
        {
            CommandManager.Load(ReplayData);
        }
    }
}