namespace CocoDoogy.GameFlow.InGame.Weather
{
    public enum WeatherType
    {
        /// <summary>
        /// 기본적인 날씨
        /// </summary>
        Sunny = 0,
        /// <summary>
        /// 폭우<br/>
        /// 흙 -> 진흙
        /// </summary>
        Rain = 1,
        /// <summary>
        /// 폭설<br/>
        /// 눈 -> 폭설, 물 -> 얼음
        /// </summary>
        Snow = 2
    }
}