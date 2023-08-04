#if IOS || MACCATALYST
using System;
using Foundation;
using Microsoft.Maui.Platform;
using UIKit;

namespace Maui.FixesAndWorkarounds
{
	public class KeyboardPageViewController : PageViewController
	{
		bool _hasRegistrations;
		readonly List<WeakReference<KeyboardBehavior>> _keyboardBehaviors = new List<WeakReference<KeyboardBehavior>>();

		internal KeyboardPageViewController(IView page, IMauiContext mauiContext)
			: base(page, mauiContext) { }

		public override void PressesBegan(NSSet<UIPress> presses, UIPressesEvent evt)
		{
			ProcessPressses(UIPressesData.Create(presses, evt));
			base.PressesBegan(presses, evt);
		}

		public override void PressesCancelled(NSSet<UIPress> presses, UIPressesEvent evt)
		{
			ProcessPressses(UIPressesData.Create(presses, evt), true);
			base.PressesCancelled(presses, evt);
		}

		public override void PressesEnded(NSSet<UIPress> presses, UIPressesEvent evt)
		{
			ProcessPressses(UIPressesData.Create(presses, evt), true);
			base.PressesEnded(presses, evt);
		}

		internal void RegisterKeyboardBehavior(KeyboardBehavior keyboardBehavior)
		{
			if (TryGetIndexOfKeyboardBehavior(keyboardBehavior))
				return;

			_keyboardBehaviors.Add(new WeakReference<KeyboardBehavior>(keyboardBehavior));
			_hasRegistrations = true;
		}

		internal void UnregisterKeyboardBehavior(KeyboardBehavior keyboardBehavior)
		{
			if (!TryGetIndexOfKeyboardBehavior(keyboardBehavior, out var index))
				return;

			_keyboardBehaviors.RemoveAt(index);

			if (!_keyboardBehaviors.Any())
				_hasRegistrations = false;
		}

		bool TryGetIndexOfKeyboardBehavior(KeyboardBehavior keyboardBehavior)
			=> TryGetIndexOfKeyboardBehavior(keyboardBehavior, out _);

		bool TryGetIndexOfKeyboardBehavior(KeyboardBehavior keyboardBehavior, out int index)
		{
			index = _keyboardBehaviors.FindIndex(weakRef =>
			{
				if (weakRef.TryGetTarget(out var target))
					return target == keyboardBehavior;

				return false;
			});

			return index >= 0;
		}

		void CleanupTargets()
		{
			var targetsToRemove = _keyboardBehaviors.Where(i => !i.TryGetTarget(out var target)).ToList();

			foreach (var target in targetsToRemove)
				_keyboardBehaviors.Remove(target);
		}

		void ProcessPressses(UIPressesData presses, bool isKeyUp = false)
		{
			if (!_hasRegistrations)
				return;

			var targets = _keyboardBehaviors.Select(i => i.TryGetTarget(out var target) ? target : null).Where(i => i != null).ToList();

			if (targets.Count != _keyboardBehaviors.Count)
				CleanupTargets();

			if (targets.Count == 0)
				return;

			var trigger = targets.First().Triggers.First();

			var matchingRegistrations = targets.Where(i => i.Triggers.Any(
				i => (i.PlatformModifiers) == presses.Modifiers &&
				i.PlatformKeys.SequenceEqual(presses.Keys))).ToArray();

			var eventArgs = new KeyPressedEventArgs
			{
				Modifiers = presses.Modifiers.ToVirtualModifiers(),
				Keys = presses.Keys.ToVirtualKeys()
			};

			foreach (var registration in matchingRegistrations)
			{
				if (isKeyUp) registration.RaiseKeyUp(eventArgs);
				else registration.RaiseKeyDown(eventArgs);
			}
		}
	}
}
#endif