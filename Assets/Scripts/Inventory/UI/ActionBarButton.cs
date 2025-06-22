using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MFarm.Inventory
{
    [RequireComponent(typeof(SlotUI))]
    public class ActionBarButton : MonoBehaviour
    {
        public KeyCode key;
        private SlotUI slotUI;

        private void Awake()
        {
            slotUI = GetComponent<SlotUI>();
        }
        private void Update()
        {
            if (Input.GetKeyDown(key))
            {
                slotUI.isSelected = !slotUI.isSelected;
                if (slotUI.isSelected)
                {
                    slotUI.inventoryUI.UpdateSlotHighlight(slotUI.slotIndex);
                }
                else
                {
                    slotUI.inventoryUI.UpdateSlotHighlight(-1);
                }
                EventHandler.CallItemSelectedEvent(slotUI.itemDetails, slotUI.isSelected);
            }
        }
    }
}