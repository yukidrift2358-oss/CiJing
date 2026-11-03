Shader "CiJing/SceneArt" {
 Properties { [PerRendererData] _MainTex("Painting",2D)="white"{} _Loss("Petals fallen",Range(0,1))=0 _Growth("Ink to colour",Range(0,1))=.5 _Reveal("Garden visible",Range(0,1))=1 }
 SubShader { Tags {"Queue"="Transparent" "RenderType"="Transparent"} Cull Off ZWrite Off ZTest [unity_GUIZTestMode] Blend SrcAlpha OneMinusSrcAlpha
 Pass { CGPROGRAM
 #pragma vertex vert
 #pragma fragment frag
 #include "UnityCG.cginc"
 #include "UnityUI.cginc"
 struct appdata{float4 vertex:POSITION;float4 color:COLOR;float2 uv:TEXCOORD0;};struct v2f{float4 vertex:SV_POSITION;float4 color:COLOR;float2 uv:TEXCOORD0;float4 world:TEXCOORD1;};
 sampler2D _MainTex;float _Loss,_Growth,_Reveal;float4 _ClipRect;
 v2f vert(appdata v){v2f o;o.world=v.vertex;o.vertex=UnityObjectToClipPos(v.vertex);o.uv=v.uv;o.color=v.color;return o;}
 fixed4 frag(v2f i):SV_Target{fixed4 c=tex2D(_MainTex,i.uv);float gray=dot(c.rgb,float3(.3,.59,.11));float rose=smoothstep(.025,.11,c.r-c.g)*smoothstep(.01,.09,c.r-c.b);c.a*=1-rose*_Loss*.88;c.a*=lerp(1,_Reveal,smoothstep(.61,.68,i.uv.x));c.rgb=lerp(gray.xxx,c.rgb,_Growth);c.a*=smoothstep(0,.10,i.uv.y)*smoothstep(0,.12,1-i.uv.y);c.a*=UnityGet2DClipping(i.world.xy,_ClipRect);return c*i.color;}
 ENDCG }
 }
}
