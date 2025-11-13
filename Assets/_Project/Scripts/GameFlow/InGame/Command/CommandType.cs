namespace CocoDoogy.GameFlow.InGame.Command
{
    public enum CommandType
    {
        /// <summary>
        /// 없다는 개념
        /// </summary>
        None = 0,

        #region ◇ 유저 조작 ◇
        /// <summary>
        /// 이동 처리
        /// </summary>
        Move = 1,
        /// <summary>
        /// 트리거 처리
        /// </summary>
        Trigger = 2,
        #endregion

        #region ◇ 유저비 조작 이동 ◇
        /// <summary>
        /// 미끄러짐 처리
        /// </summary>
        Slide = 21,
        /// <summary>
        /// 토네이도 이동처리
        /// </summary>
        Tornado = 22,
        #endregion
        
        #region ◇ 시스템 조작 ◇
        /// <summary>
        /// 유닛 배치
        /// </summary>
        Deploy = 101,
        /// <summary>
        /// 날씨 처리
        /// </summary>
        Weather = 102,
        #endregion
    }
}