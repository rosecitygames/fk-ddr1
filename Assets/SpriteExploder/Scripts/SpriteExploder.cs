using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RCG.SpriteExploder
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteExploder : MonoBehaviour
    {
        SpriteExploderSettings globalSettings;
        SpriteExploderSettings GlobalSettings
        {
            get
            {
                if (globalSettings == null)
                {
                    globalSettings = SpriteExploderSettings.GetResource();
                }
                return globalSettings;
            }
        }

        [SerializeField]
        int particlePixelSize = 8;
        int ParticlePixelSize
        {
            get
            {
                return Mathf.Max(GlobalSettings.MinimumParticlePixelSize, particlePixelSize);
            }
        }

        [SerializeField]
        bool isCollisionEnabled = true;
        bool IsCollisionEnabled
        {
            get
            {
                return GlobalSettings.IsCollidable ? isCollisionEnabled : GlobalSettings.IsCollidable;
            }
        }

        [SerializeField]
        ParticleSystemCollisionMode collisionMode = ParticleSystemCollisionMode.Collision2D;

        [SerializeField]
        float minExplosiveStrength = 0.5f;

        [SerializeField]
        float maxExplosiveStrength = 2.0f;

        [SerializeField]
        float gravityModifier = 1.0f;

        [SerializeField]
        bool isExplodingOnStart = false;

        [SerializeField]
        float delaySeconds = 0.0f;

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
            InitParticleSystem();
            if (isExplodingOnStart)
            {
                Invoke("Explode", delaySeconds);
            }
        }

        [ContextMenu("Explode")]
        public void Explode()
        {
#if UNITY_EDITOR
            if (UnityEditor.EditorApplication.isPlaying == false)
            {
                return;
            }
#endif
                Explode(Vector3.zero);
        }

        public void Explode(Vector3 explosionCenter)
        {
            if (hasExploded) return;
            hasExploded = true;

            LocalSpriteRenderer.enabled = false;

            Sprite sprite = LocalSpriteRenderer.sprite;
            float boundSizeX = sprite.bounds.size.x;
            float boundSizeY = sprite.bounds.size.y;
            float halfBoundSizeX = boundSizeX * 0.5f;
            float halfBoundSizeY = boundSizeY * 0.5f;

            int subdivisionCountX = GetSubdivisionCountX();
            int subdivisionCountY = GetSubdivisionCountY();

            float flipX = LocalSpriteRenderer.flipX ? -1.0f : 1.0f;
            float flipY = LocalSpriteRenderer.flipY ? -1.0f : 1.0f;

            float particleSizeMax = GetParticleSizeMax();
            int particleCount = GetParticleCount();

            float offsetX = -halfBoundSizeX * (1.0f - (1.0f / subdivisionCountX));
            float offsetY = -halfBoundSizeY * (1.0f - (1.0f / subdivisionCountY));

            int tileX = 0;
            int tileY = 0;
        
            ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams();

            List<Vector4> custom1ParticleDatas = new List<Vector4>(particleCount);

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
                tileX = tileIndex % subdivisionCountX;
                tileY = Mathf.FloorToInt((float)tileIndex/ subdivisionCountX);
            
                Vector3 localPosition = new Vector3();
                localPosition.x = (tileX * localScale.x * particleSizeMax) + offsetX * localScale.x;
                localPosition.y = (tileY * localScale.y * particleSizeMax) + offsetY * localScale.y;
                localPosition = transform.rotation * localPosition;

                Vector3 worldPosition = localPosition + transform.position;
                emitParams.position = worldPosition;

                Vector3 outwardVelocity = localPosition;// - localExplosionCenter;
                if (collisionMode == ParticleSystemCollisionMode.Collision3D)
                {
                    outwardVelocity.z = Random.Range(-halfBoundSizeX * 0.5f, halfBoundSizeX * 0.5f);
                }
                outwardVelocity *= Random.Range(minExplosiveStrength, maxExplosiveStrength);

                emitParams.velocity = baseVelocity + outwardVelocity;
                LocalParticleSystem.Emit(emitParams, 1);

                custom1ParticleDatas.Add(new Vector4(tileIndex, 0.0f, 0.0f, 0.0f));
            }

            LocalParticleSystem.SetCustomParticleData(custom1ParticleDatas, ParticleSystemCustomData.Custom1);
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

            LocalParticleSystem.Stop();

            ParticleSystem.MainModule main = LocalParticleSystem.main;
            main.playOnAwake = false;
            main.startLifetime = hasLocalParticleSytem ? main.startLifetime : defaultStartLifetime;
            main.duration = main.startLifetime.constantMax;
            main.loop = false;
            main.startSize = GetParticleSizeMax();
            main.startColor = LocalSpriteRenderer.color;
            main.maxParticles = GetParticleCount();
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            main.gravityModifier = gravityModifier;

            ParticleSystem.EmissionModule emission = LocalParticleSystem.emission;
            emission.enabled = false;

            ParticleSystem.ShapeModule shape = LocalParticleSystem.shape;
            shape.enabled = false;

            ParticleSystem.CollisionModule collision = LocalParticleSystem.collision;
            collision.enabled = isCollisionEnabled;
            collision.type = ParticleSystemCollisionType.World;
            collision.mode = collisionMode;
            collision.dampen = hasLocalParticleSytem ? collision.dampen : new ParticleSystem.MinMaxCurve(defaultMinDampen, defaultMaxDampen);
            collision.bounce = hasLocalParticleSytem ? collision.bounce : new ParticleSystem.MinMaxCurve(defaultMinBounce, defaultMaxBounce);
            collision.lifetimeLoss = hasLocalParticleSytem ? collision.lifetimeLoss : defaultLifetimeLoss;

            ParticleSystemRenderer particleSystemRenderer = GetComponent<ParticleSystemRenderer>();
            particleSystemRenderer.renderMode = ParticleSystemRenderMode.Mesh;
            particleSystemRenderer.mesh = Resources.GetBuiltinResource<Mesh>("Quad.fbx");
            particleSystemRenderer.enableGPUInstancing = true;
            particleSystemRenderer.minParticleSize = 0.0f;
            particleSystemRenderer.maxParticleSize = 1.0f;

            Material material = Resources.Load<Material>(materialResourcePath);
            particleSystemRenderer.material = material;

            MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
            materialPropertyBlock.SetTexture("_MainTex", LocalSpriteRenderer.sprite.texture);
            materialPropertyBlock.SetInt("_SubdivisionCountX", GetSubdivisionCountX());
            materialPropertyBlock.SetInt("_SubdivisionCountY", GetSubdivisionCountY());
            materialPropertyBlock.SetFloat("_Rotation", GetMaterialRotaion());
            materialPropertyBlock.SetVector("_Flip", GetFlipVector());
            particleSystemRenderer.SetPropertyBlock(materialPropertyBlock);

            List<ParticleSystemVertexStream> streams = new List<ParticleSystemVertexStream>();
            streams.Add(ParticleSystemVertexStream.Position);
            streams.Add(ParticleSystemVertexStream.UV);
            streams.Add(ParticleSystemVertexStream.Color);
            streams.Add(ParticleSystemVertexStream.Custom1X);

            particleSystemRenderer.SetActiveVertexStreams(streams);

            LocalParticleSystem.Play();
        }

        int GetSubdivisionCountX()
        {
            float spriteSizeX = LocalSpriteRenderer.sprite.bounds.size.x * LocalSpriteRenderer.sprite.pixelsPerUnit;
            return Mathf.CeilToInt(spriteSizeX / ParticlePixelSize); 
        }

        int GetSubdivisionCountY()
        {
            float spriteSizeY = LocalSpriteRenderer.sprite.bounds.size.y * LocalSpriteRenderer.sprite.pixelsPerUnit;
            return Mathf.CeilToInt(spriteSizeY / ParticlePixelSize);
        }

        int GetParticleCount()
        {
            return GetSubdivisionCountX() * GetSubdivisionCountY();
        }

        float GetParticleSizeMax()
        {
            return Mathf.Max(GetParticleSizeX(), GetParticleSizeY());
        }

        float GetParticleSizeX()
        {
            return LocalSpriteRenderer.sprite.bounds.size.x / GetSubdivisionCountX();
        }

        float GetParticleSizeY()
        {
            return LocalSpriteRenderer.sprite.bounds.size.y / GetSubdivisionCountY();
        }

        float GetMaterialRotaion()
        {
            return Mathf.Deg2Rad * -transform.eulerAngles.z;
        }

        Vector4 GetFlipVector()
        {
            Vector4 flip = new Vector4();
            flip.x = LocalSpriteRenderer.flipX ? -1.0f : 1.0f;
            flip.y = LocalSpriteRenderer.flipY ? -1.0f : 1.0f;
            return flip;
        }
    }
}
