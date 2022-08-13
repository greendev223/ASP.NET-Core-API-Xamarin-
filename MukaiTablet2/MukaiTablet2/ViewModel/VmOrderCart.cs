using MukaiTablet2.Model;
using MukaiTablet2.MukaiWebService;
using MukaiTablet2.Util;
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
    class VmOrderCart : VmBase
    {
        #region =================== イベント ===========================
        #endregion
        #region =================== プロパティ ===========================
        public VmHeader Header
        {
            get { return _header; }
            set { _header = value; NotifyPropertyChanged(); }
        }
        private VmHeader _header = new VmHeader("発注カート", useSearchButton: false, useBackButton: true, useSubMenu: true);

        /// <summary>
        /// 発注数系
        /// </summary>
        public int TotalOrderNum
        {
            get
            {
                int num = 0;
                foreach (var order in OrderList)
                {
                    num += order.OrderedNum;
                }
                return num;
            }
        }

        /// <summary>
        /// 合計価格
        /// </summary>
        public int TotalPrice
        {
            get
            {
                int price = 0;
                foreach (var order in OrderList)
                {
                    price += order.TotalPrice;
                }
                return price;
            }
        }


        /// <summary>
        /// 注文リスト
        /// </summary>
        public ObservableCollection<VmOrderProduct> OrderList
        {
            get { return _orderList; }
            set
            {
                _orderList = value;
                NotifyPropertyChanged();
            }
        }
        private ObservableCollection<VmOrderProduct> _orderList = new ObservableCollection<VmOrderProduct>();

        public ObservableCollection<OrderSummary> Summaries
        {
            get { return _summaries; }
            set { _summaries = value; NotifyPropertyChanged(); }
        }
        public ObservableCollection<OrderSummary> _summaries = new ObservableCollection<OrderSummary>();

        public double SummaryHeight
        {
            get
            {
                double ret = (double)Math.Ceiling((decimal)Summaries.Count / 2.0m);
                ret *= 45;
                ret += 10;
                return ret;
            }
        }

        public bool IsEditMode
        {
            get { return _isEditMode; }
            set { _isEditMode = value; NotifyPropertyChanged(); }
        }
        private bool _isEditMode = false;

        public bool HasChecked
        {
            get
            {
                foreach (var item in OrderList) if (item.IsChecked) return true;
                return false;
            }
        }

        #endregion
        #region =================== 変数 ===========================



        #endregion
        #region =================== ライフサイクル ===========================
        public VmOrderCart()
        {
        }

        public override async Task OnInit(Object param)
        {
            //商品リスト作成
            foreach (var order in Order.LocalArrayReq)
            {
                VmOrderProduct product = new VmOrderProduct(order.goods);
                product.OrderedNum = (int)order.qty;
                product.OnOrderNumChanged += OnOrderNumChanged;
                product.Parent = this;
                product.OnisCheckChanged += (sender, e) => { NotifyPropertyChanged(nameof(HasChecked)); };  //チェック変化でHasCheckedのNotify
                OrderList.Add(product);
            }

            //サマリ更新
            updateSummaries();

            string[] notifies = { nameof(TotalPrice), nameof(TotalOrderNum) };
            NotifyPropertyChanged(notifies);

            Header.OnBackButton_Clicked += async (sender, e) => { await Navigator.PopModalAsync(); };
            Header.Parent = this;
            Header.OnSubMenu_Clicked += SubMenu_Clicked;

            await Task.Delay(0);

        }

        #endregion
        #region =================== ユーザ操作 ===========================
        /// <summary>
        /// 注文ボタン
        /// </summary>
        public DelegateCommand CommandOrder
        {
            get
            {
                return _commandOrder = _commandOrder ?? new DelegateCommand(() => { Order_Clicked(this, new EventArgs()); });
            }
        }
        private DelegateCommand _commandOrder;

        private async void Order_Clicked(object sender, EventArgs e)
        {
            using (var bussy = new IsBussyHolder(this))
            {
                if (await bussy.Set() == false) return;
                bool isSuccess;
                if (Connectivity.NetworkAccess != NetworkAccess.Internet)
                {
                    await Navigator.CurrentPage.DisplayAlert("エラー", "インターネットに接続されていません", "OK");
                    return;
                }

                if (Order.LocalArrayReq.Count == 0)
                {
                    await Navigator.CurrentPage.DisplayAlert("", "発注カートが空です。", "OK");
                    return;
                }

                //オーダ実施
                IDepend dep = DependencyService.Get<IDepend>();
                isSuccess = await dep.ReqOrder(Order.LocalArrayReq, AppData.UserAuthCode);
                if (isSuccess == false)
                {
                    await Navigator.CurrentPage.DisplayAlert("エラー", "送信エラーが発生し、注文に失敗しました。", "OK");
                    return;
                }

                //発注リストをダウンロード
                await AppData.DownLoadOrderHistory(AppData.LastLoginShopMark, AppData.UserAuthCode, true);

                //GOODS状態を初期化更新
                foreach (var req in Order.LocalArrayReq) { AppData.SetGoodsOrderInit(req); }

                //カード削除
                await Order.Init();

                //オーダリストクリア
                OrderList.Clear();

                //合計を更新
                string[] notifies = { nameof(TotalOrderNum), nameof(TotalPrice) };
                NotifyPropertyChanged(notifies);
            }
        }

        /// <summary>
        /// サブメニュー
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        public DelegateCommand CommandSubMenu
        {
            get
            {
                return _commandSubMenu = _commandSubMenu ?? new DelegateCommand(() => { SubMenu_Clicked(this, new EventArgs()); });
            }
        }
        private DelegateCommand _commandSubMenu;

        private async void SubMenu_Clicked(object sender, EventArgs e)
        {
            using (var bussy = new IsBussyHolder(this))
            {
                if (await bussy.Set() == false) return;

                const string SELECT_MULTI = "複数選択";
                const string SELECT_ALL = "全選択";
                const string SELECT_OFF = "選択解除";

                string[] items = { SELECT_MULTI, SELECT_ALL, SELECT_OFF };
                string selected = await Navigator.CurrentPage.DisplayActionSheet("選択して下さい", "キャンセル", null, items);

                if (selected == SELECT_MULTI)
                {
                    IsEditMode = true;
                }
                else if (selected == SELECT_ALL)
                {
                    IsEditMode = true;
                    foreach (var order in OrderList) order.IsChecked = true;
                }
                else if (selected == SELECT_OFF)
                {
                    IsEditMode = true;
                    foreach (var order in OrderList) order.IsChecked = false;
                }
            }
        }

        /// <summary>
        /// 選択されたオーダを削除
        /// </summary>
        public DelegateCommand CommandEndEdit
        {
            get { return _commandEndEdit = _commandEndEdit ?? new DelegateCommand(EndEdit_Clicked); }
        }
        private DelegateCommand _commandEndEdit;
        private async void EndEdit_Clicked()
        {
            using (var bussy = new IsBussyHolder(this))
            {
                if (await bussy.Set() == false) return;
                foreach (var order in OrderList) order.IsChecked = false;
                IsEditMode = false;
            }
        }


        /// <summary>
        /// 選択されたオーダを削除
        /// </summary>
        public DelegateCommand CommandDeleteSelected
        {
            get { return _commandDeleteSelected = _commandDeleteSelected ?? new DelegateCommand(DeleteSelected_Clicked); }
        }
        private DelegateCommand _commandDeleteSelected;
        private async void DeleteSelected_Clicked()
        {
            using (var bussy = new IsBussyHolder(this))
            {
                if (await bussy.Set() == false) return;

                ObservableCollection<VmOrderProduct> tmpList = new ObservableCollection<VmOrderProduct>(OrderList);
                foreach (var order in tmpList)
                {
                    if (order.IsChecked)
                    {
                        var req = Order.LocalArrayReq.Where((item) =>
                        {
                            if (order.mGoods.g_seqno == item.goods.g_seqno) return true;
                            return false;
                        }).FirstOrDefault();

                        //注文削除
                        AppData.SetGoodsOrderInit(req);
                        await Order.Delete(req);

                        order.OrderedNum = 0;

                        OrderList.Remove(order);
                    }
                }
                NotifyPropertyChanged(nameof(HasChecked));
                if (OrderList.Count == 0)
                {
                    IsEditMode = false;
                }

                updateSummaries();
                string[] notifies = { nameof(TotalOrderNum), nameof(TotalPrice) };
                NotifyPropertyChanged(notifies);
            }
        }



        /// <summary>
        /// 注文数が更新された
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnOrderNumChanged(object sender, EventArgs e)
        {
            //呼び出し下でIsBussyはSet済

            VmOrderProduct vmProduct = (VmOrderProduct)sender;

            //注文数＝０は削除されたことを意味する
            if (vmProduct.OrderedNum == 0)
            {
                //注文リストから削除
                OrderList.Remove(vmProduct);
                
                NotifyPropertyChanged(nameof(HasChecked));
                if(OrderList.Count == 0)
                {
                    IsEditMode = false;
                }
            }

            //サマリ更新
            updateSummaries();

            string[] notifies = { nameof(TotalOrderNum), nameof(TotalPrice) };
            NotifyPropertyChanged(notifies);
        }




        #endregion
        #region =================== 内部関数 ===========================
        private void updateSummaries()
        {
            Summaries.Clear();
            //サマリ作成
            foreach (var req in Order.LocalArrayReq.OrderBy(r => r.goods.p.rcode))
            {
                var sum = Summaries.FirstOrDefault(s => s.rcode == req.goods.p.rcode);
                if (sum == null)
                {
                    Summaries.Add(new OrderSummary()
                    {
                        rcode = req.goods.p.rcode,
                        rname = req.goods.p.rname,
                        qty = req.qty,
                        price = req.goods.p.upprice * req.qty
                    });
                }
                else
                {
                    sum.qty += req.qty;
                    sum.price += req.goods.p.upprice * req.qty;
                }
            }

            NotifyPropertyChanged(nameof(SummaryHeight));
        }
        #endregion
    }

    public class VmOrderProduct : VmProduct
    {
        public event EventHandler OnisCheckChanged;

        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                _isChecked = value;
                OnisCheckChanged?.Invoke(this, new EventArgs());
                NotifyPropertyChanged();
            }
        }
        private bool _isChecked = false;


        public VmOrderProduct(goods goods) : base(goods) { }
    }


}
