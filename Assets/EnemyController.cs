using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WSGJ.Utils;
using WSGJ.CommonEnum;


public class EnemyController : MonoSingleton<EnemyController>
{

    void Start()
    {
        StartCoroutine(ChangeDirectionInterval());
    }

    void ChangeDirection()
    {
        var enemies = FindObjectsOfType<SpaceInvanderEntity>();

        foreach (var enemy in enemies)
        {
            if (enemy.IsAttacking) continue;
            if (enemy.direction == Directions.up) enemy.direction = Directions.down;
            else if (enemy.direction == Directions.down) enemy.direction = Directions.up;
            else if (enemy.direction == Directions.left) enemy.direction = Directions.right;
            else if (enemy.direction == Directions.right) enemy.direction = Directions.left;
        }
    }

    IEnumerator ChangeDirectionInterval()
    {
        yield return new WaitForSeconds(1f);
        ChangeDirection();
        StartCoroutine(ChangeDirectionInterval());
    }


    void Update()
    { 
    }
}
