// Copyright © 2019 Freya Holmér • freya@acegikmo.com • https://github.com/FreyaHolmer/Render-Queue
namespace RenderQueuePlugin {

	using UnityEngine;
	using UnityEditor;

	[System.Serializable]
	public class MaterialEntry {

		public enum State {
			Unchanged,
			Modified,
			Missing
		}

		static readonly Color[] stateColors = {
			Color.white,
			Color.cyan,
			Color.red
		};

		public Material material;
		public int renderQueueInput;

		public int RenderQueue => material.renderQueue;
		public State ModifiedState {
			get {
				if( material == null )
					return State.Missing;
				if( renderQueueInput != RenderQueue )
					return State.Modified;
				return State.Unchanged;
			}
		}

		public MaterialEntry( Material material ) {
			this.material = material;
			this.renderQueueInput = RenderQueue;
		}

		public string GetOffsetLabelString( State state ) {
			if( state == State.Missing )
				return "?";
			int offset = renderQueueInput - 3000;
			return RenderQueueGUI.ToSignedString( offset );
		}

		public string GetRenderTag( State state ) {
			if( state == State.Missing )
				return "?";
			return material.GetTag( "RenderType", true, "-" );
		}

		public void Draw() {

			State state = ModifiedState;
			GUI.color = stateColors[(int)state];
			// bool materialMissing = state == State.Missing;

			string offsetLabel = GetOffsetLabelString( state );
			string renderTagLabel = GetRenderTag( state );

			GUILayout.BeginHorizontal();
			{
				RenderQueueGUI.Disabled( () => {
					GUILayout.Label( offsetLabel, GUILayout.Width( 40 ) );
				} );

				RenderQueueGUI.EnabledStateGroup( state != State.Missing, () => {
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
			if( ModifiedState == State.Modified ) {
				UnityEditor.Undo.RecordObject( material, "change material render queque" );
				material.renderQueue = renderQueueInput;
				renderQueueInput = material.renderQueue; // Make sure it matches after
			}
		}

		public void RevertIfModified() {
			if( ModifiedState == State.Modified )
				renderQueueInput = material.renderQueue;
		}

	}

}