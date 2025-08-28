using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

namespace Core.UI
{
    /// <summary>
    /// UI面板基类
    /// </summary>
    [RequireComponent(typeof(Canvas), typeof(CanvasGroup))]
    public abstract class UIPanel : MonoBehaviour
    {
        private Canvas m_canvas;
        private CanvasGroup m_canvasGroup;

        private UIPanel previousPanel = null;
        private UIPanel nextPanel = null;

        UIPanel _root, _top;
        private static EventSystem m_eventSystem;

        #region 属性
        /// <summary>
        /// 此UI面板所在UI面板栈的最底层面板（谨慎）
        /// </summary>
        public UIPanel RootPanel
        {
            get
            {
                _root = _root == null ? this : _root;
                while (_root.PreviousPanel != null)
                {
                    _root = _root.PreviousPanel;
                    _root._root = _root;
                }

                return _root;
            }
        }

        /// <summary>
        /// 此UI面板所在UI面板栈的最上层面板
        /// </summary>
        public UIPanel TopPanel
        {
            get
            {
                _top = _top == null ? this : _top;
                while (_top.NextPanel != null)
                {
                    _top = _top.NextPanel;
                    _top._top = _top;
                }

                return _top;
            }
        }

        /// <summary>
        /// 此UI面板上层的UI面板
        /// </summary>
        public UIPanel NextPanel { get => nextPanel; private set => nextPanel = value; }

        /// <summary>
        /// 此UI面板下层的UI面板
        /// </summary>
        public UIPanel PreviousPanel { get => previousPanel; private set => previousPanel = value; }

        protected Canvas Canvas
        {
            get
            {
                if (m_canvas == null)
                {
                    m_canvas = GetComponent<Canvas>();
                }

                return m_canvas;
            }
        }

        protected CanvasGroup CanvasGroup
        {
            get
            {
                if (m_canvasGroup == null)
                {
                    m_canvasGroup = GetComponent<CanvasGroup>();
                }

                return m_canvasGroup;
            }
        }
        #endregion

        #region Public 方法
        /// <summary>
        /// 将指定的游戏物体设置为EventSystem当前选择的UI元素
        /// </summary>
        public static void SetSelectedUIElement(GameObject target)
        {
            InitEventSystem();
            m_eventSystem.SetSelectedGameObject(target);
        }

        /// <summary>
        /// 设置一个UI面板到顶层
        /// </summary>
        /// <param name="hidePrevious">是否隐藏之前的顶层UI面板？（无论是否隐藏，都将失活之前的顶层UI面板）</param>
        public void SetUIPanelToTop(UIPanel panel, bool hidePrevious = true)
        {
            var top = TopPanel;

            top.NextPanel = panel;
            top.CanvasGroup.alpha = hidePrevious ? 0 : top.CanvasGroup.alpha;
            top.CanvasGroup.interactable = false;
            top.OnDeactive();

            panel.PreviousPanel = top;
            panel.CanvasGroup.alpha = 1;
            panel.CanvasGroup.interactable = true;
            if (!panel.enabled) panel.enabled = true;
            if (!panel.gameObject.activeInHierarchy) panel.gameObject.SetActive(true);
            panel.Canvas.sortingOrder = top.Canvas.sortingOrder + 1;
        }

        /// <summary>
        /// 关闭此UI面板（会连带依次关闭所有在此UI面板上层的UI面板）
        /// </summary>
        public void Close()
        {
            Destroy(gameObject);
        }
        #endregion

        #region Protected 方法
        /// <summary>
        /// 当此UI面板初始化时调用
        /// </summary>
        protected virtual void OnInit() { }

        /// <summary>
        /// 当此UI面板被重新激活时调用
        /// </summary>
        protected virtual void OnReactivated() { }

        /// <summary>
        /// 当此UI面板被失活时调用（当此UI面板被销毁时不会调用此方法）
        /// </summary>
        protected virtual void OnDeactive() { }

        /// <summary>
        /// 当此UI面板被关闭时调用
        /// </summary>
        protected virtual void OnClose() { }
        #endregion

        #region Private 方法
        /// <summary>
        /// 初始化EventSystem
        /// </summary>
        private static void InitEventSystem()
        {
            if (m_eventSystem == null)
            {
                if (EventSystem.current != null)
                {
                    m_eventSystem = EventSystem.current;
                }
                else
                {
                    var esObj = new GameObject("EventSystem");
                    m_eventSystem = esObj.AddComponent<EventSystem>();
                    esObj.AddComponent<InputSystemUIInputModule>();
                }
            }

            m_eventSystem.SetSelectedGameObject(null);
            DontDestroyOnLoad(m_eventSystem.gameObject);
        }

        /// <summary>
        /// 激活此UI面板下层的（先前的）UI面板
        /// </summary>
        private void ActivatePreviousPanel()
        {
            if (PreviousPanel != null)
            {
                PreviousPanel.CanvasGroup.alpha = 1;
                PreviousPanel.CanvasGroup.interactable = true;
                previousPanel.OnReactivated();
            }
        }
        #endregion

        #region Unity 消息
        private void Awake()
        {
            InitEventSystem();

            m_canvas = GetComponent<Canvas>();
            m_canvasGroup = GetComponent<CanvasGroup>();

            CanvasGroup.alpha = 1;
            CanvasGroup.interactable = true;

            OnInit();
        }

        private void OnDestroy()
        {
            if (NextPanel != null)
            {
                NextPanel.Close();
            }

            ActivatePreviousPanel();
            OnClose();
        }
        #endregion
    }
}