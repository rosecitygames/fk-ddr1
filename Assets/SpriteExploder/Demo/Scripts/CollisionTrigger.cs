using UnityEngine;
using UnityEngine.Events;

namespace RCG.SpriteExploder.Demo
{
    public class CollisionTrigger : MonoBehaviour
    {
        [SerializeField]
        UnityEvent CollisionEnter = null;

        private void Start() { }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (enabled == false) return;

            if (CollisionEnter != null)
            {
                CollisionEnter.Invoke();
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (enabled == false) return;

            if (CollisionEnter != null)
            {
                CollisionEnter.Invoke();
            }
        }
    }
}
