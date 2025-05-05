using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


[System.Serializable]
public class TeaToolData
{
    public string toolName;
    public Transform targetObject;
    [TextArea] public string description;
    public Button uiButton;
}

public class TeaToolInteraction : MonoBehaviour
{
    [Header("Camera Settings")]
    [Range(0.1f, 5f)] public float moveSpeed = 2f;
    [Range(1f, 10f)] public float moveDistance = 2f;
    public Camera mainCamera;

    [Header("Tool Settings")]
    public List<TeaToolData> tools = new List<TeaToolData>();

    [Header("UI Settings")]
    public Color buttonHighlightColor = Color.yellow;
    public float highlightDuration = 0.3f;

    // 动态绑定的UI组件
    private GameObject _descriptionPanel;
    private Text _descriptionText;
    private CanvasGroup _descriptionCanvasGroup;

    // 协程与状态管理
    private Coroutine _currentCoroutine;
    private Vector3[] _originalPositions;
    private Dictionary<Button, Color> _originalButtonColors = new Dictionary<Button, Color>();

    void Start()
    {
        FindUIComponents(); // 动态查找UI组件
        InitializeTools();  // 初始化工具状态
    }

    //=== 核心逻辑 ===//
    void InitializeTools()
    {
        _originalPositions = new Vector3[tools.Count];
        for (int i = 0; i < tools.Count; i++)
        {
            // 记录初始位置
            _originalPositions[i] = tools[i].targetObject.position;

            // 绑定按钮事件
            int index = i;
            tools[i].uiButton.onClick.AddListener(() => OnToolButtonClicked(index));

            // 保存按钮原始颜色
            var btn = tools[i].uiButton;
            _originalButtonColors[btn] = btn.colors.normalColor;
        }

        // 初始化面板状态
        if (_descriptionCanvasGroup != null)
        {
            _descriptionCanvasGroup.alpha = 0;
            _descriptionPanel.SetActive(false);
        }
    }

    void OnToolButtonClicked(int toolIndex)
    {
        // 安全停止当前协程
        if (_currentCoroutine != null)
        {
            StopCoroutine(_currentCoroutine);
            ResetToolPosition(toolIndex); // 复位上一个工具
        }

        // 启动新协程
        _currentCoroutine = StartCoroutine(MoveToolCoroutine(toolIndex));

        // 触发按钮高亮
        StartCoroutine(HighlightButton(tools[toolIndex].uiButton));
    }

    //=== 协程逻辑 ===//
    IEnumerator MoveToolCoroutine(int index)
    {
        TeaToolData tool = tools[index];
        Vector3 targetPosition = mainCamera.transform.position +
                                mainCamera.transform.forward * moveDistance;
        Vector3 startPos = tool.targetObject.position;
        float t = 0;


        // 移动动画
        while (t < 1)
        {
            t += Time.deltaTime * moveSpeed;
            tool.targetObject.position = Vector3.Lerp(startPos, targetPosition, t);
            yield return null;
        }
        tool.targetObject.position = targetPosition;

        // 显示介绍面板（带淡入效果）
        if (_descriptionCanvasGroup != null)
        {
            _descriptionText.text = tool.description;
            yield return StartCoroutine(FadePanel(0, 1, 0.5f));
        }
    }

    IEnumerator FadePanel(float startAlpha, float targetAlpha, float duration)
    {
        _descriptionPanel.SetActive(true);
        _descriptionCanvasGroup.interactable = true;
        _descriptionCanvasGroup.blocksRaycasts = true;

        float elapsed = 0;
        while (elapsed < duration)
        {
            _descriptionCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        _descriptionCanvasGroup.alpha = targetAlpha;

        // 完全透明时禁用射线检测
        if (targetAlpha == 0)
        {
            _descriptionCanvasGroup.blocksRaycasts = false;
            _descriptionPanel.SetActive(false);
        }
    }

    //=== 工具方法 ===//
    IEnumerator HighlightButton(Button btn)
    {
        var colors = btn.colors;
        Color originalColor = _originalButtonColors[btn];

        // 修改颜色
        colors.normalColor = buttonHighlightColor;
        btn.colors = colors;

        yield return new WaitForSeconds(highlightDuration);

        // 恢复颜色
        colors.normalColor = originalColor;
        btn.colors = colors;
    }

    void ResetToolPosition(int index)
    {
        tools[index].targetObject.position = _originalPositions[index];
    }

    //=== UI动态绑定 ===//
    void FindUIComponents()
    {
        // 查找说明面板
        _descriptionPanel = GameObject.Find("DescriptionPanel");
        if (_descriptionPanel == null)
        {
            Debug.LogError("未找到名为'DescriptionPanel'的UI面板！");
            return;
        }

        // 查找文本组件
        _descriptionText = _descriptionPanel.GetComponentInChildren<Text>();
        if (_descriptionText == null)
            Debug.LogError("面板内缺少Text组件！");

        // 查找CanvasGroup组件
        _descriptionCanvasGroup = _descriptionPanel.GetComponent<CanvasGroup>();
        if (_descriptionCanvasGroup == null)
            Debug.LogError("面板需要添加CanvasGroup组件！");
    }

    //=== 生命周期管理 ===//
    void OnDisable()
    {
        if (_currentCoroutine != null)
        {
            StopCoroutine(_currentCoroutine);
            _currentCoroutine = null;
        }
    }

    void OnDestroy()
    {
        if (_currentCoroutine != null)
        {
            StopCoroutine(_currentCoroutine);
            _currentCoroutine = null;
        }
    }

    //=== 复位功能 ===//
    public void ResetAllTools()
    {
        StartCoroutine(ResetToolsAnimation());
    }

    IEnumerator ResetToolsAnimation()
    {
        // 淡出面板
        if (_descriptionCanvasGroup != null && _descriptionPanel.activeSelf)
            yield return StartCoroutine(FadePanel(1, 0, 0.3f));

        // 复位所有物体
        for (int i = 0; i < tools.Count; i++)
            tools[i].targetObject.position = _originalPositions[i];
    }
}