using UnityEngine;
using UnityEngine.Events;

namespace IndieDevTools.SpriteExploder.Demo
{
    /// <summary>
    /// A class that triggers a collision enter Unity event when it collides
    /// with another object.
    /// </summary>
    public class CollisionTrigger : MonoBehaviour
    {
        [SerializeField]
        UnityEvent CollisionEnter = null;

        void OnCollisionEnter2D(Collision2D collision)
        {
            if (enabled == false) return;

            if (CollisionEnter != null)
            {
                CollisionEnter.Invoke();
            }
        }

        void OnCollisionEnter(Collision collision)
        {
            if (enabled == false) return;

            if (CollisionEnter != null)
            {
                CollisionEnter.Invoke();
            }
        }
    }
}
