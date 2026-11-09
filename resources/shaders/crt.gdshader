shader_type canvas_item;
render_mode unshaded, skip_vertex_transform;

uniform float curvature: hint_range(1.0, 30.0, 0.1) = 10.0;
uniform float vignette_multiplier: hint_range(0.0, 5.0, 0.05) = 2.0;
uniform float vignette_power: hint_range(1.0, 8.0, 0.1) = 4.0;
uniform float vignette_scale: hint_range(1.0, 6.0, 0.1) = 3.0;
uniform float scanline_frequency: hint_range(10.0, 400.0, 1.0) = 180.0;
uniform float scanline_strength: hint_range(0.0, 0.5, 0.01) = 0.1;
uniform float texture_lod: hint_range(0.0, 4.0, 0.1) = 0.2;

uniform sampler2D screen_texture: hint_screen_texture, filter_linear_mipmap;

void fragment() {
	vec2 centered_uv = SCREEN_UV * 2.0 - 1.0;
	vec2 uv_offset = centered_uv.yx / curvature;
	vec2 warped_uv = centered_uv + centered_uv * uv_offset * uv_offset;
	vec3 cutoff = vec3(step(abs(warped_uv.x), 1.0) * step(abs(warped_uv.y), 1.0));
	vec3 scanlines = vec3(sin(2.0 * warped_uv.y * scanline_frequency) * scanline_strength + (1.0 - scanline_strength));
	vec3 vignette = vec3(length(pow(abs(centered_uv), vec2(vignette_power)) / vignette_scale));
	
	vec3 screen_color = textureLod(screen_texture, (warped_uv + 1.0) / 2.0, texture_lod).rgb * cutoff * scanlines;
	screen_color -= vignette * vignette_multiplier;
	COLOR = vec4(screen_color, 1.0);
}