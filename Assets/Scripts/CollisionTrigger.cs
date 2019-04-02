using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CollisionTrigger : MonoBehaviour
{
    [SerializeField]
    UnityEvent CollisionEnter;

    [SerializeField]
    SpriteExploder generator;

    private void Start()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (enabled == false) return;

        if (generator != null)
        {
            ContactPoint2D contactPoint = collision.GetContact(0);
            generator.Explode(contactPoint.point);
        }

        if (CollisionEnter != null)
        {
            CollisionEnter.Invoke();
        }

    }
}
