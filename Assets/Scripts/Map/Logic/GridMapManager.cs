using System.Collections;
using System.Collections.Generic;
using MFarm.CropPlant;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

namespace MFarm.Map
{
    public class GridMapManager : Singleton<GridMapManager>
    {
        private Grid currentGrid;
        [Header("�ֵ���Ƭ�л���Ϣ")]
        public RuleTile digTile;
        public RuleTile waterTile;
        private Tilemap digTilemap;
        private Tilemap waterTilemap;

        [Header("��ͼ��Ϣ")]
        public List<MapData_SO> mapDataList;
        //��������+����Ͷ�Ӧ��Ƭ��Ϣ
        private Dictionary<string, TileDetails> tileDetailsDict = new Dictionary<string, TileDetails>();
        private Dictionary<string, bool> firstLoadDict = new Dictionary<string, bool>();
        //�Ӳ��б�
        private List<ReapItem> itemInRadius;
        private Season currentSeason;


        private void OnEnable()
        {
            EventHandler.ExecuteActionAfterAnimationEvent += OnExecuteActionAfterAnimationEvent;
            EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;
            EventHandler.GameDayEvent += OnGameDayEvent;
            EventHandler.RefreshCurrentMap += RefreshMap;
        }
        private void OnDisable()
        {
            EventHandler.ExecuteActionAfterAnimationEvent -= OnExecuteActionAfterAnimationEvent;
            EventHandler.AfterSceneLoadedEvent -= OnAfterSceneLoadedEvent;
            EventHandler.GameDayEvent -= OnGameDayEvent;
            EventHandler.RefreshCurrentMap -= RefreshMap;
        }



        private void Start()
        {
            foreach (var mapData in mapDataList)
            {
                firstLoadDict.Add(mapData.sceneName, true);
                InitTileDetailsDict(mapData);
            }
        }
        private void InitTileDetailsDict(MapData_SO mapData)
        {
            foreach (TileProperty tileProperty in mapData.titleProperties)
            {
                TileDetails tileDetails = new TileDetails
                {
                    gridX = tileProperty.tileCoordinate.x,
                    gridY = tileProperty.tileCoordinate.y

                };
                //�ֵ�Key
                string key = $"{tileDetails.gridX}x{tileDetails.gridY}y{mapData.sceneName}";
                if (GetTileDetails(key) != null)
                {
                    tileDetails = GetTileDetails(key);
                }
                switch (tileProperty.gridType)
                {
                    case GridType.Diggable:
                        tileDetails.canDig = tileProperty.boolTypeValue;
                        break;
                    case GridType.DropItem:
                        tileDetails.canDropItem = tileProperty.boolTypeValue;
                        break;
                    case GridType.NPCObstacle:
                        tileDetails.isNPCObstacle = tileProperty.boolTypeValue;
                        break;
                    case GridType.PlaceFurniture:
                        tileDetails.canPlaceFurniture = tileProperty.boolTypeValue;
                        break;
                }
                if (GetTileDetails(key) != null)
                {
                    tileDetailsDict[key] = tileDetails;
                }
                else
                {
                    tileDetailsDict.Add(key, tileDetails);
                }
            }
        }

        public TileDetails GetTileDetails(string key)
        {
            if (!tileDetailsDict.ContainsKey(key))
            {
                return null;
            }
            return tileDetailsDict[key];
        }
        /// <summary>
        /// ��������������귵����Ƭ��Ϣ
        /// </summary>
        /// <param name="mouseGridPos">�����������</param>
        /// <returns></returns>
        public TileDetails GetTileDetailsOnMousePosition(Vector3Int mouseGridPos)
        {
            string key = $"{mouseGridPos.x}x{mouseGridPos.y}y{SceneManager.GetActiveScene().name}";
            return GetTileDetails(key);
        }
        /// <summary>
        /// ִ��ʵ�ʹ��߻���Ʒ����
        /// </summary>
        /// <param name="mouseWorldPos"></param>
        /// <param name="itemDetails"></param>
        private void OnExecuteActionAfterAnimationEvent(Vector3 mouseWorldPos, ItemDetails itemDetails)
        {
            var mouseGridPos = currentGrid.WorldToCell(mouseWorldPos);
            var currentTile = GetTileDetailsOnMousePosition(mouseGridPos);
            if (currentTile != null)
            {
                Crop currentCrop = GetCropObject(mouseWorldPos);
                //WORKFLOW:��Ʒʹ��ʵ�ʹ���
                switch (itemDetails.itemType)
                {
                    case ItemType.Seed:
                        EventHandler.CallPlantSeedEvent(itemDetails.itemId, currentTile);
                        EventHandler.CallDropItemEvent(itemDetails.itemId, mouseWorldPos, itemDetails.itemType);
                        break;
                    case ItemType.Commodity:
                        EventHandler.CallDropItemEvent(itemDetails.itemId, mouseWorldPos, itemDetails.itemType);
                        break;
                    case ItemType.Furniture:

                        break;
                    case ItemType.HoeTool:
                        SetDigGround(currentTile);
                        currentTile.daysSinceDug = 0;
                        currentTile.canDig = false;
                        currentTile.canDropItem = false;
                        //��Ч
                        break;
                    
                    case ItemType.ReapTool:
                        var reapCount = 0;
                        for (int i = 0; i < itemInRadius.Count; i++)
                        {
                            EventHandler.CallParticleEffectEvent(ParticaleEffectType.ReapableScenery, itemInRadius[i].transform.position + Vector3.up);
                            itemInRadius[i].SpawnHarvestItems();
                            Destroy(itemInRadius[i].gameObject);
                            reapCount++;
                            if (reapCount >= Settings.reapAmount) break;
                        }
                        break;
                    case ItemType.WaterTool:
                        SetWaterGround(currentTile);
                        currentTile.daysSinceWatered = 0;
                        //��Ч
                        break;
                    case ItemType.BreakTool:
                    case ItemType.ChopTool:
                        //ִ���ո��
                        currentCrop?.ProcessToolAction(itemDetails, currentCrop.tileDetails);
                        break;
                    case ItemType.CollectTool:
                        //ִ���ո��
                        currentCrop.ProcessToolAction(itemDetails,currentTile);
                        break;
                    case ItemType.ReapableScenery:
                        break;
                    default:
                        break;
                }
                UpdateTileDetails(currentTile);
            }
        }
        /// <summary>
        /// ͨ���������ж������λ�õ�ũ����
        /// </summary>
        /// <param name="mouseWorldPos">�������</param>
        /// <returns></returns>
        public Crop GetCropObject(Vector3 mouseWorldPos)
        {
            Collider2D[] colliders = Physics2D.OverlapPointAll(mouseWorldPos);
            Crop currentCrop = null;
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].GetComponent<Crop>())
                {
                    currentCrop = colliders[i].GetComponent<Crop>();
                }
            }
            return currentCrop;
        }
        /// <summary>
        /// ���ع��߷�Χ�ڵ��Ӳ�
        /// </summary>
        /// <param name="tool">������Ϣ</param>
        /// <returns></returns>
        public bool HaveReapableItemsInRadius(Vector3 mouseWorldPos, ItemDetails tool)
        {
            itemInRadius = new List<ReapItem>();
            Collider2D[] colliders = new Collider2D[20];
            Physics2D.OverlapCircleNonAlloc(mouseWorldPos, tool.itemUseRadius, colliders);
            Debug.Log("colliders.Length:" + colliders.Length);
            Debug.Log("Input.mousePosition :" + Input.mousePosition);
            if (colliders.Length > 0)
            {
                for (int i = 0; i < colliders.Length; i++)
                {
                    if (colliders[i] != null)
                    {
                        if (colliders[i].GetComponent<ReapItem>())
                        {
                            var item = colliders[i].GetComponent<ReapItem>();
                            itemInRadius.Add(item);
                        }
                    }
                }
            }
            return itemInRadius.Count > 0;
        }
        private void OnAfterSceneLoadedEvent()
        {
            currentGrid = FindObjectOfType<Grid>();
            digTilemap = GameObject.FindGameObjectWithTag("Dig").GetComponent<Tilemap>();
            waterTilemap = GameObject.FindGameObjectWithTag("Water").GetComponent<Tilemap>();
            //Ԥ������ũ����
            if (firstLoadDict[SceneManager.GetActiveScene().name])
            {
                EventHandler.CallGenerateCropEvent();
                firstLoadDict[SceneManager.GetActiveScene().name] = false;
            }
            RefreshMap();
        }
        private void OnGameDayEvent(int day, Season season)
        {
            currentSeason = season;
            foreach (var tile in tileDetailsDict)
            {
                if (tile.Value.daysSinceWatered > -1)
                {
                    tile.Value.daysSinceWatered = -1;
                }
                if (tile.Value.daysSinceDug > -1)
                {
                    tile.Value.daysSinceDug++;
                }
                //����ָ�����ڿ���ʧ
                if (tile.Value.daysSinceDug > 5 && tile.Value.seedItemID == -1)
                {
                    tile.Value.daysSinceDug = -1;
                    tile.Value.canDig = true;
                    tile.Value.growthDays = -1;
                }
                if (tile.Value.seedItemID != -1)
                {
                    tile.Value.growthDays++;
                }
            }
            RefreshMap();
        }
        /// <summary>
        /// ��ʾ�ڿ���Ƭ
        /// </summary>
        /// <param name="tile"></param>
        private void SetDigGround(TileDetails tile)
        {
            Vector3Int pos = new Vector3Int(tile.gridX, tile.gridY, 0);
            if (digTilemap != null)
            {
                digTilemap.SetTile(pos, digTile);
            }
        }
        /// <summary>
        /// ��ʾ��ˮ��Ƭ
        /// </summary>
        /// <param name="tile"></param>
        private void SetWaterGround(TileDetails tile)
        {
            Vector3Int pos = new Vector3Int(tile.gridX, tile.gridY, 0);
            if (waterTilemap != null)
            {
                Debug.Log("��ˮ");
                waterTilemap.SetTile(pos, waterTile);
            }
        }
        /// <summary>
        /// ������Ƭ��Ϣ
        /// </summary>
        /// <param name="tileDetails"></param>
        public void UpdateTileDetails(TileDetails tileDetails)
        {
            string key = $"{tileDetails.gridX}x{tileDetails.gridY}y{SceneManager.GetActiveScene().name}";
            if (tileDetailsDict.ContainsKey(key))
            {
                tileDetailsDict[key] = tileDetails;
            }
            else
            {
                tileDetailsDict.Add(key, tileDetails);
            }
        }
        private void RefreshMap()
        {
            if (digTilemap != null)
                digTilemap.ClearAllTiles();
            if (waterTilemap != null)
                waterTilemap.ClearAllTiles();
            foreach (var crop in FindObjectsOfType<Crop>())
            {
                Destroy(crop.gameObject);
            }
            DisplayMap(SceneManager.GetActiveScene().name);
        }
        /// <summary>
        /// ��ʾ��ͼ��Ƭ
        /// </summary>
        /// <param name="sceneName">��������</param>
        private void DisplayMap(string sceneName)
        {
            foreach (var tile in tileDetailsDict)
            {
                var key = tile.Key;
                var tileDetails = tile.Value;

                if (key.Contains(sceneName))
                {
                    if (tileDetails.daysSinceDug > -1)
                    {
                        SetDigGround(tileDetails);
                    }
                    if (tileDetails.daysSinceWatered > -1)
                    {
                        SetWaterGround(tileDetails);
                    }
                    if (tileDetails.seedItemID > -1)
                    {
                        EventHandler.CallPlantSeedEvent(tileDetails.seedItemID, tileDetails);
                    }
                }
            }
        }
        /// <summary>
        /// ���ݳ������ֹ�������Χ�������Χ��ԭ��
        /// </summary>
        /// <param name="sceneName">��������</param>
        /// <param name="gridDimensions">����Χ</param>
        /// <param name="gridOrigin">����ԭ��</param>
        /// <returns>�Ƿ��е�ǰ��������Ϣ</returns>
        public bool GetGridDimensions(string sceneName,out Vector2Int gridDimensions,out Vector2Int gridOrigin)
        {
            gridDimensions = Vector2Int.zero;
            gridOrigin = Vector2Int.zero;
            foreach (var mapData in mapDataList)
            {
                if(mapData.sceneName == sceneName)
                {
                    gridDimensions.x = mapData.gridWidth;
                    gridDimensions.y = mapData.gridHeight;

                    gridOrigin.x = mapData.originX;
                    gridOrigin.y = mapData.originY;

                    return true;
                }
            }
            return false;
        }
    }
}

