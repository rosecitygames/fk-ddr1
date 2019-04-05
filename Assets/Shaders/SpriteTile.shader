Shader "Sprites/Tile Grid"
{
    Properties
    {
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		[PerRendererData] _Flip("Flip", Vector) = (1,1,1,1)
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
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
			#pragma target 2.5
            #pragma vertex vert
            #pragma fragment frag
			#pragma multi_compile_instancing
			#pragma multi_compile _ PIXELSNAP_ON
			#pragma multi_compile _ ETC1_EXTERNAL_ALPHA
            #include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed4 _Color;
			fixed2 _Flip;

            struct appdata
            {
                float4 vertex : POSITION;
                float4 uv : TEXCOORD0;
				float4 color : COLOR;
				float4 custom : TEXCOORD1;
            };

            struct v2f
            {
				float4 vertex : SV_POSITION;
                float4 uv : TEXCOORD0;             
				fixed4 color : COLOR;
				float4 custom : TEXCOORD1;
            };

            v2f vert (appdata IN)
            {
                v2f OUT;

				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.uv.xy = TRANSFORM_TEX(IN.uv, _MainTex);
				OUT.uv.zw = IN.uv.zw;
				OUT.color = IN.color;
				OUT.custom.xy = IN.custom.xy;
				OUT.custom.z = IN.custom.z;

                return OUT;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				float2 uv = i.uv.xy;
				//uv.y *= _Flip.y;

				//float2 flip = i.custom.xy;

				float1 rotation = i.custom.z;
				float sinFactor = sin(rotation);
				float cosFactor = cos(rotation);
				float2x2 rotationMatrix = float2x2(cosFactor, sinFactor, -sinFactor, cosFactor);
				float2 uvRotationOffset = float2(0.5, 0.5);
				uv = mul(uv - uvRotationOffset, rotationMatrix) + uvRotationOffset;

				float subdivisionCount = floor(i.uv.z);
				float tileCount = subdivisionCount * subdivisionCount;

				float tileIndex = fmod(floor(i.uv.w), tileCount);	

				float2 tileScale = float2(1.0f / subdivisionCount, 1.0f / subdivisionCount);

				float tileX = fmod(tileIndex, subdivisionCount);
				float tileY = floor(tileIndex / subdivisionCount);

				float offsetX = tileX / subdivisionCount;
				float offsetY = tileY / subdivisionCount;

				float2 tileUV = float2(offsetX, offsetY);
				
				float2 textcoord = uv * tileScale + tileUV;

                fixed4 col = tex2D(_MainTex, textcoord);
				col *= i.color;

                return col;
            }
            ENDCG
        }
    }
}
