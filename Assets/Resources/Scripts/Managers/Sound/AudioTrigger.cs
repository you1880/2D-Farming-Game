using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioTrigger : MonoBehaviour
{
    [SerializeField] public Define.EffectSoundType EffectSoundType;
    [SerializeField] public float MinInterval;
    private float _lastPlayed = -999.0f;

    public void PlaySound()
        => PlaySound(EffectSoundType);

    public void PlaySound(Define.EffectSoundType effectSoundType)
    {
        if (effectSoundType == Define.EffectSoundType.None)
        {
            return;
        }

        float now = Time.time;

        if (now - _lastPlayed < MinInterval)
        {
            return;
        }

        _lastPlayed = now;
        SoundBus.Raise(new SoundBus.SoundEvent
        {
            effectSoundType = effectSoundType,
        });
    }
}
