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
                float3 wsDir : TEXCOORD0;
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

            float4 _RotationQuaternion;
            float4 _InvRotationQuaternion;

            float3 _RotationAxis;
            float3 _RotationForward;

            float4 qmul(float4 q1, float4 q2)
            {
                return float4(
                    q2.xyz * q1.w + q1.xyz * q2.w + cross(q1.xyz, q2.xyz),
                    q1.w * q2.w - dot(q1.xyz, q2.xyz)
                );
            }

            float3 rotate(float3 v, float4 r)
            {
                float4 r_c = r * float4(-1, -1, -1, 1);
                return qmul(r, qmul(float4(v, 0), r_c)).xyz;
            }


            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex*1000);
                o.wsDir = WorldSpaceViewDir(v.vertex*1000);
                return o;
            }

            float map(float val, float imin, float imax, float omin, float omax) {
                return (clamp(val, imin, imax) - imin) / (imax - imin) * (omax - omin) + omin;
            }

            float4 frag (v2f i) : SV_Target
            {
                float3 wsDir = -normalize(i.wsDir); // doesn't change through day - fixed in ground space
                float3 wsLightDir = normalize(rotate(-_MainLightDir, _InvRotationQuaternion)); // ground space direction to the sun - changes through day
                float3 skyDir = normalize(rotate(wsDir, _RotationQuaternion)); // changes through day
                float4 col = float4(0,0,0,1);

                // Horizon color based on height of sun
                float sunXZAlign = dot(wsDir.xz, wsLightDir.xz);
                float sunHeightEffect = map(wsLightDir.y, 0, 1, .3, 0);
                float horizonStrength = map(sunXZAlign, -.5, .7, .05, sunHeightEffect);
                float v = map(wsDir.y, -.1, horizonStrength, .5, 0);
                float3 horizonColor = lerp(float3(0, 0, .5), float3(1, .2, .2), pow(saturate(sunXZAlign), 3));
                col += float4(horizonColor*pow(v,2), 0);

                // Blue sky when sun is up
                float baseSkyStr = map(wsLightDir.y, -.05, .7, 0, 1);
                col += baseSkyStr*float4(float3(.5, .5, 1), 0);
                float laggingSkyStr = map(wsLightDir.y, -.3, .4, 0, 1);
                col += laggingSkyStr*float4(float3(.2, .2, .4), 0) * pow(saturate(dot(wsLightDir, wsDir)), 3);

                // Stars. Map spherical coords to cyclindrical coords to avoid seem on the star textures.
                float2 skyxz = float2(
                    dot(skyDir, _RotationForward),
                    dot(skyDir, cross(_RotationForward, _RotationAxis))
                );
                float2 starUV = float2(
                    atan2(skyxz.x, skyxz.y) / (2*3.1415) + 0.5,
                    dot(skyDir, _RotationAxis)/(2*3.1415)
                );
                float cellstr = pow(1 - tex2D(_StarCells, starUV).r, 2);
                float starStren = pow(map(wsDir.y, horizonStrength*3 - .8, 1, 0, .5), 2) * (1 - baseSkyStr);
                float3 starCol = tex2D(_StarColors, starUV);
                float twinkle = pow(saturate(tex2D(_TwinkleMap, starUV + float2(sin(_Time.y*.2), cos(_Time.y*.2))*.1)), 1.5);
                col += clamp(map(cellstr, 1 - starStren, 1, 0, starStren), 0, 1) * float4(starCol, 0) * twinkle;

                // Sun
                float3 sunColor = saturate(map(dot(wsLightDir, wsDir), .999, 1, 0, 1)) * float3(3, 1, .6);
                col += float4(sunColor, 0);

                // Moon
                float3 moonColor = saturate(map(dot(_MoonDir.xyz, skyDir), .999, 1, 0, 6)) * float3(.7, .5, .2);
                col += float4(moonColor, 0);

                return col;
            }
            ENDCG
        }
    }
}
