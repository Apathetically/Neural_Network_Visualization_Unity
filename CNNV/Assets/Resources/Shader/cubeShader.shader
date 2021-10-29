Shader "Custom/cubeRender"
{
    Properties{
         _MainTex("Main Tex", 2D) = "white" {}
    }

        SubShader{

            Pass {

                Cull Off

                CGPROGRAM

                #pragma vertex vert
                #pragma fragment frag

                #include "Lighting.cginc"

                sampler2D _MainTex;
                float4 _MainTex_ST;

                struct a2v {
                    float4 vertex : POSITION;
                    float4 texcoord : TEXCOORD0;
                };

                struct v2f {
                    float4 pos : SV_POSITION;
                    float2 uv : TEXCOORD2;
                };

                v2f vert(a2v v) {
                    v2f o;
                    o.pos = UnityObjectToClipPos(v.vertex);
                    o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target {

                    fixed4 texColor = tex2D(_MainTex, i.uv);
                    return texColor;
                    // return fixed4(texColor.rg, 0.0f, 1.0f);
                }

                ENDCG
            }
    }
}