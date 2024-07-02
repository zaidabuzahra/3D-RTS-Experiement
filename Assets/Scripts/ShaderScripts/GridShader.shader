Shader "Custom/GridShader"
{
    Properties
    {
        _LineColor ("Line Color", Color) = (1, 1, 1, 1)  // Line color property
        _SquareColor ("Square Color", Color) = (1, 1, 1, 0)  // Square color property with transparency
        _GridSize ("Grid Size", Float) = 1.0
        _LineThickness ("Line Thickness", Range(0.001, 0.1)) = 0.01
    }

    SubShader
    {
        Tags { "Queue" = "Overlay" }
        Pass
        {
            ZWrite On
            ZTest LEqual
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD0;
            };

            fixed4 _LineColor;  // Line color variable
            fixed4 _SquareColor;  // Square color variable
            float _GridSize;
            float _LineThickness;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                // Calculate grid coordinates based on object's bounds
                float2 gridCoords = float2(
                    abs(fmod(i.worldPos.x, _GridSize)),
                    abs(fmod(i.worldPos.z, _GridSize))
                );

                // Calculate grid lines
                float horzLine = step(_LineThickness, gridCoords.y);
                float vertLine = step(_LineThickness, gridCoords.x);

                // Combine horizontal and vertical lines to create grid effect
                float gridAlpha = min(horzLine, vertLine);

                // Output line color for lines, and square color for squares
                fixed4 finalColor = lerp(_SquareColor, _LineColor, gridAlpha);
                
                // Output final color with transparency
                return finalColor;
            }
            ENDCG
        }
    }
}
