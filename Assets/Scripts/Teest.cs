using UnityEngine;
using UnityEngine.UI;

public class ClickableObject : MonoBehaviour
{
    private Vector3 _initialPosition; // ��ʼλ��
    public static GameObject CurrentSelectedObject; // ��ǰѡ�����壨��̬������

    private void Start()
    {
        _initialPosition = transform.position; // �����ʼλ��
    }

    // ����¼�������������󶨵���ť����ײ����
    public void OnObjectClicked()
    {
        // ���֮ǰ��ѡ�����壬���λ
        if (CurrentSelectedObject != null && CurrentSelectedObject != gameObject)
        {
            CurrentSelectedObject.transform.position =
                CurrentSelectedObject.GetComponent<ClickableObject>()._initialPosition;
        }

        // ��������õ�ǰ�����ƶ�����ͷǰ�Ĵ����߼�
        // ��������һ������MoveToCameraFront()�������ƶ�
        MoveToCameraFront();

        // ��ʾ��ؽ��ܵĴ����߼�Ҳ���������

        // ���µ�ǰѡ������Ϊ��ǰ���������
        CurrentSelectedObject = gameObject;
    }

    // ������ƶ�����ͷǰ�ķ���������Ҫ����ʵ���������
    private void MoveToCameraFront()
    {
        // ��ȡ���λ�úͷ������Ϣ��Ȼ����������ƶ�����ͷǰ��Ŀ��λ��
        // ����
        Camera mainCamera = Camera.main;
        Vector3 targetPosition = mainCamera.transform.position + mainCamera.transform.forward * 2;
        transform.position = targetPosition;
    }

    // ��ӵ�OnMouseDown������������3D���屻���ʱ����OnObjectClicked����
    private void OnMouseDown()
    {
        OnObjectClicked();
    }
  
}





