using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class flickeringLight : MonoBehaviour
{
    [SerializeField] Light lights;
    [SerializeField] AudioSource sound;

    [SerializeField] float min;
    [SerializeField] float max;
    float timer;

    void Start()
    {
        if (lights == null)
        {
            Debug.Log("Light == null");
            return;
        }
        if(sound == null)
        {
            Debug.Log("Sound == null");
            return;
        }
        timer = Random.Range(min, max);
    }
    void Update()
    {
        LightsFlickering();
    }
    void LightsFlickering()
    {
        timer -= Time.deltaTime;

        if (timer > 0)
            return;

        timer = Random.Range(min, max);

        lights.enabled = !lights.enabled;

        if (!lights.enabled)
        {
            if (!sound.isPlaying)
            {
                sound.Play();
            }
        }
    }

}
