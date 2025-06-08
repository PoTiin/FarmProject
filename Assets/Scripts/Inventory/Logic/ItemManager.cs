using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace MFarm.Inventory
{
    public class ItemManager : MonoBehaviour
    {
        public Item itemPrefab;
        public Item bounceItemPrefab;
        private Transform itemParent;
        private Dictionary<string, List<SceneItem>> sceneItemDict = new Dictionary<string, List<SceneItem>>();
        private Transform playerTransform => FindObjectOfType<Player>().transform;
        private void OnEnable()
        {
            EventHandler.InstantiateItemInScene += OnInstantiateItemInScene;
            EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
            EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;
            EventHandler.DropItemEvent += OnDropItemEvent;
        }
        private void OnDisable()
        {
            EventHandler.InstantiateItemInScene -= OnInstantiateItemInScene;
            EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
            EventHandler.AfterSceneLoadedEvent -= OnAfterSceneLoadedEvent;
            EventHandler.DropItemEvent -= OnDropItemEvent;
        }

        

        private void OnBeforeSceneUnloadEvent()
        {
            GetAllSceneItems();
        }

        private void OnAfterSceneLoadedEvent()
        {
            itemParent = GameObject.FindWithTag("ItemParent").transform;
            RecreateAllItems();
        }

        private void OnInstantiateItemInScene(int ID, Vector3 pos)
        {
            var item = Instantiate(itemPrefab, pos, Quaternion.identity, itemParent);
            item.Init(ID);
        }
        private void OnDropItemEvent(int ID, Vector3 mousePos)
        {
            //TODO:扔东西的效果
            var item = Instantiate(bounceItemPrefab, playerTransform.position, Quaternion.identity, itemParent);
            item.Init(ID);
            var dir = (mousePos - playerTransform.position).normalized;
            item.GetComponent<ItemBounce>().InitBounceItem(mousePos, dir);
        }
        /// <summary>
        /// 获取当前场景所有item
        /// </summary>
        private void GetAllSceneItems()
        {
            List<SceneItem> currentSceneItem = new List<SceneItem>();
            foreach (var item in FindObjectsOfType<Item>())
            {
                SceneItem sceneItem = new SceneItem
                {
                    itemID = item.itemID,
                    position = new SerializableVector3(item.transform.position)
                };

                currentSceneItem.Add(sceneItem);
            }

            if (sceneItemDict.ContainsKey(SceneManager.GetActiveScene().name))
            {
                //找到数据就更新item数据列表
                sceneItemDict[SceneManager.GetActiveScene().name] = currentSceneItem;
            }
            else 
            {
                //如果是新场景
                sceneItemDict.Add(SceneManager.GetActiveScene().name, currentSceneItem);
            }
        }

        private void RecreateAllItems()
        {
            List<SceneItem> currentSceneItems = new List<SceneItem>();
            if(sceneItemDict.TryGetValue(SceneManager.GetActiveScene().name,out currentSceneItems))
            {
                if(currentSceneItems != null)
                {
                    //清场
                    foreach (var item in FindObjectsOfType<Item>())
                    {
                        Destroy(item.gameObject);
                    }
                    //重建
                    foreach (var item in currentSceneItems)
                    {
                        Item newItem = Instantiate(itemPrefab, item.position.ToVector3(), Quaternion.identity, itemParent);
                        newItem.Init(item.itemID);
                    }
                }
            }
        }
    }
}
