using MukaiTablet2.ViewModel;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MukaiTablet2
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // DLToolkit.Forms.Controls.FlowListView 初期化
            DLToolkit.Forms.Controls.FlowListView.Init();

            MainPage = new View.PageLogin(new VmLogin());
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
