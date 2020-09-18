using System;
using System.Diagnostics;
using WaveEngine.AzureRemoteRendering;
using WaveEngine.AzureRemoteRendering.Components;
using WaveEngine.Components.WorkActions;
using WaveEngine.Framework;

namespace AzureRemoteRendeging_Demo
{
    public class MyScene : Scene
    {
        private readonly string skyboxUrl = "builtin://IndustrialPipeAndValve";

        public override void RegisterManagers()
        {
            base.RegisterManagers();
            this.Managers.AddManager(new WaveEngine.Bullet.BulletPhysicManager3D());
        }

        protected override void CreateScene()
        {
            var modelLoader = this.Managers.EntityManager.FindFirstComponentOfType<ARRModelLoader>();
            modelLoader.ProgressChanged += (s, progress) => {
                Debug.WriteLine($"[ARR] Loading model {modelLoader.Url} {progress:F}%");
            };

            var arrService = Application.Current.Container.Resolve<AzureRemoteRenderingService>();
            arrService.ConnectionStatusChanged += (s, status) =>
            {
                Debug.WriteLine($"[ARR] Connection status changed: {status}");
            };

            this.CreateWaitConditionWorkAction(() => arrService.IsCurrentSessionConnected)
                .ContinueWithAction(async () =>
                {
                    var inspectorPath = await arrService.ConnectToArrInspectorAsync();
                    Debug.WriteLine($"[ARR] ArrInspector at: {inspectorPath}");

                    if (!string.IsNullOrEmpty(this.skyboxUrl))
                    {
                        Debug.WriteLine($"[ARR] Loading skybox texture \"{this.skyboxUrl}\"");
                        var skyboxTexture = await arrService.LoadTextureFromSASAsync(this.skyboxUrl, Microsoft.Azure.RemoteRendering.TextureType.CubeMap);
                        arrService.CurrentSession.Actions.SkyReflectionSettings.SkyReflectionTexture = skyboxTexture;
                    }
                })
                .Run();
        }
    }
}