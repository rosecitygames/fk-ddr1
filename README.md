# Floppy Knights - Distributable Developer Resource 1
![screenshot](https://drive.google.com/uc?export=view&id=1eFp3480sVH2uKPnwJ-gFilStaoC8Y7rt)

## Sprite Exploder

#### Description
A component that will explode any sprite into an array of particles.
The sprite exploder is highly optimized to allow for hundreds of thousands of particles in a scene.
Developers can also define global settings to specify particle attributes and allow players to leverage their CPU power for more spectacular effects.

![screenshot](https://drive.google.com/uc?export=view&id=1LPllR6xZByGBEf2dR--ibDC8UOo4LikV)

##### Component Parameters

Name | Description 
--- | --- 
**Particle Pixel Size** | The square pixel size of each particle that the sprite will separated into 
**Collision Mode** | Which collision mode the particle system will use
**Min Explosive Strength** | The minimum amount of explosive force that will be applied to the particles
**Max Explosive Strength** | The maximum amount of explosive force that will be applied to the particles
**Is Exploding On Start** | Whether or not the sprite will explode automatically on the Unity start event
**Delay Seconds** | The amount of delay before the explosion happens

##### Public Methods

Name | Description 
--- | --- 
Explode | Triggers the sprite to explode. An optional *explosionCenter* parameter can be used to offset the center of the explosion

## Sprite Exploder Settings

![screenshot](https://drive.google.com/uc?export=view&id=1uQEH1xWfqZ0I7RFUC-JbmQK_HAedNMJk)

#### Description
Settings that define global *Sprite Exploder* parameters. Values can be set in the editor or at runtime.
These are accesible via the menu item at *Edit > Sprite Exploder Settings...* or via the scriptable object located in *Assets/SpriteExploder/Resources/SpriteExploderSettings*

##### Settings Parameters

Name | Description 
--- | --- 
**Min Particle Pixel Size** | The minimum particle pixel size that all *Sprite Exploder* components will use
**Is Collidable** | Whether or not a *Sprite Exploder* component particles are able to use collision physics