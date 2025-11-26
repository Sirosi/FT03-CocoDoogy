using CocoDoogy.GameFlow.InGame;
using CocoDoogy.GameFlow.InGame.Command;
using Newtonsoft.Json;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace CocoDoogy.Test
{
    public class CommandSaveTest: MonoBehaviour
    {
        [SerializeField] private string save;


        [ContextMenu("Save")]
        private void Save()
        {
            save = CommandManager.Save();
        }
        [ContextMenu("Load")]
        private void Load()
        {
            CommandManager.Load(save);
        }
    }
}