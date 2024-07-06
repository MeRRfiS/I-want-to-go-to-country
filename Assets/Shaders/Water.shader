Shader "Unlit/Water"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _TintColor ("Tint Color", Color) = (0.0, 0.5, 0.7, 1.0)
        _WaveSpeed ("Wave Speed", Range(0.1, 1.0)) = 0.5
        _WaveHeight ("Wave Height", Range(0.0, 1.0)) = 0.1
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        LOD 200
        Cull Off

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
            fixed4 _TintColor;
            float _WaveSpeed;
            float _WaveHeight;

            v2f vert(appdata v)
            {
                v2f o;
                float wave = sin((v.vertex.x + _Time.y * _WaveSpeed) * 2.0) * _WaveHeight;
                wave += cos((v.vertex.z + _Time.y * _WaveSpeed) * 2.0) * _WaveHeight;
                v.vertex.y += wave;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                col.rgb = lerp(col.rgb, _TintColor.rgb, 0.5);
                return col;
            }
            ENDCG
        }
    }

    Fallback"Transparent/Cutout/VertexLit"
}
