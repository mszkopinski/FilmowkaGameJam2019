using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatapultRock : MonoBehaviour
{

    [SerializeField]
    private float speed = 5f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
         Shot();
    }

       private void Shot()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime);

        if (transform.position.y >= 5.4f)
        {
            Destroy(gameObject);
        } 
    }

}
