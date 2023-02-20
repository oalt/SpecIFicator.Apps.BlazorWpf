using MDD4All.FileAccess.Contracts;
using MDD4All.FileAccess.WPF;
using MDD4All.SpecIF.DataProvider.Base;
using MDD4All.SpecIF.DataProvider.Base.DataStreams;
using MDD4All.SpecIF.DataProvider.Contracts;
using MDD4All.SpecIF.DataProvider.Contracts.DataStreams;
using MDD4All.UI.BlazorComponents.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using SpecIFicator.Framework.Configuration;
using SpecIFicator.Framework.PluginManagement;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using VisNetwork.Blazor;

namespace SpecIFicator.Apps.BlazorWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            InitializeServices();
        }

        private void InitializeServices()
        {
            var services = new ServiceCollection();
            services.AddWpfBlazorWebView();
//#if DEBUG
            services.AddBlazorWebViewDeveloperTools();
//#endif

            services.AddLocalization(options =>
            {

                options.ResourcesPath = "Resources";
            });

            List<CultureInfo> supportedCultures = new List<CultureInfo>
            {
               new CultureInfo("en"),
               new CultureInfo("de")
            };

            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture =
                   new Microsoft.AspNetCore.Localization.RequestCulture("de");
                options.SupportedUICultures = supportedCultures;
            });

            services.AddSingleton<ISpecIfDataProviderFactory>(factory =>
            {
                return new SpecIfDataProviderFactory();
            });

            services.AddSingleton<ISpecIfStreamDataPublisherProvider>(_ =>
            {
                return new SpecIfStreamDataPublisherProvider();
            });

            services.AddSingleton<ISpecIfStreamDataSubscriberProvider>(_ =>
            {
                return new SpecIfStreamDataSubscriberProvider();
            });

            services.AddScoped<ClipboardDataProvider>();

            services.AddSingleton<IFileSaver>(fileSaver =>
            {
                return new WpfFileSaver();
            });

            services.AddSingleton<IFileLoader>(fileLoader =>
            {
                return new WpfFileLoader();
            });

            services.AddVisNetworkServer();

            services.AddHttpClient();

            Resources.Add("services", services.BuildServiceProvider());

            // SpecIFicator framework initialization
            DynamicConfigurationManager.LoadConfiguration();
            PluginManager.LoadPlugins(/* @"c:\Users\olli\Documents\work\github\SpecIFicator.Framework\src\plugins\ */);
        }
    }
}
