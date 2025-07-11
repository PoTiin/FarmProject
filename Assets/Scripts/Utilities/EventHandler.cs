using System;
using System.Collections.Generic;
using UnityEngine;

public static class EventHandler
{
    public static event Action<InventoryLocation, List<InventoryItem>> UpdateInventoryUI;
    public static void CallUpdateInventoryUI(InventoryLocation inventoryLocation, List<InventoryItem> inventoryItems )
    {
        UpdateInventoryUI?.Invoke(inventoryLocation, inventoryItems);
    }

    public static event Action<int, Vector3> InstantiateItemInScene;
    public static void CallInstantiateItemInScene(int ID,Vector3 pos)
    {
        InstantiateItemInScene?.Invoke(ID, pos);
    }
    public static event Action<int, Vector3,ItemType> DropItemEvent;
    public static void CallDropItemEvent(int ID, Vector3 pos,ItemType itemType)
    {
        DropItemEvent?.Invoke(ID, pos, itemType);
    }
    public static event Action<ItemDetails, bool> ItemSelectedEvent;
    public static void CallItemSelectedEvent(ItemDetails itemDetails,bool isSelected)
    {
        ItemSelectedEvent?.Invoke(itemDetails, isSelected);
    }

    public static event Action<int, int,int,Season> GameMinuteEvent;
    public static void CallGameMinuteEvent(int minute,int hour,int day,Season season)
    {
        GameMinuteEvent?.Invoke(minute, hour, day, season);
    }
    public static event Action<int, Season> GameDayEvent;
    public static void CallGameDayEvent(int minute, Season season)
    {
        GameDayEvent?.Invoke(minute, season);
    }
    public static event Action<int, int, int,int, Season> GameDataEvent;
    public static void CallGameDataEvent(int hour,int day,int month,int year,Season season)
    {
        GameDataEvent?.Invoke(hour, day, month, year, season);
    }

    public static event Action<string, Vector3> TransitionEvent;

    public static void CallTransitionEvent(string sceneName,Vector3 pos)
    {
        TransitionEvent?.Invoke(sceneName, pos);
    }

    public static event Action BeforeSceneUnloadEvent;
    public static void CallBeforeSceneUnloadEvent()
    {
        BeforeSceneUnloadEvent?.Invoke();
    }

    public static event Action AfterSceneLoadedEvent;
    public static void CallAfterSceneLoadedEvent()
    {
        AfterSceneLoadedEvent?.Invoke();
    }

    public static event Action<Vector3> MoveToPosition;

    public static void CallMoveToPosition(Vector3 pos)
    {
        MoveToPosition?.Invoke(pos);
    }
    public static event Action<Vector3, ItemDetails> MouseClickEvent;
    public static void CallMouseClickEvent(Vector3 pos,ItemDetails itemDetails)
    {
        MouseClickEvent?.Invoke(pos, itemDetails);
    }
    public static event Action<Vector3, ItemDetails> ExecuteActionAfterAnimationEvent;
    public static void CallExecuteActionAfterAnimationEvent(Vector3 pos, ItemDetails itemDetails)
    {
        ExecuteActionAfterAnimationEvent?.Invoke(pos, itemDetails);
    }
    public static event Action<int, TileDetails> PlantSeedEvent;
    public static void CallPlantSeedEvent(int ID, TileDetails tileDetails)
    {
        PlantSeedEvent?.Invoke(ID, tileDetails);
    }

    public static event Action<int> HarvestAtPlayerPosition;
    public static void CallHarvestAtPlayerPosition(int ID)
    {
        HarvestAtPlayerPosition?.Invoke(ID);
    }
    public static event Action RefreshCurrentMap;
    public static void CallRefreshCurrentMap()
    {
        RefreshCurrentMap?.Invoke();
    }

    public static event Action<ParticaleEffectType, Vector3> ParticleEffectEvent;
    public static void CallParticleEffectEvent(ParticaleEffectType effectType, Vector3 pos)
    {
        ParticleEffectEvent?.Invoke(effectType,pos);
    }

    public static event Action GenerateCropEvent;
    public static void CallGenerateCropEvent()
    {
        GenerateCropEvent?.Invoke();
    }
}
