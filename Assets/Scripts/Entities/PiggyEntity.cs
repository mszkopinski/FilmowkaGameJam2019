using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WSGJ;


public class PiggyEntity : MonoBehaviour
{
    Rigidbody2D rb2d;
    bool isLeftDirection;
    float speed = 165f;

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        transform.position += new Vector3(0f, Random.Range(0.5f, 2f), 0f);
        isLeftDirection = transform.position.x > 0;
    }

    private void FixedUpdate()
    {
        rb2d.velocity = Vector2.right * Time.deltaTime * speed * (isLeftDirection ? -1f : 1f);
        transform.localScale = new Vector3(isLeftDirection ? 1f : -1f, 1f, 1f);       
    }

    void Update()
    {
        var pointInViewport = Camera.main.WorldToViewportPoint(transform.position);
        if (pointInViewport.x > 1.5f || pointInViewport.x <-0.5f) Destroy(gameObject);
    }


}
