namespace GamePlay
{
    /// <summary>
    /// 回合当前的状态
    /// </summary>
    public enum TurnState
    {
        /// <summary>
        /// 回合开始前
        /// </summary>
        PreTurn,

        /// <summary>
        /// 回合进行中
        /// </summary>
        InTurn,

        /// <summary>
        /// 回合结束后
        /// </summary>
        PostTurn
    }
}