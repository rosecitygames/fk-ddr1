Shader "Sprites/Tile Grid"
{
    Properties
    {
		[PerRendererData] _GridTex("Sprite Texture", 2D) = "white" {}
		[PerRendererData] _Flip("Flip", Vector) = (1,1,1,1)
		[PerRendererData] _Rotation("Rotation", Float) = 0
		[PerRendererData] _SubdivisionCountX("Subdivision Count X", Int) = 0
		[PerRendererData] _SubdivisionCountY("Subdivision Count Y", Int) = 0
		_Color("Tint", Color) = (1,1,1,1)
    }

    SubShader
    {
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
		}

        LOD 100

		Cull Off
		Lighting Off
		ZWrite On
		Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM

			#pragma exclude_renderers gles
			#pragma target 4.5
            #pragma vertex vert
            #pragma fragment frag
			#pragma multi_compile_instancing
			#pragma multi_compile _ PIXELSNAP_ON
			#pragma multi_compile _ ETC1_EXTERNAL_ALPHA

			#pragma instancing_options procedural:vertInstancingSetup
			#define UNITY_PARTICLE_INSTANCE_DATA MyParticleInstanceData
			#define UNITY_PARTICLE_INSTANCE_DATA_NO_ANIM_FRAME
			struct MyParticleInstanceData
			{
				float3x4 transform;
				uint color;
				float custom;
			};
			#include "UnityCG.cginc"
			#include "UnityStandardParticleInstancing.cginc"
			
            struct appdata
            {
                float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				fixed4 color : COLOR;
				float custom : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
				float4 vertex : SV_POSITION;
				float2 texcoord : TEXCOORD0;
				fixed4 color : COLOR;
				float custom : TEXCOORD1;
            };

			sampler2D _GridTex;
			float4 _GridTex_ST;
			fixed4 _Color;
			float _Rotation;
			int _SubdivisionCountX;
			int _SubdivisionCountY;
			fixed2 _Flip;

            v2f vert (appdata IN)
            {
                v2f OUT;
				UNITY_INITIALIZE_OUTPUT(v2f, OUT);
				UNITY_SETUP_INSTANCE_ID(IN);

				OUT.color = IN.color;
				OUT.texcoord = IN.texcoord;

				vertInstancingColor(OUT.color);
				vertInstancingUVs(IN.texcoord, OUT.texcoord);
				OUT.vertex = UnityObjectToClipPos(IN.vertex);

#if defined(UNITY_PARTICLE_INSTANCING_ENABLED)
				UNITY_PARTICLE_INSTANCE_DATA data = unity_ParticleInstanceData[unity_InstanceID];
				OUT.custom = data.custom;
#endif
                return OUT;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				float2 uv = i.texcoord.xy * _Flip;

				float sinFactor = sin(_Rotation);
				float cosFactor = cos(_Rotation);
				float2x2 rotationMatrix = float2x2(cosFactor, sinFactor, -sinFactor, cosFactor);
				float2 uvRotationOffset = float2(0.5, 0.5);
				uv = mul(uv - uvRotationOffset, rotationMatrix) + uvRotationOffset;

				int tileCount = _SubdivisionCountX * _SubdivisionCountY;

				float tileIndex = fmod(floor(i.custom), tileCount);

				float minSubdivisionCount = min(_SubdivisionCountX, _SubdivisionCountY);
				float tileScalePercent = 1.0f / minSubdivisionCount;
				float2 tileScale = float2(tileScalePercent, tileScalePercent);

				float tileX = fmod(tileIndex, _SubdivisionCountX);
				if (_Flip.x < 0)
				{
					tileX = _SubdivisionCountX - tileX;
				}

				float tileY = floor(tileIndex / _SubdivisionCountX);
				if (_Flip.y < 0)
				{
					tileY = _SubdivisionCountY - tileY;
				}

				float offsetX = tileX / _SubdivisionCountX;
				float offsetY = tileY / _SubdivisionCountY;

				float2 tileUV = float2(offsetX, offsetY);
				
				float2 textcoord = uv * tileScale + tileUV;

                fixed4 col = tex2D(_GridTex, textcoord);
				col *= i.color;

                return col;
            }
            ENDCG
        }
    }
}
