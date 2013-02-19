using System.Collections.Generic;
using IcicleFramework.GameServices.CameraServices;
using Microsoft.Xna.Framework;

namespace IcicleFramework.Cameras
{
    public class CameraManager : ICameraService
    {
        protected Dictionary<string, ICamera> cameras;
 
        public ICamera MainCamera { get; protected set; }

        public CameraManager()
        {
            cameras = new Dictionary<string, ICamera>(8);
        }

        public virtual void AddCamera(ICamera camera, bool main = false)
        {
            if (cameras.ContainsKey(camera.Name))
                return;

            cameras.Add(camera.Name, camera);

            //If this Camera has been requested as the main camera, or we have only one current camera,
            //then set this camera as the main camera.
            if (main || cameras.Count == 1)
            {
                MainCamera = camera;
            }
        }

        public virtual void RemoveCamera(string name)
        {
            cameras.Remove(name);
        }

        public ICamera GetCamera(string name)
        {
            return cameras.ContainsKey(name) ? cameras[name] : null;
        }

        public Vector2 TranslateWorldPointToScreen(Vector2 worldPoint, string cameraName = "")
        {
            ICamera camera;

            if(!cameras.TryGetValue(cameraName, out camera))
            {
                camera = MainCamera;
            }

            if (camera != null)
            {
                return camera.ConvertWorldPointToCamera(worldPoint);
            }

            return Vector2.Zero;
        }

        public void Update(GameTime gameTime)
        {
            foreach (var camera in cameras.Values)
            {
                camera.Update(gameTime);
            }
        }

        public void Initialize() { }

        public void PostInitialize() { }
    }
}
