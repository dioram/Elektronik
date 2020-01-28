Shader "G42/vertex_color" {
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Color("Color", Color) = (1,1,1,1)
	}

SubShader
	{
		Tags { "Queue" = "AlphaTest" "RenderType" = "TransparentCutout" "IgnoreProjector" = "True" }

		Blend SrcAlpha OneMinusSrcAlpha

		Lighting Off

		BindChannels {
			Bind "Color", color
		}
		Pass {
			SetTexture[_] {
				Combine primary
			}
		}
	}
		FallBack "Diffuse"
}