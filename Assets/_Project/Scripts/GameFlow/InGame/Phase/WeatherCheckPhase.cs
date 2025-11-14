using CocoDoogy.GameFlow.InGame.Command;
using CocoDoogy.Tile;

namespace CocoDoogy.GameFlow.InGame.Phase
{
    /// <summary>
    /// 현재 날씨를 변경해야 하는지 판단
    /// </summary>
    public class WeatherCheckPhase: IPhase
    {
        public bool OnPhase()
        {
            if (!InGameManager.IsValid) return false;
            
            return true;
            
            int min = InGameManager.ConsumedActionPoints + InGameManager.ConsumedActionPoints + 1;
            int max = InGameManager.ConsumedActionPoints;
            if (InGameManager.ConsumedActionPoints == 0)
            {
                min = max;
            }
            
            foreach (var weather in HexTileMap.Weathers)
            {
                if (!weather.Key.IsBetween(min, max)) continue;
                CommandManager.Weather(weather.Value);
                break;
            }
            
            return true;
        }
    }
}