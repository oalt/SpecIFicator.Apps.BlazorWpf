using MDD4All.FileAccess.Contracts;
using MDD4All.FileAccess.WPF;
using MDD4All.SpecIF.DataProvider.Contracts;
using MDD4All.SpecIF.DataProvider.Contracts.DataStreams;
using MDD4All.SpecIF.DataProvider.MockupDataStream;
using MDD4All.UI.BlazorComponents.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using SpecIFicator.Framework.Configuration;
using SpecIFicator.Framework.PluginManagement;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
#if DEBUG
            services.AddBlazorWebViewDeveloperTools();
#endif

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

            services.AddScoped<ISpecIfDataSubscriber>(dataSubscriber =>
            {
                return new MockupDataSubscriber();
            });

            services.AddScoped<ClipboardDataProvider>();

            services.AddSingleton<IFileSaver>(fileSaver =>
            {
                return new WpfFileSaver();
            });

            Resources.Add("services", services.BuildServiceProvider());

            // SpecIFicator framework initialization
            DynamicConfigurationManager.LoadConfiguration();
            PluginManager.LoadPlugins(/* @"c:\Users\olli\Documents\work\github\SpecIFicator.Framework\src\plugins\ */);
        }
    }
}
