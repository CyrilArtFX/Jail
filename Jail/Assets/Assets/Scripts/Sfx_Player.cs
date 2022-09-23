using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Audio;

public class Sfx_Player : MonoBehaviour
{
    [SerializeField]
    private AudioClip[] stepsClips;
    [SerializeField]
    private AudioClip[] ladderClips;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Step()
    {
        AudioClip stepsClip = GetRandomStepClip();
        audioSource.PlayOneShot(stepsClip);
    }

    private void Ladder()
    {
        AudioClip ladderClip = GetRandomLadderClip();
        audioSource.PlayOneShot(ladderClip);
    }


    private AudioClip GetRandomStepClip()
    {
        return stepsClips[UnityEngine.Random.Range(0, stepsClips.Length)];
    }

    private AudioClip GetRandomLadderClip()
    {
        return ladderClips[UnityEngine.Random.Range(0, ladderClips.Length)];
    }
}
