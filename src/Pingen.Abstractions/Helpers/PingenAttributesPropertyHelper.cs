using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json.Serialization;
using Pingen.Abstractions.Interfaces.Data;

namespace Pingen.Abstractions.Helpers;

/// <summary>
/// Static helper for properties of type T
/// </summary>
public static class PingenAttributesPropertyHelper<T>
    where T : IAttributes
{
    /// <summary>
    /// Get name from json property name attribute of a specific property from type T
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    public static string GetJsonPropertyName<TValue>(Expression<Func<T, TValue>> selector)
    {
        Expression body = selector;
        if (body is LambdaExpression expression)
        {
            body = expression.Body;
        }

        if (body.NodeType != ExpressionType.MemberAccess)
            throw new InvalidOperationException();

        return ((PropertyInfo)((MemberExpression)body).Member)
                .CustomAttributes.FirstOrDefault(attributeData => attributeData.AttributeType == typeof(JsonPropertyNameAttribute))
                ?.ConstructorArguments[0]
                .Value?.ToString()
            ?? throw new InvalidOperationException();
    }
}
