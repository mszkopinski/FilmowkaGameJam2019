using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatapultShooting : MonoBehaviour
{

    public GameObject rockPrefab; 
    public float ShooingRate = 1f;
    // Start is called before the first frame update
    void Start()
    {
        StartSpawning();
    }

    public void StartSpawning(){
        StartCoroutine(RockSpawnCoroutine());
    }

    // Update is called once per frame
    void Update()
    {
        
    }


   public IEnumerator RockSpawnCoroutine()
    {
        while(true) {
       
            Instantiate(rockPrefab, transform.position = this.transform.position, Quaternion.identity);
            yield return new WaitForSeconds(ShooingRate);
            }
    }

}