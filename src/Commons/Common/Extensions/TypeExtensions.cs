using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Common.Extensions;

public static class TypeExtensions
{
    public static string GetDescription(this Type type)
    {
        // 使用反射获取类型的 DescriptionAttribute
        var descriptionAttribute = type.GetCustomAttribute<DescriptionAttribute>();

        // 如果类型有 DescriptionAttribute，则返回其描述，否则返回类型的名称
        return descriptionAttribute != null ? descriptionAttribute.Description : type.Name;
    }
}