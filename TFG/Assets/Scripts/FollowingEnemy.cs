using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowingEnemy : MonoBehaviour
{
    Rigidbody2D myRigidBody2D;
    CircleCollider2D myCircleCollider2D;
    [SerializeField] public float damage = 40;

    bool startFollowing = false;
    // Start is called before the first frame update
    void Start()
    {
        myRigidBody2D = GetComponent<Rigidbody2D>();
        myCircleCollider2D = GetComponent<CircleCollider2D>();
        Player.FollowMe += OnFollowMe;

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnDestroy()
    {
        Player.FollowMe -= OnFollowMe;
    }

    private void OnFollowMe(Vector3 position)
    {
        if (startFollowing)
        {
            myRigidBody2D.velocity = new Vector3(0, 0, 0);
            Vector2 velocityToAdd = new Vector2((position.x - transform.position.x)*8, -1f*8);
            myRigidBody2D.velocity += velocityToAdd;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
       
        if (myCircleCollider2D.radius.Equals(0.32f))
        {
            if (collision.gameObject.tag.Equals("Player") || collision.gameObject.tag.Equals("Wall"))
            {
                Destroy(gameObject);
            }
        }
        if (!startFollowing)
        {
            if (collision.gameObject.tag.Equals("Player"))
            {
                startFollowing = true;
                myCircleCollider2D.radius = 0.32f;
                myCircleCollider2D.offset = new Vector2(0.0f, 0.0f);
            }
        }

        
    }
}
