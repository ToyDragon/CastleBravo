Shader "Custom/water"
{
    Properties
    {
        _WaveCount ("Wave Count", Int) = 0
    }
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Opaque" "IgnoreProjector"="True" }
        LOD 100

        Pass
        {
            Cull Back
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            float4 _Waves[128];
            int _WaveCount;

            float3 _MainLightDir;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float height : TEXCOORD0;
                float3 normal : TEXCOORD1;
                float3 wsPos : TEXCOORD2;
            };

            float heightAt(float2 xz) {
                float h = 0;
                for (int waveI = 0; waveI < _WaveCount; waveI++) {
                    float4 wave = _Waves[waveI];
                    float t = dot(xz*wave.w/16.0, normalize(wave.xy)) + wave.z + (1 + wave.w/3)*_Time.w*.25;
                    h += .25 * sin(t) / wave.w;
                }
                return h;
            }

            float3 dirAt(float2 xz) {
                float3 dir = float3(0, 0, 0);
                for (int waveI = 0; waveI < _WaveCount; waveI++) {
                    float4 wave = _Waves[waveI];
                    float t = dot(xz*wave.w/16.0, normalize(wave.xy)) + wave.z + (1 + wave.w/3)*_Time.w*.25;
                    dir += 1 * normalize(float3(wave.x, 0, wave.y)) * sin(t + 3.14159*.5) / wave.w;
                }
                return dir;
            }

            float map(float val, float imin, float imax, float omin, float omax) {
                return (clamp(val, imin, imax) - imin) / (imax - imin) * (omax - omin) + omin;
            }

            v2f vert (appdata v)
            {
                v2f o;
                float2 waveXZ = mul(unity_ObjectToWorld, v.vertex).xz;
                o.height = heightAt(waveXZ);
                float3 normalOffset = dirAt(waveXZ);
                o.wsPos = mul(unity_ObjectToWorld, v.vertex).xyz + float3(0, o.height, 0) + normalOffset*2;
                o.vertex = UnityObjectToClipPos(v.vertex + float3(0, o.height, 0) + normalOffset*2);

                o.normal = normalize(float3(0, 1, 0) + .04*normalOffset);
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float t = saturate(i.height*.5 + .5);
                float3 c = float3(.02, .02, .07)*map(t, 0, .3, .2, 1);
                c += t*t*float3(.5, .5, .9)*.3;
                c += map(t, .9, 1, 0, .3) * float3(1,1,1);

                float sunFactor = map(-_MainLightDir.y, -.4, 1, .05, 1);
                c *= sunFactor;
                float3 wsViewDir = normalize(i.wsPos - _WorldSpaceCameraPos);
                float3 reflectDir = wsViewDir - 2*i.normal*dot(wsViewDir, i.normal);
                c += float3(4,2,2) * map(-dot(reflectDir, _MainLightDir), .985, 1, 0, 1) * map(-_MainLightDir.y, -.12, .2, 0, .7);
                return float4(c, 1);
            }
            ENDCG
        }
    }
}
