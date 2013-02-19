using System;
using IcicleFramework.Actions;

namespace IcicleFramework.GameServices.Factories
{
    public interface IActionFactory : IGameService
    {
        bool PreloadAction<T>() where T : class, IGameAction;

        bool PreloadAction(Type type);

        T GetAction<T>() where T : class, IGameAction;

        IGameAction GetAction(Type type);
    }
}
