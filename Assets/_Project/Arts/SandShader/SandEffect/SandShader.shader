Shader "Custom/SandShader"
{
    Properties
    {
        _Progress ("Transition Progress", Range(0, 1)) = 0
        [HDR] _SandColor ("Base Color", Color) = (1.2, 0.9, 0.6, 1)
        _NoiseScale ("Noise Scale", Float) = 8
        _EdgeSharpness ("Edge Sharpness", Range(0.1, 5)) = 1.5
        _ParticleAmount ("Particle Amount", Range(0, 1)) = 0.5
        _SandDensity ("Density", Range(0, 1)) = 0.6
        
        [Header(Trail Settings)]
        [HDR] _TrailColor ("Trail Color", Color) = (1.0, 0.7, 0.4, 0.4)
        _TrailLength ("Trail Length", Range(0, 1)) = 0.4
        _TrailFade ("Trail Fade", Range(0.1, 2)) = 0.6
        
        [Header(Sparkle Settings)]
        [HDR] _SparkleColor ("Sparkle Color", Color) = (2, 1.5, 0.8, 1)
        _SparkleAmount ("Sparkle Amount", Range(0, 1)) = 0.6
        _SparkleSize ("Sparkle Size", Range(20, 150)) = 60
        _SparkleSpeed ("Sparkle Speed", Range(0, 5)) = 2
        
        [Header(Softness)]
        _Softness ("Edge Softness", Range(0.1, 1)) = 0.5
        _Turbulence ("Turbulence", Range(0, 1)) = 0.4
    }
    
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Overlay" "IgnoreProjector"="True" }
        LOD 100
        
        Pass
        {
            ZWrite Off
            ZTest Always
            Cull Off
            Blend One One
            
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
            
            float _Progress;
            float4 _SandColor;
            float _NoiseScale;
            float _EdgeSharpness;
            float _ParticleAmount;
            float _SandDensity;
            
            float4 _TrailColor;
            float _TrailLength;
            float _TrailFade;
            
            float4 _SparkleColor;
            float _SparkleAmount;
            float _SparkleSize;
            float _SparkleSpeed;
            
            float _Softness;
            float _Turbulence;
            
            // 노이즈 함수들
            float random(float2 st)
            {
                return frac(sin(dot(st.xy, float2(12.9898, 78.233))) * 43758.5453123);
            }
            
            float noise(float2 st)
            {
                float2 i = floor(st);
                float2 f = frac(st);
                
                float a = random(i);
                float b = random(i + float2(1.0, 0.0));
                float c = random(i + float2(0.0, 1.0));
                float d = random(i + float2(1.0, 1.0));
                
                float2 u = f * f * (3.0 - 2.0 * f);
                
                return lerp(a, b, u.x) + (c - a) * u.y * (1.0 - u.x) + (d - b) * u.x * u.y;
            }
            
            // 부드러운 FBM
            float fbm(float2 st)
            {
                float value = 0.0;
                float amplitude = 0.5;
                float frequency = 1.0;
                
                for (int i = 0; i < 5; i++)
                {
                    value += amplitude * noise(st * frequency);
                    frequency *= 2.0;
                    amplitude *= 0.5;
                }
                
                return value;
            }
            
            // 부드러운 난류 효과
            float turbulence(float2 st)
            {
                float t = 0.0;
                float amplitude = 1.0;
                
                for (int i = 0; i < 3; i++)
                {
                    t += abs(noise(st) * 2.0 - 1.0) * amplitude;
                    st *= 2.0;
                    amplitude *= 0.5;
                }
                
                return t;
            }
            
            // 반짝이 효과
            float sparkle(float2 uv, float time)
            {
                float2 id = floor(uv);
                float2 gv = frac(uv) - 0.5;
                
                float sparkleVal = 0.0;
                
                for (int y = -1; y <= 1; y++)
                {
                    for (int x = -1; x <= 1; x++)
                    {
                        float2 offset = float2(x, y);
                        float rnd = random(id + offset);
                        
                        // 반짝이는 타이밍
                        float t = frac(time + rnd * 10.0);
                        float blink = smoothstep(0.7, 0.75, t) * smoothstep(1.0, 0.95, t);
                        
                        float2 p = offset + (random(id + offset + 0.5) - 0.5) * 0.6 - gv;
                        float d = length(p);
                        
                        sparkleVal += (1.0 - smoothstep(0.0, 0.3, d)) * blink;
                    }
                }
                
                return sparkleVal;
            }
            
            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
            
            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;
                
                // 왼쪽 아래에서 오른쪽 위로 진행
                float diagonal = (uv.x + uv.y) * 0.5;
                
                // 부드러운 노이즈 레이어들 (1->0 방향으로 수정)
                float n1 = fbm(uv * _NoiseScale + (1.0 - _Progress) * 2);
                float n2 = fbm(uv * _NoiseScale * 0.5 + (1.0 - _Progress) * 1.5);
                float turb = turbulence(uv * _NoiseScale * 0.3 + (1.0 - _Progress)) * _Turbulence;
                
                // 전환 진행도 (1일 때 완전히 화면 바깥, 0일 때 화면 통과)
                float transitionEdge = (1.0 - _Progress) * 3.5 - 1.0;
                
                // 메인 먼지/연기 마스크 (부드럽게)
                float dustMask = smoothstep(transitionEdge - _Softness, transitionEdge, diagonal + n1 * 0.4 + turb * 0.3) * 
                                 (1.0 - smoothstep(transitionEdge, transitionEdge + _Softness, diagonal + n1 * 0.4 + turb * 0.3));
                
                // 레이어드 밀도 (깊이감)
                float density1 = pow(dustMask, 1.5);
                float density2 = pow(dustMask, 2.5) * 0.6;
                
                // 금가루 입자들 (1->0 방향)
                float particles = 0.0;
                for (int j = 0; j < 3; j++)
                {
                    float offset = float(j) * 0.333;
                    float p = step(0.6, noise(uv * (30.0 + j * 10.0) + (1.0 - _Progress) * (15.0 + j * 5.0) + offset));
                    particles += p * (1.0 - float(j) * 0.2);
                }
                particles *= dustMask * _ParticleAmount * 0.5;
                
                // 반짝이는 금가루 (1->0 방향)
                float sparkles = sparkle(uv * _SparkleSize, _Time.y * _SparkleSpeed + (1.0 - _Progress) * 8);
                sparkles *= dustMask * _SparkleAmount;
                
                // 잔상 효과 (부드럽고 몽환적) (1->0 방향)
                float trailMask = smoothstep(transitionEdge, transitionEdge + _TrailLength * 0.5, diagonal + n2 * 0.3);
                trailMask *= (1.0 - smoothstep(transitionEdge + _TrailLength * 0.5, transitionEdge + _TrailLength, diagonal + n2 * 0.3));
                float trailNoise = pow(fbm(uv * _NoiseScale * 0.8 + (1.0 - _Progress) * 1.2), _TrailFade);
                trailMask *= trailNoise;
                
                // 색상 변화 (따뜻한 그라데이션)
                float colorVariation = n2 * 0.4 + 0.6;
                float3 baseColor = _SandColor.rgb * colorVariation;
                
                // 가장자리 밝게 (후광 효과)
                float edgeGlow = pow(dustMask, 0.8) * 1.5;
                
                // 최종 색상 합성
                float3 dustColor = baseColor * (density1 + density2 * 0.5) * _SandDensity;
                dustColor += baseColor * edgeGlow * 0.3;
                dustColor += particles * baseColor * 0.8;
                dustColor += _SparkleColor.rgb * sparkles;
                
                float3 trailFinal = _TrailColor.rgb * trailMask;
                
                float3 finalColor = dustColor + trailFinal;
                
                // 알파 계산
                float alpha = (density1 + density2 * 0.3) * _SandDensity;
                alpha += sparkles * 0.3;
                alpha += trailMask * _TrailColor.a;
                
                // 부드러운 페이드 인/아웃 (1->0 방향)
                alpha *= smoothstep(1.0, 0.85, _Progress) * (1.0 - smoothstep(0.15, 0.0, _Progress));
                
                return float4(finalColor, alpha);
            }
            ENDCG
        }
    }
}
