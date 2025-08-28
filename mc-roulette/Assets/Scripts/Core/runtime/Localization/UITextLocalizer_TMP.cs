using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// 
/// </summary>
[DisallowMultipleComponent, RequireComponent(typeof(TextMeshProUGUI))]
public class UITextLocalizer_TMP : MonoBehaviour
{
    [SerializeField]
    string m_key;
    private TextMeshProUGUI m_targetComponent;

    private void Awake()
    {
        m_targetComponent = GetComponent<TextMeshProUGUI>();
    }
}