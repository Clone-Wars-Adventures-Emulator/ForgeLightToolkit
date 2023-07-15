using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace ForgeLightToolkit.Editor.FileTypes.Dma
{
    public enum ParameterName : uint
    {
        Diffuse = 3452395312,                   // D3DXPC_OBJECT - D3DXPT_TEXTURE
        ScrollV = 3936513778,                   // D3DXPC_SCALAR - D3DXPT_FLOAT
        ScrollU = 3145928884,                   // D3DXPC_SCALAR - D3DXPT_FLOAT
        Glow = 3649488835,                      // D3DXPC_SCALAR - D3DXPT_FLOAT
        TintSemantic = 3414268513,              // D3DXPC_VECTOR - D3DXPT_FLOAT
        FadeStencil = 593879656,                // D3DXPC_SCALAR - D3DXPT_INT
        DoubleSidedDefaultFalse = 2639744342,   // D3DXPC_SCALAR - D3DXPT_BOOL
        RimLighting = 549780338,                // D3DXPC_SCALAR - D3DXPT_FLOAT
        Bias = 3796178247,                      // D3DXPC_SCALAR - D3DXPT_INT
        Diffuse2 = 2907272773,                  // D3DXPC_OBJECT - D3DXPT_TEXTURE
        TextureClamp = 2000899314,              // D3DXPC_SCALAR - D3DXPT_BOOL
        ZRange = 991690425,                     // D3DXPC_SCALAR - D3DXPT_FLOAT
        FallOff = 1747511450,                   // D3DXPC_SCALAR - D3DXPT_FLOAT
        Intensity = 4132979297,                 // D3DXPC_SCALAR - D3DXPT_FLOAT
        Density = 1172739366,                   // D3DXPC_SCALAR - D3DXPT_FLOAT
        GradientTexture = 2031810102,           // D3DXPC_OBJECT - D3DXPT_TEXTURE
        Fade = 504498101,                       // D3DXPC_SCALAR - D3DXPT_FLOAT
        Opacity = 2299629057,                   // D3DXPC_SCALAR - D3DXPT_FLOAT
        OpacityMask = 2107798039,               // D3DXPC_OBJECT - D3DXPT_TEXTURE
        XOffset = 4065717569,                   // D3DXPC_SCALAR - D3DXPT_FLOAT
        EndTipRadius = 1635026516,              // D3DXPC_SCALAR - D3DXPT_FLOAT
        StartTipRadius = 876666917,             // D3DXPC_SCALAR - D3DXPT_FLOAT
        EndTipLength = 1170793930,              // D3DXPC_SCALAR - D3DXPT_FLOAT
        StartTipLength = 2754733806,            // D3DXPC_SCALAR - D3DXPT_FLOAT
        OuterRadius = 4141648506,               // D3DXPC_SCALAR - D3DXPT_FLOAT
        TransitionRadius = 718471922,           // D3DXPC_SCALAR - D3DXPT_FLOAT
        InnerRadius = 1821084410,               // D3DXPC_SCALAR - D3DXPT_FLOAT
        Length = 3007871864,                    // D3DXPC_SCALAR - D3DXPT_FLOAT
        InnerColor = 3690655219,                // D3DXPC_VECTOR - D3DXPT_FLOAT
        SpecularMap = 4290661001,               // D3DXPC_OBJECT - D3DXPT_TEXTURE
        BumpMap3 = 1652574888,                  // D3DXPC_OBJECT - D3DXPT_TEXTURE
        BumpMap2 = 1949429259,                  // D3DXPC_OBJECT - D3DXPT_TEXTURE
        BumpMap1 = 1092061151,                  // D3DXPC_OBJECT - D3DXPT_TEXTURE
        Bumpiness3 = 900038855,                 // D3DXPC_SCALAR - D3DXPT_FLOAT
        Bumpiness2 = 1753966226,                // D3DXPC_SCALAR - D3DXPT_FLOAT
        Bumpiness1 = 1645435298,                // D3DXPC_SCALAR - D3DXPT_FLOAT
        TexScrollZ3 = 3844378250,               // D3DXPC_SCALAR - D3DXPT_FLOAT
        TexScrollX3 = 1652460352,               // D3DXPC_SCALAR - D3DXPT_FLOAT
        TexScrollZ2 = 3762586818,               // D3DXPC_SCALAR - D3DXPT_FLOAT
        TexScrollX2 = 3629741812,               // D3DXPC_SCALAR - D3DXPT_FLOAT
        TexScrollZ1 = 234774581,                // D3DXPC_SCALAR - D3DXPT_FLOAT
        TexScrollX1 = 4076514358,               // D3DXPC_SCALAR - D3DXPT_FLOAT
        TexScale3 = 4070181178,                 // D3DXPC_SCALAR - D3DXPT_FLOAT
        TexScale2 = 552363486,                  // D3DXPC_SCALAR - D3DXPT_FLOAT
        TexScale1 = 3439967770,                 // D3DXPC_SCALAR - D3DXPT_FLOAT
        Refraction = 973401990,                 // D3DXPC_SCALAR - D3DXPT_FLOAT
        Fresnel = 307170454,                    // D3DXPC_SCALAR - D3DXPT_FLOAT
        Reflection = 143846010,                 // D3DXPC_SCALAR - D3DXPT_FLOAT
        DoubleSided = 1569877796,               // D3DXPC_SCALAR - D3DXPT_BOOL
        HorizontalPower = 1143518454,           // D3DXPC_SCALAR - D3DXPT_FLOAT
        HorizontalShift = 463103586,            // D3DXPC_SCALAR - D3DXPT_FLOAT
        VTraceColor = 3292109686,               // D3DXPC_VECTOR - D3DXPT_FLOAT
        VTraceSpeed = 2076897840,               // D3DXPC_SCALAR - D3DXPT_FLOAT
        VTraceFrequency = 748049272,            // D3DXPC_SCALAR - D3DXPT_FLOAT
        ScanlineColor = 1034536264,             // D3DXPC_VECTOR - D3DXPT_FLOAT
        ScanlineSpeed = 2029061153,             // D3DXPC_SCALAR - D3DXPT_FLOAT
        ScanlineResolution = 2970250175,        // D3DXPC_SCALAR - D3DXPT_FLOAT
        FallOffScale = 3776485906,              // D3DXPC_SCALAR - D3DXPT_FLOAT
        FallOffPower = 3558720045,              // D3DXPC_SCALAR - D3DXPT_FLOAT
        TextureIntensityBoost = 1718675833,     // D3DXPC_SCALAR - D3DXPT_FLOAT
        ChromaAbsorption = 487460013,           // D3DXPC_SCALAR - D3DXPT_FLOAT
        OuterColor = 4099572798,                // D3DXPC_VECTOR - D3DXPT_FLOAT
        BaseColor = 4164406500,                 // D3DXPC_VECTOR - D3DXPT_FLOAT
        TexScrollZ0 = 1122028041,               // D3DXPC_SCALAR - D3DXPT_FLOAT
        TexScrollX0 = 3839758333,               // D3DXPC_SCALAR - D3DXPT_FLOAT
        OuterFresnel = 996175744,               // D3DXPC_SCALAR - D3DXPT_FLOAT
        OuterScale = 1907992278,                // D3DXPC_SCALAR - D3DXPT_FLOAT
        Spec = 67211600,                        // D3DXPC_SCALAR - D3DXPT_FLOAT
        GlowMap = 3234476342,                   // D3DXPC_OBJECT - D3DXPT_TEXTURE
        InsideGlow = 3386933319,                // D3DXPC_SCALAR - D3DXPT_FLOAT
        InsideSpecular = 494576346,             // D3DXPC_SCALAR - D3DXPT_FLOAT
        OutsideGlow = 892849424,                // D3DXPC_SCALAR - D3DXPT_FLOAT
        OutsideSpecular = 3579437073,           // D3DXPC_SCALAR - D3DXPT_FLOAT
        YOffset = 1776797183,                   // D3DXPC_SCALAR - D3DXPT_FLOAT
        RefractDepth = 1478254245,              // D3DXPC_SCALAR - D3DXPT_FLOAT
        Mask2 = 1437547971,                     // D3DXPC_OBJECT - D3DXPT_TEXTURE
        Mask1 = 2378182120,                     // D3DXPC_OBJECT - D3DXPT_TEXTURE
        Texture2 = 3281981744,                  // D3DXPC_OBJECT - D3DXPT_TEXTURE
        Texture1 = 904558021,                   // D3DXPC_OBJECT - D3DXPT_TEXTURE
        Divergence = 922518377,                 // D3DXPC_SCALAR - D3DXPT_FLOAT
        MaskScroll2 = 3709443648,               // D3DXPC_SCALAR - D3DXPT_FLOAT
        MaskScroll1 = 2815374252,               // D3DXPC_SCALAR - D3DXPT_FLOAT
        TexScroll2 = 246748575,                 // D3DXPC_SCALAR - D3DXPT_FLOAT
        TexScroll1 = 538392675,                 // D3DXPC_SCALAR - D3DXPT_FLOAT
        MaskScale2 = 1494389660,                // D3DXPC_SCALAR - D3DXPT_FLOAT
        MaskScale1 = 1254684425,                // D3DXPC_SCALAR - D3DXPT_FLOAT
        EdgeFalloff = 354149273,                // D3DXPC_SCALAR - D3DXPT_FLOAT
        Scale = 1850817166,                     // D3DXPC_SCALAR - D3DXPT_FLOAT
        Strength = 1402117393,                  // D3DXPC_SCALAR - D3DXPT_FLOAT
        FrequencyC = 710718286,                 // D3DXPC_SCALAR - D3DXPT_FLOAT
        FrequencyB = 479664067,                 // D3DXPC_SCALAR - D3DXPT_FLOAT
        FrequencyA = 4142320739,                // D3DXPC_SCALAR - D3DXPT_FLOAT
        SunReflection = 3213875333,             // D3DXPC_SCALAR - D3DXPT_FLOAT
        TexScroll = 448001756,                  // D3DXPC_SCALAR - D3DXPT_FLOAT
        TintMask = 170150817,                   // D3DXPC_OBJECT - D3DXPT_TEXTURE
        Tint2 = 2860884101,                     // D3DXPC_VECTOR - D3DXPT_FLOAT
        TintMaskSpec = 2177463994,              // D3DXPC_OBJECT - D3DXPT_TEXTURE
        GlowTexture = 62195427,                 // D3DXPC_OBJECT - D3DXPT_TEXTURE
        SilhouetteOpacity = 4070082442,         // D3DXPC_SCALAR - D3DXPT_FLOAT
        SilhouetteColor = 3039528493,           // D3DXPC_VECTOR - D3DXPT_FLOAT
        InnerOpacity = 3508971236,              // D3DXPC_SCALAR - D3DXPT_FLOAT
        MaxDistance = 1577020487,               // D3DXPC_SCALAR - D3DXPT_FLOAT
        Thickness = 3978882475,                 // D3DXPC_SCALAR - D3DXPT_FLOAT
        FresnelPower = 2959957172,              // D3DXPC_SCALAR - D3DXPT_FLOAT
        FoamTexture = 2355723402,               // D3DXPC_OBJECT - D3DXPT_TEXTURE
        OceanTexture = 3765304642,              // D3DXPC_OBJECT - D3DXPT_TEXTURE
        FoamIntensity = 2716952673,             // D3DXPC_SCALAR - D3DXPT_FLOAT
        FoamThreshold = 386309405,              // D3DXPC_SCALAR - D3DXPT_FLOAT
        FoamNoiseSpeed = 2417632176,            // D3DXPC_SCALAR - D3DXPT_FLOAT
        FoamNoiseScale = 4086765643,            // D3DXPC_SCALAR - D3DXPT_FLOAT
        FoamTextureScale = 2599055739,          // D3DXPC_SCALAR - D3DXPT_FLOAT
        OceanTextureScale = 2327307570,         // D3DXPC_SCALAR - D3DXPT_FLOAT
        Steepness2 = 801627988,                 // D3DXPC_SCALAR - D3DXPT_FLOAT
        Steepness1 = 4108708237,                // D3DXPC_SCALAR - D3DXPT_FLOAT
        Direction2 = 3482006672,                // D3DXPC_SCALAR - D3DXPT_FLOAT
        Speed2 = 3309491016,                    // D3DXPC_SCALAR - D3DXPT_FLOAT
        Speed1 = 3078961101,                    // D3DXPC_SCALAR - D3DXPT_FLOAT
        Amplitude2 = 1358875354,                // D3DXPC_SCALAR - D3DXPT_FLOAT
        Amplitude1 = 1596253990,                // D3DXPC_SCALAR - D3DXPT_FLOAT
        Wavelength2 = 786457581,                // D3DXPC_SCALAR - D3DXPT_FLOAT
        Wavelength1 = 495042864,                // D3DXPC_SCALAR - D3DXPT_FLOAT
        DimColor = 1433736761,                  // D3DXPC_VECTOR - D3DXPT_FLOAT
        GhostSize = 1460872178,                 // D3DXPC_SCALAR - D3DXPT_FLOAT
        GhostSpeed = 3752275428,                // D3DXPC_SCALAR - D3DXPT_FLOAT
        VTraceSize = 2345716284,                // D3DXPC_SCALAR - D3DXPT_FLOAT
        SunGlare = 3697604236,                  // D3DXPC_SCALAR - D3DXPT_FLOAT

        DetailMaskMap = 69317677,               // D3DXPC_OBJECT - D3DXPT_TEXTURE

        DetailRepeat0 = 202117273,              // D3DXPC_SCALAR - D3DXPT_FLOAT
        DetailRepeat1 = 2199912159,             // D3DXPC_SCALAR - D3DXPT_FLOAT
        DetailRepeat2 = 1893292626,             // D3DXPC_SCALAR - D3DXPT_FLOAT
        DetailRepeat3 = 1465198410,             // D3DXPC_SCALAR - D3DXPT_FLOAT
        DetailRepeat4 = 1263210294,             // D3DXPC_SCALAR - D3DXPT_FLOAT

        DetailColorMap0 = 3835265197,           // D3DXPC_OBJECT - D3DXPT_TEXTURE
        DetailColorMap1 = 762221142,            // D3DXPC_OBJECT - D3DXPT_TEXTURE
        DetailColorMap2 = 1226230182,           // D3DXPC_OBJECT - D3DXPT_TEXTURE
        DetailColorMap3 = 598736601,            // D3DXPC_OBJECT - D3DXPT_TEXTURE
        DetailColorMap4 = 2904166779            // D3DXPC_OBJECT - D3DXPT_TEXTURE
    }
}