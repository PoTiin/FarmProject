using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MFarm.Dialogue
{
    [Serializable]
    public class DialoguePiece
    {
        [Header("∂‘ª∞œÍ«È")]
        public Sprite faceImage;
        public bool onLeft;
        public string name;
        [TextArea]
        public string dialogueText;
        public bool hasToPause;
        [HideInInspector]public bool isDone;
    }
}