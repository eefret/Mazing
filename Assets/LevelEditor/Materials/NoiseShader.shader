Shader "CustomShaders/FlatColor" {

	Properties {
		_Color ("Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_NoiseRes ("Noise Resolution", Float) = 16
		_NoiseAlpha ("Noise Alpha", Range(0, 1)) = 0.1
	}
	
	SubShader {
		
		Pass {
			CGPROGRAM
			
			//pragmas
			#pragma vertex vert
			#pragma fragment frag
			
			//user defined variables
			uniform float4 _Color;
			uniform float _NoiseRes;
			uniform float _NoiseAlpha;
			
			//base input struct
			struct vertexInput {
				float4 vertex : POSITION;
			};
			
			struct vertexOutput {
				float4 pos : SV_POSITION;
				float4 worldPos : TEXCOORD0;
			};
			
			//vertex function
			vertexOutput vert(vertexInput v) {
				vertexOutput o;
				
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.worldPos = mul(_Object2World, v.vertex);
				
				return o;
			}
			
	         float rand(float3 myVector)  {
	             return frac(sin( dot(myVector ,float3(12.9898,78.233,45.5432) )) * 43758.5453);
	         }
         
			//fragment function
            float4 frag(vertexOutput i) : COLOR {
            	float4 noise = rand((round((i.worldPos + 0.5) * _NoiseRes)).xyz + round(_Time.w));
            
				return _Color * (1 - _NoiseAlpha) + noise * _NoiseAlpha;
			}
			
			ENDCG
		}
		
	}
	
	//Fallback "Diffuse"
}