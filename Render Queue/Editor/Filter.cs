// Copyright © 2019 Freya Holmér • freya@acegikmo.com • https://github.com/FreyaHolmer/Render-Queue
namespace RenderQueuePlugin {

	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using System.Linq;
	using System;

	public class Filter {

		public string name;
		public Func<string, bool> filter;

		public Filter( string name, Func<string, bool> filter ) {
			this.name = name;
			this.filter = filter;
		}

	}

}