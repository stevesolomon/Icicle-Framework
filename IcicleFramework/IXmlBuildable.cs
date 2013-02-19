using System.Xml.Linq;

namespace IcicleFramework
{
    /// <summary>
    /// An interface that enforces any class implementing it
    /// provide a method for rebuilding an instance of that class
    /// from an XElement object.
    /// </summary>
    public interface IXmlBuildable
    {
        void Deserialize(XElement element);
    }
}
