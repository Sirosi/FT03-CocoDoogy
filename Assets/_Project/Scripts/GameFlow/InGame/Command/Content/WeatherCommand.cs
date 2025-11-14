using CocoDoogy.GameFlow.InGame.Weather;

namespace CocoDoogy.GameFlow.InGame.Command.Content
{
    [System.Serializable]
    public class WeatherCommand: CommandBase
    {
        public override bool IsUserCommand => false;
        
        
        /// <summary>
        /// 기존 위치
        /// </summary>
        public WeatherType PreWeather = WeatherType.Sunny;
        /// <summary>
        /// 이동한 위치
        /// </summary>
        public WeatherType NextWeather = WeatherType.Sunny;


        public WeatherCommand(object param): base(CommandType.Weather, param)
        {
            (WeatherType, WeatherType) data = ((WeatherType, WeatherType))param;

            PreWeather = data.Item1;
            NextWeather = data.Item2;
        }


        public override void Execute()
        {
            WeatherManager.StartGlobalWeather(NextWeather);
        }

        public override void Undo()
        {
            WeatherManager.StartGlobalWeather(PreWeather);
        }
    }
}