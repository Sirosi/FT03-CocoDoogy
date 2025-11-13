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

            // TODO: HexTileMap Data에 맞춰 Weather 변경
            
            return true;
        }
    }
}