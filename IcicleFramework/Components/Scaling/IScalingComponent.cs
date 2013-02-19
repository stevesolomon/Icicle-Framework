namespace IcicleFramework.Components.Scaling
{
    public interface IScalingComponent : IBaseComponent
    {
        /// <summary>
        /// Gets the current scale of this IScalingComponent.
        /// </summary>
        float Scale { get; }

        /// <summary>
        /// Gets the default scale of this IScalingComponent.
        /// </summary>
        float DefaultScale { get; }

        /// <summary>
        /// Applies the given scaling value to this IScalingComponent and its parent IGameObject.
        /// </summary>
        /// <param name="scalingValue"></param>
        void ApplyScaling(float scalingValue);

        /// <summary>
        /// Resets this IScalingComponent back to default scaling.
        /// </summary>
        void ResetDefaultScaling();
    }
}
