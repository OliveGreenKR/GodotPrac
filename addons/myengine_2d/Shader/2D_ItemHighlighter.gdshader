shader_type canvas_item;

uniform vec4 shine_color : source_color = vec4( 1.0, 1.0, 1.0, 1.0 );
uniform float frequency : hint_range(0.0, 25.0) = 0.55;
uniform float shine_speed : hint_range(1.0, 5.0) = 3.0;
uniform float shine_width : hint_range(1.0, 100.0) = 3.0;

void fragment( )
{
	vec4 input_color = texture(TEXTURE, UV);
	float width = shine_width * 0.001 * frequency;
	float value = floor(sin(frequency * ((UV.x - UV.y) + TIME * shine_speed)) + width);
	
	float isOrigin = step( floor(sin(frequency * ((UV.x - UV.y) + TIME * shine_speed)) + width), 0.5);
	//vec3 new_color = input_color.rgb * (isOrigin) + shine_color.rgb * (1.0-isOrigin);
	vec4 new_color = bool(isOrigin) ? input_color : shine_color ;
	//new_color.a = sin()
	COLOR = vec4(new_color.rgb, input_color.a);
}