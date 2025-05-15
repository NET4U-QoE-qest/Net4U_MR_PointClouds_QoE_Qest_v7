public static class PCObjectTypeExtensions
{
    public static bool TryParse(string name, out PCObjectType result)
    {
        foreach (PCObjectType val in System.Enum.GetValues(typeof(PCObjectType)))
        {
            if (val.ToString().Equals(name, System.StringComparison.OrdinalIgnoreCase))
            {
                result = val;
                return true;
            }
        }

        result = default;
        return false;
    }

    public static PCObjectType? FromString(string name)
    {
        if (TryParse(name, out PCObjectType result))
            return result;
        return null;
    }
}
