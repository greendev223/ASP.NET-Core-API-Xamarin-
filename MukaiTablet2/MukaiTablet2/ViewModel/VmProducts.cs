using MukaiTablet2.Model;
using MukaiTablet2.Util;
using MukaiTablet2.ViewModel.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace MukaiTablet2.ViewModel
{
    class VmProducts : VmBase
    {
        #region イベント
        public event EventHandler NotifyProductsUpdated;
        #endregion

        #region プロパティ
        public VmHeader Header
        {
            get { return _header; }
            set { _header = value; NotifyPropertyChanged(); }
        }
        private VmHeader _header = new VmHeader("", true, true);

        public ObservableCollection<BreadcrumbItem> Breadcrumb
        {
            get { return _breadcrumb; }
            set { _breadcrumb = value; NotifyPropertyChanged(); }
        }
        private ObservableCollection<BreadcrumbItem> _breadcrumb;

        public ObservableCollection<VmProduct> ProductList
        {
            get { return _productList; }
            set { _productList = value; NotifyPropertyChanged(); }
        }
        private ObservableCollection<VmProduct> _productList = new ObservableCollection<VmProduct>();
        #endregion

        #region 内部変数
        private readonly int mParentSeq;
        #endregion

        #region ライフサイクル
        public VmProducts(int parentSeq, ObservableCollection<BreadcrumbItem> breadcrumb)
        {
            Breadcrumb = breadcrumb;
            Header.OnBackButton_Clicked += BtnBack_Clicked;
            mParentSeq = parentSeq;
        }

        public override void OnInit(Object param)
        {
            Breadcrumb.Add(new BreadcrumbItem("TOP", -1,false));
            updateDisplay();
        }

        #endregion

        #region ユーザ操作
        public void LvBreadcrumb_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            //選択されたINDEXまでを削除する。
            while ((e.SelectedItemIndex + 1) < Breadcrumb.Count)
            {
                Breadcrumb.RemoveAt(Breadcrumb.Count - 1);
            }
            //bugbug
        }

        private async void BtnBack_Clicked(object sender, EventArgs e)
        {
            await Navigator.PopModalAsync();
        }

        public  void DEBUG_Clicked(object sender, EventArgs e)
        {
            updateDisplay();
        }

        #endregion

        #region 内部処理
        void updateDisplay()
        {
            var linqgoods = from g in AppData.LocalarrayGoods
                            join j in AppData.LocalarrayJoin on g.p.seqno equals j.c_seqno
                            where j.p_seqno == mParentSeq
                            orderby j.c_line
                            select g;

            var linqindex = from i in AppData.LocalarrayIndex
                            where i.seqno == mParentSeq
                            select i;
            Header.Title = linqindex.FirstOrDefault().idx_name;

            foreach (var item in linqgoods)
            {
                var vm = new VmProduct(item);
                ProductList.Add(vm);
            }

            NotifyProductsUpdated?.Invoke(this, new EventArgs());
        }

        #endregion
    }
}
