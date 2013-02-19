namespace IcicleFramework
{
    public interface IInitializable
    {
        /// <summary>
        /// Performs any necessary initialization for this component.
        /// </summary>
        /// <remarks>Assumes that all components are prepared in the parent IGameObject, but not that
        /// all IGameObjects have been created.</remarks>
        void Initialize();

        /// <summary>
        /// Performs any necessary pre-use, post-initialization for this component.
        /// </summary>
        /// <remarks>This is run after all components are prepared and Initialized in the parent IGameObject, and all
        /// initial IGameObjects for the scene have been created.</remarks>
        void PostInitialize();
    }
}
