Shader "Custom/skybox"
{
    Properties
    {
        _StarColors ("Star Colors", 2D) = "white" {}
        _StarCells ("Star Cells", 2D) = "white" {}
        _TwinkleMap ("Twinkle Map", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Opaque" "IgnoreProjector"="True" }
        LOD 100
        
        Pass
        {
            Cull Front
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
                float3 camDir : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _StarColors;
            float4 _StarColors_ST;
            sampler2D _StarCells;
            float4 _StarCells_ST;
            sampler2D _TwinkleMap;
            float4 _TwinkleMap_ST;

            float3 _MainLightDir;
            float3 _MoonDir;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.camDir = WorldSpaceViewDir(v.vertex);
                return o;
            }

            float map(float val, float imin, float imax, float omin, float omax) {
                return (clamp(val, imin, imax) - imin) / (imax - imin) * (omax - omin) + omin;
            }

            float4 frag (v2f i) : SV_Target
            {
                float3 wsdir = -normalize(i.camDir);
                float sunXZAlign = dot(wsdir.xz, _MainLightDir.xz);
                float sunHeightEffect = map(_MainLightDir.y, 0, 1, .3, 0);
                float horizonStrength = map(sunXZAlign, -.5, .7, .05, sunHeightEffect);
                float v = map(wsdir.y, -.1, horizonStrength, .5, 0);
                float3 horizonColor = lerp(float3(0, 0, .5), float3(1, .2, .2), pow(saturate(sunXZAlign), 3));
                float4 col = float4(horizonColor*pow(v,2), 1);

                float baseSkyStr = map(_MainLightDir.y, -.05, .7, 0, 1);
                col += baseSkyStr*float4(float3(.5, .5, 1), 0);

                float laggingSkyStr = map(_MainLightDir.y, -.3, .4, 0, 1);
                col += laggingSkyStr*float4(float3(.2, .2, .4), 0) * pow(saturate(dot(wsdir, _MainLightDir)), 3);

                float2 starUV = wsdir.xz*.5 + float2(.5, .5);
                float cellstr = pow(1 - tex2D(_StarCells, starUV).r, 2);
                float starStren = pow(map(wsdir.y, horizonStrength*3 - .8, 1, 0, .5), 2) * (1 - baseSkyStr);
                float3 starCol = tex2D(_StarColors, starUV);
                float twinkle = pow(saturate(tex2D(_TwinkleMap, starUV + float2(sin(_Time.y*.2), cos(_Time.y*.2))*.1)), 1.5);
                col += clamp(map(cellstr, 1 - starStren, 1, 0, starStren), 0, 1) * float4(starCol, 0) * twinkle;

                float3 sunColor = saturate(map(dot(_MainLightDir.xyz, wsdir), .999, 1, 0, 1)) * float3(3, 1, .6);
                col += float4(sunColor, 0);

                float3 moonColor = saturate(map(dot(_MoonDir.xyz, wsdir), .999, 1, 0, 6)) * float3(.7, .5, .2);
                col += float4(moonColor, 0);

                return col;
            }
            ENDCG
        }
    }
}
