using System;
using System.Diagnostics;
using System.Linq;
using WaveEngine.AzureRemoteRendering;
using WaveEngine.AzureRemoteRendering.Components;
using WaveEngine.Common.Input;
using WaveEngine.Common.Input.Keyboard;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Mathematics;
using WaveEngine.MRTK.Emulation;
using WaveEngine.MRTK.SDK.Features.Input.Handlers.Manipulation;
using MRTKBoundingBox = WaveEngine.MRTK.SDK.Features.UX.Components.BoundingBox.BoundingBox;

namespace AzureRemoteRendeging_Demo.GUI
{
    /// <summary>
    /// This component handles commands listening to voice commands and keyboard keys press.
    /// 
    /// Key --------------- Command action
    /// 
    /// D0 ---------------- Reset experience position (in front of camera)
    /// D1 ---------------- Toggle cut plane visibility
    /// D2 ---------------- Toggle reference visibility
    /// D3 ---------------- Toggle model manipulation
    /// 
    /// D7 ---------------- Toggle wireframe
    /// D8 ---------------- Toggle polygon count
    /// D9 ---------------- Toggle frame count
    /// </summary>
    public class CommandsController : Behavior
    {
        [BindService]
        protected AzureRemoteRenderingService arrService;

        [BindService(isRequired: false)]
        protected IVoiceCommandService voiceCommandService;

        [BindComponent(source: BindComponentSource.Scene)]
        protected Camera3D camera;

        private Entity cutPlaneEntity;
        private Vector3 cutPlaneInitialScale;
        protected SimpleManipulationHandler cutPlaneManipulationHandler;
        protected MRTKBoundingBox cutPlaneBoundingBox;

        private Entity modelEntity;
        private Vector3 modelPlaneInitialScale;
        protected SimpleManipulationHandler modelManipulationHandler;
        protected MRTKBoundingBox modelBoundingBox;

        private Entity modelReferenceEntity;

        private bool IsCutPlaneActive
        {
            get => this.cutPlaneEntity.IsEnabled;
            set
            {
                this.cutPlaneEntity.IsEnabled = value;
            }
        }

        private bool IsModelReferenceActive
        {
            get => this.modelReferenceEntity.IsEnabled;
            set
            {
                this.modelReferenceEntity.IsEnabled = value;
            }
        }

        private bool IsModelManipulationActive
        {
            get => this.modelManipulationHandler.IsEnabled;
            set
            {
                this.modelManipulationHandler.IsEnabled = value;
                this.modelBoundingBox.IsEnabled = value;
            }
        }

        public bool RenderWireframe { get; private set; } = false;

        public bool RenderFrameCount { get; private set; } = false;

        public bool RenderPolygonCount { get; private set; } = false;

        /// <inheritdoc />
        protected override bool OnAttached()
        {
            if (Application.Current.IsEditor ||
                !base.OnAttached())
            {
                return false;
            }

            var cutPlaneComponent = this.Managers.EntityManager.FindFirstComponentOfType<ARRCutPlaneComponent>();
            this.cutPlaneManipulationHandler = cutPlaneComponent.Owner.FindComponentInParents<SimpleManipulationHandler>();
            this.cutPlaneBoundingBox = cutPlaneComponent.Owner.FindComponentInParents<MRTKBoundingBox>();
            this.cutPlaneEntity = this.cutPlaneManipulationHandler.Owner;
            this.cutPlaneInitialScale = this.cutPlaneEntity.FindComponent<Transform3D>().Scale;

            var modelLoaderComponent = this.Managers.EntityManager.FindFirstComponentOfType<ARRModelLoader>();
            this.modelEntity = modelLoaderComponent.Owner;
            this.modelManipulationHandler = this.modelEntity.FindComponentInParents<SimpleManipulationHandler>();
            this.modelBoundingBox = this.modelEntity.FindComponentInParents<MRTKBoundingBox>();
            this.modelPlaneInitialScale = this.modelEntity.FindComponent<Transform3D>().Scale;

            this.modelReferenceEntity = this.modelEntity.ChildEntities.FirstOrDefault();

            this.ConfigureVoiceCommands();

            return true;
        }

        /// <inheritdoc />
        protected override void OnActivated()
        {
            base.OnActivated();

            if (this.voiceCommandService != null)
            {
                this.voiceCommandService.CommandRecognized += this.VoiceCommandService_CommandRecognized;
            }
        }

        /// <inheritdoc />
        protected override void OnDeactivated()
        {
            base.OnDeactivated();

            if (this.voiceCommandService != null)
            {
                this.voiceCommandService.CommandRecognized -= this.VoiceCommandService_CommandRecognized;
            }
        }

        protected override void Update(TimeSpan gameTime)
        {
            var keyboard = this.camera.Display.KeyboardDispatcher;

            if (keyboard.ReadKeyState(Keys.D0) == ButtonState.Pressing)
            {
                this.ResetModelExperience();
            }

            if (keyboard.ReadKeyState(Keys.D1) == ButtonState.Pressing)
            {
                this.IsCutPlaneActive = !this.IsCutPlaneActive;
            }

            if (keyboard.ReadKeyState(Keys.D2) == ButtonState.Pressing)
            {
                this.IsModelReferenceActive = !this.IsModelReferenceActive;
            }

            if (keyboard.ReadKeyState(Keys.D3) == ButtonState.Pressing)
            {
                this.IsModelManipulationActive = !this.IsModelManipulationActive;
            }

            if (keyboard.ReadKeyState(Keys.D7) == ButtonState.Pressing)
            {
                this.RenderWireframe = !this.RenderWireframe;
            }

            if (keyboard.ReadKeyState(Keys.D8) == ButtonState.Pressing)
            {
                this.RenderPolygonCount = !this.RenderPolygonCount;
            }

            if (keyboard.ReadKeyState(Keys.D9) == ButtonState.Pressing)
            {
                this.RenderFrameCount = !this.RenderFrameCount;
            }


            if (!this.arrService.IsCurrentSessionConnected)
            {
                return;
            }

            var debugRenderingSettings = this.arrService.CurrentSession.Actions.DebugRenderingSettings;
            debugRenderingSettings.RenderWireframe = this.RenderWireframe;
            debugRenderingSettings.RenderPolygonCount = this.RenderPolygonCount;
            debugRenderingSettings.RenderFrameCount = this.RenderFrameCount;
        }

        private void ConfigureVoiceCommands()
        {
            if (this.voiceCommandService == null)
            {
                return;
            }

            this.voiceCommandService.ConfigureVoiceCommands(new[]
            {
                VoiceCommands.ResetExperienceCmd,
                VoiceCommands.ToggleCutPlaneCmd,
                VoiceCommands.ToggleModelReferenceCmd,
                VoiceCommands.ToggleModelManipulationCmd,
                VoiceCommands.ToggleWireframeCmd,
                VoiceCommands.TogglePolygonCountCmd,
                VoiceCommands.ToggleFrameCountCmd,
            });
        }

        private void VoiceCommandService_CommandRecognized(object sender, string voiceCommand)
        {
            switch (voiceCommand)
            {
                case VoiceCommands.ResetExperienceCmd:
                    this.ResetModelExperience();
                    break;

                case VoiceCommands.ToggleCutPlaneCmd:
                    this.IsCutPlaneActive = !this.IsCutPlaneActive;
                    break;

                case VoiceCommands.ToggleModelReferenceCmd:
                    this.IsModelReferenceActive = !this.IsModelReferenceActive;
                    break;

                case VoiceCommands.ToggleModelManipulationCmd:
                    this.IsModelManipulationActive = !this.IsModelManipulationActive;
                    break;

                case VoiceCommands.ToggleWireframeCmd:
                    this.RenderWireframe = !this.RenderWireframe;
                    break;

                case VoiceCommands.TogglePolygonCountCmd:
                    this.RenderPolygonCount = !this.RenderPolygonCount;
                    break;

                case VoiceCommands.ToggleFrameCountCmd:
                    this.RenderFrameCount = !this.RenderFrameCount;
                    break;

                default:
                    Debug.WriteLine($"No action for voice command: {voiceCommand}");
                    break;
            }
        }

        private void ResetModelExperience()
        {
            var cameraWorld = this.camera.Transform.WorldTransform;

            var rootModelTransform = this.modelEntity.FindComponent<Transform3D>();
            var rootModelSync = this.modelEntity.FindComponent<ARREntitySync>();
            rootModelTransform.Position = cameraWorld.Translation + (cameraWorld.Forward * 1.5f);
            var direction = cameraWorld.Translation;
            direction.Y = rootModelTransform.Position.Y;
            rootModelTransform.LocalLookAt(direction, Vector3.Up);
            rootModelTransform.Scale = this.modelPlaneInitialScale;
            rootModelSync.SyncToRemote();

            var cutPlaneTransform = this.cutPlaneEntity.FindComponent<Transform3D>();
            var cutPlaneSync = this.cutPlaneEntity.FindComponentInChildren<ARREntitySync>();
            cutPlaneTransform.Position = rootModelTransform.Position;
            cutPlaneTransform.Orientation = rootModelTransform.Orientation * Quaternion.CreateFromAxisAngle(Vector3.Up, MathHelper.Pi);
            cutPlaneTransform.Scale = cutPlaneInitialScale;
            cutPlaneSync.SyncToRemote();

            this.IsCutPlaneActive = true;
            this.IsModelReferenceActive = true;
        }
    }
}
