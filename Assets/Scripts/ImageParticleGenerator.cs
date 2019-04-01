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

    [ContextMenu("Generate")]
    void Generate()
    {
        sourceSpriteRenderer.enabled = false;
        Sprite sourceSprite = sourceSpriteRenderer.sprite;

        float particleSpriteRectSize = sourceSprite.rect.width / subdivisionCount;

        float particleSize = sourceSprite.bounds.size.x / subdivisionCount;
        float offsetX = -sourceSprite.bounds.size.x * 0.5f * (1.0f - (1.0f / subdivisionCount));
        float offsetY = -sourceSprite.bounds.size.x * 0.5f * (1.0f - (1.0f / subdivisionCount));

        ParticleSystem.MainModule main = particleSystem.main;
        main.startSize = particleSize;

        ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams();

        int particleCount = subdivisionCount * subdivisionCount;
        int tileX = 0;
        int tileY = 0;

        List<Vector4> customParticleDatas = new List<Vector4>(particleCount);

        Vector3 centerPosition = Vector3.zero;

        for (int tileIndex = 0; tileIndex < particleCount; tileIndex++)
        {
            tileX = tileIndex % subdivisionCount;
            tileY = Mathf.FloorToInt((float)tileIndex/ subdivisionCount);

            Vector3 position = new Vector3();
            position.x = (tileX * particleSize) + offsetX;
            position.y = (tileY * particleSize) + offsetY;

            emitParams.position = position;

            Vector3 direction = position - centerPosition;
            direction.y += Random.Range(1.0f, 3.0f);

            emitParams.velocity = direction * Random.Range(0.5f, 3.0f);// new Vector3(Random.Range(0.0f, 0.5f), Random.Range(0.0f, 0.5f));

            particleSystem.Emit(emitParams, 1);

            customParticleDatas.Add(new Vector4(subdivisionCount, tileIndex, 0, 0));
        }

        particleSystem.SetCustomParticleData(customParticleDatas, ParticleSystemCustomData.Custom1);

    }

}
