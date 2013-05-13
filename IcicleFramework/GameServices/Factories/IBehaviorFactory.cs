using System;
using System.Xml.Linq;
using IcicleFramework.Behaviors;
using IcicleFramework.Components.Behaviors;

namespace IcicleFramework.GameServices.Factories
{
    public interface IBehaviorFactory : IGameService
    {
        bool PreloadBehavior<T>() where T : class, IBehavior;

        bool PreloadBehavior(Type type);

        T GetBehavior<T>(string templateName = "") where T : class, IBehavior;

        IBehavior GetBehavior(Type type, string templateName = "");

        IBehavior LoadBehaviorFromXML(XElement element);

        bool SaveBehaviorAsTemplate(IBehavior behavior, string templateName, bool replaceExisting = false);
    }
}
