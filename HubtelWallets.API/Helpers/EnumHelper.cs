using System.Reflection;

namespace HubtelWallets.API.Helpers;

public static class EnumHelper
{
    public static T GetEnumValue<T>(this string str) where T : struct, IConvertible
    {
        Type enumType = typeof(T);

        if (!enumType.GetTypeInfo().IsEnum) throw new ArgumentException("T must be an enumerated type");

        return Enum.TryParse<T>(str, true, out T val) ? val : throw new Exception($"The value {str} is not a valid enum value for {enumType.Name}");
    }
}
