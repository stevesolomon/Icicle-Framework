using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace IcicleFramework.GameServices
{
    public static class GameServiceManager
    {
        private static Dictionary<Type, IGameService> services = new Dictionary<Type, IGameService>();

        public static Dictionary<Type, IGameService>.ValueCollection Services
        {
            get { return services.Values; }
        }

        /// <summary>
        /// Adds a service of the provided type.
        /// </summary>
        /// <param name="type">The type of the Service to be added.</param>
        /// <param name="gameService">The Service itself.</param>
        public static void AddService(Type type, IGameService gameService)
        {
            if (!services.ContainsKey(type))
            {
                services.Add(type, gameService);
            }
        }

        /// <summary>
        /// Gets a Service of the requested type if one is registered.
        /// </summary>
        /// <param name="type">The type of service to retrieve.</param>
        /// <returns>The Service of the requested type, or null if no such Service is stored.</returns>
        public static IGameService GetService(Type type)
        {
            if (services.ContainsKey(type))
                return services[type];
            else
                return null;
        }

        /// <summary>
        /// Gets a Service of the requested type if one is registered.
        /// </summary>
        /// <typeparam name="T">The type of the Service to retrieve.</typeparam>
        /// <returns>The Service of the requested type, or null if no such Service is stored.</returns>
        public static T GetService<T>()
        {
            IGameService result = GetService(typeof (T));

            if (result != null)
                return (T)result;
            else
                return default(T);
        }

        public static void Update(GameTime gameTime)
        {
            foreach (IGameService service in Services)
                service.Update(gameTime);
        }

        public static void Initialize()
        {
            foreach (IGameService service in Services)
                service.Initialize();
        }

        public static void PostInitialize()
        {
            foreach (IGameService service in Services)
                service.PostInitialize();
        }
    }
}
