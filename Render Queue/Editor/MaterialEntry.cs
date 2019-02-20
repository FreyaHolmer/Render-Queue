// Copyright © 2019 Freya Holmér • freya@acegikmo.com • https://github.com/FreyaHolmer/Render-Queue
namespace RenderQueuePlugin {

	using UnityEngine;
	using UnityEditor;

	[System.Serializable]
	public class MaterialEntry {

		public Material material;
		public int renderQueueInput;

		public int RenderQueue => material.renderQueue;
		public EntryState ModifiedState {
			get {
				if( material == null )
					return EntryState.Missing;
				if( renderQueueInput != RenderQueue )
					return EntryState.Modified;
				return EntryState.Unchanged;
			}
		}

		public MaterialEntry( Material material ) {
			this.material = material;
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
			return material.GetTag( "RenderType", true, "-" );
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
					renderQueueInput = EditorGUILayout.IntField( renderQueueInput, GUILayout.Width( 40 ) );
					EditorGUILayout.ObjectField( material, typeof( Material ), allowSceneObjects: false, GUILayout.ExpandWidth( true ) );
				} );

				RenderQueueGUI.Fade( 0.4f, () => {
					GUILayout.Label( renderTagLabel, EditorStyles.miniLabel, GUILayout.Width( 100 ) );
				} );
			}
			GUILayout.EndHorizontal();

			GUI.color = Color.white;
		}


		public void ApplyIfModified() {
			if( ModifiedState == EntryState.Modified ) {
				UnityEditor.Undo.RecordObject( material, "change material render queque" );
				material.renderQueue = renderQueueInput;
				renderQueueInput = material.renderQueue; // Make sure it matches after
			}
		}

		public void RevertIfModified() {
			if( ModifiedState == EntryState.Modified )
				renderQueueInput = material.renderQueue;
		}

	}

}