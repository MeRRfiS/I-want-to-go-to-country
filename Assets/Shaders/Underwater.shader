Shader"Unlit/Underwater"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _TintColor ("Tint Color", Color) = (0.0, 0.5, 0.7, 1.0)
        _Distortion ("Distortion", Range(0.0, 0.1)) = 0.01 // Зменшення діапазону спотворення
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

#include "UnityCG.cginc"

struct appdata_t
{
    float4 vertex : POSITION;
    float2 uv : TEXCOORD0;
};

struct v2f
{
    float4 pos : POSITION;
    float2 uv : TEXCOORD0;
};

sampler2D _MainTex;
fixed4 _TintColor;
float _Distortion;

v2f vert(appdata_t v)
{
    v2f o;
    o.pos = UnityObjectToClipPos(v.vertex);
    o.uv = v.uv;
    return o;
}

fixed4 frag(v2f i) : SV_Target
{
                // М'яке спотворення UV-координат
    float2 distortedUV = i.uv + sin(float2(i.uv.y * 10.0 + _Time.y * 5.0, i.uv.x * 10.0 + _Time.y * 5.0)) * _Distortion;
    fixed4 col = tex2D(_MainTex, distortedUV);
    col.rgb = lerp(col.rgb, _TintColor.rgb, 0.5);
    return col;
}
            ENDCG
        }
    }
FallBack"Diffuse"
}
