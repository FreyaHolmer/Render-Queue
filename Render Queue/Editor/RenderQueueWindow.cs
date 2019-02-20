// Copyright © 2019 Freya Holmér • freya@acegikmo.com • https://github.com/FreyaHolmer/Render-Queue
namespace RenderQueuePlugin {

	using UnityEngine;
	using UnityEditor;
	using System.Linq;
	using System.Collections.Generic;
	using System;

	public class RenderQueueWindow : EditorWindow {

		// Constants
		const string EDITOR_PREF_PREFIX = "render_queue_window_";
		const string EDITOR_PREF_KEY_FILTER_INDEX = EDITOR_PREF_PREFIX + "filter_index";
		const string EDITOR_PREF_KEY_LIST_INDEX   = EDITOR_PREF_PREFIX + "list_index";
		
		// Asset types & icons
		static (Type type, Func<GUIContent> getIcon )[] typesAndIcons = {
			(typeof(Material),	() => RenderQueueGUI.IconMaterial),
			(typeof(Shader),	() => RenderQueueGUI.IconShader)
		};

		[SerializeField] int listIndex = 0;
		[SerializeField] AssetList[] lists = new AssetList[typesAndIcons.Length];
		[SerializeField] int filterIndex = 0;
		[SerializeField] string[] filterNames;
		[SerializeField] int[] filterIndices;

		AssetList CurrentList {
			get => lists[listIndex];
			set => lists[listIndex] = value;
		}
		Type CurrentType => typesAndIcons[listIndex].type;

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
			CurrentList.Draw();
			DeselectIfClickedNothing();
		}

		void SavePrefs() {
			EditorPrefs.SetInt( EDITOR_PREF_KEY_FILTER_INDEX, filterIndex );
			EditorPrefs.SetInt( EDITOR_PREF_KEY_LIST_INDEX, listIndex );
		}
		void LoadPrefs() {
			filterIndex = EditorPrefs.GetInt( EDITOR_PREF_KEY_FILTER_INDEX, 0 );
			listIndex = EditorPrefs.GetInt( EDITOR_PREF_KEY_LIST_INDEX, 0 );
		}

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
			if( CurrentList == null )
				CurrentList = new AssetList();
			CurrentList.UpdateList( Filters.filters[filterIndex], CurrentType );
			RenderQueueGUI.Deselect();
		}


		void DrawHeader() {

			bool hasChanges = CurrentList.HasPendingChanges;

			GUILayout.BeginHorizontal( EditorStyles.toolbar );
			{
				if( GUILayout.Button( typesAndIcons[listIndex].getIcon(), EditorStyles.toolbarButton, GUILayout.Width( 24 ), GUILayout.Height(16) ) ) {
					listIndex = 1 - listIndex;
					RefreshList();
				}
				if( GUILayout.Button( "Refresh", EditorStyles.miniButtonLeft, GUILayout.Width( 66 ) ) ) {
					RefreshList();
				}
				EditorGUI.BeginDisabledGroup( hasChanges == false );
				if( GUILayout.Button( "Revert", EditorStyles.miniButtonMid, GUILayout.Width( 60 ) ) ) {
					CurrentList.RevertAllChanges();
					RefreshList();
				}
				if( GUILayout.Button( "Apply", EditorStyles.miniButtonRight, GUILayout.Width( 50 ) ) ) {
					CurrentList.ApplyAllChanges();
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