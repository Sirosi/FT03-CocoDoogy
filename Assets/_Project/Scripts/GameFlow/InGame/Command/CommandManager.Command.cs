using CocoDoogy.Tile;
using CocoDoogy.Weather;
using UnityEngine;

namespace CocoDoogy.GameFlow.InGame.Command
{
    public static partial class CommandManager
    {
        public static void Move(HexDirection direction)
        {
            ExecuteCommand(CommandType.Move, direction);
        }
        public static void Trigger(Vector2Int gridPos)
        {
            ExecuteCommand(CommandType.Trigger, gridPos);
        }
        
        
        public static void Slide(HexDirection direction)
        {
            ExecuteCommand(CommandType.Slide, direction);
        }
        public static void Tornado(Vector2Int gridPos)
        {
            ExecuteCommand(CommandType.Tornado, (PlayerHandler.GridPos, gridPos));
        }

        
        public static void Deploy(Vector2Int gridPos, HexDirection direction)
        {
            ExecuteCommand(CommandType.Deploy, (gridPos, direction));
        }

        public static void Weather(WeatherType weather)
        {
            ExecuteCommand(CommandType.Weather, (WeatherManager.NowWeather, weather));
        }
    }
}