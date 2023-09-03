void ColorSelect_float(float4 Source, float4 Target, float Range, out float4 Output)
{
    Output = distance(Source, Target) <= Range ? Source : float4(0, 0, 0, 0);
}
