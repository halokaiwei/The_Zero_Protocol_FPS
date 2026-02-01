Shader "Custom/CyberAbyss"
{
    Properties
    {
        _MainColor ("Core Color", Color) = (0, 1, 1, 1) // Electric Cyan
        _GridScale ("Grid Scale", Float) = 20.0
        _Speed ("Flow Speed", Float) = 0.5
        _Depth ("Depth Intensity", Range(0, 1)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Blend One One // Additive blending for "glow" look
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
            };

            float4 _MainColor;
            float _GridScale;
            float _Speed;
            float _Depth;

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

           fixed4 frag (v2f i) : SV_Target {
                float2 uv1 = i.uv * _GridScale + _Time.y * _Speed;
                float2 uv2 = i.uv * (_GridScale * 1.5) - _Time.y * (_Speed * 0.7);
    
                float grid1 = step(0.95, frac(uv1.x)) + step(0.95, frac(uv1.y));
                float grid2 = step(0.98, frac(uv2.x)) + step(0.98, frac(uv2.y));
    
                float pulse = sin(_Time.y * 2.0) * 0.2 + 0.8;
    
                float intensity = (grid1 + grid2 * _Depth) * pulse;
    
                float3 finalRGB = _MainColor.rgb * intensity;
    
                return float4(finalRGB, 1.0);
            }
            ENDCG
        }
    }
}