using MukaiTablet2.Model;
using MukaiTablet2.MukaiWebService;
using MukaiTablet2.Util;
using MukaiTablet2.View;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MukaiTablet2.ViewModel.Common
{
    public class VmProduct : VmBase
    {
        #region イベント
        public event EventHandler OnClicked;
        public event EventHandler OnOrderNumChanged;
        #endregion

        #region プロパティ
        public VmHeader Header
        {
            get { return _header; }
            set { _header = value; NotifyPropertyChanged(); }
        }
        private VmHeader _header = new VmHeader("詳細", false, true);

        public bool IsEnd
        {
            get { return (mGoods.isend); }
        }
        public bool IsOos
        {
            get { return (mGoods.isoos); }
        }
        public bool IsLim
        {
            get { return (mGoods.islim); }
        }
        public bool IsAcc
        {
            get { return (mGoods.isacc); }
        }

        public bool IsNew
        {
            get { return (mGoods.isnew); }
        }
        public bool IsOrderOk
        {
            get { return (mGoods.isorderOK); }
        }
        public bool IsBookmark
        {
            get { return mGoods.isbookmark; }
        }
        public ImageSource Image
        {
            get
            {
                ImageSource ret = null;
                IDepend dep = DependencyService.Get<IDepend>();
                string path = dep.GetLocalStoragePath() + mGoods.image_name;

                //ローカルにファイルがなくネットワークに接続されていればダウンロードする。
                if (File.Exists(path) == false && Connectivity.NetworkAccess == NetworkAccess.Internet)
                {
                    Task.Run(() =>
                    {
                        try
                        {
                            using (WebClient wc = new WebClient())
                            {
                                wc.DownloadFile(mGoods.image_serveruri, path);
                            }
                        }
                        catch (Exception e)
                        {
                            Logger.Inst.WriteLine($"dl error {mGoods.image_serveruri}: {e.Message} ");
                        }
                    });
                    return mGoods.image_serveruri;
                }
                //ファイルがあれば採用
                if (File.Exists(path))
                {
                    try
                    {
                        ret = ImageSource.FromFile(path);
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine(e);
                        return mGoods.image_serveruri;
                    }
                }
                return ret;
            }
        }

        public string DpGname1
        {
            get { return mGoods.dp_gname1; }
        }

        public string DpGname2
        {
            get { return mGoods.dp_gname2; }
        }
        public string MakerNo
        {
            get { return mGoods.p.makerno; }
        }

        public string UreMark
        {
            get { return mGoods.p.uremark; }
        }

        public decimal Upprice
        {
            get { return mGoods.p.upprice; }
        }
        public string AvgString
        {
            get { return (mGoods.res != null) ? mGoods.res.avg_string : ""; }
        }

        public bool IsOrder
        {
            get { return mGoods.isorder; }
        }
        public string ReqQty
        {
            get { return mGoods.req.qty.ToString(); }
        }
        public int Vecode
        {
            get { return mGoods.p.vecode; }
        }

        public List<int> OrderNumList
        {
            get { return _orderNumList; }
            set { _orderNumList = value; NotifyPropertyChanged(); }
        }
        private List<int> _orderNumList = new List<int>();

        public int OrderNum
        {
            get { return _orderNum; }
            set
            {
                _orderNum = value;
                string[] notifies = { nameof(OrderNum), nameof(TotalPrice), nameof(HasOrder) };
                NotifyPropertyChanged(notifies);
            }
        }
        private int _orderNum;

        public int OrderedNum
        {
            get { return _orderedNum; }
            set
            {
                _orderedNum = value;
                string[] notifies = { nameof(OrderedNum), nameof(TotalPrice), nameof(HasOrder) };
                NotifyPropertyChanged(notifies);
            }
        }
        private int _orderedNum;


        public bool HasOrder
        {
            get { return (0 < OrderedNum); }
        }


        public int TotalPrice
        {
            get { return (int)((decimal)OrderedNum * Upprice); }
        }

        public string EndYmd
        {
            get
            {
                if (mGoods.end == null) return string.Empty;
                string s = mGoods.end.ymd;
                if (s == null) return string.Empty;
                return s.Substring(0, 4) + "/" + s.Substring(4, 2) + "/" + s.Substring(6, 2);
            }
        }

        public string OosYmd
        {
            get
            {
                if (mGoods.oos == null) return string.Empty;
                string s = mGoods.oos.ymd;
                if (s == null) return string.Empty;
                return s.Substring(0, 4) + "/" + s.Substring(4, 2) + "/" + s.Substring(6, 2);
            }
        }

        public string AccYmd
        {
            get
            {
                if (mGoods.acc == null) return string.Empty;
                string s = mGoods.acc.ymd;
                if (s == null) return string.Empty;
                return s.Substring(0, 4) + "/" + s.Substring(4, 2) + "/" + s.Substring(6, 2);
            }
        }

        public int AccQty
        {
            get { return (int)((mGoods.acc != null) ? mGoods.acc.qty : 0.0m); }
        }

        public int AccNosNum
        {
            get { return (int)((mGoods.acc != null) ? mGoods.acc.nos_num : 0.0m); }
        }

        public string Gcode
        {
            get { return mGoods.gcode; }
        }

        public int GSeqno
        {
            get { return mGoods.g_seqno; }
        }

        public VmBase ParentVm { get; set; } = null;

        #endregion

        #region 公開変数
        #endregion

        #region 内部変数
        public goods mGoods;
        #endregion

        #region ライフサイクル

        public VmProduct(goods goodsPrm)
        {
            mGoods = goodsPrm;
            Header.OnBackButton_Clicked += BtnBack_Clicked;
            Header.Parent = this;

            List<int> orderNumList = new List<int>();
            int spq = (int)(goodsPrm.p.ship_ent == 0 ? 1 : goodsPrm.p.ship_ent);
            for (int ii = 1; ii <= 10; ii++)
            {
                orderNumList.Add(ii * spq);
            }
            OrderNumList = orderNumList;

            OrderNum = OrderNumList.First();

        }
        public VmProduct() { }
        #endregion

        #region ユーザ操作
        /// <summary>
        /// 商品選択ボタン(ITEM表示時に使用)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        public DelegateCommand CommandSelect
        {
            get { return _commandSelect = _commandSelect ?? new DelegateCommand(BtnSelect_Clicked); }
        }
        private DelegateCommand _commandSelect;

        private void BtnSelect_Clicked()
        {
            OnClicked?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// 注文リスト登録ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public DelegateCommand CommandOrder
        {
            get { return _commandOrder = _commandOrder ?? new DelegateCommand(ButtonOrder_Clicked); }
        }
        private DelegateCommand _commandOrder;
        private async void ButtonOrder_Clicked()
        {
            using (var bussy = new IsBussyHolder(this))
            {
                if (await bussy.Set() == false) return;

                await updateOrderNum(OrderNum);

                //発注履歴以外は、発注カート登録済数を更新
                if ((ParentVm is VmOrderHistoryItem) == false)
                {
                    OrderedNum = OrderNum;
                }

                await Navigator.PopModalAsync();
            }
        }

        /// <summary>
        /// 戻るボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnBack_Clicked(object sender, EventArgs e)
        {
            using (var bussy = new IsBussyHolder(this))
            {
                if (await bussy.Set() == false) return;

                await Navigator.PopModalAsync();
            }
        }


        /// <summary>
        /// 注文－ボタン
        /// </summary>
        public DelegateCommand CommandMinus
        {
            get { return _commandMinus = _commandMinus ?? new DelegateCommand(BtnMinus_Clicked); }
        }
        private DelegateCommand _commandMinus;

        private async void BtnMinus_Clicked()
        {
            using (var bussy = new IsBussyHolder(this))
            {
                if (await bussy.Set() == false) return;
                if (1 < OrderedNum)
                {
                    OrderedNum--;
                    await updateOrderNum(OrderedNum);
                    OnOrderNumChanged?.Invoke(this, new EventArgs());
                }
            }
        }
        /// <summary>
        /// 注文＋ボタン
        /// </summary>
        public DelegateCommand CommandPlus
        {
            get { return _commandPlus = _commandPlus ?? new DelegateCommand(BtnPlus_Clicked); }
        }
        private DelegateCommand _commandPlus;

        private async void BtnPlus_Clicked()
        {
            using (var bussy = new IsBussyHolder(this))
            {
                if (await bussy.Set() == false) return;
                OrderedNum++;
                await updateOrderNum(OrderedNum);
                OnOrderNumChanged?.Invoke(this, new EventArgs());
            }
        }
        /// <summary>
        /// 注文削除ボタン
        /// </summary>
        public DelegateCommand CommandDelete
        {
            get { return _commandDelete = _commandDelete ?? new DelegateCommand(OrderDetele_Click); }
        }
        private DelegateCommand _commandDelete;

        private async void OrderDetele_Click()
        {
            var req = Order.LocalArrayReq.Where((item) =>
            {
                if (mGoods.g_seqno == item.goods.g_seqno) return true;
                return false;
            }).FirstOrDefault();

            //注文削除
            AppData.SetGoodsOrderInit(req);
            await Order.Delete(req);

            //注文数=0は注文なしを意味する
            OrderedNum = 0;

            OnOrderNumChanged?.Invoke(this, new EventArgs());
        }


        /// <summary>
        /// 気に入りボタン
        /// </summary>
        public DelegateCommand CommandFavorite
        {
            get { return _commandFavorite = _commandFavorite ?? new DelegateCommand(Favorite_Click); }
        }
        private DelegateCommand _commandFavorite;

        private async void Favorite_Click()
        {
            using (var bussy = new IsBussyHolder(this))
            {
                if (await bussy.Set() == false) return;

            
                // 未調整フォルダに保存
                AppData.SaveFavoriteProduct( favoriteProduct: new FavoriteProduct()
                {
                    ProductId = FavoriteProduct.NewProductId,
                    Upprice = mGoods.p.upprice,
                    DpGname1 = mGoods.dp_gname1,
                    GCode = mGoods.gcode,
                    GSeqno = mGoods.g_seqno,
                    MakerNo = mGoods.p.makerno,
                    Vecode = mGoods.p.vecode,
                    image_serveruri = mGoods.image_serveruri,
                    image_name = mGoods.image_name
                }, folderId: string.Empty);

                var pageIndex = (PageIndex)Application.Current.MainPage.Navigation.ModalStack.Where( p => p.BindingContext is VmIndex).FirstOrDefault();
                VmIndex vmIndex = (VmIndex)pageIndex.BindingContext;
                vmIndex.updateFavoriteProduct();
            }
        }

        #endregion

        #region 内部関数
        private async Task updateOrderNum(int orderNum)
        {
            var req = new MukaiWebService.req();
            req.mkk = AppData.LastLoginShopMark.Substring(0, 1);
            req.shopcode = int.Parse(AppData.LastLoginShopMark.Substring(1));
            req.vid = 0;
            req.ymd = DateTime.Now;
            req.pcname = new System.Net.NetworkCredential().Domain;
            req.h_opt1 = "";
            req.h_opt2 = AppData.AppVersion;
            req.bkk = 1;
            req.gcode1 = mGoods.p.gcode1;
            req.gcode2 = mGoods.c.gcode2;
            req.qty = orderNum;

            req.opt1 = "";
            req.opt2 = "\"" + DateTime.Now.ToString() + "\"";

            req.goods = AppData.GetGoodsClone(mGoods);
            await Order.Add(req);

            AppData.SetGoodsOrder(req);
        }

        #endregion
    }
}
