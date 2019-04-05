using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteExploder : MonoBehaviour
{
    [SerializeField]
    int subdivisionCount = 2;

    [SerializeField]
    float minExplosiveStrength = 0.5f;

    [SerializeField]
    float maxExplosiveStrength = 2.0f;

    [SerializeField]
    float gravityModifier = 1.0f;

    [SerializeField]
    bool isExplodingOnStart = false;

    [SerializeField]
    float delaySeconds;

    SpriteRenderer localSpriteRenderer;
    SpriteRenderer LocalSpriteRenderer
    {
        get
        {
            if (localSpriteRenderer == null)
            {
                localSpriteRenderer = GetComponent<SpriteRenderer>();
            }
            return localSpriteRenderer;
        }
    }

    ParticleSystem localParticleSystem;
    ParticleSystem LocalParticleSystem
    {
        get
        {
            if (localParticleSystem == null)
            {
                InitParticleSystem();
            }
            return localParticleSystem;
        }
    }

    bool hasExploded = false;

    void Start()
    {
        if (isExplodingOnStart)
        {
            Invoke("Explode", delaySeconds);
        }
    }

    [ContextMenu("Explode")]
    public void Explode()
    {
        Explode(Vector3.zero);
    }

    public void Explode(Vector3 explosionCenter)
    {
        if (hasExploded) return;
        hasExploded = true;

        LocalSpriteRenderer.enabled = false;

        Sprite sprite = LocalSpriteRenderer.sprite;

        float flipX = LocalSpriteRenderer.flipX ? -1.0f : 1.0f;
        float flipY = LocalSpriteRenderer.flipY ? -1.0f : 1.0f;

        float particleSize = GetParticleSize();
        int particleCount = GetParticleCount();

        float offsetX = -sprite.bounds.size.x * 0.5f * (1.0f - (1.0f / subdivisionCount));
        float offsetY = -sprite.bounds.size.x * 0.5f * (1.0f - (1.0f / subdivisionCount));

        int tileX = 0;
        int tileY = 0;
        float tileRotation = 0.0f;

        ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams();

        List<Vector4> custom1ParticleDatas = new List<Vector4>(particleCount);
        List<Vector4> custom2ParticleDatas = new List<Vector4>(particleCount);

        Vector3 baseVelocity = Vector3.zero;
        float baseAngularVelocity = 0.0f;

        Rigidbody2D rigidbody2d = GetComponent<Rigidbody2D>();
        if (rigidbody2d != null)
        {
            Vector2 rigidbodyVelocity = rigidbody2d.velocity;
            baseVelocity.x = rigidbodyVelocity.x;
            baseVelocity.y = rigidbodyVelocity.y;

            baseAngularVelocity = rigidbody2d.angularVelocity;
        }

        Vector3 localScale = transform.localScale;
        Vector3 localExplosionCenter = (explosionCenter - transform.position);

        for (int tileIndex = 0; tileIndex < particleCount; tileIndex++)
        {
            tileX = tileIndex % subdivisionCount;
            tileY = Mathf.FloorToInt((float)tileIndex/ subdivisionCount);
            
            Vector3 localPosition = new Vector3();
            localPosition.x = (tileX * localScale.x * particleSize) + offsetX * localScale.x;
            localPosition.y = (tileY * localScale.y * particleSize) + offsetY * localScale.y;
            localPosition = transform.rotation * localPosition;

            Vector3 worldPosition = localPosition + transform.position;
            emitParams.position = worldPosition;

            tileRotation = Mathf.Deg2Rad * -transform.eulerAngles.z;

            /*
            Vector3 angularVelocityOffset = Vector3.zero;
            bool isBaseAngularVelocity = baseAngularVelocity != 0.0f;
            if (isBaseAngularVelocity)
            {
                Quaternion angularVelocityRotation = Quaternion.Euler(0.0f, 0.0f, baseAngularVelocity);
                Vector3 angularVelocityLocalPosition = angularVelocityRotation * localPosition;
                angularVelocityOffset = angularVelocityLocalPosition - localPosition;
            }
            */

            Vector3 outwardVelocity = localPosition;// - localExplosionCenter;
            outwardVelocity *= Random.Range(minExplosiveStrength, maxExplosiveStrength);
            emitParams.velocity = baseVelocity + outwardVelocity;
            LocalParticleSystem.Emit(emitParams, 1);

            custom1ParticleDatas.Add(new Vector4(subdivisionCount, tileIndex, flipX, flipY));
            custom2ParticleDatas.Add(new Vector4(tileRotation, 0.0f, 0.0f, 0.0f));
        }

        LocalParticleSystem.SetCustomParticleData(custom1ParticleDatas, ParticleSystemCustomData.Custom1);
        LocalParticleSystem.SetCustomParticleData(custom2ParticleDatas, ParticleSystemCustomData.Custom2);
    }

    const float defaultStartLifetime = 10.0f;
    const float defaultMinDampen = 0.2f;
    const float defaultMaxDampen = 0.2f;
    const float defaultMinBounce = 0.7f;
    const float defaultMaxBounce = 0.9f;
    const float defaultLifetimeLoss = 0.1f;

    const string materialResourcePath = "Materials/SpriteTileGridMaterial";

    void InitParticleSystem()
    {
        localParticleSystem = GetComponent<ParticleSystem>();
        bool hasLocalParticleSytem = localParticleSystem != null;
        if (hasLocalParticleSytem == false)
        {
            localParticleSystem = gameObject.AddComponent<ParticleSystem>();
        }

        ParticleSystem.MainModule main = LocalParticleSystem.main;
        main.playOnAwake = false;
        main.startLifetime = hasLocalParticleSytem ? main.startLifetime : defaultStartLifetime;
        main.startSize = GetParticleSize();
        main.startColor = LocalSpriteRenderer.color;
        main.maxParticles = GetParticleCount();
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.gravityModifier = gravityModifier;

        ParticleSystem.EmissionModule emission = LocalParticleSystem.emission;
        emission.enabled = false;

        ParticleSystem.ShapeModule shape = LocalParticleSystem.shape;
        shape.enabled = false;

        ParticleSystem.CollisionModule collision = LocalParticleSystem.collision;
        collision.enabled = true;
        collision.type = ParticleSystemCollisionType.World;
        collision.mode = ParticleSystemCollisionMode.Collision2D;
        collision.dampen = hasLocalParticleSytem ? collision.dampen : new ParticleSystem.MinMaxCurve(defaultMinDampen, defaultMaxDampen);
        collision.bounce = hasLocalParticleSytem ? collision.bounce : new ParticleSystem.MinMaxCurve(defaultMinBounce, defaultMaxBounce);
        collision.lifetimeLoss = hasLocalParticleSytem ? collision.lifetimeLoss : defaultLifetimeLoss;

        ParticleSystemRenderer particleSystemRenderer = GetComponent<ParticleSystemRenderer>();
        particleSystemRenderer.renderMode = ParticleSystemRenderMode.Billboard;
        particleSystemRenderer.minParticleSize = 0.0f;
        particleSystemRenderer.maxParticleSize = 1.0f;

        Material material = Resources.Load<Material>(materialResourcePath);
        particleSystemRenderer.material = material;

        MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
        materialPropertyBlock.SetTexture("_MainTex", LocalSpriteRenderer.sprite.texture);
        materialPropertyBlock.SetVector("_Flip", new Vector4(1.0f, -1.0f, 0.0f, 0.0f));
        particleSystemRenderer.SetPropertyBlock(materialPropertyBlock);

        List<ParticleSystemVertexStream> streams = new List<ParticleSystemVertexStream>();
        streams.Add(ParticleSystemVertexStream.Position);
        streams.Add(ParticleSystemVertexStream.UV);
        streams.Add(ParticleSystemVertexStream.Color);
        streams.Add(ParticleSystemVertexStream.Custom1XYZW);
        streams.Add(ParticleSystemVertexStream.Custom2X);

        particleSystemRenderer.SetActiveVertexStreams(streams);
    }

    float GetParticleSize()
    {
        return LocalSpriteRenderer.sprite.bounds.size.x / subdivisionCount;
    }

    int GetParticleCount()
    {
        return subdivisionCount * subdivisionCount;
    }

}
