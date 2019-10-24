using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IndieDevTools.Exploders
{
    /// <summary>
    /// A component that explodes a sprite into an array of particles.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteExploder : MonoBehaviour
    {
        /// <summary>
        /// An event that gets triggered when the sprite explodes.
        /// </summary>
        public event Action OnExploded;

        /// <summary>
        /// A reference to the SpriteExploderSettings resource.
        /// The settings are used to set performance overrides of lcoal
        /// values when initializing the partice system.
        /// </summary>
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
        SpriteExploderSettings globalSettings;

        /// <summary>
        /// The minimum particle pixel size possible.
        /// </summary>
        public int MinParticlePixelSize
        {
            get
            {
                return GlobalSettings.MinimumParticlePixelSize;
            }
        }

        /// <summary>
        /// The size of the generated particles.
        /// The effect essentially slices the sprite into a grid of square tiles.
        /// Use larger values for better performance since the larger the particle tiles are,
        /// the less that will be generated.
        /// </summary>
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
        [SerializeField, Tooltip("The size of the generated particles")]
        int particlePixelSize = 8;

        /// <summary>
        /// The type of collision the particles will use.
        /// Note, that global setting can be used to override the local value.
        /// </summary>
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
        [SerializeField, Tooltip("The type of collision the particles will use")]
        SpriteExploderCollisionMode collisionMode = SpriteExploderCollisionMode.Collision2D;

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
        [SerializeField, Tooltip("The minimum explosive strength that will be applied to particle velocity")]
        float minExplosiveStrength = 0.5f;

        /// <summary>
        /// The maximum explosive strength that will be applied to particle velocity.
        /// </summary>    
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
        [SerializeField]
        [Tooltip("The maximum explosive strength that will be applied to particle velocity")]
        float maxExplosiveStrength = 2.0f;

        /// <summary>
        /// The amount of gravity applied to particles.
        /// </summary>    
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
        [SerializeField, Tooltip("The amount of gravity applied to particles")]
        float gravityModifier = 1.0f;

        /// <summary>
        /// Whether or not the sprite will automatically explode on start.
        /// </summary>    
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
        [SerializeField, Tooltip("Whether or not the sprite will automatically explode on start")]
        bool isExplodingOnStart = false;

        /// <summary>
        /// The amount of delay before the explosion occurs.
        /// </summary>    
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
        [SerializeField, Tooltip("The amount of delay before the explosion occurs")]
        float delaySeconds = 0.0f;

        /// <summary>
        /// A reference to the local sprite renderer component.
        /// </summary>   
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
        SpriteRenderer localSpriteRenderer;

        /// <summary>
        /// Gets the particles of this Particle System.
        /// </summary>
        /// <param name="particles">Output particle buffer, containing the current particle state.</param>
        /// <returns>The number of particles written to the input particle array (the number of particles currently alive).</returns>
        public int GetParticles(ParticleSystem.Particle[] particles)
        {
            if (isExploded)
            {
                return LocalParticleSystem.GetParticles(particles);
            }

            return 0;
        }

        /// <summary>
        /// Gets the stream of custom per-particle data.
        /// </summary>
        /// <param name="customDatas">The array of per-particle data.</param>
        /// <returns>The processed array of per-particle data.</returns>
        public int GetCustomParticleData(List<Vector4> customDatas)
        {
            if (isExploded)
            {
                return LocalParticleSystem.GetCustomParticleData(customDatas, ParticleSystemCustomData.Custom1);
            }

            return 0;
        }

        /// <summary>
        /// A reference to the local particle system.
        /// </summary>
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
        ParticleSystem localParticleSystem;

        /// <summary>
        /// Whether or not the sprite has exploded.
        /// </summary>
        public bool IsExploded => isExploded;
        bool isExploded = false;
        
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
            if (isExploded) yield break;
            isExploded = true;

            // If the max particle count is only one, then complete explosion.
            int maxParticleCount = GetMaxParticleCount();
            if (maxParticleCount <= 1)
            {
                LocalSpriteRenderer.enabled = false;
                OnExploded?.Invoke();
                yield break;
            }

            // Set the amount of x and y subdivisions will be used. Similar to defining
            // the size of a grid.
            int subdivisionCountX = GetSubdivisionCountX();
            int subdivisionCountY = GetSubdivisionCountY();

            // Disable the sprite renderer so that particle textures will be seen instead.
            LocalSpriteRenderer.enabled = false;

            // Get a reference to the sprite renderer sprite and set bound size values.
            Sprite sprite = LocalSpriteRenderer.sprite;
            float boundSizeX = sprite.bounds.size.x;
            float boundSizeY = sprite.bounds.size.y;
            float halfBoundSizeX = boundSizeX * 0.5f;
            float halfBoundSizeY = boundSizeY * 0.5f;

            // Set the flip values the particles will use.
            float flipX = LocalSpriteRenderer.flipX ? -1.0f : 1.0f;
            float flipY = LocalSpriteRenderer.flipY ? -1.0f : 1.0f;

            // Set the max particle size. We want the particles to be square.
            // So, this grabs the biggest size from either the width of height values.
            float particleSizeMax = GetMaxParticleSize();

            // Set the amount of particles that will generated.
            int particleCount = GetMaxParticleCount();

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
            Vector3 lossyScale = transform.lossyScale;

            // Emit all the particle tiles in a for loop.
            for (int tileIndex = 0; tileIndex < particleCount; tileIndex++)
            {
                // Set the tile coordinates based on index and the number of subdivisions.
                tileX = GetTileX(tileIndex, subdivisionCountX);
                tileY = GetTileY(tileIndex, subdivisionCountY);
            
                // Set the tile position and then apply rotation to the values.
                Vector3 localPosition = new Vector3();
                localPosition.x = (tileX * lossyScale.x * particleSizeMax) + offsetX * lossyScale.x;
                localPosition.y = (tileY * lossyScale.y * particleSizeMax) + offsetY * lossyScale.y;
                localPosition = transform.rotation * localPosition;

                // Set the emit params position with local position offset plus the world position.
                Vector3 worldPosition = localPosition + transform.position;
                emitParams.position = worldPosition;

                // Set a random outward velocity to apply to the particle tile.
                Vector3 outwardVelocity = localPosition - explosionCenter;
                if (collisionMode == SpriteExploderCollisionMode.Collision3D)
                {
                    outwardVelocity.z = UnityEngine.Random.Range(-halfBoundSizeX * 0.5f, halfBoundSizeX * 0.5f);
                }
                outwardVelocity *= UnityEngine.Random.Range(MinExplosiveStrength, MaxExplosiveStrength);

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

            // Invoke exploded event
            OnExploded?.Invoke();
        }

        // Particle system default consts
        const float defaultStartLifetime = 10.0f;
        const float defaultMinDampen = 0.2f;
        const float defaultMaxDampen = 0.2f;
        const float defaultMinBounce = 0.7f;
        const float defaultMaxBounce = 0.9f;
        const float defaultLifetimeLoss = 0.1f;

        // Material resource path
        const string materialResourcePath = "Materials/SpriteTileGridMaterial";

        /// <summary>
        /// Initialize the particle system. Any existing particle system on the
        /// game object will be modified to be compatible with the tile grid material.
        /// If no particle system exists, then one will be created.
        /// </summary>
        void InitParticleSystem()
        {
            // Get the local particle system or create one if it doesn't exist.
            localParticleSystem = GetComponent<ParticleSystem>();
            bool hasLocalParticleSytem = localParticleSystem != null;
            if (hasLocalParticleSytem == false)
            {
                localParticleSystem = gameObject.AddComponent<ParticleSystem>();
            }

            // Stop the particle system before modifying its modules.
            LocalParticleSystem.Stop();

            // Modify the main module.
            ParticleSystem.MainModule main = LocalParticleSystem.main;
            main.playOnAwake = false;
            main.startLifetime = hasLocalParticleSytem ? main.startLifetime : defaultStartLifetime;
            main.duration = main.startLifetime.constantMax;
            main.loop = false;
            main.startSize = GetMaxParticleSize() * GetParticleSystemScale();
            main.startColor = hasLocalParticleSytem ? main.startColor : LocalSpriteRenderer.color;
            main.maxParticles = GetMaxParticleCount();
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            main.gravityModifier = GravityModifier;

            // Modify the emission module to be disabled since we'll manually be emitting particles.
            ParticleSystem.EmissionModule emission = LocalParticleSystem.emission;
            emission.enabled = false;

            // Modify the shape module so that it's disabled. We'll be manually be emitting the particles
            // at specific locations.
            ParticleSystem.ShapeModule shape = LocalParticleSystem.shape;
            shape.enabled = false;

            // Modify the collision module. Some properties retain their original settings
            // if a prexisting component existed. Otherwise, use default values.
            ParticleSystem.CollisionModule collision = LocalParticleSystem.collision;
            collision.enabled = IsCollisionEnabled;
            collision.type = ParticleSystemCollisionType.World;
            collision.mode = CollisionMode == SpriteExploderCollisionMode.Collision3D ? ParticleSystemCollisionMode.Collision3D : ParticleSystemCollisionMode.Collision2D;
            collision.dampen = hasLocalParticleSytem ? collision.dampen : new ParticleSystem.MinMaxCurve(defaultMinDampen, defaultMaxDampen);
            collision.bounce = hasLocalParticleSytem ? collision.bounce : new ParticleSystem.MinMaxCurve(defaultMinBounce, defaultMaxBounce);
            collision.lifetimeLoss = hasLocalParticleSytem ? collision.lifetimeLoss : defaultLifetimeLoss;

            // Modify the particle system renderer component. In particular, setting the material,
            // enabling gpu instancing, and setting the active vertex streams.
            ParticleSystemRenderer particleSystemRenderer = GetComponent<ParticleSystemRenderer>();
            particleSystemRenderer.renderMode = ParticleSystemRenderMode.Mesh;
            particleSystemRenderer.mesh = Resources.GetBuiltinResource<Mesh>("Quad.fbx");
            particleSystemRenderer.enableGPUInstancing = true;
            particleSystemRenderer.minParticleSize = 0.0f;
            particleSystemRenderer.maxParticleSize = 1.0f;

            Material material = Resources.Load<Material>(materialResourcePath);
            particleSystemRenderer.material = material;

            MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
            materialPropertyBlock.SetTexture("_GridTex", GetTexture());
            materialPropertyBlock.SetInt("_SubdivisionCountX", GetSubdivisionCountX());
            materialPropertyBlock.SetInt("_SubdivisionCountY", GetSubdivisionCountY());
            materialPropertyBlock.SetFloat("_Rotation", GetMaterialRotaion());
            materialPropertyBlock.SetVector("_Flip", GetFlipVector4());
            particleSystemRenderer.SetPropertyBlock(materialPropertyBlock);

            List<ParticleSystemVertexStream> streams = new List<ParticleSystemVertexStream>();
            streams.Add(ParticleSystemVertexStream.Position);
            streams.Add(ParticleSystemVertexStream.UV);
            streams.Add(ParticleSystemVertexStream.Color);
            streams.Add(ParticleSystemVertexStream.Custom1X); // Used to pass tile data to the shader

            particleSystemRenderer.SetActiveVertexStreams(streams);

            // Play the particle system now that the modules and renderer are setup.
            LocalParticleSystem.Play();
        }

        public ParticleSystem GetParticleSystem()
        {
            return LocalParticleSystem;
        }

        public Texture2D GetTexture()
        {
            return LocalSpriteRenderer.sprite.texture;
        }

        public int GetParticleCount()
        {
            return LocalParticleSystem.particleCount;
        }

        /// <summary>
        /// Helper method to get the max particle count.
        /// </summary>
        public int GetMaxParticleCount()
        {
            return GetSubdivisionCountX() * GetSubdivisionCountY();
        }

        /// <summary>
        /// Helper method to get the x tile position value of a particle.
        /// </summary>
        /// <param name="tileIndex">Index value of the particle tile</param>
        /// <param name="subdivisionCountX">The amount of horizatonal subdivisions</param>
        /// <returns>The x tile position value of a particle</returns>
        public int GetTileX(int tileIndex, int subdivisionCountX)
        {
            return tileIndex % subdivisionCountX;
        }

        /// <summary>
        /// Helper method to get the y tile position value of a particle.
        /// </summary>
        /// <param name="tileIndex">Index value of the particle tile</param>
        /// <param name="subdivisionCountX">The amount of vertical subdivisions</param>
        /// <returns>The y tile position value of a particle</returns>
        public int GetTileY(int tileIndex, int subdivisionCountX)
        {
            return Mathf.FloorToInt((float)tileIndex / subdivisionCountX);
        }

        /// <summary>
        /// Helper method to get the horizontal tile subdivisions count.
        /// </summary>
        public int GetSubdivisionCountX()
        {
            float spriteSizeX = LocalSpriteRenderer.sprite.bounds.size.x * LocalSpriteRenderer.sprite.pixelsPerUnit * transform.lossyScale.x;
            return Mathf.CeilToInt(spriteSizeX / ParticlePixelSize); 
        }

        /// <summary>
        /// Helper method to get the vertical tile subdivisions count.
        /// </summary>
        public int GetSubdivisionCountY()
        {
            float spriteSizeY = LocalSpriteRenderer.sprite.bounds.size.y * LocalSpriteRenderer.sprite.pixelsPerUnit * transform.lossyScale.y;
            return Mathf.CeilToInt(spriteSizeY / ParticlePixelSize);
        } 

        /// <summary>
        /// Helper method to get the world particle scale. 
        /// </summary>
        /// <returns>The particle scale relative to the world.</returns>
        public Vector2 GetParticleScale()
        {
            float maxBound = Mathf.Max(LocalSpriteRenderer.sprite.bounds.size.x, LocalSpriteRenderer.sprite.bounds.size.y);
            float scale = GetMaxParticleSize() / maxBound;
            return new Vector2(scale * transform.lossyScale.x, scale * transform.lossyScale.y);
        }

        /// <summary>
        /// Helper method to get the particle scale relative to the component scale.
        /// </summary>
        /// <returns>The particle scale relative the Sprite Exploder scale.</returns>
        public float GetParticleSystemScale()
        {
            return Mathf.Max(transform.lossyScale.x / transform.localScale.x, transform.lossyScale.y / transform.localScale.y);
        }

        /// <summary>
        /// Helper method to get the max particle size between horizontal and vertical subdivisions.
        /// </summary>
        public float GetMaxParticleSize()
        {
            return Mathf.Max(GetParticleSizeX(), GetParticleSizeY());
        }

        /// <summary>
        /// Helper method to get the horizontal particle (tile) count.
        /// </summary>
        float GetParticleSizeX()
        {
            return LocalSpriteRenderer.sprite.bounds.size.x / GetSubdivisionCountX();
        }

        /// <summary>
        /// Helper method to get the vertical particle (tile) count.
        /// </summary>
        float GetParticleSizeY()
        {
            return LocalSpriteRenderer.sprite.bounds.size.y / GetSubdivisionCountY();
        }

        /// <summary>
        /// Helper method to get the material rotation.
        /// </summary>
        float GetMaterialRotaion()
        {
            return Mathf.Deg2Rad * -transform.eulerAngles.z;
        }

        /// <summary>
        /// Helper method to get the flip values as a Vector4.  
        /// </summary>
        Vector4 GetFlipVector4()
        {
            Vector4 flip = new Vector4();
            flip.x = LocalSpriteRenderer.flipX ? -1.0f : 1.0f;
            flip.y = LocalSpriteRenderer.flipY ? -1.0f : 1.0f;
            return flip;
        }

        /// <summary>
        /// Helper method to get the flip values as a Vector2.  
        /// </summary>
        public Vector2 GetFlipVector2()
        {
            Vector2 flip = new Vector2();
            flip.x = LocalSpriteRenderer.flipX ? -1.0f : 1.0f;
            flip.y = LocalSpriteRenderer.flipY ? -1.0f : 1.0f;
            return flip;
        }

        /// <summary>
        /// The types of collision the emitting particle system can use.
        /// </summary>
        public enum SpriteExploderCollisionMode
        {
            None,
            Collision2D,
            Collision3D
        }
    }
}
