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
using Xamarin.Forms;

namespace MukaiTablet2.ViewModel
{
    class VmSearch : VmBase
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
        private VmHeader _header = new VmHeader("こだわり検索", useSearchButton: false, useBackButton: true);

        public bool IsConditionVisible
        {
            get { return _isConditionVisible; }
            set
            {
                _isConditionVisible = value;
                string[] properties = { "ConditionExpandMark", "IsConditionVisible", "IsResultVisible", "ResultExpandMark" };
                NotifyPropertyChanged(properties);
            }
        }
        private bool _isConditionVisible = true;

        public string ConditionExpandMark
        {
            get
            {
                if (IsConditionVisible) return "⊟";
                else return "⊞";
            }
        }
        public bool IsResultVisible
        {
            get { return (_isConditionVisible == false); }
            set
            {
                _isConditionVisible = (value == false);
                string[] properties = { "ConditionExpandMark", "IsConditionVisible", "IsResultVisible", "ResultExpandMark" };
                NotifyPropertyChanged(properties);
            }
        }

        public string ResultExpandMark
        {
            get
            {
                if (IsConditionVisible == false) return "⊟";
                else return "⊞";
            }
        }


        public string Keyword
        {
            get { return _keyword; }
            set { _keyword = value; NotifyPropertyChanged(); }
        }
        private string _keyword = null;

        public int? PriceLowerLimit
        {
            get { return _priceLowerLimit; }
            set { _priceLowerLimit = value; NotifyPropertyChanged(); }
        }
        private int? _priceLowerLimit;

        public int? PriceUpperLimit
        {
            get { return _priceUpperLimit; }
            set { _priceUpperLimit = value; NotifyPropertyChanged(); }
        }
        private int? _priceUpperLimit;


        public ObservableCollection<SelectItem> Materials
        {
            get { return _materials; }
            set { _materials = value; NotifyPropertyChanged(); }
        }
        private ObservableCollection<SelectItem> _materials;
        public double MaterialsHeight
        {
            get
            {
                int rowNum = (int)Math.Ceiling((decimal)SelectedMaterials.Count / (decimal)SelectedItemsColNum);
                if (rowNum == 0) rowNum = 1;
                return rowNum * 45;
            }
        }

        public ObservableCollection<string> SelectedMaterials
        {
            get { return _selectedMaterials; }
            set { _selectedMaterials = value; NotifyPropertyChanged(); }
        }
        private ObservableCollection<string> _selectedMaterials = new ObservableCollection<string>();
        public ObservableCollection<SelectItem> Shapes
        {
            get { return _shapes; }
            set { _shapes = value; NotifyPropertyChanged(); }
        }
        private ObservableCollection<SelectItem> _shapes;
        public double ShapesHeight
        {
            get
            {
                int rowNum = (int)Math.Ceiling((decimal)SelectedShapes.Count / (decimal)SelectedItemsColNum);
                if (rowNum == 0) rowNum = 1;
                return rowNum * 45;
            }
        }
        public ObservableCollection<string> SelectedShapes
        {
            get { return _selectedShapes; }
            set { _selectedShapes = value; NotifyPropertyChanged(); }
        }
        private ObservableCollection<string> _selectedShapes = new ObservableCollection<string>();

        public ObservableCollection<SelectItem> SecNames1
        {
            get { return _secNames1; }
            set { _secNames1 = value; NotifyPropertyChanged(); }
        }
        private ObservableCollection<SelectItem> _secNames1;
        public double SecNames1Height
        {
            get
            {
                int rowNum = (int)Math.Ceiling((decimal)SelectedSecNames1.Count / (decimal)SelectedItemsColNum);
                if (rowNum == 0) rowNum = 1;
                return rowNum * 45;
            }
        }
        public ObservableCollection<string> SelectedSecNames1
        {
            get { return _selectedSecNames1; }
            set { _selectedSecNames1 = value; NotifyPropertyChanged(); }
        }
        private ObservableCollection<string> _selectedSecNames1 = new ObservableCollection<string>();

        public ObservableCollection<SelectItem> SecNames2
        {
            get { return _secNames2; }
            set { _secNames2 = value; NotifyPropertyChanged(); }
        }
        private ObservableCollection<SelectItem> _secNames2;
        public double SecNames2Height
        {
            get
            {
                int rowNum = (int)Math.Ceiling((decimal)SelectedSecNames2.Count / (decimal)SelectedItemsColNum);
                if (rowNum == 0) rowNum = 1;
                return rowNum * 45;
            }
        }

        public ObservableCollection<string> SelectedSecNames2
        {
            get { return _selectedSecNames2; }
            set { _selectedSecNames2 = value; NotifyPropertyChanged(); }
        }
        private ObservableCollection<string> _selectedSecNames2 = new ObservableCollection<string>();

        public ObservableCollection<SelectItem> SecNames3
        {
            get { return _secNames3; }
            set { _secNames3 = value; NotifyPropertyChanged(); }
        }
        private ObservableCollection<SelectItem> _secNames3;
        public double SecNames3Height
        {
            get
            {
                int rowNum = (int)Math.Ceiling((decimal)SelectedSecNames3.Count / (decimal)SelectedItemsColNum);
                if (rowNum == 0) rowNum = 1;
                return rowNum * 45;
            }
        }

        public ObservableCollection<string> SelectedSecNames3
        {
            get { return _selectedSecNames3; }
            set { _selectedSecNames3 = value; NotifyPropertyChanged(); }
        }
        private ObservableCollection<string> _selectedSecNames3 = new ObservableCollection<string>();

        public int SelectedItemsColNum
        {
            get
            {
                if (mPageWidth == 0) return 1;
                int num = (int)Math.Floor((mPageWidth - 270) / 120);
                if (num <= 0) num = 1;
                return num;
            }
        }


        public ObservableCollection<VmIndexItem> IndexList
        {
            get { return _indexList; }
            set { _indexList = value; NotifyPropertyChanged(); }
        }
        private ObservableCollection<VmIndexItem> _indexList = new ObservableCollection<VmIndexItem>();


        public ObservableCollection<VmProduct> ProductList
        {
            get { return _productList; }
            set { _productList = value; NotifyPropertyChanged(); }
        }
        private ObservableCollection<VmProduct> _productList = new ObservableCollection<VmProduct>();

        public double IndexListHeight
        {
            get
            {
                return (IndexList.Count == 0) ? 0 : 80;
            }
        }

        //public int ProductColNum
        //{
        //    get
        //    {
        //        if (mProductAreaWidth == 0) return 1;
        //        int num = (int)Math.Floor(mProductAreaWidth / 250);
        //        if (num <= 0) num = 1;
        //        return num;
        //    }
        //}
        public int ProductColNum
        {
            get
            {
                if (mProductAreaWidth == 0) return 1; //デフォルト
                int num = (int)Math.Floor((mProductAreaWidth / 120));
                if (num <= 0) num = 1;
                return num;
            }
        }

        public int ProductRowNum
        {
            get
            {
                if (mProductAreaHeight == 0) return 1;
                int num = (int)Math.Floor((mProductAreaHeight) / 250);
                if (num <= 0) num = 1;
                return num;
            }
        }

        #endregion


        #region 内部変数
        private double mProductAreaHeight = 0;
        private double mProductAreaWidth = 0;
        #endregion


        #region ライフサイクル
        public VmSearch()
        {
            //バックボタン
            Header.OnBackButton_Clicked += BtnBack_Clicked;
            Header.Parent = this;

            Materials = new ObservableCollection<SelectItem>();
            foreach (var name in AppData.LocalarrayMtname)
            {
                SelectItem item = new SelectItem(name);
                Materials.Add(item);
            }

            Shapes = new ObservableCollection<SelectItem>();
            foreach (var name in AppData.LocalarrayDename)
            {
                SelectItem item = new SelectItem(name);
                Shapes.Add(item);
            }

            SecNames1 = new ObservableCollection<SelectItem>();
            foreach (var name in AppData.LocalarraySecName1)
            {
                SelectItem item = new SelectItem(name);
                SecNames1.Add(item);
            }

            SecNames2 = new ObservableCollection<SelectItem>();
            foreach (var name in AppData.LocalarraySecName2)
            {
                SelectItem item = new SelectItem(name);
                SecNames2.Add(item);
            }

            SecNames3 = new ObservableCollection<SelectItem>();
            foreach (var name in AppData.LocalarraySecName3)
            {
                SelectItem item = new SelectItem(name);
                SecNames3.Add(item);
            }
        }

        public override async Task OnAppear(object param = null)
        {
            await base.OnAppear(param);

            using (var bussy = new IsBussyHolder(this))
            {
                await bussy.Set();

                List<string> materials = new List<string>();
                foreach (var item in Materials) { if (item.IsSelected) { materials.Add(item.Name); } }
                SelectedMaterials = new ObservableCollection<string>(materials);
                NotifyPropertyChanged(nameof(MaterialsHeight));


                List<string> shapes = new List<string>();
                foreach (var item in Shapes) { if (item.IsSelected) { shapes.Add(item.Name); } }
                SelectedShapes = new ObservableCollection<string>(shapes);
                NotifyPropertyChanged(nameof(ShapesHeight));

                List<string> secNames1 = new List<string>();
                foreach (var item in SecNames1) { if (item.IsSelected) { secNames1.Add(item.Name); } }
                SelectedSecNames1 = new ObservableCollection<string>(secNames1);
                NotifyPropertyChanged(nameof(SecNames1Height));

                List<string> secNames2 = new List<string>();
                foreach (var item in SecNames2) { if (item.IsSelected) { secNames2.Add(item.Name); } }
                SelectedSecNames2 = new ObservableCollection<string>(secNames2);
                NotifyPropertyChanged(nameof(SecNames2Height));

                List<string> secNames3 = new List<string>();
                foreach (var item in SecNames3) { if (item.IsSelected) { secNames3.Add(item.Name); } }
                SelectedSecNames3 = new ObservableCollection<string>(secNames3);
                NotifyPropertyChanged(nameof(SecNames3Height));
            }
        }

        public void ProductArea_SizeChanged(double width, double height)
        {
            bool isChange = false;
            if (mProductAreaWidth != width)
            {
                mProductAreaWidth = width;
                isChange = true;
            }
            if (mProductAreaHeight != height)
            {
                mProductAreaHeight = height;
                isChange = true;
            }
            if (isChange)
            {
                string[] notifies = { nameof(ProductColNum), nameof(ProductRowNum), nameof(IndexListHeight),nameof(SelectedItemsColNum),
                    nameof(MaterialsHeight),nameof(ShapesHeight),nameof(SecNames1Height),nameof(SecNames2Height),nameof(SecNames3Height) };
                NotifyPropertyChanged(notifies);
            }
        }

        #endregion
        #region ボタン処理
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
        /// 素材
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public DelegateCommand CommandMaterial
        {
            get { return _commandMaterial = _commandMaterial ?? new DelegateCommand(BtnMaterial_Tapped); }
        }
        private DelegateCommand _commandMaterial;

        private async void BtnMaterial_Tapped()
        {
            using (var bussy = new IsBussyHolder(this))
            {
                if (await bussy.Set() == false) return;

                VmBase vm = new VmItemSelect("素材選択", Materials);
                PageBase page = new PageItemSelect(vm);
                await Navigator.PushModalAsync(page);
            }
        }


        /// <summary>
        /// 形状
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public DelegateCommand CommandShape
        {
            get { return _commandShape = _commandShape ?? new DelegateCommand(BtnShape_Tapped); }
        }
        private DelegateCommand _commandShape;
        private async void BtnShape_Tapped()
        {
            using (var bussy = new IsBussyHolder(this))
            {
                if (await bussy.Set() == false) return;
                VmBase vm = new VmItemSelect("デザイン選択", Shapes);
                PageBase page = new PageItemSelect(vm);
                await Navigator.PushModalAsync(page);
            }
        }

        /// <summary>
        /// 個別分類１
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public DelegateCommand CommandSecName1
        {
            get { return _commandSecName1 = _commandSecName1 ?? new DelegateCommand(BtnSecName1_Tapped); }
        }
        private DelegateCommand _commandSecName1;
        private async void BtnSecName1_Tapped()
        {
            using (var bussy = new IsBussyHolder(this))
            {
                if (await bussy.Set() == false) return;

                VmBase vm = new VmItemSelect("個別分類１", SecNames1);
                PageBase page = new PageItemSelect(vm);
                await Navigator.PushModalAsync(page);
            }
        }

        /// <summary>
        /// 個別分類２
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public DelegateCommand CommandSecName2
        {
            get { return _commandSecName2 = _commandSecName2 ?? new DelegateCommand(BtnSecName2_Tapped); }
        }
        private DelegateCommand _commandSecName2;
        private async void BtnSecName2_Tapped()
        {
            using (var bussy = new IsBussyHolder(this))
            {
                if (await bussy.Set() == false) return;

                VmBase vm = new VmItemSelect("個別分類２", SecNames2);
                PageBase page = new PageItemSelect(vm);
                await Navigator.PushModalAsync(page);
            }
        }

        /// <summary>
        /// 個別分類３
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public DelegateCommand CommandSecName3
        {
            get { return _commandSecName3 = _commandSecName3 ?? new DelegateCommand(BtnSecName3_Tapped); }
        }
        private DelegateCommand _commandSecName3;
        private async void BtnSecName3_Tapped()
        {
            using (var bussy = new IsBussyHolder(this))
            {
                if (await bussy.Set() == false) return;

                VmBase vm = new VmItemSelect("個別分類３", SecNames3);
                PageBase page = new PageItemSelect(vm);
                await Navigator.PushModalAsync(page);
            }
        }

        /// <summary>
        /// 検索
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public DelegateCommand CommandSearch
        {
            get { return _commandSearch = _commandSearch ?? new DelegateCommand(BtnSearch_Tapped); }
        }
        private DelegateCommand _commandSearch;
        public async void BtnSearch_Tapped()
        {
            using (var bussy = new IsBussyHolder(this))
            {
                if (await bussy.Set() == false) return;

                ProductList.Clear();
                var result = AppData.LocalarrayGoods
                    .Where(item =>
                    {
                        bool isInput = false;
                        //キーワード
                        if (String.IsNullOrEmpty(Keyword) == false)
                        {
                            isInput = true;
                            //サーチテキストかGnameにHITしたら表示対象にする
                            if (item.p.gname1.Contains(Keyword) == true) { }//HIT
                            else if (item.searchtext.Contains(Keyword) == true) { }//HIT
                            else if (item.p.secname1.Contains(Keyword) == true) { }
                            else if (item.p.secname2.Contains(Keyword) == true) { }
                            else if (item.p.secname3.Contains(Keyword) == true) { }
                            else return false;
                        }

                        //価格
                        if (PriceLowerLimit != null)
                        {
                            isInput = true;
                            if (item.p.upprice < PriceLowerLimit) return false;
                        }
                        if (PriceUpperLimit != null)
                        {
                            isInput = true;
                            if (PriceUpperLimit < item.p.upprice) return false;
                        }


                        //素材
                        if (0 < SelectedMaterials.Count)
                        {
                            isInput = true;
                            if (SelectedMaterials.Contains(item.p.mtname) == false) return false;
                        }

                        //形状
                        if (0 < SelectedShapes.Count)
                        {
                            isInput = true;
                            if (SelectedShapes.Contains(item.p.dename) == false) return false;
                        }
                        //個別分類１
                        if (0 < SelectedSecNames1.Count)
                        {
                            isInput = true;
                            if (SelectedSecNames1.Contains(item.p.secname1) == false) return false;
                        }
                        //個別分類２
                        if (0 < SelectedSecNames2.Count)
                        {
                            isInput = true;
                            if (SelectedSecNames2.Contains(item.p.secname1) == false) return false;
                        }
                        //個別分類３
                        if (0 < SelectedSecNames3.Count)
                        {
                            isInput = true;
                            if (SelectedSecNames3.Contains(item.p.secname1) == false) return false;
                        }

                        return (isInput) ? true : false; //条件がなにも入力されていなければHITさせない
                    });

                foreach (var item in result)
                {
                    var vm = new VmProduct(item);
                    vm.Parent = this;
                    vm.OnClicked += ProductItemClicked;
                    ProductList.Add(vm);
                }

                IndexList.Clear();
                if (string.IsNullOrEmpty(Keyword) == false)
                {
                    var indexResult = AppData.LocalarrayIndex
                        .Where(item =>
                    {
                        if (item.joinm_c_id != "PT") return false;  //最下層以外は対象外
                        if (item.idx_name.Contains(Keyword) == false) return false; //キーワードが含まれてない場合は対象外
                        return true;
                    });
                    foreach (var item in indexResult)
                    {
                        VmIndexItem indexItem = new VmIndexItem();
                        indexItem.IdxName = item.idx_name;
                        indexItem.TagColor = Xamarin.Forms.Color.FromHex(item.idx_bcolor);
                        indexItem.Index = item;
                        indexItem.NotifyIndexClicked += IndexItem_Clicked;
                        IndexList.Add(indexItem);
                    }

                    NotifyPropertyChanged(nameof(IndexListHeight));
                }
                if (IndexList.Count == 0 && ProductList.Count == 0)
                {
                    await Navigator.CurrentPage.DisplayAlert("", "HITするアイテムはありませんでした", "OK");
                }
                else
                {
                    IsResultVisible = true; //結果を表示
                    NotifyProductsUpdated?.Invoke(this, new EventArgs());
                }
            }
        }

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

                //Goodsを検索
                var linqgoods = (from g in AppData.LocalarrayGoods
                                 join j in AppData.LocalarrayJoin on g.p.seqno equals j.c_seqno
                                 where j.p_seqno == indexItem.Index.joinm_c_seqno
                                 orderby j.c_line
                                 select g);
                //Goodsを更新
                ProductList.Clear();
                foreach (var item in linqgoods)
                {
                    var vm = new VmProduct(item);
                    vm.Parent = this;
                    vm.OnClicked += ProductItemClicked;
                    ProductList.Add(vm);
                }
            }
        }


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
        #endregion
        #region 内部処理


        #endregion

    }
}
