using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Audio;

public class Sfx_Player : MonoBehaviour
{
    [SerializeField]
    private AudioClip[] clips;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Step()
    {
        AudioClip clip = GetRandomClip();
        audioSource.PlayOneShot(clip);
    }

    private AudioClip GetRandomClip()
    {
        return clips[UnityEngine.Random.Range(0, clips.Length)];
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
