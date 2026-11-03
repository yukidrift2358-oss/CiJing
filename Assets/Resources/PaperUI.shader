Shader "CiJing/PaperUI" {
 Properties {
  [PerRendererData] _MainTex("Texture",2D)="white"{}
  _StencilComp("Stencil Comparison",Float)=8 _Stencil("Stencil",Float)=0
  _StencilOp("Stencil Operation",Float)=0 _StencilWriteMask("Write Mask",Float)=255
  _StencilReadMask("Read Mask",Float)=255 _ColorMask("Color Mask",Float)=15
 }
 SubShader {
  Tags {"Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True"}
  Stencil {Ref [_Stencil] Comp [_StencilComp] Pass [_StencilOp] ReadMask [_StencilReadMask] WriteMask [_StencilWriteMask]}
  Cull Off Lighting Off ZWrite Off ZTest [unity_GUIZTestMode] Blend SrcAlpha OneMinusSrcAlpha ColorMask [_ColorMask]
  Pass { CGPROGRAM
   #pragma vertex vert
   #pragma fragment frag
   #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
   #include "UnityCG.cginc"
   #include "UnityUI.cginc"
   struct appdata{float4 vertex:POSITION;float4 color:COLOR;float2 uv:TEXCOORD0;};
   struct v2f{float4 vertex:SV_POSITION;float4 color:COLOR;float2 uv:TEXCOORD0;float4 world:TEXCOORD1;};
   float4 _ClipRect;
   v2f vert(appdata v){v2f o;o.world=v.vertex;o.vertex=UnityObjectToClipPos(v.vertex);o.uv=v.uv;o.color=v.color;return o;}
   float hash(float2 p){return frac(sin(dot(p,float2(127.1,311.7)))*43758.5453);}
   fixed4 frag(v2f i):SV_Target {
    float2 edge=min(i.uv,1-i.uv);float grain=hash(floor(i.world.xy*1.5));
    float feather=smoothstep(0,.012,min(edge.x,edge.y)+sin(i.uv.x*181)*.0015+sin(i.uv.y*217)*.001);
    float fiber=sin(i.world.x*.32+sin(i.world.y*.1))*sin(i.world.y*.75)*.003;
    fixed4 c=i.color;c.rgb*=.985+grain*.021+fiber;c.a*=feather;
    #ifdef UNITY_UI_CLIP_RECT
     c.a*=UnityGet2DClipping(i.world.xy,_ClipRect);
    #endif
    return c;
   }
  ENDCG }
 }
}
