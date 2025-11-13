using CocoDoogy.GameFlow.InGame.Command.Content;
using CocoDoogy.Tile;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CocoDoogy.GameFlow.InGame.Command
{
    public static partial class CommandManager
    {
        public static string Save()
        {
            List<CommandData> data = new();
            foreach (var command in Executed)
            {

                data.Add(new()
                {
                    Type = command.Type,
                    DataJson = JsonUtility.ToJson(command)
                });
            }
            CommandSave result = new()
            {
                StartPos = HexTileMap.StartPos,
                Data = data
            };
            return JsonUtility.ToJson(result);
        }
        public static void Load(string json)
        {
            Clear();

            CommandSave save = JsonUtility.FromJson<CommandSave>(json);
            
            PlayerHandler.Deploy(save.StartPos);

            foreach(var data in save.Data)
            {
                CommandBase command = data.Type switch
                {
                    CommandType.Move => JsonUtility.FromJson<MoveCommand>(data.DataJson),
                    CommandType.Trigger => JsonUtility.FromJson<TriggerCommand>(data.DataJson),
                    
                    CommandType.Slide => JsonUtility.FromJson<SlideCommand>(data.DataJson),
                    CommandType.Teleport => JsonUtility.FromJson<TeleportCommand>(data.DataJson),
                    
                    CommandType.Deploy => JsonUtility.FromJson<DeployCommand>(data.DataJson),
                    CommandType.Refill => JsonUtility.FromJson<RefillCommand>(data.DataJson),
                    CommandType.Weather => JsonUtility.FromJson<WeatherCommand>(data.DataJson),
                    _ => null
                };

                if (command == null) continue;
                Undid.Push(command);
            }
        }


        [System.Serializable]
        private class CommandSave
        {
            public Vector2Int StartPos = Vector2Int.zero;
            public List<CommandData> Data = null;
        }
        [System.Serializable]
        private class CommandData
        {
            public CommandType Type = CommandType.None;
            public string DataJson = string.Empty;
        }
    }
}