using Sirenix.OdinInspector;
using UnityEngine;

[System.Serializable, InlineProperty] // InlineProperty draws fields inline where used
public struct CharacterResource
{
    [LabelText("Current"), SerializeField]
    private int _value;

    [LabelText("Max"), SerializeField]
    private int _maxValue;

    [ShowInInspector, ReadOnly]
    public bool IsEmpty => _value <= 0;

    public CharacterResource(int maxValue, int currentValue, bool clampCurrent = true)
    {
        _maxValue = Mathf.Max(0, maxValue);
        if (clampCurrent)
        {
            _value = Mathf.Clamp(currentValue, 0, _maxValue);
        }
        else
        {
            _value = currentValue;
        }
    }

    public int Current
    {
        get => _value;
        set => _value = Mathf.Clamp(value, 0, _maxValue);
    }

    public int Max
    {
        get => _maxValue;
        set
        {
            _maxValue = Mathf.Max(0, value);
            _value = Mathf.Min(_value, _maxValue);
        }
    }
}
