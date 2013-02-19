using IcicleFramework.Actions;

namespace IcicleFramework.GameServices
{
    public delegate void ActionCompletedCallback(IGameAction action);

    public interface IActionManager : IGameService
    {
        /// <summary>
        /// Registers an <see cref="IGameAction"/> for execution immediately.
        /// </summary>
        /// <param name="action">The <see cref="IGameAction"/> to execute.</param>
        /// <param name="callback">A method to be called when the provided <see cref="IGameAction"/> has completed its execution.</param>
        void RegisterAction(IGameAction action, ActionCompletedCallback callback = null);

        /// <summary>
        /// Registers an <see cref="IGameAction"/> for execution after a provided delay in seconds.
        /// </summary>
        /// <param name="action">The <see cref="IGameAction"/> to execute.</param>
        /// <param name="delayInSeconds">The amount of time, in seconds, to wait before beginning execution of the given <see cref="IGameAction"/>.</param>
        /// <param name="callback">A method to be called when the provided <see cref="IGameAction"/> has completed its execution.</param>
        void RegisterDelayedAction(IGameAction action, float delayInSeconds, ActionCompletedCallback callback = null);
    }
}
