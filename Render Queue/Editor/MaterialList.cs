// Copyright © 2019 Freya Holmér • freya@acegikmo.com • https://github.com/FreyaHolmer/Render-Queue
namespace RenderQueuePlugin {

	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;
	using System.Linq;

	[System.Serializable]
	public class MaterialList {

		[SerializeField] Vector2 scrollPosition = Vector2.zero;
		[SerializeField] List<MaterialEntry> entries;

		public bool HasPendingChanges  => entries.Any( e => e.ModifiedState == EntryState.Modified );
		public void ApplyAllChanges()  => entries.ForEach( e => e.ApplyIfModified() );
		public void RevertAllChanges() => entries.ForEach( e => e.RevertIfModified() );

		public void UpdateList( Filter filter ) {
			if( entries == null )
				entries = new List<MaterialEntry>();

			// Collect any pending modifications before refreshing
			IEnumerable<(Material,int)> modifications = entries
			.Where(  x => x.ModifiedState == EntryState.Modified )
			.Select( x => (x.material, x.renderQueueInput) );

			// Get all materials from the project filtered by the selected filter
			entries = AssetDatabase.FindAssets( "t:material" )
				.Select( guid => AssetDatabase.GUIDToAssetPath( guid ) )
				.Where( filter.filter )
				.Select( path => new MaterialEntry( AssetDatabase.LoadAssetAtPath<Material>( path ) ) )
				.ToList();

			// Re-add the pending changes to corresponding entries if they still exist
			foreach( var (modMat, modValue) in modifications ) {
				MaterialEntry entry = entries.FirstOrDefault( e => e.material == modMat );
				if( entry != null )
					entry.renderQueueInput = modValue;
			}

			// Order by Render Queue input
			// entries = entries.OrderByDescending( e => e.renderQueueInput ).ToList();
			entries.Sort( ( a, b ) => b.renderQueueInput.CompareTo( a.renderQueueInput ) );

		}

		public void Draw() {

			scrollPosition = GUILayout.BeginScrollView( scrollPosition );
			{
				GUILayout.BeginVertical( RenderQueueGUI.PanelStyle );
				for( int i = 0; i < entries.Count; i++ ) {

					// Entry
					entries[i].Draw();

					// Spacing between groups
					if( i < entries.Count - 1 ) {
						bool validEntries = entries[i].ModifiedState != EntryState.Missing && entries[i+1].ModifiedState != EntryState.Missing;
						if( validEntries ) {
							int delta = entries[i].RenderQueue - entries[i + 1].RenderQueue;
							if( delta > 1 ) {
								GUILayout.EndVertical();
								RenderQueueGUI.Fade( 0.4f, () => {
									RenderQueueGUI.LabelSmallRight( delta.ToString() );
								} );
								GUILayout.BeginVertical( RenderQueueGUI.PanelStyle );
							}
						}
					}

				}
				GUILayout.EndVertical();
			}
			GUILayout.EndScrollView();
		}



	}

}