SAMPLER(sampler_point_clamp);


void GetDepth_float(float2 uv, out float Depth)
{
    Depth = SHADERGRAPH_SAMPLE_SCENE_DEPTH(uv);
}


void GetNormal_float(float2 uv, out float3 Normal)
{
    Normal = SAMPLE_TEXTURE2D(_NormalsBuffer, sampler_point_clamp, uv).rgb;
}

void GetCrossSampleUVs_float(float4 uv, float2 texelSize, float OffsetMult, out float2 UVoriginal, out float2 UVtopright, out float2 UVbottomleft, out float2 UVtopleft, out float2 UVbottomright) {
    UVoriginal = uv;
    UVtopright = uv.xy + float2(texelSize.x, texelSize.y) * OffsetMult;
    UVbottomleft = uv.xy - float2(texelSize.x, texelSize.y) * OffsetMult;
    UVtopleft = uv.xy + float2(-texelSize.x * OffsetMult, texelSize.y * OffsetMult);
    UVbottomright = uv.xy + float2(texelSize.x * OffsetMult, -texelSize.y * OffsetMult);
}