// Copyright © 2019 Freya Holmér • freya@acegikmo.com • https://github.com/FreyaHolmer/Render-Queue
namespace RenderQueuePlugin {

	using UnityEngine;
	using UnityEditor;

	public enum EntryState {
		Unchanged,
		Modified,
		Missing
	}

	[System.Serializable]
	public static class EntryStateExtensions {
		static readonly Color[] stateColors = {
			Color.white,
			Color.cyan,
			Color.red
		};
		public static Color GetColor( this EntryState state ) => stateColors[(int)state];
	}

}