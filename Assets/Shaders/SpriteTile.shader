Shader "Sprites/Tile Grid"
{
    Properties
    {
		_MainTex("Base (RGB) Trans (A)", 2D) = "white" {}
    }
    SubShader
    {
		Tags {"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent"}
        LOD 100

		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float4 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv.xy = TRANSFORM_TEX(v.uv, _MainTex);
				o.uv.zw = v.uv.zw;

                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				float2 uv = i.uv.xy;

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
                return col;
            }
            ENDCG
        }
    }
}
