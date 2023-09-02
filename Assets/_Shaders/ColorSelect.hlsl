void ColorSelect_float(float4 Source, float4 Target, float Range, out float4 Output){
    float4 result =  {0,0,0,0};
    
    if (Source.x >= Target.x - Range && Source.x <= Target.x + Range)
    {
        result.x = Source.x;
        if (Source.y >= Target.y - Range && Source.y <= Target.y + Range)
        {
            result.y = Source.y;
            if (Source.z >= Target.z - Range && Source.z <= Target.z + Range)
                result.z = Source.z;
        }
    }
    
    Output = result;
}