using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    [SerializeField] AudioSource sfxChannel;
    public void PlaySfx(AudioClip clip)
    {
        sfxChannel.clip = clip;
        sfxChannel.Play();
    }
}
