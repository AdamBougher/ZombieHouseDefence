public class CharacterResource
{
    public bool isRange;

    private int value;
    private int maxValue;

    public bool isEmpty
    {
        get
        {
            if (value <= 0) { return true; } else { return false; }
        }
    }

    public CharacterResource(int _value, int _maxValue)
    {
        value = _value;
        maxValue = _maxValue;
    }

    public int GetCurrent() { return value; }
    public void SetCurrent(int i) { value = i; }
    public void DecreaseCurrent(int amt)
    {
        value -= amt;
    }
    public bool IncreaseCurrent(int amt)
    {
        if (value + amt <= maxValue)
        {
            value += amt;
            return true;
        }
        else if (value < maxValue)
        {
            value = maxValue;
            return true;
        }
        return false;
    }

    public void SetCurrentToMax() { value = maxValue; }

    public int GetMax() { return maxValue; }
    public void setMax(int i) { maxValue = i; }
    public void DecreaseMax(int amt) { maxValue -= amt; }
    public void IncreaseMax(int amt) { maxValue += amt; }
}
