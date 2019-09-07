using DragonBones;
using UnityEngine;

namespace WSGJ
{
    public class PiggyEntity : MonoBehaviour
    {
        [SerializeField]
        float velocity = 165f;
        [SerializeField]
        float minSpawnHeight = 0.5f;
        [SerializeField]
        float maxSpawnHeight = 2f;

        UnityArmatureComponent armatureComponent;
        Rigidbody2D rb2d;
        bool isLeftDirection;
        bool canMove = true;

        void Awake()
        {
            armatureComponent = GetComponent<UnityArmatureComponent>();
            rb2d = GetComponent<Rigidbody2D>();
        }
        
        void Start()
        {
            transform.position += new Vector3(0f, 
                Random.Range(minSpawnHeight, maxSpawnHeight), 0f);
            isLeftDirection = transform.position.x > 0;
        }

        void FixedUpdate()
        {
            if(canMove == false)
                return;
            
            rb2d.velocity = Time.deltaTime * velocity * (isLeftDirection ? -1f : 1f) * Vector2.right;
            transform.localScale = new Vector3(isLeftDirection ? 1f : -1f, 1f, 1f);       
        }

        void Update()
        {
            var pointInViewport = CameraController.Instance.MainCamera.WorldToViewportPoint(transform.position);
            if (pointInViewport.x > 1.5f || pointInViewport.x <-0.5f) Destroy(gameObject);
        }
        
        void OnTriggerEnter2D(Collider2D other)
        {
            if(other.CompareTag("Weapon") == false) return;
            OnEntityDied();
        }

        void OnEntityDied()
        {
            canMove = false;
            
            armatureComponent.animation.Play("die", 1);
            Invoke(nameof(DestroyEntity), .5f);
        }
        
        void DestroyEntity()
        {
            Destroy(gameObject);			
        }
    }
}
