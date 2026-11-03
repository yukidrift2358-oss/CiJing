Shader "CiJing/InkGrowth" {
 Properties { [PerRendererData] _MainTex("Sketch",2D)="white"{} _LineTex("Line",2D)="white"{} _ColorTex("Color",2D)="white"{} _Growth("Growth",Range(0,1))=0 _Motion("Motion",Float)=1 }
 SubShader { Tags {"Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" "CanUseSpriteAtlas"="True"} Cull Off Lighting Off ZWrite Off ZTest [unity_GUIZTestMode] Blend SrcAlpha OneMinusSrcAlpha
 Pass { CGPROGRAM
 #pragma vertex vert
 #pragma fragment frag
 #include "UnityCG.cginc"
 struct appdata{float4 vertex:POSITION;float4 color:COLOR;float2 uv:TEXCOORD0;};struct v2f{float4 vertex:SV_POSITION;float4 color:COLOR;float2 uv:TEXCOORD0;};
 sampler2D _MainTex,_LineTex,_ColorTex;float _Growth,_Motion;float4 _InkPulse;
 v2f vert(appdata v){v2f o;o.vertex=UnityObjectToClipPos(v.vertex);o.uv=v.uv;o.color=v.color;return o;}
 float hash(float2 p){return frac(sin(dot(p,float2(127.1,311.7)))*43758.5453);}
 float noise(float2 p){float2 i=floor(p),f=frac(p);f=f*f*(3-2*f);return lerp(lerp(hash(i),hash(i+float2(1,0)),f.x),lerp(hash(i+float2(0,1)),hash(i+1),f.x),f.y);}
 float mask(float g,float2 uv){float field=.52*noise(uv*5)+.28*noise(uv*17)+.20*uv.x;return smoothstep(field-.08,field+.08,g*1.22-.11);}
 fixed4 frag(v2f i):SV_Target {float2 uv=i.uv;float time=_Time.y;float grain=noise(uv*110);float wet=exp(-max(0,time-_InkPulse.z)*1.7)*_InkPulse.w;float d=distance(uv,_InkPulse.xy);float wave=sin(d*48-time*1.3)*exp(-d*5)*wet;uv+=float2(noise(uv*8+time*.06)-.5,wave)*.0016*_Motion;
 fixed4 sketch=tex2D(_MainTex,uv),outlineColor=tex2D(_LineTex,uv),paint=tex2D(_ColorTex,uv);float grey=dot(paint.rgb,float3(.299,.587,.114));paint.rgb=lerp(grey.xxx,paint.rgb,.62);paint.rgb=lerp(float3(.965,.95,.90),paint.rgb,.83);float focus=1-smoothstep(.035,.105,distance(i.uv,float2(.46,.45)));float sketchReveal=max(mask(saturate(.17+_Growth*1.8),i.uv),focus*.65);sketch.rgb=lerp(float3(.965,.95,.90),sketch.rgb,sketchReveal);float a=mask(saturate(_Growth*2),i.uv),b=mask(saturate((_Growth-.5)*2),i.uv);float3 col=lerp(lerp(sketch.rgb,outlineColor.rgb,a),paint.rgb,b);float ink=1-smoothstep(.55,.96,dot(col,float3(.299,.587,.114)));col-=ink*wet*(.012+.012*grain);return fixed4(col,i.color.a);}
 ENDCG }
 }
}
