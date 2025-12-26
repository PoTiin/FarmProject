using MFarm.Save;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveSlotUI : MonoBehaviour
{
    public Text dataTime, dataScene;
    private Button currentButton;
    private DataSlot currentData;
    private int Index => transform.GetSiblingIndex();

    private void Awake()
    {
        currentButton = GetComponent<Button>();
        currentButton.onClick.AddListener(LoadGameData);
    }

    private void SetupSlotUI()
    {
        currentData = SaveLoadManager.Instance.dataSlots[Index];
        if(currentData != null)
        {
            dataTime.text = currentData.DataTime;

        }
    }

    private void LoadGameData()
    {
        Debug.Log(Index);
    }
}
