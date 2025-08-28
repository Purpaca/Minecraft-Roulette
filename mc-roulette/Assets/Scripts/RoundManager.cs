using Purpaca;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace GamePlay
{
    /// <summary>
    /// 回合管理器
    /// </summary>
    public class RoundManager : MonoManagerBase<RoundManager>
    {
        private List<Unit> units;
        private int currentActiveUnitID = 0;
        private TurnState currentTurnState;

        private Coroutine gameProcessCoroutine;

        protected override void OnInit()
        {
            currentActiveUnitID = 0;
            this.units = new List<Unit>();
            this.units.AddRange(units);

            currentTurnState = TurnState.PreTurn;

            gameProcessCoroutine = StartCoroutine(GameProcess());
        }

        IEnumerator GameProcess()
        {
            while (true)
            {
                if (currentTurnState == TurnState.PreTurn)
                {
                    // 进行回合前的准备
                    currentTurnState = TurnState.InTurn;
                }
                else if (currentTurnState == TurnState.InTurn)
                {
                    // 进行回合中的操作
                    yield return new WaitForSeconds(1f); // 模拟回合中的操作
                    currentActiveUnitID = (currentActiveUnitID + 1) % units.Count;
                }
                else if (currentTurnState == TurnState.PostTurn)
                {
                    // 进行回合结束后的操作
                    currentTurnState = TurnState.PreTurn;
                }
            }
        }
    }
}