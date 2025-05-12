public class MinMax
{
    public float Min { get; private set; }
    public float Max { get; private set; }

    public MinMax(){
        this.Min = float.MaxValue;
        this.Max = float.MinValue;
    }

    public void AddValue(float value)
    {
        if (value < Min)
        {
            Min = value-.2f;
        }
        if (value > Max)
        {
            Max = value;
        }
    }
}