// Copyright © 2019 Freya Holmér • freya@acegikmo.com • https://github.com/FreyaHolmer/Render-Queue
namespace RenderQueuePlugin {

	using UnityEngine;
	using UnityEditor;

	[System.Serializable]
	public class Entry {

		public enum EntryType {
			Material,
			Shader
		}

		public Object asset;
		public int renderQueueInput;
		public EntryType entryType;

		Material AssetMaterial => asset as Material;
		Shader AssetShader => asset as Shader;

		public int RenderQueue {
			get {
				switch( entryType ) {
					case EntryType.Material:
						return AssetMaterial.renderQueue;
					case EntryType.Shader:
						return AssetShader.renderQueue;
				}
				return default;
			}
			set {
				if( entryType == EntryType.Material ) {
					AssetMaterial.renderQueue = value;
					return;
				}
				// Note: Shader render queue is read only
			}
		}
		public EntryState ModifiedState {
			get {
				if( asset == null )
					return EntryState.Missing;
				if( renderQueueInput != RenderQueue )
					return EntryState.Modified;
				return EntryState.Unchanged;
			}
		}

		public Entry( Object asset ) {
			this.asset = asset;
			if( asset is Material )
				entryType = EntryType.Material;
			else if( asset is Shader )
				entryType = EntryType.Shader;
			this.renderQueueInput = RenderQueue;
		}

		public string GetOffsetLabelString( EntryState state ) {
			if( state == EntryState.Missing )
				return "?";
			int offset = renderQueueInput - 3000;
			return RenderQueueGUI.ToSignedString( offset );
		}

		public string GetRenderTag( EntryState state ) {
			if( state == EntryState.Missing )
				return "?";
			if( entryType == EntryType.Material )
				return AssetMaterial.GetTag( "RenderType", true, "-" );
			return string.Empty;
		}

		public void Draw() {

			EntryState state = ModifiedState;
			GUI.color = state.GetColor();

			string offsetLabel = GetOffsetLabelString( state );
			string renderTagLabel = GetRenderTag( state );

			GUILayout.BeginHorizontal();
			{
				RenderQueueGUI.Disabled( () => {
					GUILayout.Label( offsetLabel, GUILayout.Width( 40 ) );
				} );

				RenderQueueGUI.EnabledStateGroup( state != EntryState.Missing, () => {
					if(entryType == EntryType.Material ) {
						renderQueueInput = EditorGUILayout.IntField( renderQueueInput, GUILayout.Width( 40 ) );
					} else {
						GUILayout.Space( 1 ); // To make the label align with the textbox when viewing things you can edit
						GUILayout.Label( renderQueueInput.ToString(), GUILayout.Width( 40 ) ); // Read only
					}
					EditorGUILayout.ObjectField( asset, typeof( Material ), allowSceneObjects: false, GUILayout.ExpandWidth( true ) );
				} );

				if( entryType == EntryType.Material ) {
					RenderQueueGUI.Fade( 0.4f, () => {
						GUILayout.Label( renderTagLabel, EditorStyles.miniLabel, GUILayout.Width( 100 ) );
					} );
				}

			}
			GUILayout.EndHorizontal();

			GUI.color = Color.white;
		}


		public void ApplyIfModified() {
			if( ModifiedState == EntryState.Modified ) {
				UnityEditor.Undo.RecordObject( asset, "change asset render queque" );
				RenderQueue = renderQueueInput;
				renderQueueInput = RenderQueue; // Make sure it matches asset afterwards after
			}
		}

		public void RevertIfModified() {
			if( ModifiedState == EntryState.Modified )
				renderQueueInput = RenderQueue;
		}

	}

}