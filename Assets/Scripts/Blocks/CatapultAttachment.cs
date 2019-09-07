using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WSGJ
{
    public class CatapultAttachment : MonoBehaviour
    {
        [SerializeField, Header("Catapult Settings")]
        GameObject rockPrefab;
        [SerializeField]
        float delayBetweenThrows = 1.5f;
        [SerializeField]
        Transform[] spawnSlots;

        Animator animator;
        readonly int isThrowingHash = Animator.StringToHash("isThrowing");
        readonly List<CatapultRock> projectiles = new List<CatapultRock>();
        AudioSource thisAudioSource;
        FallingBlock parentBlock;

        void Awake()
        {
            animator = GetComponent<Animator>();
            thisAudioSource = GetComponent<AudioSource>();
            parentBlock = GetComponentInParent<FallingBlock>();
        }

        void Start()
        {
            StartCoroutine(ThrowCoroutine());

            foreach(var spawnSlot in spawnSlots)
            {
                var spawnedProjectile = Instantiate(rockPrefab, spawnSlot.transform)
                    .GetComponent<CatapultRock>();
                
                projectiles.Add(spawnedProjectile);
            }
        }

        IEnumerator ThrowCoroutine()
        {
            while(true)
            {
                yield return new WaitForSeconds(delayBetweenThrows);
                animator.SetTrigger(isThrowingHash);
            }
        }

        // invoked by animator events
        void ThrowRock(int rockIndex)
        {
            var rockToThrow = projectiles.ElementAtOrDefault(rockIndex);

            if(rockToThrow == null)
            {
                var spawnSlot = spawnSlots.ElementAtOrDefault(rockIndex);
                if(spawnSlot != null)
                {
                    var spawnedProjectile = Instantiate(rockPrefab, spawnSlot.transform)
                        .GetComponent<CatapultRock>();
                
                    projectiles.Add(spawnedProjectile);  
                //thisAudioSource.clip = SoundManager.Instance.CatapultShot;
				//thisAudioSource.PlayOneShot(SoundManager.Instance.CatapultShot);                  
                }
            }

            if(parentBlock == null)
            {
                parentBlock = GetComponentInParent<FallingBlock>();
            }
            
            if(rockToThrow != null && parentBlock != null)
            {
                rockToThrow.Launch(parentBlock.transform.right);
                projectiles.Remove(rockToThrow);
            }
        }
    }
}