Shader "Thinker/CustomSpriteShader"
{
	Properties
	{
		[Header(Texture)]
		_MainTex ("Main Texture", 2D) = "white" {}

		[Header(Color)]
		_Color("Tint", Color) = (1,1,1,1)
		_Colorize("Colorize",Color) = (1,1,1,0)

		[Header(UV Scrolling)]
		_HAutoScroll ("Scroll - Horizontal Scrolling Speed", float) = 0
		_VAutoScroll("Scroll - Vertical Scrolling Speed", float) = 0
		_ScrollStep("Scroll - Pixel Stepping", float) = 0
	}
	SubShader
	{

		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		LOD 100
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha 
		Cull Off
		
		Pass
		{
			CGPROGRAM
			#pragma target 3.0
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
				float2 screenPos:TEXCOORD2;
				UNITY_FOG_COORDS(1)
				float4 vertex : POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _MainTex_TexelSize;
			fixed4 _Color;
			fixed4 _Colorize;
						
			float _HAutoScroll;
			float _VAutoScroll;
			float _ScrollStep;
	
			float step(float value, float step){
			  float absValue = abs(value);
			  step = abs(step);

			  float low = absValue - absValue % step;
			  float high = low + step;
			   
			  float result = absValue - low < high - absValue ? low : high;
			  return result * sign(value);
			}

			
       /* static float GetStep(float value, float step)
        {
            var absValue = Math.Abs(value);
            step = Math.Abs(step);

            var low = absValue - absValue % step;
            var high = low + step;

            var result = absValue - low < high - absValue ? low : high;
            return result * Math.Sign(value);
        }*/

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.vertex = UnityPixelSnap(o.vertex);
				o.screenPos = ComputeScreenPos(o.vertex);

				// UV Scrolling
				o.uv = (TRANSFORM_TEX(v.uv, _MainTex) - float2(_Time[1] * _HAutoScroll, _Time[1] * _VAutoScroll) % float2(1,1));
				if (abs(_ScrollStep)>0) {
					o.uv = float2(step(o.uv.x,_ScrollStep),step(o.uv.y,_ScrollStep));
				}
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{				
				fixed4 col = tex2D(_MainTex, i.uv) * _Color;

				// Colorize
				col = fixed4(_Colorize.r* _Colorize.a + col.r*(1-_Colorize.a),
							 _Colorize.g * _Colorize.a + col.g*(1 - _Colorize.a),
							 _Colorize.b * _Colorize.a + col.b*(1 - _Colorize.a), col.a);

				return col;
			}

			ENDCG
		}
	}
}