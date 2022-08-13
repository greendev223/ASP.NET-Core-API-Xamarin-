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
    public partial class PageOrderHistory : PageBase
    {
        VmOrderHistory mVmOrderHistory;
        public PageOrderHistory(VmBase vm) : base(vm)
        {
            mVmOrderHistory = (VmOrderHistory)vm;
            InitializeComponent();
        }

        protected override void OnInitDone(object sender,EventArgs e)
        {
            base.OnInitDone(sender,e);
        }
    }
}