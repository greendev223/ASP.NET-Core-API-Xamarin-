using MukaiTablet2.Util;
using MukaiTablet2.View.Common;
using MukaiTablet2.ViewModel;
using MukaiTablet2.ViewModel.Common;
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
    public partial class PageSearch : PageBase
    {
        private VmSearch mVmSearch;
        public PageSearch(VmBase vm) : base(vm)
        {
            InitializeComponent();
            mVmSearch = (VmSearch)vm;
        }

        private void ProductArea_SizeChanged(object sender, EventArgs e)
        {
            var grid = (Grid)sender;

            mVmSearch.ProductArea_SizeChanged(grid.Width, grid.Height);

        }
    }
}