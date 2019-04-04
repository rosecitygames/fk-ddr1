Shader "Sprites/Tile Grid"
{
    Properties
    {
		_MainTex("Base (RGB) Trans (A)", 2D) = "white" {}
		_Color("Color", Color) = (1,1,1,1)
    }
    SubShader
    {
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
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

            struct appdata
            {
                float4 vertex : POSITION;
                float4 uv : TEXCOORD0;
				float4 color : COLOR;
				float4 rotation : TEXCOORD1;
            };

            struct v2f
            {
				float4 vertex : SV_POSITION;
                float4 uv : TEXCOORD0;             
				fixed4 color : COLOR;
				float4 rotation : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
			fixed4 _Color;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv.xy = TRANSFORM_TEX(v.uv, _MainTex);
				o.uv.zw = v.uv.zw;
				o.color = v.color;
				o.rotation.x = v.rotation.x;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				float2 uv = i.uv.xy;

				float sinFactor = sin(i.rotation.x);
				float cosFactor = cos(i.rotation.x);

				float2x2 rotationMatrix = float2x2(cosFactor, sinFactor, -sinFactor, cosFactor);

				uv = mul(uv - float2(0.5, 0.5), rotationMatrix) + float2(0.5, 0.5);

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
