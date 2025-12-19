using System.Collections;
using System.Collections.Generic;
using MFarm.CropPlant;
using MFarm.Inventory;
using MFarm.Map;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CursorManager : MonoBehaviour
{
    public Sprite normal, tool, seed,item;
    private Sprite currentSprite;
    private Image cursorImage;
    private RectTransform cursorCanvas;

    private Image buildImage;

    private Camera mainCamera;
    private Grid currentGrid;

    private Vector3 mouseWorldPos;
    private Vector3Int mouseGridPos;

    private bool cursorEnable;

    private bool cursorPositionVaild;

    private ItemDetails currentItem;
    private Transform playerTransform => FindObjectOfType<Player>().transform;
    private void OnEnable()
    {
        EventHandler.ItemSelectedEvent += OnItemSelectedEvent;
        EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;

    }
    private void OnDisable()
    {
        EventHandler.ItemSelectedEvent -= OnItemSelectedEvent;
        EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadedEvent -= OnAfterSceneLoadedEvent;
    }

    

    private void Start()
    {
        cursorCanvas = GameObject.FindGameObjectWithTag("CursorCanvas").GetComponent<RectTransform>();
        cursorImage = cursorCanvas.Find("Cursor Image").GetComponent<Image>();
        //拿到建造图标
        buildImage = cursorCanvas.Find("Build Cursor").GetComponent<Image>();
        buildImage.gameObject.SetActive(false);
        currentSprite = normal;
        SetCursorImage(normal);
    }
    private void Update()
    {
        if (cursorCanvas == null) return;
        cursorImage.transform.position = Input.mousePosition;
        if (!InteractWithUI() && cursorEnable)
        {
            SetCursorImage(currentSprite);
            CheckCursorValid();
            CheckPlayerInput();
        }
        else
        {
            SetCursorImage(normal);
            buildImage.gameObject.SetActive(false);
        }
        
    }

    private void CheckPlayerInput()
    {
        if (Input.GetMouseButtonDown(0) && cursorPositionVaild)
        {
            EventHandler.CallMouseClickEvent(mouseWorldPos, currentItem);
        }
    }
    
    
    private void OnBeforeSceneUnloadEvent()
    {
        cursorEnable = false;
    }
    private void OnAfterSceneLoadedEvent()
    {
        currentGrid = FindObjectOfType<Grid>();
        mainCamera = Camera.main;
    }
    private void OnItemSelectedEvent(ItemDetails itemDetails, bool isSelected)
    {
        if (!isSelected)
        {
            currentItem = null;
            cursorEnable = false;
            currentSprite = normal;
            buildImage.gameObject.SetActive(false);
        }
        else
        {
            currentItem = itemDetails;
            //WORKFLOW:添加所有类型图片
            currentSprite = itemDetails.itemType switch
            {
                ItemType.Seed => seed,
                ItemType.ChopTool => tool,
                ItemType.Commodity => item,
                ItemType.HoeTool => tool,
                ItemType.WaterTool=> tool,
                ItemType.BreakTool => tool,
                ItemType.ReapTool => tool,
                ItemType.Furniture => tool,
                ItemType.CollectTool => tool,
                _ => normal

            };
            cursorEnable = true;

            if(itemDetails.itemType == ItemType.Furniture)
            {
                buildImage.gameObject.SetActive(true);
                buildImage.sprite = itemDetails.itemOnWorldSprite;
                buildImage.SetNativeSize();
            }
        }
            
    }
    #region 设置鼠标样式
    private void SetCursorImage(Sprite sprite)
    {
        cursorImage.sprite = sprite;
        cursorImage.color = new Color(1, 1, 1, 1);
    }
    private void SetCursorVaild()
    {
        cursorPositionVaild = true;
        cursorImage.color = new Color(1, 1, 1, 1);
        buildImage.color = new Color(1, 1, 1, 0.5f);
    }
    private void SetCursorInVaild()
    {
        cursorPositionVaild = false;
        cursorImage.color = new Color(1, 0, 0, 0.4f);
        buildImage.color = new Color(1, 0, 0, 0.5f);
    }
    #endregion
    private void CheckCursorValid()
    {
        mouseWorldPos = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -mainCamera.transform.position.z));
        mouseGridPos = currentGrid.WorldToCell(mouseWorldPos);
        var playerGridPos = currentGrid.WorldToCell(playerTransform.position);

        //建造图片跟随移动
        buildImage.rectTransform.position = Input.mousePosition;


        if(Mathf.Abs(mouseGridPos.x - playerGridPos.x) > currentItem.itemUseRadius || Mathf.Abs(mouseGridPos.y - playerGridPos.y) > currentItem.itemUseRadius)
        {
            SetCursorInVaild();
            return;
        }
        TileDetails currentTile = GridMapManager.Instance.GetTileDetailsOnMousePosition(mouseGridPos);
        if (currentTile != null)
        {
            CropDetails currentCrop = CropManager.Instance.GetCropDetails(currentTile.seedItemID);
            Crop crop = GridMapManager.Instance.GetCropObject(mouseWorldPos);
            //WORKFLOW:补充所有物品类型的判断
            switch (currentItem.itemType)
            {
                case ItemType.Seed:
                    if (currentTile.daysSinceDug > -1 && currentTile.seedItemID == -1) SetCursorVaild(); else SetCursorInVaild();
                        break;
                case ItemType.Commodity:
                    if (currentTile.canDropItem && currentItem.canDropped) SetCursorVaild(); else SetCursorInVaild();
                    break;
                case ItemType.Furniture:
                    if (currentTile.canPlaceFurniture && InventoryManager.Instance.CheckStock(currentItem.itemId)) SetCursorVaild(); else SetCursorInVaild();
                    
                    break;
                case ItemType.HoeTool:
                    if (currentTile.canDig) SetCursorVaild(); else SetCursorInVaild();
                    break;
                case ItemType.ReapTool:
                    if(GridMapManager.Instance.HaveReapableItemsInRadius(mouseWorldPos,currentItem)) SetCursorVaild(); else SetCursorInVaild();
                    break;
                case ItemType.WaterTool:
                    if (currentTile.daysSinceDug > -1 && currentTile.daysSinceWatered == -1) SetCursorVaild(); else SetCursorInVaild();
                    break;
                case ItemType.BreakTool:
                case ItemType.ChopTool:
                    if(crop != null)
                    {
                        if(crop.CanHarvest && crop.cropDetails.CheckToolAvailable(currentItem.itemId)) SetCursorVaild(); else SetCursorInVaild();
                    }
                    else
                    {
                        SetCursorInVaild();
                    }
                        break;
                case ItemType.CollectTool:
                    if(currentCrop != null)
                    {
                        if (currentCrop.CheckToolAvailable(currentItem.itemId))
                        {
                            if (currentTile.growthDays >= currentCrop.TotalGrowthDays) SetCursorVaild(); else SetCursorInVaild();
                        } else SetCursorInVaild();
                    }
                    else
                    {
                        SetCursorInVaild();
                    }
                    break;
                case ItemType.ReapableScenery:
                    break;
                default:
                    break;
            }
        }
        else
        {
            SetCursorInVaild();
        }
    }
    /// <summary>
    /// 是否与UI互动
    /// </summary>
    /// <returns></returns>
    private bool InteractWithUI()
    {
        if(EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            return true;
        }
        return false;
    }
}
