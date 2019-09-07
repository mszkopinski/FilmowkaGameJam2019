using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WSGJ.Utils;

public class SoundManager : MonoSingleton<SoundManager>
{
    // Start is called before the first frame update

    public AudioClip Theme;
    public AudioClip CatapultShot;
    public AudioClip BoxingHit;
    public AudioClip GreatAxe;
    public AudioClip BlockHitsTheCart;
    public AudioClip BlockHitsOtherBlock;
    public AudioClip BlockHitsGround;
    public AudioClip EnemyDies;

    
    void Start()
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        //audioSource.clip = Theme;
       // audioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayBlockHitsGround()
    {
        AudioSource audioSource = GetComponent<AudioSource>();
       audioSource.clip = BlockHitsGround;
       audioSource.Play();
    }
}
