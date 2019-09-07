using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WSGJ.CommonEnum;


public class SpaceInvanderEntity : MonoBehaviour
{
    public Directions direction;
    public bool IsAttacking = false;
    public Transform target;
    Rigidbody2D rb2d;

    float speed = 0.5f;
    


    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    
    void Update()
    {
        if (direction == Directions.down) rb2d.velocity = Vector2.down * speed;
        if (direction == Directions.up) rb2d.velocity = Vector2.up * speed;
        if (direction == Directions.right) rb2d.velocity = Vector2.right * speed;
        if (direction == Directions.left) rb2d.velocity = Vector2.left * speed;

    }

    private void FixedUpdate()
    {
        if (IsAttacking) rb2d.MovePosition(rb2d.position + (Vector2)(target.position - transform.position).normalized * Time.fixedDeltaTime);
    }
}
