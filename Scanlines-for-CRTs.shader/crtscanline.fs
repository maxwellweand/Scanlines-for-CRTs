#version 150

/*
	240p Scanlines for CRT Monitors
	Author: mweand
	
	This shader was developed for use with the higan emulator. When used with emulated 240p consoles and a CRT monitor, it will produce very accurate scanlines.
	
	The vertical output resolution should be some integer multiple of 240, as low as 480. If a non-integer scaling is used, the shader will default to a much less accurate scanline style.
*/


in Vertex {
  vec2 texCoord;
};

out vec4 fragColor;

uniform sampler2D source[];
uniform vec4 sourceSize[];
uniform vec4 outputSize;

void main()
{
	vec4 base = texture(source[0], texCoord); // Base Image
	vec2 npos = vec2(gl_FragCoord.x/outputSize.x, gl_FragCoord.y/outputSize.y); // Normalized position
	float line = npos.y * 240; // Line in 240p
	
	float linef = fract(line); 
	float scale = outputSize.y / 240.0; // The output size multiplier for the final render
	
	if (scale <= 1.0) {
		// Don't bother inserting scanlines at this resolution
		fragColor = base;
	} else if (fract(scale) < 0.00001){
		// Integer scaling is being used :)
		if (mod(int(scale),2)==0){
			// Case: even integer scale
			if (linef > 0.5) fragColor = vec4(0.0, 0.0, 0.0, 1.0); // Black line
			else fragColor = base;
		} else {
			// Case: odd integer scale
			if (linef > 0.6666) fragColor = vec4(0.0, 0.0, 0.0, 1.0); // Black line
			else if (linef > 0.3333) fragColor = base * 0.5; // Blended line
			else fragColor = base;
		}
		
	} else {
		// Some weird non-integer scaling is happening :(
		float ln_triple = fract(gl_FragCoord.y * (1.0/3.0)); // Compact 3 line coordinates into 1 line
		if (ln_triple > 0.5) fragColor = vec4(0.0,0.0,0.0,0.0); // Black out every third line for an even look
		else fragColor = base;
	}
}





