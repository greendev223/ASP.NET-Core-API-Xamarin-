using MukaiTablet2.Model;
using MukaiTablet2.MukaiWebService;
using MukaiTablet2.Util;
using MukaiTablet2.View;
using MukaiTablet2.ViewModel.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MukaiTablet2.ViewModel
{
    public class VmOrderHistory : VmBase
    {
        #region =================== イベント ===========================

        #endregion
        #region =================== プロパティ ===========================
        public VmHeader Header
        {
            get { return _header; }
            set { _header = value; NotifyPropertyChanged(); }
        }
        private VmHeader _header = new VmHeader("発注履歴", useSearchButton: false, useBackButton: true);

        public ObservableCollection<VmOrderHistoryItem> OrderHistItemList
        {
            get { return _orderHistItemList; }
            set { _orderHistItemList = value; NotifyPropertyChanged(); }
        }
        private ObservableCollection<VmOrderHistoryItem> _orderHistItemList = new ObservableCollection<VmOrderHistoryItem>();


        #endregion
        #region =================== プロパティ ===========================


        #endregion
        #region =================== 変数 ===========================
        #endregion
        #region =================== ライフサイクル ===========================
        public VmOrderHistory() { }


        public override async Task OnInit(Object param)
        {
            using (var bussy = new IsBussyHolder(this))
            {
                await bussy.Set();

                Header.OnBackButton_Clicked += async (sender, e) => { await Navigator.PopModalAsync(); };
                Header.Parent = this;
                if (Connectivity.NetworkAccess == NetworkAccess.Internet)
                {
                    await AppData.DownLoadOrderHistory(AppData.LastLoginShopMark, AppData.UserAuthCode,true);
                }

                //発注リストをロード
                var storage = new DataStorageHelper<ArrayOfOrderhistory>("orderHistory.xml", AppData.LastLoginShopMark);
                ArrayOfOrderhistory orderHistory = await storage.Load();
                foreach (var orderHistItem in orderHistory.OrderByDescending(item => item.senddate).Select(item => new VmOrderHistoryItem(item) { Parent = this }))
                {
                    OrderHistItemList.Add(orderHistItem);
                    orderHistItem.OnPageSizeChanged(mPageWidth, mPageHeight);
                }
            }
        }

        public override void OnPageSizeChanged(double width, double height)
        {
            base.OnPageSizeChanged(width, height);
            foreach(var item in OrderHistItemList)
            {
                item.OnPageSizeChanged(width, height);
            }
        }
        #endregion
        #region =================== ユーザ操作 ===========================
        #endregion
        #region =================== 内部関数 ===========================
        #endregion
    }

    public class VmOrderHistoryItem : VmBase
    {
        #region =================== イベント ===========================
        #endregion
        #region =================== プロパティ ===========================
        public string OrderDateTime
        {
            get { return _orderDateTime; }
            set { _orderDateTime = value; NotifyPropertyChanged(); }
        }
        private string _orderDateTime;

        

        public double Height
        {
            get { return Math.Ceiling(OrderList.Count / (double)ItemColNum) * 140; }
        }
        public double Width
        {
            get { return Math.Ceiling(OrderList.Count / (double)ItemRowNum) * 250; }
        }
        
        public int ItemColNum
        {
            get {
                if (mPageWidth == 0) return 1;
                int num = (int)Math.Floor((mPageWidth - 40) / 120);
                if (num <= 0) num = 1;
                return num;
            }
        }

        public int ItemRowNum
        {
            get
            {
                if (mPageHeight == 0) return 1;
                int num = (int)Math.Floor((mPageHeight - 100) / 255);
                if (num <= 0) num = 1;
                return num;
            }
        }

        public ObservableCollection<VmProduct> OrderList
        {
            get { return _orderList; }
            set { _orderList = value; NotifyPropertyChanged(); }
        }
        private ObservableCollection<VmProduct> _orderList = new ObservableCollection<VmProduct>();
        #endregion
        #region =================== 変数 ===========================
        #endregion
        #region =================== ライフサイクル ===========================
        public VmOrderHistoryItem(MukaiTablet2.MukaiWebService.orderhistory orderHistory)
        {
            OrderDateTime = orderHistory.senddate.ToString("yyyy/MM/dd HH:mm:ss");
            var bc = new System.Collections.Concurrent.BlockingCollection<VmProduct>();
            var po = new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount };
            Parallel.ForEach(orderHistory.reqs, po, (req) =>
            {
                VmProduct product = new VmProduct(req.goods);
                product.ParentVm = this;
                product.OrderedNum = (int)req.qty;

                product.OnClicked += async (sender, e) =>
                {
                    using (var bussy = new IsBussyHolder(this))
                    {
                        if (await bussy.Set() == false) return;
                        VmProduct item = (VmProduct)sender;
                        Page page = new PageProduct(item);
                        await Navigator.PushModalAsync(page);
                    }
                };
                bc.TryAdd(product, System.Threading.Timeout.Infinite);
            });
            OrderList = new ObservableCollection<VmProduct>(bc);
        }
        public VmOrderHistoryItem() { }

        public override void OnPageSizeChanged(double width, double height)
        {
            base.OnPageSizeChanged(width, height);
            string[] notifies = {  nameof(ItemColNum), nameof(ItemRowNum), nameof(Height), nameof(Width) };
            NotifyPropertyChanged(notifies);

        }

        #endregion
        #region =================== ユーザ操作 ===========================
        #endregion
        #region =================== 内部関数 ===========================
        #endregion


    }

}
