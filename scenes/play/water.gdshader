shader_type spatial;

// This shader creates animated water by displacing vertices using noise
// The noise texture provides the wave pattern
uniform sampler2D noise_img;
uniform float wave_height = 1.0;      // How tall the waves are
uniform float wave_speed = 0.05;      // How fast waves move (slower for ocean feel)
uniform vec4 water_color : source_color = vec4(0.0, 0.4, 0.8, 1.0);

varying float height_current;

void vertex() {
	// Move the noise sampling over time to create wave animation
	// TIME is a built-in Godot variable that counts seconds
	vec2 uv_movement = vec2(TIME * wave_speed, 0.0);

	// Sample the noise texture at current UV + time offset
	// This reads a grayscale value (0.0 to 1.0) from the texture
	float tex_height = texture(noise_img, UV + uv_movement).r;
	height_current = tex_height;

	// Displace the vertex upward based on noise value
	// This creates the wave shapes by moving vertices up and down
	VERTEX.y += tex_height * wave_height;
}

void fragment() {
	// Set the water color
	ALBEDO = water_color.rgb;

	// Make it a bit metallic for reflections
	METALLIC = 0.5;

	// Low roughness makes it shiny/reflective like water
	ROUGHNESS = 0.05;

	// Slight transparency
	ALPHA = 0.9;
}
