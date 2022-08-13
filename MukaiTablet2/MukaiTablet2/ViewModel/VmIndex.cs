using MukaiTablet2.Model;
using MukaiTablet2.MukaiWebService;
using MukaiTablet2.Util;
using MukaiTablet2.View;
using MukaiTablet2.ViewModel.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Color = Xamarin.Forms.Color;

namespace MukaiTablet2.ViewModel
{
    /// <summary>
    /// 編集台帳のIndexVM
    /// </summary>
    class VmIndex : VmBase
    {
        #region イベント
        public event EventHandler NotifyIndexUpdated;
        public event EventHandler NotifyProductsUpdated;
        #endregion

        #region プロパティ
        public VmHeader Header
        {
            get { return _header; }
            set { _header = value; NotifyPropertyChanged(); }
        }
        private VmHeader _header = new VmHeader("台帳", true, true);

        public bool IsIndexVisible
        {
            get { return _isIndexVisible; }
            set
            {
                _isIndexVisible = value;
                string[] notifies = { nameof(IsIndexVisible), nameof(IsProductVisible) };
                NotifyPropertyChanged(notifies);
            }
        }
        public bool _isIndexVisible = true;

        public bool IsProductVisible
        {
            get { return (_isIndexVisible == false); }
        }

        /// <summary>
        /// パンくずリスト
        /// </summary>
        public ObservableCollection<VmBreadcrumbItem> Breadcrumb
        {
            get { return _breadcrumb; }
            set { _breadcrumb = value; NotifyPropertyChanged(); }
        }
        private ObservableCollection<VmBreadcrumbItem> _breadcrumb = new ObservableCollection<VmBreadcrumbItem>();

        //public double BreadcrumbWidth
        //{
        //    get
        //    {
        //        if (mPageWidth == 0) return 55; //デフォルト
        //        return Math.Floor((mPageWidth - 20) / 5);
        //    }
        //}

        /// <summary>
        /// グループリスト(INDEX)
        /// </summary>
        public ObservableCollection<VmIndexItem> IndexList
        {
            get { return _indexList; }
            set { _indexList = value; NotifyPropertyChanged(); }
        }
        private ObservableCollection<VmIndexItem> _indexList = new ObservableCollection<VmIndexItem>();

        /// <summary>
        /// 商品リスト
        /// </summary>
        public ObservableCollection<VmProduct> ProductList
        {
            get { return _productList; }
            set { _productList = value; NotifyPropertyChanged(); }
        }
        private ObservableCollection<VmProduct> _productList = new ObservableCollection<VmProduct>();

        public int IndexColNum
        {
            get {
                double breadcrumbSize = (IsDispBreadcrumb) ? 140 : 40;
                if (mPageWidth == 0) return 1; //デフォルト
                int num = (int)Math.Floor((mPageWidth- breadcrumbSize) / 220.0);
                if (num <= 0) num = 1;
                return num;
            }
        }
        public int ProductColNum
        {
            get
            {
                double breadcrumbSize = (IsDispBreadcrumb) ? 120 : 40;
                if (mPageWidth == 0) return 1; //デフォルト
                int num = (int)Math.Floor((mPageWidth / breadcrumbSize));
                if (num <= 0) num = 1;
                return num;
            }
        }
        public double ProductListWidth
        {
            get { return (240.0 + 5) * ProductColNum; }
        }

        public int ProductRowNum
        {
            get
            {
                if (mPageHeight == 0) return 1; //デフォルト
                int num = (int)Math.Floor((mPageHeight - 50) / 245.0);
                if (num <= 0) num = 1;
                return num;
            }
        }

        public bool IsDispBreadcrumb
        {
            get { return _isDispBreadcrumb; }
            set { 
                _isDispBreadcrumb = value;
                string[] notifies = { nameof(ProductColNum),nameof(ProductListWidth), nameof(IndexColNum) ,nameof(IsDispBreadcrumb) };
                NotifyPropertyChanged(notifies); 
            }
        }
        private bool _isDispBreadcrumb = true;

        #endregion
        #region 内部変数
        #endregion

        #region ライフサイクル

        public VmIndex()
        {
            //バックボタン
            Header.OnBackButton_Clicked += BtnBack_Clicked;

            //検索ボタン
            Header.OnSeachButton_Clicked += BtnSearch_Clicked;
            Header.Parent = this;

        }
        public override async Task OnInit(Object param)
        {
            Breadcrumb.Add(new VmBreadcrumbItem("TOP", -1, false, "#FFFFFF"));
            updateIndex();
            await Task.Delay(0);
        }

        /// <summary>
        /// Viewのページサイズが変わった
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public override void OnPageSizeChanged(double width, double height)
        {
            base.OnPageSizeChanged(width, height);

            string[] notifies = { nameof(IndexColNum), nameof(ProductColNum), nameof(ProductListWidth), nameof(ProductRowNum)};
            NotifyPropertyChanged(notifies);
            
        }

        #endregion

        #region ボタン処理

        /// <summary>
        /// インデックスボックスクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public async void IndexItem_Clicked(object sender, EventArgs e)
        {
            using (var bussy = new IsBussyHolder(this))
            {
                if (await bussy.Set() == false) return;

                VmIndexItem indexItem = (VmIndexItem)sender;
                if (indexItem.Index.joinm_c_id == "PT")
                {
                    //商品リスト
                    Breadcrumb.Add(new VmBreadcrumbItem(indexItem.IdxName, indexItem.Index.joinm_c_seqno, true, indexItem.Index.idx_bcolor));
                }
                else
                {
                    //INDEXリスト
                    Breadcrumb.Add(new VmBreadcrumbItem(indexItem.IdxName, indexItem.Index.seqno, false, indexItem.Index.idx_bcolor));
                }
                //表示更新
                updateDisplay();
            }
        }

        /// <summary>
        /// パンくずクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //public async void LvBreadcrumb_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        //{
        //    using (var bussy = new IsBussyHolder(this))
        //    {
        //        if (await bussy.Set() == false) return;
        //        //最後のパンくずであればRETURN
        //        if ((e.SelectedItemIndex + 1) == Breadcrumb.Count) return;
        //        //選択されたINDEXまでを削除する。
        //        while ((e.SelectedItemIndex + 1) < Breadcrumb.Count)
        //        {
        //            Breadcrumb.RemoveAt(Breadcrumb.Count - 1);
        //        }
        //        //表示更新
        //        updateIndex();
        //    }
        //}
        public Command LvBreadcrumb_ItemSelected => new Command(async(obj) =>
        {
            if (obj is null)
                return;
            var item = Breadcrumb.Where(w => w.Name == obj.ToString()).LastOrDefault();
            if (item is null)
                return;
            int selectedItemIndex = Breadcrumb.IndexOf(item);
            using (var bussy = new IsBussyHolder(this))
            {
                if (await bussy.Set() == false) return;
                //最後のパンくずであればRETURN
                if ((selectedItemIndex + 1) == Breadcrumb.Count) return;
                //選択されたINDEXまでを削除する。
                while ((selectedItemIndex + 1) < Breadcrumb.Count)
                {
                    Breadcrumb.RemoveAt(Breadcrumb.Count - 1);
                }
                //表示更新
                updateIndex();
            }
        });

        /// <summary>
        /// 戻るボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnBack_Clicked(object sender, EventArgs e)
        {
#if false
            if (false/*1 < Breadcrumb.Count*/)
            {
                //最終要素を削除
                Breadcrumb.RemoveAt(Breadcrumb.Count - 1);
                //表示更新
                updateDisplay();
            }
            else
#endif
            using (var bussy = new IsBussyHolder(this))
            {
                if (await bussy.Set() == false) return;
                {
                    await Navigator.PopModalAsync();
                }
            }
        }

        /// <summary>
        /// 検索ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnSearch_Clicked(object sender, EventArgs e)
        {
            using (var bussy = new IsBussyHolder(this))
            {
                if (await bussy.Set() == false) return;

                VmBase vm = new VmSearch();
                Page page = new PageSearch(vm);
                await Navigator.PushModalAsync(page);
            }
        }

        /// <summary>
        /// 商品クリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public async void ProductItemClicked(object sender, EventArgs e)
        {
            using (var bussy = new IsBussyHolder(this))
            {
                if (await bussy.Set() == false) return;
                VmProduct item = (VmProduct)sender;
                Page page = new PageProduct(item);
                await Navigator.PushModalAsync(page);
            }
        }

        /// <summary>
        /// パンくず表示
        /// </summary>
        public DelegateCommand CommadDispBreadcrumb
        {
            get { return _commadDispBreadcrumb = _commadDispBreadcrumb ?? new DelegateCommand(()=> {
                IsDispBreadcrumb = (!IsDispBreadcrumb); //反転
            } );
            }
        }
        private DelegateCommand _commadDispBreadcrumb;

        

        /// <summary>
        /// デバッグボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void DEBUG_Clicked(object sender, EventArgs e)
        {
            updateDisplay();
        }

        #endregion

        #region 内部処理
        private void updateDisplay()
        {
            if (Breadcrumb.LastOrDefault().IsProduct)
            {
                updateProducts();
            }
            else
            {
                updateIndex();
            }


        }
        private void updateIndex()
        {
            int parentSeq = Breadcrumb.LastOrDefault().SeqNo;

            var linqIndex = from ii in AppData.LocalarrayIndex where ii.joinm_p_seqno == parentSeq orderby ii.joinm_c_line select ii;
            if (parentSeq == -1)
            {
                Header.Title = "TOP";
            }
            else
            {
                string s = (from i in AppData.LocalarrayIndex where i.seqno == parentSeq select i.idx_name).Single();
                Header.Title = s;
            }

            ProductList.Clear();
            IndexList.Clear();
            foreach (var item in linqIndex)
            {
                VmIndexItem indexItem = new VmIndexItem();
                indexItem.IdxName = item.idx_name;
                indexItem.TagColor = Xamarin.Forms.Color.FromHex(item.idx_bcolor);
                indexItem.Index = item;
                indexItem.NotifyIndexClicked += IndexItem_Clicked;
                IndexList.Add(indexItem);
            }
            IsIndexVisible = true;
            //Viewに通知
            NotifyIndexUpdated?.Invoke(this, new EventArgs());
        }

        void updateProducts()
        {
            int parentSeq = Breadcrumb.LastOrDefault().SeqNo;
            var linqgoods = (from g in AppData.LocalarrayGoods
                             join j in AppData.LocalarrayJoin on g.p.seqno equals j.c_seqno
                             where j.p_seqno == parentSeq
                             orderby j.c_line
                             select g);

            var linqindex = from i in AppData.LocalarrayIndex
                            where i.seqno == parentSeq
                            select i;
            Header.Title = linqindex.FirstOrDefault().idx_name;

            ProductList.Clear();
            foreach (var item in linqgoods)
            {
                var vm = new VmProduct(item);
                vm.Parent = this;
                vm.OnClicked += ProductItemClicked;
                ProductList.Add(vm);
            }
            IsIndexVisible = false;
            //Viewに通知
            NotifyProductsUpdated?.Invoke(this, new EventArgs());
        }

        public void updateFavoriteProduct()
        {
            ProductList = new ObservableCollection<VmProduct>(ProductList);
        }
        #endregion

    }
    /// <summary>
    /// パンくずItem
    /// </summary>
    public class VmBreadcrumbItem : VmBase
    {
        public string Name
        {
            get { return _name; }
            set { _name = value; NotifyPropertyChanged(); }
        }
        private string _name = string.Empty;

        public Xamarin.Forms.Color BgColor
        {
            get { return _bgColor; }
            set { _bgColor = value; NotifyPropertyChanged(); }
        }
        private Xamarin.Forms.Color _bgColor;

        public readonly int SeqNo;

        public readonly bool IsProduct;


        public VmBreadcrumbItem(string name, int seqNo, bool isProduct, string colorStr)
        {
            Name = name;
            SeqNo = seqNo;
            IsProduct = isProduct;
            BgColor = Xamarin.Forms.Color.FromHex(colorStr);
        }
    }

    /// <summary>
    /// インデックスボックス
    /// </summary>
    public class VmIndexItem
    {
        public string IdxName { get; set; }
        public Xamarin.Forms.Color TagColor { get; set; }

        public MukaiTablet2.MukaiWebService.index Index;

        public void Clicked(object sender, EventArgs e)
        {
            NotifyIndexClicked?.Invoke(this, e);
        }

        public event EventHandler NotifyIndexClicked;

        public DelegateCommand Command
        {
            get
            {
                return _command = _command ?? new DelegateCommand(() =>
                {
                    NotifyIndexClicked?.Invoke(this, new EventArgs());
                });
            }
        }
        private DelegateCommand _command;

    }







}
