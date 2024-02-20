public class CharacterResource
{
    public bool IsRange;

    private int _value;
    private int _maxValue;

    public bool IsEmpty
    {
        get
        {
            if (_value <= 0) { return true; } else { return false; }
        }
    }

    public CharacterResource(int value, int maxValue)
    {
        this._value = value;
        this._maxValue = maxValue;
    }

    public int GetCurrent() { return _value; }
    public void SetCurrent(int i) { _value = i; }
    public void DecreaseCurrent(int amt)
    {
        _value -= amt;
    }
    public bool IncreaseCurrent(int amt)
    {
        if (_value + amt <= _maxValue)
        {
            _value += amt;
            return true;
        }
        else if (_value < _maxValue)
        {
            _value = _maxValue;
            return true;
        }
        return false;
    }

    public void SetCurrentToMax() { _value = _maxValue; }

    public int GetMax() { return _maxValue; }
    public void SetMax(int i) { _maxValue = i; }
    public void DecreaseMax(int amt) { _maxValue -= amt; }
    public void IncreaseMax(int amt) { _maxValue += amt; }
}
