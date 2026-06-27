Shader "HammerGolf/HighlightOutline"
{
    Properties
    {
        _HighlightColor ("Highlight Color", Color) = (1, 1, 0, 1)
        _OutlineThickness ("Outline Thickness", Range(1.0, 1.2)) = 1.05
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent+100" "RenderPipeline"="UniversalPipeline" }
        LOD 100

        Pass
        {
            Name "Outline"
            Tags { "LightMode"="UniversalForward" }

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            ZTest LEqual
            Cull Front

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _HighlightColor;
                float _OutlineThickness;
            CBUFFER_END

            Varyings vert(Attributes input)
            {
                Varyings output;
                
                // Expand along normal
                float3 positionOS = input.positionOS.xyz + input.normalOS * (_OutlineThickness - 1.0);
                output.positionCS = TransformObjectToHClip(positionOS);
                
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                return _HighlightColor;
            }
            ENDHLSL
        }
    }
}
