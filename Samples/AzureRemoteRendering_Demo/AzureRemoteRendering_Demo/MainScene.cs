using System;
using System.Diagnostics;
using Evergine.Common.Graphics;
using Evergine.Components.Graphics3D;
using Evergine.Components.WorkActions;
using Evergine.Framework;
using Evergine.Framework.Graphics;
using Evergine.Framework.Services;
using Evergine.Mathematics;
using Evergine.AzureRemoteRendering;
using Evergine.AzureRemoteRendering.Components;
using Evergine.MRTK.Scenes;

namespace AzureRemoteRendering_Demo
{
    public class MainScene : XRScene
    {
        protected override Guid CursorMatPressed => EvergineContent.MRTK.Materials.Cursor.CursorPinch;

        protected override Guid CursorMatReleased => EvergineContent.MRTK.Materials.Cursor.CursorBase;

        protected override Guid HoloHandsMat => Guid.Empty;

        protected override Guid SpatialMappingMat => Guid.Empty;

        protected override Guid HandRayTexture => EvergineContent.MRTK.Textures.line_dots_png;

        protected override Guid HandRaySampler => EvergineContent.MRTK.Samplers.LinearWrapSampler;

        protected override Guid LeftControllerModelPrefab => EvergineContent.MRTK.Prefabs.DefaultLeftController_weprefab;

        protected override Guid RightControllerModelPrefab => EvergineContent.MRTK.Prefabs.DefaultRightController_weprefab;

        protected override float MaxFarCursorLength => 0.5f;

        protected override void OnPostCreateXRScene()
        {
            base.OnPostCreateXRScene();

            var remoteSessionConfig = this.Managers.FindManager<ARRSessionManager>().AccountInfo;

            // Users need to fill out the following with their account data and model
            remoteSessionConfig.AccountId = "00000000-0000-0000-0000-000000000000";
            remoteSessionConfig.AccountKey = "<account key>";
            remoteSessionConfig.AccountDomain = "westeurope.mixedreality.azure.com"; // <change to your region>
            remoteSessionConfig.RemoteRenderingDomain = "westeurope.mixedreality.azure.com"; // <change to your region>

            var modelLoader = this.Managers.EntityManager.FindFirstComponentOfType<ARRModelLoader>();
            modelLoader.Url = "builtin://Engine";

            var skyboxUrl = "builtin://IndustrialPipeAndValve";
            var arrService = Application.Current.Container.Resolve<AzureRemoteRenderingService>();
            this.CreateWaitConditionWorkAction(() => arrService.IsCurrentSessionConnected)
                .ContinueWithAction(async () =>
                {
                    var inspectorPath = await arrService.ConnectToArrInspectorAsync();
                    Debug.WriteLine($"[ARR] ArrInspector at: {inspectorPath}");

                    if (!string.IsNullOrEmpty(skyboxUrl))
                    {
                        Debug.WriteLine($"[ARR] Loading skybox texture \"{skyboxUrl}\"");
                        var skyboxTexture = await arrService.LoadTextureFromSASAsync(skyboxUrl, Microsoft.Azure.RemoteRendering.TextureType.CubeMap);
                        arrService.CurrentSession.Connection.SkyReflectionSettings.SkyReflectionTexture = skyboxTexture;
                    }
                })
                .Run();
        }
    }
}