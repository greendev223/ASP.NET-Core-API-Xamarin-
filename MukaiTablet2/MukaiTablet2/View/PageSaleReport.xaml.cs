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
    public partial class PageSaleReport : PageBase
    {
        public PageSaleReport(VmBase vm) : base(vm)
        {
            InitializeComponent();
            SetupMargin();
            flexLayout.SizeChanged += FlexLayout_SizeChanged;
        }

        private void FlexLayout_SizeChanged(object sender, EventArgs e)
        {
            SetupMargin();
        }

        void SetupMargin()
        {
            double margin = Application.Current.MainPage.Width % 120;
            flexLayout.Margin = new Thickness(margin / 2, 0, 0, 0);
            stack_monthSelect.Margin = new Thickness(margin / 2 + 10, 0, 0, 0);
            btn_send.Margin = new Thickness(0, 0, margin / 2 + 10, 0);
        }
    }
}