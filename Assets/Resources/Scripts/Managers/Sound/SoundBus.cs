using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SoundBus
{
    public struct SoundEvent
    {
        public Define.EffectSoundType effectSoundType;
        public Vector3? position;
    }

    public static event Action<SoundEvent> OnSoundEvent;
    public static void Raise(in SoundEvent e)
        => OnSoundEvent?.Invoke(e);
}
