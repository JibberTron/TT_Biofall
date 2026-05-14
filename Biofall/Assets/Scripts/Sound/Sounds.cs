using UnityEngine;

public static class Sounds
{
    public static void MakeSounds(Sound _sound)
    {
        Collider[] col = Physics.OverlapSphere(_sound.position, _sound.range);
        for(int i = 0; i < col.Length; ++i)
        {
            if (col[i].GetComponentInParent<ISound>()is ISound component)
            {
                component.ReactToSound(_sound);
            }
        }
    }
}
