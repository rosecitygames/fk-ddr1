using UnityEngine;

namespace IndieDevTools.Exploders.Demo
{
    /// <summary>
    /// A class the calls the Explode method on a Sprite Exploder component
    /// on collision.
    /// </summary>
    [RequireComponent(typeof(SpriteExploder))]
    public class ExplodeOnCollision : MonoBehaviour
    {
        void OnCollisionEnter(Collision collision)
        {
            SpriteExploder spriteExploder = GetComponent<SpriteExploder>();
            spriteExploder.Explode();
        }
    }
}
