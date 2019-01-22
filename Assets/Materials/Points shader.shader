Shader "G42/vertex_color" {
	SubShader{
		Tags { "RenderType" = "Opaque" }
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