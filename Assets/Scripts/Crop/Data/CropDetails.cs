using UnityEngine;
[System.Serializable]
public class CropDetails
{
    public int seedItemID;
    [Header("不同阶段需要的天数")]
    public int[] growthDays;
    public int TotalGrowthDays
    {
        get
        {
            int amount = 0;
            foreach (var days in growthDays)
            {
                amount += days;
            }
            return amount;
        }
    }
    [Header("不同生长阶段物品Prefab")]
    public GameObject[] growthPrefabs;

    [Header("不同阶段的图片")]
    public Sprite[] growthSprites;

    [Header("可种植的季节")]
    public Season[] seasons;

    [Space]
    [Header("收割工具")]
    public int[] harvesToolItemID;

    [Header("每种工具使用次数")]
    public int[] requireActionCount;

    [Header("转换新物品ID")]
    public int transferItemID;

    [Space]
    [Header("收割果实信息")]
    public int[] producedItemID;
    public int[] producedMinAmount;
    public int[] ProduceMaxAmount;
    public Vector2 spawnRadius;

    [Header("再次生长时间")]
    public int daysToRegrow;
    public int regrowTimes;

    [Header("Options")]
    public bool generateAtPlayerPosition;
    public bool hasAnimation;
    public bool hasParticalEffect;
    //TODO:特效、音效
    public ParticaleEffectType effectType;
    public Vector3 effectPos;
    public bool CheckToolAvailable(int toolID)
    {
        foreach (var tool in harvesToolItemID)
        {
            if (tool == toolID)
                return true;
        }
        return false;
    }
    public int GetTotalRequireCount(int toolID)
    {
        for (int i = 0; i < harvesToolItemID.Length; i++)
        {
            if (harvesToolItemID[i] == toolID)
            {
                return requireActionCount[i];
            }
        }
        return -1;
    }
}
