using Purpaca;
using UnityEngine;
using Core.UI;

public class GameApplication : MonoManagerBase<GameApplication>
{
    private UIPanel rootUIPanel;

    protected override void OnInit()
    {
        Application.wantsToQuit += () =>
        {
            return false;
        };
    }

    #region Public 方法
    /// <summary>
    /// 将一个UI面板添加到游戏主UI面板栈中
    /// </summary>
    public static void AddUIPanelToStack(UIPanel panel) 
    {
        if(instance.rootUIPanel == null)
        {
            instance.rootUIPanel = panel;
            panel.GetComponent<Canvas>().sortingOrder = 0;
        }
        else
        {
            instance.rootUIPanel.SetUIPanelToTop(panel);
        }
    }

    /// <summary>
    /// 关闭游戏主UI面板栈中的所有UI面板
    /// </summary>
    public static void ClearUIPanelStack() 
    {
        if (instance.rootUIPanel != null)
        {
            instance.rootUIPanel.Close();
        }
    }
    #endregion
}