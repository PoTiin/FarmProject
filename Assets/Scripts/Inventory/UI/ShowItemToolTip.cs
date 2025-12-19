using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
namespace MFarm.Inventory
{
    [RequireComponent(typeof(SlotUI))]
    public class ShowItemToolTip : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
    {
        private SlotUI slotUI;
        private InventoryUI inventoryUI => GetComponentInParent<InventoryUI>();

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (slotUI.itemDetails != null)
            {
                inventoryUI.itemTooltip.gameObject.SetActive(true);
                inventoryUI.itemTooltip.SetUpToolTip(slotUI.itemDetails, slotUI.slotType);
                inventoryUI.itemTooltip.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0);
                inventoryUI.itemTooltip.transform.position = transform.position + Vector3.up * 60;

                if(slotUI.itemDetails.itemType == ItemType.Furniture)
                {
                    inventoryUI.itemTooltip.resourcePanel.SetActive(true);
                    inventoryUI.itemTooltip.SetupResourcePanel(slotUI.itemDetails.itemId);
                }
                else
                {
                    inventoryUI.itemTooltip.resourcePanel.SetActive(false);
                }
            }
            else
            {
                inventoryUI.itemTooltip.gameObject.SetActive(false);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            inventoryUI.itemTooltip.gameObject.SetActive(false);
        }

        private void Awake()
        {
            slotUI = GetComponent<SlotUI>();
        }
    }
}

