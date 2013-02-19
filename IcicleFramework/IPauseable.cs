namespace IcicleFramework
{
    public interface IPauseable
    {
        /// <summary>
        /// Gets whether or not this <see cref="IPauseable"/> object is paused.
        /// </summary>
        bool Paused { get; }

        /// <summary>
        /// Pauses this <see cref="IPauseable"/> object in time.
        /// </summary>
        void Pause();

        /// <summary>
        /// Resumes this <see cref="IPauseable"/> object from a paused state.
        /// </summary>
        void Resume();
    }
}
