shader_type canvas_item;

// Flow parameters
uniform float flow_speed : hint_range(0.1, 3.0) = 1.0;
uniform float flow_strength : hint_range(0.0, 0.2) = 0.05;
uniform float swirl_intensity : hint_range(0.0, 2.0) = 0.8;
uniform float wave_scale : hint_range(1.0, 10.0) = 4.0;

// Additional liquid effects
uniform float viscosity : hint_range(0.1, 2.0) = 1.0;
uniform bool enable_secondary_flow = true;
uniform float turbulence : hint_range(0.0, 1.0) = 0.3;

// Opacity and visual controls
uniform float opacity : hint_range(0.0, 1.0) = 0.5;
uniform float fade_edges : hint_range(0.0, 0.5) = 0.0;

// Simple noise function
float noise(vec2 uv) {
    return fract(sin(dot(uv, vec2(12.9898, 78.233))) * 43758.5453);
}

// Smooth noise for organic movement
float smooth_noise(vec2 uv) {
    vec2 i = floor(uv);
    vec2 f = fract(uv);
    f = f * f * (3.0 - 2.0 * f);

    float a = noise(i);
    float b = noise(i + vec2(1.0, 0.0));
    float c = noise(i + vec2(0.0, 1.0));
    float d = noise(i + vec2(1.0, 1.0));

    return mix(mix(a, b, f.x), mix(c, d, f.x), f.y);
}

// Create flowing distortion
vec2 liquid_flow(vec2 uv, float time) {
    vec2 flow = vec2(0.0);

    // Primary flow direction
    float flow_time = time * flow_speed / viscosity;

    // Create swirling motion
    vec2 center = vec2(0.5);
    vec2 to_center = uv - center;
    float dist_to_center = length(to_center);
    float angle = atan(to_center.y, to_center.x);

    // Swirl effect
    float swirl = sin(flow_time + dist_to_center * 3.14159) * swirl_intensity * flow_strength;
    angle += swirl;

    // Convert back to cartesian and apply swirl
    vec2 swirled = vec2(cos(angle), sin(angle)) * dist_to_center + center;
    flow += (swirled - uv) * 0.3;

    // Primary wave distortion
    flow.x += sin(uv.y * wave_scale + flow_time) * flow_strength;
    flow.y += cos(uv.x * wave_scale * 0.8 + flow_time * 0.7) * flow_strength * 0.8;

    // Secondary wave for complexity
    if (enable_secondary_flow) {
        flow.x += sin(uv.y * wave_scale * 1.3 + flow_time * 1.1) * flow_strength * 0.4;
        flow.y += cos(uv.x * wave_scale * 0.6 + flow_time * 0.9) * flow_strength * 0.5;
    }

    // Add turbulence with noise
    float noise_scale = 8.0;
    vec2 noise_offset = vec2(flow_time * 0.1, flow_time * 0.15);

    float noise_x = smooth_noise(uv * noise_scale + noise_offset);
    float noise_y = smooth_noise(uv * noise_scale + noise_offset + vec2(100.0, 50.0));

    flow += vec2(noise_x - 0.5, noise_y - 0.5) * turbulence * flow_strength;

    // Create flowing rivers effect
    float river_flow = sin(uv.x * 6.28318 + flow_time * 0.5) * cos(uv.y * 6.28318 + flow_time * 0.3);
    flow += vec2(river_flow, river_flow * 0.7) * flow_strength * 0.3;

    return flow;
}

void fragment() {
    vec2 uv = UV;
    float time = TIME;

    // Calculate liquid flow distortion
    vec2 flow_distortion = liquid_flow(uv, time);

    // Apply the distortion to UV coordinates
    vec2 distorted_uv = uv + flow_distortion;

    // Sample the texture with distorted coordinates
    vec4 tex_color = texture(TEXTURE, distorted_uv);

    // Optional: Add slight color shift for more liquid feel
    vec2 color_shift_uv = uv + flow_distortion * 0.5;
    vec4 shifted_color = texture(TEXTURE, color_shift_uv);

    // Blend original and shifted for subtle chromatic effect
    vec3 final_color = mix(tex_color.rgb, shifted_color.rgb, 0.1);

    // Calculate final alpha with opacity control
    float final_alpha = tex_color.a * opacity;

    // Apply edge fading if enabled
    if (fade_edges > 0.0) {
        float edge_dist = min(min(uv.x, 1.0 - uv.x), min(uv.y, 1.0 - uv.y));
        float edge_fade = smoothstep(0.0, fade_edges, edge_dist);
        final_alpha *= edge_fade;
    }

    COLOR = vec4(final_color, final_alpha);
}