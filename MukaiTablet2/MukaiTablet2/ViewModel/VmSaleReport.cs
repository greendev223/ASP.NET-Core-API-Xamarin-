using MukaiTablet.Model;
using MukaiTablet2.Model;
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
    public class VmSaleReport : VmBase
    {
        #region イベント
        #endregion

        #region プロパティ


        public Thickness SaleInputDialogMargin
        {
            get { return _saleInputDialogMargin; }
            set { _saleInputDialogMargin = value; NotifyPropertyChanged(); }
        }
        private Thickness _saleInputDialogMargin;

        public VmHeader Header
        {
            get { return _header; }
            set { _header = value; NotifyPropertyChanged(); }
        }
        private VmHeader _header = new VmHeader("売上日報", useSearchButton: false, useBackButton: true);

        public bool IsIncludeTax
        {
            get { return _isIncludeTax; }
            set { _isIncludeTax = value; NotifyPropertyChanged(); }
        }
        private bool _isIncludeTax = false;

        public string YyyyMm
        {
            get { return mTargetDate.ToString("yyyy年MM月"); }
        }

        public string StoreName
        {
            get { return AppData.LastLoginShopName; }
        }

        public ObservableCollection<VmSaleItem> SaleItems
        {
            get { return _saleItems; }
            set { _saleItems = value; NotifyPropertyChanged(); }
        }
        private ObservableCollection<VmSaleItem> _saleItems = new ObservableCollection<VmSaleItem>();

        public bool IsEnableInput
        {
            get
            {
                var thisMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                var prevMonth = thisMonth.AddMonths(-1);
                if (mSalesReprt == null) return false;
                bool isEnable = mSalesReprt.TargetMonth < prevMonth ? false :
                                 (mSalesReprt.TargetMonth >= thisMonth) | ((mSalesReprt.TargetMonth == prevMonth) && DateTime.Today <= new DateTime(DateTime.Today.Year, DateTime.Today.Month, mCanUpdatePrevMonthDay));
                return isEnable;
            }
        }



        /// <summary>
        /// 選択中のItem
        /// </summary>
        public VmSaleItem SelectedItem
        {
            get { return _selectedItem; }
            set { _selectedItem = value; NotifyPropertyChanged(); }
        }
        private VmSaleItem _selectedItem = new VmSaleItem(new MukaiWebService.sal()); //Dummy

        public int RowSpan
        {
            get { return (IsAndroid) ? 8 : 5; }
        }

        #endregion

        #region 外部変数
        #endregion

        #region 内部変数
        private int mCanUpdatePrevMonthDay;
        private SalesReport mSalesReprt;
        private DateTime mTargetDate;
        #endregion

        #region ライフサイクル
        public VmSaleReport(DateTime target)
        {
            Header.OnBackButton_Clicked += BackBtn_Clicked;
            Header.Parent = this;
            mTargetDate = target;
        }
        public VmSaleReport() : this(new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1)) { }


        public override async Task OnInit(object param = null)
        {
            using (var bussy = new IsBussyHolder(this))
            {
                await bussy.Set();
                //税抜き or 税込みの判定
                var store = AppData.LoginStore;
                int incTax;
                if (int.TryParse(store.opt_1, out incTax) == false) incTax = 0;
                IsIncludeTax = (incTax == 1);

                //前月データの変更可能日取得
                IDepend dep = DependencyService.Get<IDepend>();
                mCanUpdatePrevMonthDay = await dep.GetSalUpdateDays();

                //報告データをロード
                mSalesReprt = new SalesReport(mTargetDate);
                await mSalesReprt.LoadLocalData();

                //報告リストを追加
                updateSals();

                //アイテム選択を有効にする
                NotifyPropertyChanged(nameof(IsEnableInput));

            }
        }

        #endregion

        #region ユーザ操作
        /// <summary>
        /// 戻る
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BackBtn_Clicked(object sender, EventArgs e)
        {
            using (var bussy = new IsBussyHolder(this))
            {
                if (await bussy.Set() == false) return;
                await Navigator.PopModalAsync();
            }
        }

        /// <summary>
        /// 日報の選択
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void SaleItem_Select(object sender, EventArgs e)
        {
            using (var bussy = new IsBussyHolder(this))
            {
                if (await bussy.Set() == false) return;

                VmSaleItem tmp = (VmSaleItem)sender;
                var input = mSalesReprt.GetInitSal(tmp.Sal.ymd);
                input.acc_tax = tmp.Sal.acc_tax;
                input.daysale = tmp.Sal.daysale;
                input.sumsale = tmp.Sal.sumsale;
                input.holiday = tmp.Sal.holiday;
                input.valid = tmp.Sal.valid;

                //SALをコピーしてダイアログを表示する。
                SelectedItem = new VmSaleItem(input);
                SelectedItem.OnAccepted += SaleItem_Accept;
                SelectedItem.OnDeleted += SaleItem_Delete;
                SelectedItem.OnCanceled += SaleItem_Cancel;
                SelectedItem.OnCanceled1 += SaleItem_Cancel1;

                SelectedItem.IsInputDialogVisible = true;
            }
        }
        /// <summary>
        /// 日報の削除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void SaleItem_Delete(object sender, EventArgs e)
        {
            using (var bussy = new IsBussyHolder(this))
            {
                if (await bussy.Set() == false) return;
                VmSaleItem tmp = (VmSaleItem)sender;
                tmp.Daysale = 0;
                tmp.IsHoliday = false;
                tmp.IsValid = true;
                tmp.IsInputDialogVisible = false;
                await mSalesReprt.Add(tmp.Sal);
                updateSals();
            }
        }

        /// <summary>
        /// 日報の確定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void SaleItem_Accept(object sender, EventArgs e)
        {
            using (var bussy = new IsBussyHolder(this))
            {
                if (await bussy.Set() == false) return;
                VmSaleItem tmp = (VmSaleItem)sender;
                //休みなら売上0に
                if (tmp.IsHoliday)
                {
                    tmp.Daysale = 0;
                    tmp.IsValid = false;
                }
                tmp.IsValid = false;
                tmp.IsInputDialogVisible = false;

                await mSalesReprt.Add(tmp.Sal);

                updateSals();

            }
        }
        /// <summary>
        /// 日報のキャンセル
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void SaleItem_Cancel(object sender, EventArgs e)
        {
            using (var bussy = new IsBussyHolder(this))
            {
                if (await bussy.Set() == false) return;   
                VmSaleItem tmp = (VmSaleItem)sender;
                tmp.IsInputDialogVisible = false;

            }
        }

        /// <summary>
        /// 日報のキャンセル
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaleItem_Cancel1(object sender, EventArgs e)
        {
            return;
        }

        /// <summary>
        /// 次へ
        /// </summary>

        public DelegateCommand CommandNext
        {
            get { return _commandNext = _commandNext ?? new DelegateCommand(() => { Next_Clicked(this, new EventArgs()); }); }
        }
        private DelegateCommand _commandNext;
        private async void Next_Clicked(object sender, EventArgs e)
        {
            using (var bussy = new IsBussyHolder(this))
            {
                if (await bussy.Set() == false) return;

                DateTime prevTime = mSalesReprt.TargetMonth.AddMonths(1);
                VmBase vm = new VmSaleReport(prevTime);
                Page page = new PageSaleReport(vm);
                await Navigator.PopAndPush(page, false);
            }
        }

        /// <summary>
        /// 前へ
        /// </summary>

        public DelegateCommand CommandPrev
        {
            get { return _commandPrev = _commandPrev ?? new DelegateCommand(() => { Prev_Clicked(this, new EventArgs()); }); }
        }
        private DelegateCommand _commandPrev;
        private async void Prev_Clicked(object sender, EventArgs e)
        {
            using (var bussy = new IsBussyHolder(this))
            {
                if (await bussy.Set() == false) return;

                DateTime prevTime = mSalesReprt.TargetMonth.AddMonths(-1);
                VmBase vm = new VmSaleReport(prevTime);
                Page page = new PageSaleReport(vm);
                await Navigator.PopAndPush(page, false);
            }
        }

        /// <summary>
        /// 本部送信
        /// </summary>

        public DelegateCommand CommandSend
        {
            get { return _commandSend = _commandSend ?? new DelegateCommand(() => { Send_Clicked(this, new EventArgs()); }); }
        }
        private DelegateCommand _commandSend;
        private async void Send_Clicked(object sender, EventArgs e)
        {
            using (var bussy = new IsBussyHolder(this))
            {
                if (await bussy.Set() == false) return;

                if(Connectivity.NetworkAccess != NetworkAccess.Internet)
                {
                    await Navigator.CurrentPage.DisplayAlert("", "インターネットに接続されていません。", "OK");
                    return;
                }

                //入力がなければ終了
                if (mSalesReprt.Sales.Where(sal => sal.valid == 0).Count() == 0) return;
                /*
               foreach(var item in mSalesReprt.Sales)
                {
                    Logger.Inst.WriteLine($"ymd:{item.ymd.ToString()} valid:{item.valid} holiday:{item.holiday} daysale:{item.daysale} sumsale:{item.sumsale} ");
                }
                */

                IDepend dep = DependencyService.Get<IDepend>();
                bool result = await dep.SetSal(mSalesReprt.Sales ,AppData.UserAuthCode);
                if(result == false)
                {
                    await Navigator.CurrentPage.DisplayAlert("", "送信に失敗しました。", "OK");
                }
                else
                {
                    await Navigator.CurrentPage.DisplayAlert("", "送信が完了しました。", "OK");
                }
            }
        }

        #endregion

        #region 外部関数
        #endregion

        #region 内部関数
        private void updateSals()
        {
            SaleItems.Clear();
            foreach (var item in mSalesReprt.Sales)
            {
                var salItem = new VmSaleItem(item);
                salItem.OnSelected += SaleItem_Select;
                SaleItems.Add(salItem);
            }
        }

        #endregion
    }


    /// <summary>
    /// 日報アイテム
    /// </summary>
    public class VmSaleItem : VmBase
    {
        #region イベント
        /// <summary>
        /// クリックされた時に呼び出す
        /// </summary>
        public event EventHandler OnSelected;
        public event EventHandler OnAccepted;
        public event EventHandler OnDeleted;
        public event EventHandler OnCanceled;
        public event EventHandler OnCanceled1;

        #endregion

        #region プロパティ
        public string Ymd
        {
            get { return Sal.ymd.ToString("yyyy/MM/dd"); }
        }

        public decimal Daysale
        {
            get { return Sal.daysale; }
            set { Sal.daysale = value; NotifyPropertyChanged(); }
        }

        public decimal SumSale
        {
            get { return Sal.sumsale; }
            set { Sal.sumsale = value; NotifyPropertyChanged(); }
        }

        public bool IsHoliday
        {
            get { return (Sal.holiday != 0); }
            set
            {
                Sal.holiday = (value) ? 1 : 0;
                if (value) Daysale = 0;
                NotifyPropertyChanged();
            }
        }

        public bool IsDispValue
        {
            get { return (IsHoliday == false && IsValid == false); }
        }

        public bool IsValid
        {
            get { return (Sal.valid != 0); }
            set { Sal.valid = (value) ? 1 : 0; NotifyPropertyChanged(); }
        }

        public bool IsInputDialogVisible
        {
            get { return _isInputDialogVisible; }
            set { _isInputDialogVisible = value; NotifyPropertyChanged(); }
        }
        private bool _isInputDialogVisible = false;


        #endregion

        #region 外部変数
        public MukaiWebService.sal Sal;
        #endregion

        #region 内部変数
        #endregion

        #region ライフサイクル
        public VmSaleItem(MukaiWebService.sal sal)
        {
            Sal = sal;
        }
        #endregion

        #region ユーザ操作
        /// <summary>
        /// 一覧から選択
        /// </summary>
        public DelegateCommand CommandSelect
        {
            get { return _commandSelect = _commandSelect ?? new DelegateCommand(() => { OnSelected?.Invoke(this, new EventArgs()); }); }
        }
        private DelegateCommand _commandSelect;

        /// <summary>
        /// 確定
        /// </summary>
        public DelegateCommand CommandAccept
        {
            get { return _commandAccept = _commandAccept ?? new DelegateCommand(() => { OnAccepted?.Invoke(this, new EventArgs()); }); }
        }
        private DelegateCommand _commandAccept;

        /// <summary>
        /// キャンセル
        /// </summary>
        public DelegateCommand CommandCancel
        {
            get { return _commandCancel = _commandCancel ?? new DelegateCommand(() => { OnCanceled?.Invoke(this, new EventArgs()); }); }
        }
        private DelegateCommand _commandCancel;


        /// <summary>
        /// キャンセル
        /// </summary>
        public DelegateCommand CommandCancel1
        {
            get { return _commandCancel1 = _commandCancel1 ?? new DelegateCommand(() => { OnCanceled1?.Invoke(this, new EventArgs()); }); }
        }
        private DelegateCommand _commandCancel1;

        /// <summary>
        /// 削除
        /// </summary>
        public DelegateCommand CommandDelete
        {
            get { return _commandDelete = _commandDelete ?? new DelegateCommand(() => { OnDeleted?.Invoke(this, new EventArgs()); }); }
        }
        private DelegateCommand _commandDelete;

        #endregion

        #region 外部関数
        #endregion

        #region 内部関数
        #endregion
    }


}
