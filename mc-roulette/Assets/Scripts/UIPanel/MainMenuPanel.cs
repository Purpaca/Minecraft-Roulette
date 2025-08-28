using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Core.UI;
using Newtonsoft.Json;
using IEnumerator = System.Collections.IEnumerator;

using Button = UnityEngine.UI.Button;

public class MainMenuPanel : UIPanel
{
    [Header("UI控件绑定"), SerializeField]
    private Button m_playButton;
    [SerializeField]
    private Button m_optionsButton;
    [SerializeField]
    private Button m_quitButton;
    [SerializeField]
    private Button m_changeSkinButton;
    [Space(3), SerializeField]
    private Text m_yellowText;
    [SerializeField]
    private Text m_versionText;
    [SerializeField]
    private RectTransform m_playerSkinPreviewRect;
    [Space(3), SerializeField]
    private Image m_logoImage_minecraft;
    [SerializeField]
    private Image m_logoImage_roulette;

    [Space, Header("场景物体绑定"), SerializeField]
    private Transform m_skinPreviewObject;
    [SerializeField]
    private Camera m_backgroundCam;
    [Space(3), SerializeField]
    AudioSource m_audioSource;

    private UIPanel _optionsPanelPrefab;
    private Coroutine _cameraRotationCoroutine, _yellowTextScalingCoroutine;

    #region Protected 方法
    protected override void OnInit()
    {
        m_versionText.text = Application.version;

        #region Button按下事件 初始化
        m_playButton.onClick.AddListener(() =>
        {
            PlayButtonClickSound();
        });

        m_optionsButton.onClick.AddListener(() =>
        {
            PlayButtonClickSound();
            if (_optionsPanelPrefab == null)
            {
                //_optionsPanelPrefab = Resources.Load<UIPanel>(ResourcesPaths.Prefab.UIPanel.OptionsPanel);
            }
            
            var optionsPanel = Instantiate(_optionsPanelPrefab.gameObject).GetComponent<UIPanel>();
            SetUIPanelToTop(optionsPanel);
        });

        m_quitButton.onClick.AddListener(() =>
        {
            PlayButtonClickSound();
            Application.Quit();
        });

        m_changeSkinButton.onClick.AddListener(() =>
        {
            PlayButtonClickSound();
        });
        #endregion

        try
        {
            /*
            Addressables.LoadAssetAsync<TextAsset>("text_splashes").Completed += (handle) =>
            {
                if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
                {
                    var json = handle.Result.text;
                    var splashes = JsonConvert.DeserializeObject<string[]>(json);
                    var index = UnityEngine.Random.Range(0, splashes.Length);
                    m_yellowText.text = splashes[index];
                }
                else 
                {
                    m_yellowText.text = "If you see this, it means something went wrong.";
                }

                Addressables.Release(handle);
            };
            */
        }
        catch (Exception exception)
        {
            Debug.LogError($"An exception occurred when try to set main menu yellow text splash! INFO:\"{exception}\"");
        }

        var preview = m_playerSkinPreviewRect.gameObject.AddComponent<PlayerSkinPreviewDragEventHandler>();
        preview.skinPreview = m_skinPreviewObject;
    }

    protected override void OnReactivated()
    {
        if (_cameraRotationCoroutine == null) _cameraRotationCoroutine = StartCoroutine(CameraRotationCoroutine());
        _yellowTextScalingCoroutine = StartCoroutine(YellowTextScalingCoroutine());
    }

    protected override void OnDeactive()
    {
        if(_yellowTextScalingCoroutine != null)
        {
            StopCoroutine(_yellowTextScalingCoroutine);
        }

        _yellowTextScalingCoroutine = null;
    }

    protected override void OnClose() { }
    #endregion

    #region Private 方法
    private void PlayButtonClickSound() 
    {
        m_audioSource.PlayOneShot(m_audioSource.clip);
    }
    #endregion

    #region 协程
    /// <summary>
    /// 相机持续缓慢向右旋转的协程
    /// </summary>
    /// <returns></returns>
    private IEnumerator CameraRotationCoroutine()
    {
        while (true)
        {
            var eularY = m_backgroundCam.transform.eulerAngles.y + 2.5f * Time.unscaledDeltaTime;
            m_backgroundCam.transform.eulerAngles = new Vector3(m_backgroundCam.transform.eulerAngles.x, eularY, m_backgroundCam.transform.eulerAngles.z);
            yield return null;
        }
    }

    /// <summary>
    /// 黄色文字缩小放大往复的协程
    /// </summary>
    /// <returns></returns>
    private IEnumerator YellowTextScalingCoroutine()
    {
        float minScale = 0.9f;
        float maxScale = 1.0f;
        var pause = new WaitForSecondsRealtime(0.1f);

        var eular = m_yellowText.rectTransform.localEulerAngles;
        eular.z = 17.5f;
        m_yellowText.rectTransform.localEulerAngles = eular;

        float scale = 0.95f;
        bool isScalingUp = false;
        while (true)
        {
            var size = Mathf.Clamp(scale, minScale, maxScale);
            m_yellowText.rectTransform.localScale = new Vector3(size, size, 1);

            float delta = Time.unscaledDeltaTime * 0.65f;

            // MagicFix: 在游戏失焦或暂停时，协程会仍然运行导致缩放值过度溢出（远小于最小值或远大于最大值），
            // 从而出现黄色文字卡住不再缩放的情况。推测为unity的问题，这里钳制scale为最大最小值±0.05f，以修复此问题。
            // =======================================================================================
            // 补充：这个问题似乎与使用了Time.unscaledDeltaTime有关。
            // scale = isScalingUp ? scale + delta : scale - delta;
            scale = Mathf.Clamp(isScalingUp ? scale + delta : scale - delta, minScale - 0.05f, maxScale + 0.05f);

            if (scale >= maxScale)
            {
                isScalingUp = false;
                yield return pause;
            }
            else if (scale <= minScale)
            {
                isScalingUp = true;
            }

            yield return null;
        }
    }
    #endregion

    #region Unity 消息
    private void Start()
    {
        OnReactivated();
    }
    #endregion

    #region 内部类型
    /// <summary>
    /// 玩家皮肤预览拖拽事件处理器
    /// </summary>
    private class PlayerSkinPreviewDragEventHandler : MonoBehaviour, IDragHandler
    {
        public Transform skinPreview;
        public float rotateSpeed = 120.0f;

        public void OnDrag(PointerEventData eventData)
        {
            var eularY = skinPreview.eulerAngles.y - eventData.delta.x * rotateSpeed * Time.unscaledDeltaTime;
            skinPreview.eulerAngles = new Vector3(skinPreview.eulerAngles.x, eularY, skinPreview.eulerAngles.z);
        }
    }
    #endregion
}