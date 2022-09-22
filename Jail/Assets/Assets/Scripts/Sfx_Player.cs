using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Audio;

public class Sfx_Player : MonoBehaviour
{
    [SerializeField]
    private AudioClip[] stepsClips;
    

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Step()
    {
        AudioClip stepsClip = GetRandomClip();
        audioSource.PlayOneShot(stepsClip);
    }


    private AudioClip GetRandomClip()
    {
        return stepsClips[UnityEngine.Random.Range(0, stepsClips.Length)];

    }
}
