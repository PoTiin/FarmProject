using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

namespace MFarm.Inventory
{
    public class InventoryManager : Singleton<InventoryManager>
    {
        [Header("物品数据")]
        public ItemDataList_SO itemDataList_SO;
        [Header("背包数据")]
        public InventoryBag_SO playerBag;
        private void OnEnable()
        {
            EventHandler.DropItemEvent += OnDropItemEvent;
            EventHandler.HarvestAtPlayerPosition += OnHarvestAtPlayerPosition;
        }
        private void OnDisable()
        {
            EventHandler.DropItemEvent -= OnDropItemEvent;
            EventHandler.HarvestAtPlayerPosition -= OnHarvestAtPlayerPosition;
        }

        

        private void Start()
        {
            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);
        }
        private void OnHarvestAtPlayerPosition(int ID)
        {
            var index = GetItemIndexInBag(ID);
            AddItemAtIndex(ID, index, 1);
            //更新UI
            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);
        }
        private void OnDropItemEvent(int ID, Vector3 pos,ItemType itemType)
        {
            RemoveItem(ID, 1);
        }
        /// <summary>
        /// 通过ID返回物体信息
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public ItemDetails GetItemDetails(int ID)
        {
            return itemDataList_SO.itemDetailsList.Find(i => i.itemId == ID);
        }
        /// <summary>
        /// 添加物品到Player
        /// </summary>
        /// <param name="item"></param>
        /// <param name="toDestory">是否销毁物品</param>
        public void AddItem(Item item, bool toDestory)
        {

            //背包是否有该物品
            var index = GetItemIndexInBag(item.itemID);
            AddItemAtIndex(item.itemID, index, 1);

            Debug.Log(GetItemDetails(item.itemID).itemName);
            if (toDestory)
            {
                Destroy(item.gameObject);
            }

            //更新UI
            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);
        }
        /// <summary>
        /// 检查背包是否有空位
        /// </summary>
        /// <returns></returns>
        private bool CheckBagCapacity()
        {
            for (int i = 0; i < playerBag.itemList.Count; i++)
            {
                if (playerBag.itemList[i].itemID == 0)
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 通过物品ID找到背包已有物品位置
        /// </summary>
        /// <param name="ID">物品ID</param>
        /// <returns>返回物品序号否则返回-1</returns>
        private int GetItemIndexInBag(int ID)
        {
            for (int i = 0; i < playerBag.itemList.Count; i++)
            {
                if (playerBag.itemList[i].itemID == ID)
                {
                    return i;
                }
            }
            return -1;
        }
        /// <summary>
        /// 在指定背包序号位置添加物品
        /// </summary>
        /// <param name="ID">物品ID</param>
        /// <param name="index">序号</param>
        /// <param name="amount">数量</param>
        private void AddItemAtIndex(int ID, int index, int amount)
        {
            if(index == -1 && CheckBagCapacity()) //背包没有这个物品 同时背包有空位
            {
                var item = new InventoryItem { itemID = ID, itemAmount = amount };
                for (int i = 0; i < playerBag.itemList.Count; i++)
                {
                    if (playerBag.itemList[i].itemID == 0)
                    {
                        playerBag.itemList[i] = item;
                        break;
                    }
                }
            }
            else if(index != -1) //背包有这个物品
            {
                int currentAmount = playerBag.itemList[index].itemAmount + amount;
                var item = new InventoryItem { itemID = ID, itemAmount = currentAmount };
                playerBag.itemList[index] = item;
            }
        }
        /// <summary>
        /// player背包范围内交换
        /// </summary>
        /// <param name="fromIndex"></param>
        /// <param name="targetIndex"></param>
        public void SwapItem(int fromIndex, int targetIndex)
        {
            InventoryItem currentItem = playerBag.itemList[fromIndex];
            InventoryItem targetItem = playerBag.itemList[targetIndex];

            if(targetItem.itemID != 0)
            {
                playerBag.itemList[fromIndex] = targetItem;
                playerBag.itemList[targetIndex] = currentItem;
            }
            else
            {
                playerBag.itemList[targetIndex] = currentItem;
                playerBag.itemList[fromIndex] = new InventoryItem();
            }
            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);
        }

        /// <summary>
        /// 移除指定数量的背包物品
        /// </summary>
        /// <param name="ID">物品id</param>
        /// <param name="removeAmount">数量</param>
        private void RemoveItem(int ID,int removeAmount)
        {
            var index = GetItemIndexInBag(ID);
            if (playerBag.itemList[index].itemAmount > removeAmount)
            {
                var amount = playerBag.itemList[index].itemAmount - removeAmount;
                var item = new InventoryItem
                {
                    itemID = ID,
                    itemAmount = amount
                };
                playerBag.itemList[index] = item;
            }
            else if (playerBag.itemList[index].itemAmount == removeAmount)
            {
                var item = new InventoryItem();
                playerBag.itemList[index] = item;
            }
            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);
        }
    }
}

