using UnityEngine;

public class Sound
{
    public enum SoundType
    {
        INVESTIGATIVE,
        DANGEROUS,
        DEFAULT
    }

    public readonly SoundType soundType;
    public readonly float range;
    public readonly Vector3 position;

    public Sound(Vector3 _pos, float _range = 100f, SoundType _type = SoundType.DEFAULT)
    {
        position = _pos;
        range = _range;
        soundType = _type;
    }
}
