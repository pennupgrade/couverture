Shader "Hidden/DepthOnly"
{
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            ZWrite On
            ColorMask R // Only write to R channel
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float depth : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.depth = o.pos.z / o.pos.w; // NDC depth
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float d = i.depth * 0.5 + 0.5; // remap to 0-1
                return fixed4(d, 0, 0, 1);
            }
            ENDCG
        }
    }
}
