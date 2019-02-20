// Copyright © 2019 Freya Holmér • freya@acegikmo.com • https://github.com/FreyaHolmer/Render-Queue
namespace RenderQueuePlugin {

	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;
	using System.Linq;

	[System.Serializable]
	public class AssetList {

		[SerializeField] Vector2 scrollPosition = Vector2.zero;
		[SerializeField] List<Entry> entries;

		public bool HasPendingChanges  => entries.Any( e => e.ModifiedState == EntryState.Modified );
		public void ApplyAllChanges()  => entries.ForEach( e => e.ApplyIfModified() );
		public void RevertAllChanges() => entries.ForEach( e => e.RevertIfModified() );

		public void UpdateList( Filter filter, System.Type type ) {
			if( entries == null )
				entries = new List<Entry>();

			// Collect any pending modifications before refreshing
			IEnumerable<(Object,int)> modifications = entries
			.Where(  x => x.ModifiedState == EntryState.Modified )
			.Select( x => (x.asset, x.renderQueueInput) );

			// Get all assets from the project filtered by the selected filter
			string assetSearch = $"t:{type.Name}";
			entries = AssetDatabase.FindAssets( assetSearch )
				.Select( guid => AssetDatabase.GUIDToAssetPath( guid ) )
				.Where( filter.filter )
				.Select( path => new Entry( AssetDatabase.LoadAssetAtPath( path, type ) ) )
				.ToList();

			// Re-add the pending changes to corresponding entries if they still exist
			foreach( var (modMat, modValue) in modifications ) {
				Entry entry = entries.FirstOrDefault( e => e.asset == modMat );
				if( entry != null )
					entry.renderQueueInput = modValue;
			}

			// Order by Render Queue input
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