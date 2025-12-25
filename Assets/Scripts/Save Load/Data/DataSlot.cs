using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MFarm.Save
{
    public class DataSlot
    {
        /// <summary>
        /// 进度条、string是GUID
        /// </summary>
        public Dictionary<string, GameSaveData> dataDict = new Dictionary<string, GameSaveData>();
    }
}