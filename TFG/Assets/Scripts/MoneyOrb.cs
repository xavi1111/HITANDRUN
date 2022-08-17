using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyOrb : MonoBehaviour
{
    Rigidbody2D myRigidBody2D;
    CircleCollider2D myCircleCollider2D;
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
        if (startFollowing) { 
            myRigidBody2D.velocity = new Vector3(0, 0, 0);
            Vector3 velocityToAdd = new Vector3(transform.position.x < position.x ? 4 : (transform.position.x > position.x ? -4 : 0), transform.position.y < position.y ? 4 : (transform.position.y > position.y ? -4 : 0), 0);
            myRigidBody2D.velocity = velocityToAdd;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (myCircleCollider2D.radius.Equals(0.2f))
        {
            if (collision.gameObject.tag.Equals("Player"))
            {
                Destroy(gameObject);
            }
        }
        if (!startFollowing)
        {
            if (collision.gameObject.tag.Equals("Player"))
            {
                startFollowing = true;
                myCircleCollider2D.radius = 0.2f;
            }
        }
    }
}
