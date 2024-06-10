struct vertex_t {
    UNITY_VERTEX_INPUT_INSTANCE_ID
    float4	position		: POSITION;
    float3	normal			: NORMAL;
    float4	color			: color;
    float2	texcoord0		: TEXCOORD0;
    float2	texcoord1		: TEXCOORD1;
};

struct pixel_t {
    UNITY_VERTEX_INPUT_INSTANCE_ID
    UNITY_VERTEX_OUTPUT_STEREO
    float4	position		: SV_POSITION;
    float4	facecolor		: color;
    float4	outlinecolor	: color1;
    float4	texcoord0		: TEXCOORD0;
    float4	param			: TEXCOORD1;		// weight, scaleRatio
    float2	mask			: TEXCOORD2;
    #if (UNDERLAY_ON || UNDERLAY_INNER)
    float4	texcoord2		: TEXCOORD3;
    float4	underlaycolor	: color2;
    #endif
};

float4 SRGBToLinear(float4 rgba) {
    return float4(lerp(rgba.rgb / 12.92f, pow((rgba.rgb + 0.055f) / 1.055f, 2.4f), step(0.04045f, rgba.rgb)), rgba.a);
}

pixel_t VertShader(vertex_t input)
{
    pixel_t output;

    UNITY_INITIALIZE_OUTPUT(pixel_t, output);
    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_TRANSFER_INSTANCE_ID(input, output);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

    float bold = step(input.texcoord1.y, 0);

    float4 vert = input.position;
    vert.x += _VertexOffsetX;
    vert.y += _VertexOffsetY;

    float4 vPosition = UnityObjectToClipPos(vert);

    float weight = lerp(_WeightNormal, _WeightBold, bold) / 4.0;
    weight = (weight + _FaceDilate) * _ScaleRatioA * 0.5;

    // Generate UV for the Masking Texture
    float4 clampedRect = clamp(_ClipRect, -2e10, 2e10);
    float2 maskUV = (vert.xy - clampedRect.xy) / (clampedRect.zw - clampedRect.xy);

    float4 color = input.color;
    #if (FORCE_LINEAR && !UNITY_colorSPACE_GAMMA)
    color = SRGBToLinear(input.color);
    #endif

    float opacity = color.a;
    #if (UNDERLAY_ON | UNDERLAY_INNER)
    opacity = 1.0;
    #endif

    float4 facecolor = float4(color.rgb, opacity) * _Facecolor;
    facecolor.rgb *= facecolor.a;

    float4 outlinecolor = _Outlinecolor;
    outlinecolor.a *= opacity;
    outlinecolor.rgb *= outlinecolor.a;

    output.position = vPosition;
    output.facecolor = facecolor;
    output.outlinecolor = outlinecolor;
    output.texcoord0 = float4(input.texcoord0.xy, maskUV.xy);
    output.param = float4(0.5 - weight, 1.3333 * _GradientScale * (_Sharpness + 1) / _TextureWidth, _OutlineWidth * _ScaleRatioA * 0.5, 0);

    float2 mask = float2(0, 0);
    #if UNITY_UI_CLIP_RECT
    mask = vert.xy * 2 - clampedRect.xy - clampedRect.zw;
    #endif
    output.mask = mask;

    #if (UNDERLAY_ON || UNDERLAY_INNER)
    float4 underlaycolor = _Underlaycolor;
    underlaycolor.rgb *= underlaycolor.a;

    float x = -(_UnderlayOffsetX * _ScaleRatioC) * _GradientScale / _TextureWidth;
    float y = -(_UnderlayOffsetY * _ScaleRatioC) * _GradientScale / _TextureHeight;

    output.texcoord2 = float4(input.texcoord0 + float2(x, y), input.color.a, 0);
    output.underlaycolor = underlaycolor;
    #endif

    return output;
}

float4 PixShader(pixel_t input) : SV_Target
{
    UNITY_SETUP_INSTANCE_ID(input);

    float d = tex2D(_MainTex, input.texcoord0.xy).a;

    float2 UV = input.texcoord0.xy;
    float scale = rsqrt(abs(ddx(UV.x) * ddy(UV.y) - ddy(UV.x) * ddx(UV.y))) * input.param.y;

    #if (UNDERLAY_ON | UNDERLAY_INNER)
    float layerScale = scale;
    layerScale /= 1 + ((_UnderlaySoftness * _ScaleRatioC) * layerScale);
    float layerBias = input.param.x * layerScale - .5 - ((_UnderlayDilate * _ScaleRatioC) * .5 * layerScale);
    #endif

    scale /= 1 + (_OutlineSoftness * _ScaleRatioA * scale);

    float4 facecolor = input.facecolor * saturate((d - input.param.x) * scale + 0.5);

    #ifdef OUTLINE_ON
    float4 outlinecolor = lerp(input.facecolor, input.outlinecolor, sqrt(min(1.0, input.param.z * scale * 2)));
    facecolor = lerp(outlinecolor, input.facecolor, saturate((d - input.param.x - input.param.z) * scale + 0.5));
    facecolor *= saturate((d - input.param.x + input.param.z) * scale + 0.5);
    #endif

    #if UNDERLAY_ON
    d = tex2D(_MainTex, input.texcoord2.xy).a * layerScale;
    facecolor += float4(_Underlaycolor.rgb * _Underlaycolor.a, _Underlaycolor.a) * saturate(d - layerBias) * (1 - facecolor.a);
    #endif

    #if UNDERLAY_INNER
    float bias = input.param.x * scale - 0.5;
    float sd = saturate(d * scale - bias - input.param.z);
    d = tex2D(_MainTex, input.texcoord2.xy).a * layerScale;
    facecolor += float4(_Underlaycolor.rgb * _Underlaycolor.a, _Underlaycolor.a) * (1 - saturate(d - layerBias)) * sd * (1 - facecolor.a);
    #endif

    #ifdef MASKING
    float a = abs(_MaskInverse - tex2D(_MaskTex, input.texcoord0.zw).a);
    float t = a + (1 - _MaskWipeControl) * _MaskEdgeSoftness - _MaskWipeControl;
    a = saturate(t / _MaskEdgeSoftness);
    facecolor.rgb = lerp(_MaskEdgecolor.rgb * facecolor.a, facecolor.rgb, a);
    facecolor *= a;
    #endif

    // Alternative implementation to UnityGet2DClipping with support for softness
    #if UNITY_UI_CLIP_RECT
    float2 maskZW = 0.25 / (0.25 * half2(_MaskSoftnessX, _MaskSoftnessY) + (1 / scale));
    float2 m = saturate((_ClipRect.zw - _ClipRect.xy - abs(input.mask.xy)) * maskZW);
    facecolor *= m.x * m.y;
    #endif

    #if (UNDERLAY_ON | UNDERLAY_INNER)
    facecolor *= input.texcoord2.z;
    #endif

    #if UNITY_UI_ALPHACLIP
    clip(facecolor.a - 0.001);
    #endif

    return facecolor;
}
