using Masuit.Tools;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using System.Xml;

namespace Application.Common.Helper;

public class WebApiDocHelper
{
    public class ControllerInfo
    {
        public string ControllerName { get; set; }
        public List<ActionInfo> Actions { get; set; }
    }

    public class ActionInfo
    {
        public string ActionName { get; set; }
        public string Route { get; set; }
        public string Description { get; set; }
    }

    /// <summary>
    /// 获取控制器及动作名称
    /// </summary>
    /// <returns></returns>
    public static List<ControllerInfo> GetWebApiControllersWithActions()
    {
        var controllers = new List<ControllerInfo>();

        // 获取当前程序集中的所有控制器类型
        var controllerTypes = Assembly.GetEntryAssembly()?.GetTypes()
            .Where(type => typeof(ControllerBase).IsAssignableFrom(type))!;

        foreach (var controllerType in controllerTypes)
        {
            var controllerInfo = new ControllerInfo
            {
                ControllerName = controllerType.Name.Replace("Controller", ""),
                Actions = new List<ActionInfo>()
            };

            var methods = controllerType.GetMethods(BindingFlags.Public | BindingFlags.Instance);

            foreach (var method in methods)
            {
                var actionAttributes = method.GetCustomAttributes()
                    .Where(attr =>
                       attr.GetType().Name == "HttpGetAttribute"
                    || attr.GetType().Name == "HttpPostAttribute"
                    || attr.GetType().Name == "HttpPutAttribute"
                    || attr.GetType().Name == "HttpDeleteAttribute")
                    .ToList();

                if (actionAttributes.Any())
                {
                    var actionInfo = new ActionInfo
                    {
                        ActionName = method.Name,
                        Route = GetRouteFromAttributes(actionAttributes),
                        Description = GetDescriptionFromXmlDocumentation(controllerType, method)
                    };

                    controllerInfo.Actions.Add(actionInfo);
                }
            }

            if (controllerInfo.Actions.Any())
            {
                controllers.Add(controllerInfo);
            }
        }

        return controllers;
    }

    /// <summary>
    /// 获取路由信息
    /// </summary>
    /// <param name="attributes"></param>
    /// <returns></returns>
    public static string GetRouteFromAttributes(List<Attribute> attributes)
    {
        var routeAttribute = attributes.FirstOrDefault();
        if (routeAttribute != null)
        {
            return routeAttribute.GetType()?.GetProperty("Template")?.GetValue(routeAttribute)?.ToString()!;
        }
        return string.Empty;
    }

    /// <summary>
    /// 获取注释
    /// </summary>
    /// <param name="controllerType"></param>
    /// <param name="actionMethod"></param>
    /// <returns></returns>
    public static string GetDescriptionFromXmlDocumentation(Type controllerType, MethodInfo actionMethod)
    {
        string xmlCommentsFile = Path.Combine(AppContext.BaseDirectory, $"{controllerType.Assembly.GetName().Name}.xml");
        if (System.IO.File.Exists(xmlCommentsFile))
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlCommentsFile);

            var memberName = $"M:{controllerType.FullName}.{actionMethod.Name}";
            ParameterInfo[] parameters = actionMethod.GetParameters();
            var parameter = string.Empty;
            if (parameters.Length > 0)
            {
                parameter = parameters?[0]?.ParameterType?.FullName;
                if (!parameter.IsNullOrEmpty())
                {
                    parameter = $"({parameter})";
                }
            }

            var summaryNode = xmlDoc.SelectSingleNode($"/doc/members/member[@name='{memberName}{parameter}']/summary");

            if (summaryNode != null)
            {
                return summaryNode.InnerText.Trim();
            }
        }

        return "";
    }
}
