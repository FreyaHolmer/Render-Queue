// Copyright © 2019 Freya Holmér • freya@acegikmo.com • https://github.com/FreyaHolmer/Render-Queue
namespace RenderQueuePlugin {

	using UnityEngine;
	using UnityEditor;
	using System.Linq;

	public class RenderQueueWindow : EditorWindow {

		// Constants
		const string EDITOR_PREF_PREFIX = "render_queue_window_";
		const string EDITOR_PREF_KEY_FILTER_INDEX = EDITOR_PREF_PREFIX + "filter_index";

		[SerializeField] MaterialList materialList;
		[SerializeField] int filterIndex = 0;
		[SerializeField] string[] filterNames;
		[SerializeField] int[] filterIndices;

		[MenuItem( "Tools/Render Queue" )]
		public static void Intialize() => GetWindow<RenderQueueWindow>( "Render Queue" );

		private void OnEnable() {
			LoadPrefs();
			RefreshFilters();
			RefreshList();
			Undo.undoRedoPerformed += OnUndoRedo;
		}

		private void OnDisable() {
			SavePrefs();
			Undo.undoRedoPerformed -= OnUndoRedo;
		}

		public void OnGUI() {
			DrawHeader();
			materialList.Draw();
			DeselectIfClickedNothing();
		}

		void SavePrefs() => EditorPrefs.SetInt( EDITOR_PREF_KEY_FILTER_INDEX, filterIndex );
		void LoadPrefs() => filterIndex = EditorPrefs.GetInt( EDITOR_PREF_KEY_FILTER_INDEX, 0 );

		void OnUndoRedo() {
			RefreshList();
			Repaint();
		}

		void RefreshFilters() {
			filterNames = Filters.filters.Select( f => f.name ).ToArray();
			filterIndices = new int[filterNames.Length];
			for( int i = 0; i < filterIndices.Length; i++ )
				filterIndices[i] = i;
			filterIndex = Mathf.Clamp( filterIndex, 0, Filters.filters.Count - 1 );
		}

		void RefreshList() {
			if( materialList == null )
				materialList = new MaterialList();
				materialList = new MaterialList();
			materialList.UpdateList( Filters.filters[filterIndex] );
			RenderQueueGUI.Deselect();
		}


		void DrawHeader() {

			bool hasChanges = materialList.HasPendingChanges;

			GUILayout.BeginHorizontal( EditorStyles.toolbar );
			{
				if( GUILayout.Button( "Refresh", EditorStyles.miniButtonLeft, GUILayout.Width( 66 ) ) ) {
					RefreshList();
				}
				EditorGUI.BeginDisabledGroup( hasChanges == false );
				if( GUILayout.Button( "Revert", EditorStyles.miniButtonMid, GUILayout.Width( 60 ) ) ) {
					materialList.RevertAllChanges();
					RefreshList();
				}
				if( GUILayout.Button( "Apply", EditorStyles.miniButtonRight, GUILayout.Width( 50 ) ) ) {
					materialList.ApplyAllChanges();
					RefreshList();
				}
				EditorGUI.EndDisabledGroup();

				GUILayout.Space( 5 );
				EditorGUI.BeginChangeCheck();
				GUILayout.Label( "Folder Filter", EditorStyles.miniLabel, GUILayout.ExpandWidth( false ) );
				filterIndex = EditorGUILayout.IntPopup( filterIndex, filterNames, filterIndices );
				if( EditorGUI.EndChangeCheck() ) {
					RefreshList();
				}

			}
			GUILayout.EndHorizontal();
		}

		void DeselectIfClickedNothing() {
			if( Event.current.type == EventType.MouseDown && Event.current.button == 0 ) {
				RenderQueueGUI.Deselect();
				Repaint();
			}
		}



	}

}