#if WINDOWS

namespace Maui.FixesAndWorkarounds;

public sealed partial class KeyboardBehaviorTrigger
{
	void SetPlatformModifiers(KeyboardModifiers modifiers)
		=> throw new NotImplementedException();

	void SetPlatformKeys(KeyboardKeys keys)
		=> throw new NotImplementedException();
}

internal class KeyboardBehaviorTriggerComparer : IEqualityComparer<KeyboardBehaviorTrigger>
{
	public bool Equals(KeyboardBehaviorTrigger x, KeyboardBehaviorTrigger y)
	{
		throw new NotImplementedException();
		//if (x == null && y == null)
		//    return true;
		//if (x == null || y == null)
		//    return false;

		//return x.PlatformKeys.SequenceEqual(y.PlatformKeys) && x.PlatformModifiers == y.PlatformModifiers;
	}

	public int GetHashCode(KeyboardBehaviorTrigger obj)
	{
		throw new NotImplementedException();
		//unchecked
		//{
		//    int hash = 17;
		//    hash = hash * 23 + obj.Keys.GetHashCode();
		//    hash = hash * 23 + obj.PlatformModifiers.GetHashCode();
		//    return hash;
		//}
	}
}
#endif