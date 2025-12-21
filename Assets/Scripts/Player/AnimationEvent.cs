using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEvent : MonoBehaviour
{
    public void FootstepHardSound()
    {
        EventHandler.CallPlaySoundEvent(SoundName.FootStepHard);
    }
    public void FootstepSoftSound()
    {
        EventHandler.CallPlaySoundEvent(SoundName.FootStepSoft);
    }
}
