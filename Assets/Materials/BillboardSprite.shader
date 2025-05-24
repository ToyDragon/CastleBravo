Shader "Custom/BillboardSprite"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1, 1, 1)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 100
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;

            v2f vert (appdata v)
            {
                v2f o;
                float4 camPos = float4(UnityObjectToViewPos(float3(0,0,0)).xyz, 1.0);
                float2 x = mul(UNITY_MATRIX_M, float4(1, 0, 0, 0)).xy * v.vertex.x;
                float2 y = mul(UNITY_MATRIX_M, float4(0, 1, 0, 0)).xy * v.vertex.y;
                float4 viewDir = float4(x + y, 0, 0.0);
                o.vertex = mul(UNITY_MATRIX_P, camPos + viewDir);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return tex2D(_MainTex, i.uv) * _Color;
            }
            ENDCG
        }
    }
}
