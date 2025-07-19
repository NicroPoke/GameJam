Shader "Custom/SpriteLitUnlit" {
  Properties {
    _Color ("Tint", Color) = (1,1,1,1)
    _MainTex ("Texture", 2D) = "white" {}

    _ColorStart ("StartColor", Color) = (1, 0, 1, 1)
    _EndColor ("End color", Color) = (0, 0, 1, 1)

    _SpiiralTex("Spiral", 2D) = "white" {}
    _SpiralCol("Spiral col", Color) = (0, 0, 0, 0)
  }
  SubShader {
    Tags { "RenderType"="Transparent" "Queue"="Transparent" }
    Blend SrcAlpha OneMinusSrcAlpha
    Cull Off
    ZWrite Off
    Pass {
      CGPROGRAM
      #pragma vertex vert
      #pragma fragment frag
      #include "UnityCG.cginc"

      sampler2D _MainTex;
      float4 _MainTex_ST;
      fixed4 _Color;

      fixed4 _ColorStart;
      fixed4 _EndColor;

      sampler2D _SpiiralTex;
      fixed4 _SpiralCol;

      struct appdata { float4 vertex : POSITION; float2 uv : TEXCOORD0; fixed4 color : COLOR; };
      struct v2f { float4 pos : SV_POSITION; float2 uv : TEXCOORD0; fixed4 color : COLOR; };

      v2f vert (appdata v) {
        v2f o;
        o.pos   = UnityObjectToClipPos(v.vertex);
        o.uv    = TRANSFORM_TEX(v.uv, _MainTex);
        o.color = v.color;
        return o;
      }

      fixed4 frag (v2f i) : SV_Target {
        fixed4 texColor = tex2D(_MainTex, i.uv);

        fixed4 gradientColor = lerp(_ColorStart, _EndColor, i.uv.x);

        fixed4 finalColor = texColor * gradientColor * _Color * i.color;

        float2 spiralUV = i.uv * 3;
        spiralUV.x = frac(spiralUV.x - _Time.y * 0.9); 
        spiralUV.y = frac(spiralUV.y);
        fixed4 spiralColor = tex2D(_SpiiralTex, spiralUV);
        bool hasBase = texColor.a > 0.01 && dot(texColor.rgb, texColor.rgb) > 0.001;
        bool hasSpiral = spiralColor.a > 0.01 && dot(spiralColor.rgb, spiralColor.rgb) > 0.001;

        if (hasBase && hasSpiral){
            finalColor.rgb = lerp(finalColor.rgb, fixed4(0, 0, 0, 0), spiralColor.a); 
        }

        return finalColor;
      }
      ENDCG
    }
  }
}