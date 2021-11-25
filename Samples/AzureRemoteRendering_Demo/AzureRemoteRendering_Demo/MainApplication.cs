using Evergine.Framework;
using Evergine.Framework.Services;
using Evergine.Framework.Threading;
using Evergine.Platform;
using Evergine.NoesisGUI;
using Evergine.AzureRemoteRendering;

namespace AzureRemoteRendering_Demo
{
    public partial class MainApplication : Application
    {
        public MainApplication()
        {
            this.Container.RegisterType<Clock>();
            this.Container.RegisterType<TimerFactory>();
            this.Container.RegisterType<Random>();
            this.Container.RegisterType<ErrorHandler>();
            this.Container.RegisterType<ScreenContextManager>();
            this.Container.RegisterType<GraphicsPresenter>();
            this.Container.RegisterType<AssetsDirectory>();
            this.Container.RegisterType<AssetsService>();
            this.Container.RegisterType<ForegroundTaskSchedulerService>();
            this.Container.RegisterType<WorkActionScheduler>();
            this.Container.RegisterType<NoesisService>();
            this.Container.RegisterInstance(new AzureRemoteRenderingService());

            ForegroundTaskScheduler.Foreground.Configure(this.Container);
            BackgroundTaskScheduler.Background.Configure(this.Container);
        }

        public override void Initialize()
        {
            base.Initialize();

            // Get ScreenContextManager
            var screenContextManager = this.Container.Resolve<ScreenContextManager>();
            var assetsService = this.Container.Resolve<AssetsService>();

            // Navigate to scene
            var scene = assetsService.Load<MainScene>(EvergineContent.Scenes.MainScene_wescene);
            ScreenContext screenContext = new ScreenContext(scene);
            screenContextManager.To(screenContext);
        }
    }
}
