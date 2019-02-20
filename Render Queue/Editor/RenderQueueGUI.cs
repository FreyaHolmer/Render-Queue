// Copyright © 2019 Freya Holmér • freya@acegikmo.com • https://github.com/FreyaHolmer/Render-Queue
namespace RenderQueuePlugin {

	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;
	using System.Linq;

	public static class RenderQueueGUI {

		static GUIStyle panelStyle;
		public static GUIStyle PanelStyle {
			get {
				if( panelStyle == null ) {
					panelStyle = new GUIStyle( EditorStyles.helpBox ) {
						padding = new RectOffset( 2, 2, 4, 4 ),
						margin = new RectOffset( 0, 0, 0, 0 )
					};
				}
				return panelStyle;
			}
		}

		static GUIContent iconMaterial;
		public static GUIContent IconMaterial {
			get {
				if( iconMaterial == null )
					iconMaterial = EditorGUIUtility.IconContent( "Material Icon" );
				return iconMaterial;
			}
		}

		static GUIContent iconShader;
		public static GUIContent IconShader {
			get {
				if( iconShader == null )
					iconShader = EditorGUIUtility.IconContent( "Shader Icon" );
				return iconShader;
			}
		}

		static Color cachedPrevColor;

		public static void Fade( float opacity, System.Action content ) {
			cachedPrevColor = GUI.color;
			Color c = cachedPrevColor;
			c.a *= opacity;
			GUI.color = c;
			content();
			GUI.color = cachedPrevColor;
		}

		public static void Disabled( System.Action content ) {
			EditorGUI.BeginDisabledGroup( true );
			content();
			EditorGUI.EndDisabledGroup();
		}

		public static void EnabledStateGroup( bool enabled, System.Action content ) {
			EditorGUI.BeginDisabledGroup( enabled == false );
			content();
			EditorGUI.EndDisabledGroup();
		}

		public static string ToSignedString( int value ) {
			if( value == 0 )
				return "±" + value;
			if( value > 0 )
				return "+" + value;
			return value.ToString(); // Negative symbol is already included
		}

		public static void LabelSmallRight( string str ) {
			GUILayout.BeginHorizontal();
			GUILayout.Label( string.Empty, GUILayout.ExpandWidth( true ) );
			GUILayout.Label( str, EditorStyles.centeredGreyMiniLabel, GUILayout.ExpandWidth( false ) );
			GUILayout.EndHorizontal();
		}

		public static void Deselect() => GUI.FocusControl( "" );

	}

}