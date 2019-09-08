using DragonBones;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using System;


namespace WSGJ
{
    public class PiggyEntity : MonoBehaviour
    {
        public static event Action<PiggyEntity> Died;

        [SerializeField]
        float velocity = 165f;
        [SerializeField]
        float minSpawnHeight = 0.5f;
        [SerializeField]
        float maxSpawnHeight = 2f;

        public float ScoreValue;

        UnityArmatureComponent armatureComponent;
        Rigidbody2D rb2d;


        bool isLeftDirection;
        bool canMove = true;

        void Awake()
        {
            armatureComponent = GetComponentInChildren<UnityArmatureComponent>();
            
            rb2d = GetComponent<Rigidbody2D>();
        }
        
        void Start()
        {
            transform.position += new Vector3(0f, 
                UnityEngine.Random.Range(minSpawnHeight, maxSpawnHeight), 0f);
            isLeftDirection = transform.position.x > 0;
        }

        void FixedUpdate()
        {
            if (canMove)
            {
                rb2d.velocity = Time.deltaTime * velocity * (isLeftDirection ? -1f : 1f) * Vector2.right;
                transform.localScale = new Vector3(isLeftDirection ? 1f : -1f, 1f, 1f);
            }
            else
            {

            }
        }

        void Update()
        {
            var pointInViewport = CameraController.Instance.MainCamera.WorldToViewportPoint(transform.position);
            if (pointInViewport.x > 1.5f || pointInViewport.x <-0.5f) Destroy(gameObject);
        }
        
        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Weapon") || canMove)
            {
                OnEntityDied();
                rb2d.gravityScale = 1f;
                rb2d.AddForce(Vector2.down * 10f);
            }       
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.collider.CompareTag("FallingBlock"))
            {
                var blockComponent = collision.collider.GetComponentInParent<FallingBlock>();
                if(blockComponent != null && blockComponent.AttachedTruck == false)
                    blockComponent.OnBlockDestroyed();

                OnEntityDied();
            };    
        }

        void OnEntityDied()
        {
            canMove = false;
            
            armatureComponent.animation.Play("die", 1);

            Invoke(nameof(DestroyEntity), 1f);
        }
        
        void DestroyEntity()
        {
            var particles = GetComponentsInChildren<ParticleSystem>().ToList();

            foreach (var particle in particles)
            {
                particle.transform.parent = null;
                particle.Play();
            }

            Died?.Invoke(this);
            Destroy(gameObject);			
        }
    }
}
