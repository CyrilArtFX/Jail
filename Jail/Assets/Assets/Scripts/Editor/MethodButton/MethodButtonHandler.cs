using System;
using System.Collections.Generic;
using System.Reflection;

using UnityEngine;
using Object = UnityEngine.Object;

using Jail.Utility;

namespace Jail.Unity.MethodButton
{
	public class MethodButton
	{
		public MethodInfo Method { get; }
		public string Text { get; }

		public MethodButton(MethodInfo method, MethodButtonAttribute attribute)
		{
			Method = method;
			Text = attribute.Text;
		}

		public void Draw(Object[] targets)
		{
			//  draw button
			if (!GUILayout.Button(Text)) return;

			//  invoke all targets
			foreach (Object target in targets)
			{
				Method.Invoke(target, null);
			}
		}
	}

	public class MethodButtonHandler
	{
		readonly List<MethodButton> buttons = new List<MethodButton>();

		public MethodButtonHandler(Object target)
		{
			//  get type & flags
			BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;
			Type type = target.GetType();
			
			//  retrieve all attributed methods
			foreach (MethodInfo method in type.GetMethods(flags))
			{
				MethodButtonAttribute attribute = method.GetCustomAttribute<MethodButtonAttribute>();
				if (attribute == null) continue;

				buttons.Add(new MethodButton(method, attribute));
			}
		}

		public void Draw(Object[] targets)
		{
			int count = buttons.Count;
			if (count == 0) return;

			//  some margin so it's cleaner
			GUILayout.Space(12.0f);

			//  draw all buttons
			for (int i = 0; i < count; i++)
			{
				MethodButton button = buttons[i];
				button.Draw(targets);
			}
		}
	}
}
