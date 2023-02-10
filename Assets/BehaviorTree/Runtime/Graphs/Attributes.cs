using GraphProcessor;
using System;

namespace MyBehaviorTree
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class TaskNodeMenuItemAttribute : NodeMenuItemAttribute
	{
		public TaskNodeMenuItemAttribute(string menuTitle = null) : base(menuTitle)
		{
			this.menuTitle = menuTitle;
			this.onlyCompatibleWithGraph = typeof(BehaviorTreeGraph);
		}
	}
}
