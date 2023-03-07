Shader "IsisLab/Skybox 6 Sided - Dissolve" {
Properties {
    _Tint ("Tint Color", Color) = (.5, .5, .5, .5)
    [Gamma] _Exposure ("Exposure", Range(0, 8)) = 1.0
    _Rotation ("Rotation", Range(0, 360)) = 0
    _Dissolve ("Dissolve", Range(-1, 1)) = 1
    _StripeWidth ("Stripe Width", Range(0, 1)) = 0.1
    _LeadSharp("Leading Edge Sharpness", float) = 10
    _LeadColor("Leading Edge Color", Color) = (1, 1, 1, 0)
    _MidColor("Mid Color", Color) = (1, 1, 1, 0)
    _TrailColor("Trail Color", Color) = (1, 1, 1, 0)
    _HBarColor("Horizontal Bar Color", Color) = (0.5, 0.5, 0.5, 0)
    [NoScaleOffset] _FrontTex ("Front [+Z]   (HDR)", 2D) = "grey" {}
    [NoScaleOffset] _BackTex ("Back [-Z]   (HDR)", 2D) = "grey" {}
    [NoScaleOffset] _LeftTex ("Left [+X]   (HDR)", 2D) = "grey" {}
    [NoScaleOffset] _RightTex ("Right [-X]   (HDR)", 2D) = "grey" {}
    [NoScaleOffset] _UpTex ("Up [+Y]   (HDR)", 2D) = "grey" {}
    [NoScaleOffset] _DownTex ("Down [-Y]   (HDR)", 2D) = "grey" {}
}

SubShader {
    Tags { "Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox" }
    Cull Off ZWrite Off

    CGINCLUDE
    #include "UnityCG.cginc"

    half4 _Tint;
    half _Exposure;
    float _Rotation;
    float _Dissolve;
    float _StripeWidth;
    float _LeadSharp;
    float4 _LeadColor;
    float4 _MidColor;
    float4 _TrailColor;
    float4 _HBarColor;

    float3 RotateAroundYInDegrees (float3 vertex, float degrees)
    {
        float alpha = degrees * UNITY_PI / 180.0;
        float sina, cosa;
        sincos(alpha, sina, cosa);
        float2x2 m = float2x2(cosa, -sina, sina, cosa);
        return float3(mul(m, vertex.xz), vertex.y).xzy;
    }

    struct appdata_t {
        float4 vertex : POSITION;
        float2 texcoord : TEXCOORD0;
        UNITY_VERTEX_INPUT_INSTANCE_ID
    };
    struct v2f {
        float4 vertex : SV_POSITION;
        float2 texcoord : TEXCOORD0;
        float4 interpolatedPos : TEXCOORD1;
        UNITY_VERTEX_OUTPUT_STEREO
    };
    v2f vert (appdata_t v)
    {
        v2f o;
        UNITY_SETUP_INSTANCE_ID(v);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
        float3 rotated = RotateAroundYInDegrees(v.vertex, _Rotation);
        o.vertex = UnityObjectToClipPos(rotated);
        o.texcoord = v.texcoord;
        o.interpolatedPos = v.vertex;
        return o;
    }

    float4 horizBars(float2 p)
	{
		return 1 - saturate(round(abs(frac(p.y * 100) * 2)));
	}

    half4 skybox_frag (v2f i, sampler2D smp, half4 smpDecode)
    {
        float3 rayworld = mul((float3x3)unity_CameraToWorld, normalize(i.interpolatedPos));
        float3 cameraUp = UNITY_MATRIX_V[1];
        float dotRay = dot(rayworld, cameraUp);
        
        if(dotRay >= _Dissolve + _StripeWidth)
        {
            return half4(0,0,0,0);
        }
        half4 tex = tex2D (smp, i.texcoord);
        half3 c = DecodeHDR (tex, smpDecode);
        c = c * _Tint.rgb * unity_ColorSpaceDouble.rgb;
        c *= _Exposure;
        if(dotRay >= _Dissolve - _StripeWidth)
        {
            float diff = 1 - (dotRay - _Dissolve + _StripeWidth) / (_StripeWidth * 2);
            half4 edge = lerp(_MidColor, _LeadColor, pow(diff, _LeadSharp));
            half4 scannerCol = lerp(_TrailColor, edge, diff) + horizBars(i.texcoord) * _HBarColor;
            scannerCol *= diff;            
            c += scannerCol;
        }        
        return half4(c, 1);
    }
    ENDCG

    Pass {
        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        #pragma target 2.0
        sampler2D _FrontTex;
        half4 _FrontTex_HDR;
        half4 frag (v2f i) : SV_Target { return skybox_frag(i,_FrontTex, _FrontTex_HDR); }
        ENDCG
    }
    Pass{
        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        #pragma target 2.0
        sampler2D _BackTex;
        half4 _BackTex_HDR;
        half4 frag (v2f i) : SV_Target { return skybox_frag(i,_BackTex, _BackTex_HDR); }
        ENDCG
    }
    Pass{
        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        #pragma target 2.0
        sampler2D _LeftTex;
        half4 _LeftTex_HDR;
        half4 frag (v2f i) : SV_Target { return skybox_frag(i,_LeftTex, _LeftTex_HDR); }
        ENDCG
    }
    Pass{
        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        #pragma target 2.0
        sampler2D _RightTex;
        half4 _RightTex_HDR;
        half4 frag (v2f i) : SV_Target { return skybox_frag(i,_RightTex, _RightTex_HDR); }
        ENDCG
    }
    Pass{
        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        #pragma target 2.0
        sampler2D _UpTex;
        half4 _UpTex_HDR;
        half4 frag (v2f i) : SV_Target { return skybox_frag(i,_UpTex, _UpTex_HDR); }
        ENDCG
    }
    Pass{
        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        #pragma target 2.0
        sampler2D _DownTex;
        half4 _DownTex_HDR;
        half4 frag (v2f i) : SV_Target { return skybox_frag(i,_DownTex, _DownTex_HDR); }
        ENDCG
    }
}
}