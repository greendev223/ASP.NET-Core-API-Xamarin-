using MukaiTablet2.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MukaiTablet2.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PageLogin : PageBase
    {
        public PageLogin(VmBase vm) : base(vm)
        {
            InitializeComponent();
        }
    }
}