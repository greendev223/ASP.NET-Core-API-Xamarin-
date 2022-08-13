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
    public partial class PageProducts : PageBase
    {

        #region ローカル変数
        private int mWitdh = 0;
        private bool mUpdated = false;
        private VmProducts mVmProducts;
        #endregion

        #region ライフサイクル
        public PageProducts(VmBase vm) : base(vm)
        {
            InitializeComponent();
            mVmProducts = (VmProducts)vm;
            mVmProducts.NotifyProductsUpdated += onUpdateProducts;
        }
        #endregion


        #region イベント処理
        private void onUpdateProducts(object sender, EventArgs e)
        {

        }
        #endregion

        #region ボタン処理
        private void LvBreadcrumb_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            ListView listView = (ListView)sender;
            if (listView.SelectedItem == null) return;

            mVmProducts.LvBreadcrumb_ItemSelected(sender, e);
            listView.SelectedItem = null;
        }
        private void DEBUG_Clicked(object sender, EventArgs e)
        {
            mVmProducts.DEBUG_Clicked(sender, e);
        }
        #endregion
    }
}