namespace Maui.FixesAndWorkarounds;

public sealed class KeyboardBehaviorTriggers : List<KeyboardBehaviorTrigger> { }

public sealed partial class KeyboardBehaviorTrigger
{
    KeyboardModifiers _modifiers;
    KeyboardKeys _keys;

    public KeyboardModifiers Modifiers
    {
        get => _modifiers;

        set
        {
            _modifiers = value;
#if IOS || MACCATALYST || WINDOWS
            SetPlatformModifiers(_modifiers);
#endif
        }
    }

    public KeyboardKeys Keys
    {
        get => _keys;

        set
        {
            _keys = value;
#if IOS || MACCATALYST || WINDOWS
			SetPlatformKeys(_keys);
#endif
		}
	}

    public override bool Equals(object obj)
    {
        if (obj is not KeyboardBehaviorTrigger trigger)
            return false;

        return Keys == trigger.Keys && Modifiers == trigger.Modifiers;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 23 + Keys.GetHashCode();
            hash = hash * 23 + Modifiers.GetHashCode();
            return hash;
        }
    }
}