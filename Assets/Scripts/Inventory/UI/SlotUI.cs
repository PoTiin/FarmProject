using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
namespace MFarm.Inventory
{
    public class SlotUI : MonoBehaviour,IPointerClickHandler,IBeginDragHandler,IDragHandler,IEndDragHandler
    {
        [Header("组件获取")]
        [SerializeField] private Image slotImage;
        [SerializeField] private TextMeshProUGUI amountText;
        [SerializeField] public Image SlotHighlight;
        [SerializeField] private Button button;
        [Header("格子类型")]
        public SlotType slotType;
        public bool isSelected;

        public int slotIndex;

        //物品信息
        public ItemDetails itemDetails;
        public int itemAmount;
        public InventoryUI inventoryUI => GetComponentInParent<InventoryUI>();
        private void Start()
        {
            isSelected = false;
            if (itemDetails == null)
            {
                UpdateEmptySlot();
            }
        }
        /// <summary>
        /// 更新格子UI和信息
        /// </summary>
        /// <param name="item">ItemDetails</param>
        /// <param name="amount">持有数量</param>
        public void UpdateSlot(ItemDetails item, int amount)
        {
            if(amount == 0)
            {
                UpdateEmptySlot();
                return;
            }
            itemDetails = item;
            slotImage.sprite = item.itemIcon;
            slotImage.enabled = true;
            itemAmount = amount;
            amountText.text = amount.ToString();
            button.interactable = true;
        }
        /// <summary>
        /// 将Slot更新为空
        /// </summary>
        public void UpdateEmptySlot()
        {
            if (isSelected)
            {
                isSelected = false;
                inventoryUI.UpdateSlotHighlight(-1);
                EventHandler.CallItemSelectedEvent(itemDetails, isSelected);
            }
            itemDetails = null;
            slotImage.enabled = false;
            itemAmount = 0;
            amountText.text = string.Empty;
            button.interactable = false;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (itemAmount == 0) return;
            isSelected = !isSelected;

            inventoryUI.UpdateSlotHighlight(slotIndex);
            if(slotType == SlotType.Bag)
            {
                //通知物品被选中后播放动画
                EventHandler.CallItemSelectedEvent(itemDetails, isSelected);
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (itemDetails == null) return;
            inventoryUI.dragImage.enabled = true;
            inventoryUI.dragImage.sprite = slotImage.sprite;
            inventoryUI.dragImage.SetNativeSize();
            isSelected = true;
            inventoryUI.UpdateSlotHighlight(slotIndex);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (itemAmount == 0) return;
            inventoryUI.dragImage.transform.position = Input.mousePosition;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (itemAmount == 0) return;
            inventoryUI.dragImage.enabled = false;
            //Debug.Log(eventData.pointerCurrentRaycast.gameObject);

            if(eventData.pointerCurrentRaycast.gameObject != null)
            {
                SlotUI targetSlot = eventData.pointerCurrentRaycast.gameObject.GetComponent<SlotUI>();
                if (targetSlot == null) return;
                int targetIndex = targetSlot.slotIndex;
                //在player自身背包范围内交换
                if(slotType == SlotType.Bag && targetSlot.slotType == SlotType.Bag)
                {

                    //修正bug
                    ItemDetails targetDetails = targetSlot.itemDetails;
                    int targetAmount = targetSlot.itemAmount;
                    targetSlot.UpdateSlot(itemDetails, itemAmount);
                    UpdateSlot(targetDetails, targetAmount);


                    InventoryManager.Instance.SwapItem(slotIndex, targetIndex);

                    
                }
                inventoryUI.UpdateSlotHighlight(-1);
            }
            //else //测试扔在地上
            //{
            //    if (itemDetails.canDropped)
            //    {
            //        //鼠标对应世界地图坐标
            //        var pos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));
            //        EventHandler.CallInstantiateItemInScene(itemDetails.itemId, pos);
            //    }
                
            //}
        }

        
    }
}
