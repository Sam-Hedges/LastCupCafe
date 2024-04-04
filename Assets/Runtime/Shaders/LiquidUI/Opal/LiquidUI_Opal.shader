Shader "SamHedges/LiquidUI_Opal" {
	
	// Properties are options set per material, exposed by the material inspector
		Properties {
			[Header(UI Options)]
			// [MainColor] & [MainTexture] allows Material.color & Material.mainTexture in C# to use the correct properties
			_MainTex("Main Texture", 2D) = "white" {}
			_DiffractionMap("Diffraction Map", 2D) = "white" {}
			_Progress("Progress", Range(0,1)) = 0.5
			_Colour("Colour", Color) = (1, 0, 0, 1)
			_BackgroundColour("Background Colour", Color) = (0, 0, 0, 0.4)
			_BorderNoiseScale("Noise Scale", Float) = 5
			_MovingAmount("Border Light Intensity", Float) = 1
			_DissolveTransition("Dissolve Transition", Range(0,1)) = 0
			_DissolveAmount("Dissolve Amount", Range(0,1)) = 0
			_Rotation("Rotation", Float) = 0
			_NoiseScale("Noise Scale", Float) = 20
			_NoiseIntensity("Noise Intensity", Range(0,1)) = 1
			_Spherize("Spherize", Int) = 0
			_BorderDistortionAmount("Border Distortion Amount", Range(0,0.2)) = 0
			_NoiseRoundFactor("Noise Round Factor", Range(1,255)) = 50
			_Distance("Grating Distance", Float) = 1
		}
	
		
	// Sub-shaders allow for different behaviour and options for different pipelines and platforms
	Subshader {
		// Tags are shared by all passed in this subshader
		Tags {
			"RenderPipeline" = "UniversalPipeline"
			"RenderType" = "Transparent"
			"Queue" = "Transparent"
		}

		// Shader can have several passes which are used to render different data about the material and
		// each pass has it's own vertex and fragment function and shader variant keywords
		Pass {
			Name "ForwardLit" // For debugging 
			Tags {"LightMode" = "UniversalForward"} // Pass specific tags
			// "UniversalForward" tells unity this is the main lighting pass of this shader
			
			Blend SrcAlpha OneMinusSrcAlpha // Traditional transparency
			
			HLSLPROGRAM // Begin HLSL code
			
			// Register our programmable stage functions
			// #pragma has variety of uses related to shader metadata
			// vertex and fragment sub-commands register the corresponding functions
			// to the containing pass; the names MUST MATCH those within the hlsl file
			#pragma vertex Vertex
			#pragma fragment Fragment
			
			// Include our hlsl file
			#include "Assets/Runtime/Shaders/_Includes/LiquidUIOpalForwardPass.hlsl"

			ENDHLSL
		}
	}
	
	CustomEditor "LiquidGui"
}
