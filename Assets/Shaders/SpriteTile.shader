Shader "Sprites/Tile Grid"
{
    Properties
    {
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		[PerRendererData] _Flip("Flip", Vector) = (1,1,1,1)
		[PerRendererData] _Rotation("Rotation", Float) = 0
		[PerRendererData] _SubdivisionCount("Subdivision Count", Int) = 0
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
			float _Rotation;
			int _SubdivisionCount;
			fixed2 _Flip;

            struct appdata
            {
                float4 vertex : POSITION;
                float4 texcoord : TEXCOORD0;
				float4 color : COLOR;
            };

            struct v2f
            {
				float4 vertex : SV_POSITION;
                float4 texcoord : TEXCOORD0;             
				fixed4 color : COLOR;
            };

            v2f vert (appdata IN)
            {
                v2f OUT;
				UNITY_INITIALIZE_OUTPUT(v2f, OUT);

				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord.xy = TRANSFORM_TEX(IN.texcoord, _MainTex);
				OUT.texcoord.z = IN.texcoord.z;
				OUT.color = IN.color;

                return OUT;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				float2 uv = i.texcoord.xy;
				
				float sinFactor = sin(_Rotation);
				float cosFactor = cos(_Rotation);
				float2x2 rotationMatrix = float2x2(cosFactor, sinFactor, -sinFactor, cosFactor);
				float2 uvRotationOffset = float2(0.5, 0.5);
				uv = mul(uv - uvRotationOffset, rotationMatrix) + uvRotationOffset;

				int tileCount = _SubdivisionCount * _SubdivisionCount;

				float tileIndex = fmod(floor(i.texcoord.z), tileCount);

				float2 tileScale = float2(1.0f / _SubdivisionCount, 1.0f / _SubdivisionCount);

				float tileX = fmod(tileIndex, _SubdivisionCount);
				float tileY = floor(tileIndex / _SubdivisionCount);

				float offsetX = tileX / _SubdivisionCount;
				float offsetY = tileY / _SubdivisionCount;

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
