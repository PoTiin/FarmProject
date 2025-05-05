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

    // ��̬�󶨵�UI���
    private GameObject _descriptionPanel;
    private Text _descriptionText;
    private CanvasGroup _descriptionCanvasGroup;

    // Э����״̬����
    private Coroutine _currentCoroutine;
    private Vector3[] _originalPositions;
    private Dictionary<Button, Color> _originalButtonColors = new Dictionary<Button, Color>();

    void Start()
    {
        FindUIComponents(); // ��̬����UI���
        InitializeTools();  // ��ʼ������״̬
    }

    //=== �����߼� ===//
    void InitializeTools()
    {
        _originalPositions = new Vector3[tools.Count];
        for (int i = 0; i < tools.Count; i++)
        {
            // ��¼��ʼλ��
            _originalPositions[i] = tools[i].targetObject.position;

            // �󶨰�ť�¼�
            int index = i;
            tools[i].uiButton.onClick.AddListener(() => OnToolButtonClicked(index));

            // ���水ťԭʼ��ɫ
            var btn = tools[i].uiButton;
            _originalButtonColors[btn] = btn.colors.normalColor;
        }

        // ��ʼ�����״̬
        if (_descriptionCanvasGroup != null)
        {
            _descriptionCanvasGroup.alpha = 0;
            _descriptionPanel.SetActive(false);
        }
    }

    void OnToolButtonClicked(int toolIndex)
    {
        // ��ȫֹͣ��ǰЭ��
        if (_currentCoroutine != null)
        {
            StopCoroutine(_currentCoroutine);
            ResetToolPosition(toolIndex); // ��λ��һ������
        }

        // ������Э��
        _currentCoroutine = StartCoroutine(MoveToolCoroutine(toolIndex));

        // ������ť����
        StartCoroutine(HighlightButton(tools[toolIndex].uiButton));
    }

    //=== Э���߼� ===//
    IEnumerator MoveToolCoroutine(int index)
    {
        TeaToolData tool = tools[index];
        Vector3 targetPosition = mainCamera.transform.position +
                                mainCamera.transform.forward * moveDistance;
        Vector3 startPos = tool.targetObject.position;
        float t = 0;


        // �ƶ�����
        while (t < 1)
        {
            t += Time.deltaTime * moveSpeed;
            tool.targetObject.position = Vector3.Lerp(startPos, targetPosition, t);
            yield return null;
        }
        tool.targetObject.position = targetPosition;

        // ��ʾ������壨������Ч����
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

        // ��ȫ͸��ʱ�������߼��
        if (targetAlpha == 0)
        {
            _descriptionCanvasGroup.blocksRaycasts = false;
            _descriptionPanel.SetActive(false);
        }
    }

    //=== ���߷��� ===//
    IEnumerator HighlightButton(Button btn)
    {
        var colors = btn.colors;
        Color originalColor = _originalButtonColors[btn];

        // �޸���ɫ
        colors.normalColor = buttonHighlightColor;
        btn.colors = colors;

        yield return new WaitForSeconds(highlightDuration);

        // �ָ���ɫ
        colors.normalColor = originalColor;
        btn.colors = colors;
    }

    void ResetToolPosition(int index)
    {
        tools[index].targetObject.position = _originalPositions[index];
    }

    //=== UI��̬�� ===//
    void FindUIComponents()
    {
        // ����˵�����
        _descriptionPanel = GameObject.Find("DescriptionPanel");
        if (_descriptionPanel == null)
        {
            Debug.LogError("δ�ҵ���Ϊ'DescriptionPanel'��UI��壡");
            return;
        }

        // �����ı����
        _descriptionText = _descriptionPanel.GetComponentInChildren<Text>();
        if (_descriptionText == null)
            Debug.LogError("�����ȱ��Text�����");

        // ����CanvasGroup���
        _descriptionCanvasGroup = _descriptionPanel.GetComponent<CanvasGroup>();
        if (_descriptionCanvasGroup == null)
            Debug.LogError("�����Ҫ���CanvasGroup�����");
    }

    //=== �������ڹ��� ===//
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

    //=== ��λ���� ===//
    public void ResetAllTools()
    {
        StartCoroutine(ResetToolsAnimation());
    }

    IEnumerator ResetToolsAnimation()
    {
        // �������
        if (_descriptionCanvasGroup != null && _descriptionPanel.activeSelf)
            yield return StartCoroutine(FadePanel(1, 0, 0.3f));

        // ��λ��������
        for (int i = 0; i < tools.Count; i++)
            tools[i].targetObject.position = _originalPositions[i];
    }
}