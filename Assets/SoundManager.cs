using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WSGJ.Utils;

public class SoundManager : MonoSingleton<SoundManager>
{
    // Start is called before the first frame update

    [SerializeField] private AudioClip Theme;
    [SerializeField] private AudioClip CatapultShot;
    [SerializeField] private AudioClip BoxingHit;
    [SerializeField] private AudioClip GreatAxe;
    [SerializeField] private AudioClip BlockHitsTheCart;
    [SerializeField] private AudioClip BlockHitsOtherBlock;
    [SerializeField] private AudioClip BlockHitsOtherGround;
    [SerializeField] private AudioClip EnemyDies;

    
    void Start()
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.clip = Theme;
        audioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
