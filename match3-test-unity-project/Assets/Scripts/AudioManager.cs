using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public float music_multiplier, sound_multiplier;
    public Slider music_slider, sound_slider;
    public Audio[] audios;
    // Start is called before the first frame update
    void Awake()
    {

        instance = this;


        foreach (Audio audio_ in audios)
        {

            AudioSource current_audio_ = gameObject.AddComponent<AudioSource>();

            current_audio_.clip = audio_.audio_clip;

            current_audio_.playOnAwake = audio_.play_on_awake;
            current_audio_.loop = audio_.loop;

            current_audio_.volume = audio_.volume;

            current_audio_.spatialBlend = audio_.spatial_blend;
            current_audio_.pitch = audio_.pitch;

            audio_.audio_source = current_audio_;

        }

    }

    public void playAudio(string audio_name_)
    {

        bool has_find = false;

        foreach (Audio audio_ in audios)
        {

            if (audio_.name == audio_name_)
            {

                audio_.audio_source.Play();

                has_find = true;

            }

        }

        if (!has_find)
        {

            Debug.Log("- Audio not found!");

        }

    }

    public void stopAudio(string audio_name_)
    {

        bool has_find = false;

        foreach (Audio audio_ in audios)
        {

            if (audio_.name == audio_name_)
            {

                audio_.audio_source.Stop();

                has_find = true;

            }

        }

        if (!has_find)
        {

            Debug.Log("- Audio not found!");

        }

    }


    // Update is called once per frame
    void Update()
    {

        sound_multiplier = sound_slider.value;
        music_multiplier = music_slider.value;

        foreach (Audio audio_ in audios)
        {

            if (audio_.audio_type == AudioType.Sound)
            {

                audio_.audio_source.volume = audio_.volume * sound_multiplier;

            }
            else
            {

                audio_.audio_source.volume = audio_.volume * music_multiplier;

            }

        }

    }
}
