using UnityEngine;

public class testSound : MonoBehaviour
{
    [SerializeField] AudioSource source = null;
    [SerializeField] float range = 100f;
    [SerializeField] Sound.SoundType soundType = Sound.SoundType.DEFAULT;

    private void OnMouseDown()
    {
        if (source.isPlaying) return;

        source.Play();

        Sound sound = new Sound(transform.position, range, soundType);
        Sounds.MakeSounds(sound);
        Debug.Log($"Sound Position: {sound.position} Sound Range: {sound.range}");
    }
}
