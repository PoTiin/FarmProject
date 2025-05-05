using UnityEngine;
using UnityEngine.UI;

public class ClickableObject : MonoBehaviour
{
    private Vector3 _initialPosition; // 初始位置
    public static GameObject CurrentSelectedObject; // 当前选中物体（静态变量）

    private void Start()
    {
        _initialPosition = transform.position; // 保存初始位置
    }

    // 点击事件触发方法（需绑定到按钮或碰撞器）
    public void OnObjectClicked()
    {
        // 如果之前有选中物体，则归位
        if (CurrentSelectedObject != null && CurrentSelectedObject != gameObject)
        {
            CurrentSelectedObject.transform.position =
                CurrentSelectedObject.GetComponent<ClickableObject>()._initialPosition;
        }

        // 这里添加让当前物体移动到镜头前的代码逻辑
        // 假设你有一个方法MoveToCameraFront()来处理移动
        MoveToCameraFront();

        // 显示相关介绍的代码逻辑也在这里添加

        // 更新当前选中物体为当前点击的物体
        CurrentSelectedObject = gameObject;
    }

    // 假设的移动到镜头前的方法，你需要根据实际情况完善
    private void MoveToCameraFront()
    {
        // 获取相机位置和方向等信息，然后计算物体移动到镜头前的目标位置
        // 比如
        Camera mainCamera = Camera.main;
        Vector3 targetPosition = mainCamera.transform.position + mainCamera.transform.forward * 2;
        transform.position = targetPosition;
    }

    // 添加的OnMouseDown方法，用于在3D物体被点击时触发OnObjectClicked方法
    private void OnMouseDown()
    {
        OnObjectClicked();
    }
  
}





