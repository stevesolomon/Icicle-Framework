namespace IcicleFramework
{
    public interface IDeepCopyable<T>
    {
        /// <summary>
        /// Deep Copies the parameters of the current instance into the TargetPosition instance.
        /// </summary>
        /// <param name="newObject">The new instance to deep copy parameters into.</param>
        void CopyInto(T newObject);
    }
}
