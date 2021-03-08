public interface IStatusable
{
    void TakeStatus(EStatusEffect statusEffect);
}

public enum EStatusEffect
{
    Freeze,
    Poison
}
