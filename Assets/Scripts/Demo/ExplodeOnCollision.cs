using UnityEngine;

namespace RCG.SpriteExploder.Demo
{
    public class ExplodeOnCollision : MonoBehaviour
    {
        void OnCollisionEnter(Collision collision)
        {
            SpriteExploder spriteExploder = GetComponent<SpriteExploder>();
            spriteExploder.Explode();
        }
    }
}
