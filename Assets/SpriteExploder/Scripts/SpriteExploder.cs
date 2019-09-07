﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RCG.SpriteExploder
{
    /// <summary>
    /// A component that will explode a sprite into an array of particles.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteExploder : MonoBehaviour
    {
        /// <summary>
        /// The types of collision the emitting particle system can use.
        /// </summary>
        public enum SpriteExploderCollisionMode
        {
            None,
            Collision2D,
            Collision3D
        }

        /// <summary>
        /// A reference to the SpriteExploderSettings resource.
        /// The settings are used to set performance overrides of lcoal
        /// values when initializing the partice system.
        /// </summary>
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

        /// <summary>
        /// The size of the generated particles.
        /// The effect essentially slices the sprite into a grid of square tiles.
        /// Use larger values for better performance since the larger the particle tiles are,
        /// the less that will be generated.
        /// </summary>
        [SerializeField]
        [Tooltip("The size of the generated particles")]
        int particlePixelSize = 8;
        public int ParticlePixelSize
        {
            get
            {
                // Global settings will override particlePixelSize value if it is greater.
                return Mathf.Max(GlobalSettings.MinimumParticlePixelSize, particlePixelSize);
            }
            set
            {
                particlePixelSize = value;
            }
        }

        /// <summary>
        /// The type of collision the particles will use.
        /// Note, that global setting can be used to override the local value.
        /// </summary>
        [SerializeField]
        [Tooltip("The type of collision the particles will use")]
        SpriteExploderCollisionMode collisionMode = SpriteExploderCollisionMode.Collision2D;
        public SpriteExploderCollisionMode CollisionMode
        {
            get
            {
                // If the global settings value is not collidable, then override the local value to not use collision.
                return (GlobalSettings.IsCollidable == false) ? SpriteExploderCollisionMode.None : collisionMode;
            }
            set
            {
                collisionMode = value;
            }
        }

        /// <summary>
        /// Whether or not collision is enabled.
        /// </summary>
        bool IsCollisionEnabled
        {
            get
            {
                return CollisionMode != SpriteExploderCollisionMode.None;
            }
        }

        /// <summary>
        /// The minimum explosive strength that will be applied to particle velocity.
        /// </summary>
        [SerializeField]
        [Tooltip("The minimum explosive strength that will be applied to particle velocity")]
        float minExplosiveStrength = 0.5f;
        public float MinExplosiveStrength
        {
            get
            {
                return minExplosiveStrength;
            }
            set
            {
                minExplosiveStrength = value;
            }
        }

        /// <summary>
        /// The maximum explosive strength that will be applied to particle velocity.
        /// </summary>
        [SerializeField]
        [Tooltip("The maximum explosive strength that will be applied to particle velocity")]
        float maxExplosiveStrength = 2.0f;
        public float MaxExplosiveStrength
        {
            get
            {
                return maxExplosiveStrength;
            }
            set
            {
                maxExplosiveStrength = value;
            }
        }

        /// <summary>
        /// The amount of gravity applied to particles.
        /// </summary>
        [SerializeField]
        [Tooltip("The amount of gravity applied to particles")]
        float gravityModifier = 1.0f;
        public float GravityModifier
        {
            get
            {
                return gravityModifier;
            }
            set
            {
                gravityModifier = value;
            }
        }

        /// <summary>
        /// Whether or not the sprite will automatically explode on start.
        /// </summary>
        [SerializeField]
        [Tooltip("Whether or not the sprite will automatically explode on start")]
        bool isExplodingOnStart = false;
        public bool IsExplodingOnStart
        {
            get
            {
                return isExplodingOnStart;
            }
            set
            {
                isExplodingOnStart = value;
            }
        }

        /// <summary>
        /// The amount of delay before the explosion occurs.
        /// </summary>
        [SerializeField]
        [Tooltip("The amount of delay before the explosion occurs")]
        float delaySeconds = 0.0f;
        public float DelaySeconds
        {
            get
            {
                return delaySeconds;
            }
            set
            {
                delaySeconds = value;
            }
        }

        /// <summary>
        /// A reference to the local sprite renderer component.
        /// </summary>
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

        /// <summary>
        /// A reference to the local particle system.
        /// </summary>
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

        /// <summary>
        /// Whether or not the sprite has exploded.
        /// </summary>
        bool hasExploded = false;

        /// <summary>
        /// Unity event function.
        /// Initializes the particle system and explodes the sprite
        /// if set to do so on start.
        /// </summary>
        void Start()
        {
            InitParticleSystem();
            if (isExplodingOnStart)
            {
                Explode();
            }
        }

        /// <summary>
        /// Explodes the sprite.
        /// </summary>
        [ContextMenu("Explode")] // Explode can be called from the context menu in the inspector for testing purposes.
        public void Explode()
        {
#if UNITY_EDITOR
            // Prevent the explosion from happening if called from the editor when it's not playing.
            if (UnityEditor.EditorApplication.isPlaying == false) 
            {
                return;
            }
#endif
            // Explode from the center of the sprite.
            Explode(Vector3.zero);
        }

        /// <summary>
        /// Explodes the sprite.
        /// </summary>
        /// <param name="explosionCenter">The center position of the explosion. Vector3.zero is the center of the sprite.</param>
        public void Explode(Vector3 explosionCenter)
        {
            StopAllCoroutines();
            StartCoroutine(ExplodeCoroutine(explosionCenter));
        }

        /// <summary>
        /// Explodes the sprite after a delay.
        /// </summary>
        /// <param name="explosionCenter">The center position of the explosion. Vector3.zero is the center of the sprite.</param>
        /// <returns></returns>
        IEnumerator ExplodeCoroutine(Vector3 explosionCenter)
        {
            yield return new WaitForSeconds(delaySeconds); // Wait for delay seconds

            // If the explosion has already occurred, break the coroutine.
            if (hasExploded) yield break;
            hasExploded = true;

            // Disable the sprite renderer so that particle textures will be seen instead.
            LocalSpriteRenderer.enabled = false;

            // Get a reference to the sprite renderer sprite and set bound size values.
            Sprite sprite = LocalSpriteRenderer.sprite;
            float boundSizeX = sprite.bounds.size.x;
            float boundSizeY = sprite.bounds.size.y;
            float halfBoundSizeX = boundSizeX * 0.5f;
            float halfBoundSizeY = boundSizeY * 0.5f;

            // Set the amount of x and y subdivisions will be used. Similar to defining
            // the size of a grid.
            int subdivisionCountX = GetSubdivisionCountX();
            int subdivisionCountY = GetSubdivisionCountY();

            // Set the flip values the particles will use.
            float flipX = LocalSpriteRenderer.flipX ? -1.0f : 1.0f;
            float flipY = LocalSpriteRenderer.flipY ? -1.0f : 1.0f;

            // Set the max particle size. We want the particles to be square.
            // So, this grabs the biggest size from either the width of height values.
            float particleSizeMax = GetParticleSizeMax();

            // Set the amount of particles that will generated.
            int particleCount = GetParticleCount();

            // Set the base particle offset values.
            float offsetX = -halfBoundSizeX * (1.0f - (1.0f / subdivisionCountX));
            float offsetY = -halfBoundSizeY * (1.0f - (1.0f / subdivisionCountY));

            // Define tile coordinate vars.
            int tileX;
            int tileY;
        
            // Create particle emission paramaters.
            ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams();

            // Define custom particle data var.
            List<Vector4> custom1ParticleDatas = new List<Vector4>(particleCount);

            // Define the base velocity var.
            Vector3 baseVelocity = Vector3.zero;

            // Set base velocity values from the attached rigid body if it exists.
            Rigidbody2D rigidbody2d = GetComponent<Rigidbody2D>();
            if (rigidbody2d != null)
            {
                Vector2 rigidbodyVelocity = rigidbody2d.velocity;
                baseVelocity.x = rigidbodyVelocity.x;
                baseVelocity.y = rigidbodyVelocity.y;
            }

            // Set the local scale value.
            Vector3 localScale = transform.localScale;

            // Emit all the particle tiles in a for loop.
            for (int tileIndex = 0; tileIndex < particleCount; tileIndex++)
            {
                // Set the tile coordinates based on index and the number of subdivisions.
                tileX = tileIndex % subdivisionCountX;
                tileY = Mathf.FloorToInt((float)tileIndex/ subdivisionCountX);
            
                // Set the tile position and then apply rotation to the values.
                Vector3 localPosition = new Vector3();
                localPosition.x = (tileX * localScale.x * particleSizeMax) + offsetX * localScale.x;
                localPosition.y = (tileY * localScale.y * particleSizeMax) + offsetY * localScale.y;
                localPosition = transform.rotation * localPosition;

                // Set the emit params position with local position offset plus the world position.
                Vector3 worldPosition = localPosition + transform.position;
                emitParams.position = worldPosition;

                // Set a random outward velocity to apply to the particle tile.
                Vector3 outwardVelocity = localPosition - explosionCenter;
                if (collisionMode == SpriteExploderCollisionMode.Collision3D)
                {
                    outwardVelocity.z = Random.Range(-halfBoundSizeX * 0.5f, halfBoundSizeX * 0.5f);
                }
                outwardVelocity *= Random.Range(MinExplosiveStrength, MaxExplosiveStrength);

                // Set the emit params velocity with the base velocity of the rigid body plus the outward explosion velocity.
                emitParams.velocity = baseVelocity + outwardVelocity;

                // Emit the particle tile.
                LocalParticleSystem.Emit(emitParams, 1);

                // Add to the custom particle data array.
                // This is used to pass the tile index to the shader.
                // A Vector4 is required to pass this type of data.
                // In this case, we only need to use the first value since we only have one index value to pass.
                custom1ParticleDatas.Add(new Vector4(tileIndex, 0.0f, 0.0f, 0.0f));
            }

            // Set the custom particle data for all the particles.
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
            main.gravityModifier = GravityModifier;

            ParticleSystem.EmissionModule emission = LocalParticleSystem.emission;
            emission.enabled = false;

            ParticleSystem.ShapeModule shape = LocalParticleSystem.shape;
            shape.enabled = false;

            ParticleSystem.CollisionModule collision = LocalParticleSystem.collision;
            collision.enabled = IsCollisionEnabled;
            collision.type = ParticleSystemCollisionType.World;
            collision.mode = CollisionMode == SpriteExploderCollisionMode.Collision3D ? ParticleSystemCollisionMode.Collision3D : ParticleSystemCollisionMode.Collision2D;
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
            materialPropertyBlock.SetTexture("_GridTex", LocalSpriteRenderer.sprite.texture);
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
