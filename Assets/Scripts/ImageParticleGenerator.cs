using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageParticleGenerator : MonoBehaviour
{

    [SerializeField]
    SpriteRenderer sourceSpriteRenderer;

    [SerializeField]
    new ParticleSystem particleSystem;

    [SerializeField]
    int subdivisionCount = 2;

    [SerializeField]
    float minExplosiveStrength = 0.5f;

    [SerializeField]
    float maxExplosiveStrength = 2.0f;

    bool hasExploded = false;

    [ContextMenu("Explode")]
    public void Explode()
    {
        Explode(Vector3.zero);
    }

    public void Explode(Vector3 explosionCenter)
    {
        if (hasExploded) return;
        hasExploded = true;

        sourceSpriteRenderer.enabled = false;
        Sprite sourceSprite = sourceSpriteRenderer.sprite;

        float particleSpriteRectSize = sourceSprite.rect.width / subdivisionCount;

        float particleSize = sourceSprite.bounds.size.x / subdivisionCount;
        float offsetX = -sourceSprite.bounds.size.x * 0.5f * (1.0f - (1.0f / subdivisionCount));
        float offsetY = -sourceSprite.bounds.size.x * 0.5f * (1.0f - (1.0f / subdivisionCount));

        int particleCount = subdivisionCount * subdivisionCount;
        int tileX = 0;
        int tileY = 0;

        ParticleSystem.MainModule main = particleSystem.main;
        main.startSize = particleSize;
        main.maxParticles = particleCount;

        ParticleSystem.EmissionModule emission = particleSystem.emission;
        emission.enabled = false;

        ParticleSystem.ShapeModule shape = particleSystem.shape;
        shape.enabled = false;

        ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams();

        List<Vector4> customParticleDatas = new List<Vector4>(particleCount);

        Vector3 baseVelocity = Vector3.zero;

        Rigidbody2D rigidbody2d = GetComponent<Rigidbody2D>();
        if (rigidbody2d != null)
        {
            Vector2 rigidbodyVelocity = rigidbody2d.velocity;
            baseVelocity.x = rigidbodyVelocity.x;
            baseVelocity.y = rigidbodyVelocity.y;
            //baseVelocity *= 4.0f;
        }

        for (int tileIndex = 0; tileIndex < particleCount; tileIndex++)
        {
            tileX = tileIndex % subdivisionCount;
            tileY = Mathf.FloorToInt((float)tileIndex/ subdivisionCount);

            Vector3 localPosition = new Vector3();
            localPosition.x = (tileX * particleSize) + offsetX;
            localPosition.y = (tileY * particleSize) + offsetY;

            Vector3 worldPosition = new Vector3();
            worldPosition.x = localPosition.x + sourceSpriteRenderer.gameObject.transform.position.x;
            worldPosition.y = localPosition.y + sourceSpriteRenderer.gameObject.transform.position.y;

            emitParams.position = worldPosition;

            Vector3 outwardVelocity = Vector3.Normalize(localPosition - explosionCenter);
            float upwardOffset = Random.Range(1.0f, 3.0f);
            outwardVelocity.y += upwardOffset;
            outwardVelocity *= Random.Range(minExplosiveStrength, maxExplosiveStrength);

            emitParams.velocity = baseVelocity + outwardVelocity;
            particleSystem.Emit(emitParams, 1);

            customParticleDatas.Add(new Vector4(subdivisionCount, tileIndex, 0, 0));
        }

        particleSystem.SetCustomParticleData(customParticleDatas, ParticleSystemCustomData.Custom1);

    }

}
