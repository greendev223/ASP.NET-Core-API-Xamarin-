using MukaiTablet2.ViewModel.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MukaiTablet2.View.Common
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ViewProduct : ContentView
    {
        public ViewProduct(VmProduct vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }

        public ViewProduct()
        {
            InitializeComponent();
        }


    }


}