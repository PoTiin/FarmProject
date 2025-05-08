using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class ItemDetails
{
    public int itemId;
    public string itemName;
    public ItemType itemType;
    public Sprite itemIcon;
    public Sprite itemOnWorldSprite;
    public string itemDescription;
    public int itemUseRadius;
    public bool canPickedUp;
    public bool canDropped;
    public bool canCarried;
    public int itemPrice;
    [Range(0,1)]
    public float sellPercentage;
    public ItemDetails()
    {

    }
    public ItemDetails(ItemDetails itemDetails)
    {
        this.itemId = itemDetails.itemId;
        this.itemName = itemDetails.itemName;
        this.itemType = itemDetails.itemType;
        this.itemIcon = itemDetails.itemIcon;
        this.itemOnWorldSprite = itemDetails.itemOnWorldSprite;
        this.itemDescription = itemDetails.itemDescription;
        this.itemUseRadius = itemDetails.itemUseRadius;
        this.canPickedUp = itemDetails.canPickedUp;
        this.canDropped = itemDetails.canDropped;
        this.canCarried = itemDetails.canCarried;
        this.itemPrice = itemDetails.itemPrice;
        this.sellPercentage = itemDetails.sellPercentage;
    }
}
[System.Serializable]
public struct InventoryItem
{
    public int itemID;
    public int itemAmount;
}
