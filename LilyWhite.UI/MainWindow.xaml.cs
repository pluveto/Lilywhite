using LilyWhite.Lib.Runtime;
using LilyWhite.Lib.Type;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace LilyWhite.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private SynchronizationContext synchronizationContext;
        public MainWindow()
        {
            InitializeComponent();
            Engine.App.Start();
            Engine.App.SetOutput(new Type.ControlWriter(Dispatcher, this.OutputBox, () => {
                Scroller.ScrollToBottom();
            }));
            this.synchronizationContext = SynchronizationContext.Current;
            Engine.App.SetBuildCallback(OnProgressUpdated);
            Console.WriteLine("Main Window Loaded");
        }

        private void Debug_Generate_Click(object sender, RoutedEventArgs e)
        {
            //var configDir = AppDomain.CurrentDomain.BaseDirectory.TrimEnd('/');
            //var config = new Lib.Helper.Config(configDir + "/config");
            //MessageBox.Show(config.Get<string>("site.inputDir"));

        }

        private void Debug_AppDir_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", System.AppDomain.CurrentDomain.BaseDirectory);
        }

        private async void ActionUpdate_Click(object sender, RoutedEventArgs e)
        {
            await Task.Run(() =>
            {
                Engine.App.Update();
            });
        }

        private void OnProgressUpdated(object sender, BuildProgressEventArgs args)
        {
            this.synchronizationContext.Post((args) =>
            {
                var eventArgs = args as BuildProgressEventArgs;
                this.StatusText.Text = eventArgs.Message;
                this.BuildProgressBar.Value = eventArgs.GetPercent();
                if (eventArgs.BuildStatus == BuildStatus.Failed)
                {
                    this.BuildProgressBar.Foreground = Brushes.Red;
                }
                if (eventArgs.BuildStatus == BuildStatus.Success)
                {
                    if (!Engine.App.Config.Get<bool>("site.debug"))
                    {
                        this.OutputBox.Clear(); // 防止内存占用不断增大
                    }
                }
            }, args);
            
        }

        private void ActionReload_Click(object sender, RoutedEventArgs e)
        {
            Engine.App.Reload();
        }
    }
}
