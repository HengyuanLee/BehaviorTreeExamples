﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GraphProcessor
{
	public class ProcessorView : PinnedElementView
	{
		BaseGraphProcessor	processor;

		protected override void Initialize(BaseGraphView graphView)
		{
			processor = new ProcessGraphProcessor(graphView.graph);

			graphView.computeOrderUpdated += processor.UpdateComputeOrder;

			Button	b = new Button(OnPlay) { name = "ActionButton", text = "Play !" };

			content.Add(b);
		}

		void OnPlay()
		{
			processor.Run();
		}
	}
}
