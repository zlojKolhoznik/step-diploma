using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Olx.Extensions;

public static class EnumExtensions
{
    public static string GetDisplayName(this Enum value)
    {
        var field = value.GetType().GetField(value.ToString());
        var attribute = (DisplayAttribute)field.GetCustomAttribute(typeof(DisplayAttribute));
        return attribute?.Name ?? value.ToString();
    }
}