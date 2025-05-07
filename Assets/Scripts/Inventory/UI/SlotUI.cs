using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlotUI : MonoBehaviour
{
    [Header("�����ȡ")]
    [SerializeField] private Image slotImage;
    [SerializeField] private TextMeshProUGUI amountText;
    [SerializeField] private Image SlotHighlight;
    [SerializeField] private Button button;
    [Header("��������")]
    public SlotType slotType;
    public bool isSelected;

    //��Ʒ��Ϣ
    public ItemDetails itemDetails;
    public int itemAmount;
    private void Start()
    {
        isSelected = false;
        if(itemDetails.itemId == 0)
        {
            UpdateEmptySlot();
        }
    }
    /// <summary>
    /// ���¸���UI����Ϣ
    /// </summary>
    /// <param name="item">ItemDetails</param>
    /// <param name="amount">��������</param>
    public void UpdateSlot(ItemDetails item,int amount)
    {
        itemDetails = item;
        slotImage.sprite = item.itemIcon;
        itemAmount = amount;
        amountText.text = amount.ToString();
        button.interactable = true;
    }
    /// <summary>
    /// ��Slot����Ϊ��
    /// </summary>
    public void UpdateEmptySlot()
    {
        if (isSelected)
        {
            isSelected = false;
        }
        slotImage.enabled = false;
        amountText.text = string.Empty;
        button.interactable = false;
    }
}
