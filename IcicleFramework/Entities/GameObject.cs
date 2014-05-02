using System;
using System.Collections.Generic;
using System.Xml.Linq;
using IcicleFramework.Components;
using IcicleFramework.GameServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Newtonsoft.Json;

namespace IcicleFramework.Entities
{
    public sealed class GameObject : IGameObject
    {
        #region Internal Members

        private Vector2 position;

        private Vector2 lastPosition;

        private float rotation;

        private float lastRotation;

        private bool active;

        private Dictionary<string, object> metadata;

        private IComponentFactory componentFactory;
        
        #endregion


        #region Properties

        public Guid GUID { get; private set; }

        public bool Active
        {
            get { return active; }

            set
            {
                if (active != value && components != null)
                {
                    foreach (IBaseComponent component in components.Values)
                        component.Active = value;
                }

                active = value;
            }
        }

        public string Name { get; set; }

        public bool Destroyed { get; private set; }

        public bool HasMoved { get { return lastPosition != position; } }

        public bool HasRotated { get { return Math.Abs(lastRotation - rotation) >= 0.0000001f; } }

        public Dictionary<string, object> Metadata
        {
            get { return metadata; }
        }

        public string Layer { get; set; }

        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        public Vector2 LastMovementAmount
        {
            get { return Position - LastFramePosition; }
        }

        public Vector2 LastFramePosition { get { return lastPosition; } }

        public float Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }

        public float LastRotationAmount
        {
            get { return Rotation - lastRotation; }
        }

        [JsonPropertyAttribute]
        private Dictionary<Type, IBaseComponent> components; 

        #endregion


        #region Events

        public event MoveHandler OnMove;

        public event DestroyedHandler<IGameObject> OnDestroyed;

        public event InitializeHandler OnInitialize;

        #endregion


        #region Constructors

        /// <summary>
        /// Creates a new GameObject with basic initialized settings.
        /// </summary>
        public GameObject()
        {
            OnMove = null;

            components = new Dictionary<Type, IBaseComponent>();
            metadata = new Dictionary<string, object>();
        }

        #endregion


        #region Component Manipulation
        
        public T GetComponent<T>() where T : IBaseComponent
        {
            T foundComponent = default(T);

            if (components.ContainsKey(typeof(T)))
            {
                foundComponent = (T) components[typeof (T)];
            }

            return foundComponent;
        }

        public void AddComponent(Type baseInterfaceType, IBaseComponent component)
        {
            if (!components.ContainsKey(baseInterfaceType))
            {
                components.Add(baseInterfaceType, component);
                component.Parent = this;
            }
        }

        public IEnumerable<IBaseComponent> AllComponents()
        {
            return components.Values;
        }

        #endregion


        #region Update, Load, Initialize

        public void Dispose()
        {
            OnMove = null;
            OnDestroyed = null;
            Layer = null;

            //Don't Dispose() components here, that's the responsibility of someone else.

            components.Clear();
        }

        public void Initialize()
        {
            GUID = Guid.NewGuid();

            componentFactory = GameServiceManager.GetService<IComponentFactory>();

            foreach (var component in components.Values)
            {
                component.Initialize();
                component.Active = Active;
            }

            if (OnInitialize != null)
                OnInitialize(this);
        }

        public void PostInitialize()
        {
            foreach (var component in components.Values)
                component.PostInitialize();
        }

        public void Update(GameTime gameTime)
        {
            if (!Active)
                return;

            foreach (var component in components.Values)
            {
                component.Update(gameTime);
            }

            if (HasMoved)
            {
                if (OnMove != null)
                    OnMove(this);
            }

            lastPosition = position;

            if (HasRotated)
                lastRotation = rotation;
        }
        
        public void Load(ContentManager content)
        {
            foreach (var component in components.Values)
            {
                var loadable = component as ILoadable;

                if (loadable != null)
                    loadable.Load(content);
            }
        }

        #endregion


        #region Metadata Methods

        public bool AddMetadata(string name, object value)
        {
            var added = false;

            if (!metadata.ContainsKey(name))
            {
                metadata.Add(name, value);
                added = true;
            }

            return added;
        }

        public bool RemoveMetadata(string name)
        {
            var removed = false;

            if (metadata.ContainsKey(name))
            {
                metadata.Remove(name);
                removed = true;
            }

            return removed;
        }

        public bool UpdateMetadata(string name, object newValue)
        {
            var updated = false;

            if (HasMetadata(name))
            {
                metadata[name] = newValue;
                updated = true;
            }
            else
            {
                AddMetadata(name, newValue);
                updated = true;
            }

            return updated;
        }

        public object GetMetadata(string name)
        {
            return HasMetadata(name) ? metadata[name] : null;
        }

        public bool HasMetadata(string name)
        {
            return metadata.ContainsKey(name);
        }

        #endregion


        #region Cloning

        public void CopyInto(IGameObject newObject)
        {
            //Copy components
            foreach(var component in AllComponents())
            {
                var newComponent = componentFactory.GetComponent(component.ConcreteType);
                newObject.AddComponent(component.InterfaceType, newComponent);
                component.CopyInto(newComponent);
            }

            //Copy metadata and layer info
            foreach (var entry in metadata)
            {
                newObject.AddMetadata(entry.Key, entry.Value);
            }

            newObject.Layer = Layer;
        }

        private void DeserializeMetadata(XElement metadataElem)
        {
            if (metadataElem == null)
                return;

            var elems = metadataElem.Elements("add");

            foreach (var elem in elems)
            {
                var nameAttrib = elem.Attribute("name");
                if (nameAttrib != null)
                {
                    Metadata.Add(nameAttrib.Value, null);

                    var valueAttrib = elem.Attribute("value");

                    if (valueAttrib != null)
                        Metadata[nameAttrib.Value] = valueAttrib.Value;
                }
            }
        }

        private void DeserializeLayer(XElement layerElem)
        {
            if (layerElem == null)
                return;

            Layer = layerElem.Value.ToLowerInvariant();
        }

        public void Deserialize(XElement element)
        {
            var metadataElem = element.Element("metadata");
            var layerElem = element.Element("layer");

            DeserializeMetadata(metadataElem);
            DeserializeLayer(layerElem);

            Name = element.Attribute("name") != null ? element.Attribute("name").ToString() : "";
        }

        #endregion


        #region Destroy

        public void Destroy()
        {
            foreach (var component in components.Values)
            {
                component.Destroy();
            }

            if (OnDestroyed != null)
            {
                OnDestroyed(this);
            }

            Destroyed = true;
            Active = false;
        }
        
        #endregion


        #region IPoolable Methods

        public void Reallocate()
        {
            Active = false;
            Destroyed = false;
            GUID = new Guid();

            foreach (var component in components.Values)
                component.Reallocate();
        }

        #endregion
    }
}
