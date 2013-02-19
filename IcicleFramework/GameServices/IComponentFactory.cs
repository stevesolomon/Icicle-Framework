using System;
using System.Xml.Linq;
using IcicleFramework.Components;

namespace IcicleFramework.GameServices
{
    public interface IComponentFactory : IGameService
    {
        /// <summary>
        /// Loads an <see cref="IBaseComponent"/> of type T for use in the future.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="IBaseComponent"/> to load for use in the future.</typeparam>
        /// <returns>True if the component was loaded, false if otherwise.</returns>
        bool PreLoadComponent<T>() where T : class, IBaseComponent;

        /// <summary>
        /// Loads an <see cref="IBaseComponent"/> of type 'type' for use in the future.
        /// </summary>
        /// <param name="type">The type of the <see cref="IBaseComponent"/> to generate.</param>
        /// <returns>True if the component was loaded, false if otherwise.</returns>
        bool PreLoadComponent(Type type);

        /// <summary>
        /// Gets an instance of <see cref="IBaseComponent"/> of type T, using the pre-defined settings defined by the key name.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="IBaseComponent"/> to generate.</typeparam>
        /// <returns>The generated <see cref="IBaseComponent"/> of type T with pre-set properties matching the name template..</returns>
        /// <remarks>This can be called before, and in place of, PreLoadComponent(), but it will result in a performance penalty as this method performs the preload.</remarks>
        T GetComponent<T>() where T : class, IBaseComponent;

        /// <summary>
        /// Gets an instance of <see cref="IBaseComponent"/> of the given type.
        /// </summary>
        /// <param name="type">The type of the <see cref="IBaseComponent"/> to generate.</param>
        /// <returns>The generated <see cref="IBaseComponent"/> of type 'type' with pre-set properties matching the name template.</returns>
        IBaseComponent GetComponent(Type type);

        /// <summary>
        /// Gets an instance of <see cref="IBaseComponent"/> of type T, using the definition from an XML file.
        /// </summary>
        /// <param name="element">The XML element containing a definition for this component.</param>
        /// <returns>The generated <see cref="IBaseComponent"/> deserialized from XML.</returns>
        IBaseComponent LoadComponentFromXML(XElement element);
    }
}
