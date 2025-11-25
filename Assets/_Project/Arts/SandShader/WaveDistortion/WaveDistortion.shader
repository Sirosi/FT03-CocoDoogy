Shader "Custom/WaveDistortion"
{
   Properties
    {
        _WaveSpeed ("Wave Speed", Range(0, 10)) = 2
        _WaveFrequency ("Wave Frequency", Range(0, 50)) = 10
        _WaveAmplitude ("Wave Amplitude", Range(0, 0.2)) = 0.05
        _SecondWaveFreq ("Second Wave Frequency", Range(0, 30)) = 5
        _SecondWaveAmp ("Second Wave Amplitude", Range(0, 0.1)) = 0.02
        _VerticalStretch ("Vertical Stretch", Range(0, 0.5)) = 0.1
        _Tint ("Tint Color", Color) = (1,1,1,0.1)
    }
    
    SubShader
    {
        Tags 
        { 
            "RenderType"="Transparent" 
            "Queue"="Transparent"
            "RenderPipeline"="UniversalPipeline"
        }
        LOD 100
        
        Pass
        {
            Name "DistortionPass"
            
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 screenPos : TEXCOORD1;
            };

            TEXTURE2D(_CameraOpaqueTexture);
            SAMPLER(sampler_CameraOpaqueTexture);
            
            CBUFFER_START(UnityPerMaterial)
                float _WaveSpeed;
                float _WaveFrequency;
                float _WaveAmplitude;
                float _SecondWaveFreq;
                float _SecondWaveAmp;
                float _VerticalStretch;
                float4 _Tint;
            CBUFFER_END

            Varyings vert (Attributes input)
            {
                Varyings output;
                
                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                output.positionCS = vertexInput.positionCS;
                output.uv = input.uv;
                output.screenPos = ComputeScreenPos(output.positionCS);
                
                return output;
            }

            half4 frag (Varyings input) : SV_Target
            {
                float time = _Time.y * _WaveSpeed;
                
                // 첫 번째 물결 - 세로 방향 주파수
                float wave1 = sin(input.uv.y * _WaveFrequency + time) * _WaveAmplitude;
                
                // 두 번째 물결 - 복잡한 패턴
                float wave2 = sin(input.uv.y * _SecondWaveFreq - time * 1.3) * _SecondWaveAmp;
                
                // 세로 방향 늘어짐
                float verticalWave = sin(input.uv.y * 3.14159 * 2 + time * 0.5) * _VerticalStretch;
                
                // 스크린 UV 계산 및 왜곡
                float2 screenUV = input.screenPos.xy / input.screenPos.w;
                float2 distortion = float2(wave1 + wave2, verticalWave * 0.3) * 0.1;
                float2 distortedUV = screenUV + distortion;
                
                // 배경 샘플링
                half4 bgcolor = SAMPLE_TEXTURE2D(_CameraOpaqueTexture, sampler_CameraOpaqueTexture, distortedUV);
                
                // 틴트 적용
                bgcolor = lerp(bgcolor, _Tint, _Tint.a);
                
                return bgcolor;
            }
            ENDHLSL
        }
    }
}
