// Copyright © 2019 Freya Holmér • freya@acegikmo.com • https://github.com/FreyaHolmer/Render-Queue
namespace RenderQueuePlugin {

	using System.Collections.Generic;

	public static class Filters {
		public static List<Filter> filters = new List<Filter>(){
			// Add new filters here
			// Do not use these symbols in filter names: &/^%
			// (They completely mess up formatting in the picker menu)
			new Filter( "All",
				(path) => true
			),
			new Filter( "Materials + Shaders",
				(path) => path.StartsWith("Assets/Materials/") || path.StartsWith("Assets/Shaders/")
			),
		};
	}

}