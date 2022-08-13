using MukaiTablet2.Model;
using MukaiTablet2.ViewModel.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace MukaiTablet2.ViewModel
{
    public class VmFavorite : VmBase
    {
        public VmHeader Header
        {
            get { return _header; }
            set { _header = value; NotifyPropertyChanged(); }
        }
        private VmHeader _header = new VmHeader("お気に入り", useSearchButton: false, useBackButton: true, useSubMenu: false, useAddButton: true);

        public ObservableCollection<FavoriteFolder> FavoriteFolders
        {
            get { return favoriteFolders; }
            set { favoriteFolders = value; NotifyPropertyChanged(); }
        }
        public ObservableCollection<FavoriteFolder> favoriteFolders;


        public ObservableCollection<FavoriteProduct> FavoriteProducts
        {
            get { return favoriteProducts; }
            set { favoriteProducts = value; NotifyPropertyChanged(); }
        }
        public ObservableCollection<FavoriteProduct> favoriteProducts;

        public int ProductColNum
        {
            get
            {
                double breadcrumbSize = 120;
                if (mPageWidth == 0) return 1; //デフォルト
                int num = (int)Math.Floor((mPageWidth / breadcrumbSize));
                if (num <= 0) num = 1;
                return num;
            }
        }

        public bool IsInputDialogVisible
        {
            get { return _isInputDialogVisible; }
            set { _isInputDialogVisible = value; NotifyPropertyChanged(); }
        }
        private bool _isInputDialogVisible = false;

        public bool IsFavoriteFolderVisible
        {
            get { return _isFavoriteFolderVisible; }
            set
            {
                _isFavoriteFolderVisible = value;
                _isFavoriteProductVisible = !value;
                if (value)
                {
                    ListViewTitle = "すべてのフォルダ";
                }
                NotifyPropertyChanged("IsFavoriteFolderVisible");
                NotifyPropertyChanged("IsFavoriteProductVisible");
            }
        }
        private bool _isFavoriteFolderVisible = true;

        public bool IsFavoriteProductVisible
        {
            get { return _isFavoriteProductVisible; }
            set
            {
                _isFavoriteProductVisible = value;
                _isFavoriteFolderVisible = !value;
                if (value)
                {
                    ListViewTitle = $"フォルダ名：{ CurrentFolderName }";
                }
                NotifyPropertyChanged("IsFavoriteProductVisible");
                NotifyPropertyChanged("IsFavoriteFolderVisible");
            }
        }
        private bool _isFavoriteProductVisible = false;

        public string CurrentFolderId
        {
            get { return _currentFolderId; }
            set
            {
                _currentFolderId = value;

                // favoriteフォルダに入る場合
                if (IsFolderRightClick == false)
                {
                    var favoriteFolder = AppData.FavoriteFolders.Where(f => f.FolderId == value).FirstOrDefault();
                    FavoriteProducts = favoriteFolder.FavoriteProducts;
                    IsFavoriteProductVisible = true;
                    CurrentFavoriteProductCount = favoriteFolder.FavoriteProductCount.ToString();
                }

                NotifyPropertyChanged();
            }
        }
        private string _currentFolderId = string.Empty;

        public string DestinationFolderId { get; set; }


        public string CurrentProductId
        {
            get { return _currentProducId; }
            set
            {
                _currentProducId = value;
                NotifyPropertyChanged();
            }
        }
        private string _currentProducId;

        public bool IsFolderRightClick { get; set; } = false;

        public bool IsRenameFavoriteFolder
        {
            get
            {
                return _isRenameFavoriteFolder;
            }
            set
            {
                _isRenameFavoriteFolder = value;
                if (value)
                {
                    NewFolderName = CurrentFolderName;
                    InputDialogTitle = "フォルダ名変更";
                    IsInputDialogVisible = value;
                }
                else
                {
                    InputDialogTitle = "フォルダ作成";
                }
            }
        }
        public bool _isRenameFavoriteFolder;

        public string CurrentFolderName { get { return AppData.FavoriteFolders.Where(f => f.FolderId == CurrentFolderId).FirstOrDefault()?.FolderName ?? string.Empty; } }

        public string CurrentProductCode
        { 
            get
            {
                var folder = AppData.FavoriteFolders.Where(f => f.FavoriteProducts.Any(p => p.ProductId == CurrentProductId)).FirstOrDefault();
                return folder.FavoriteProducts.Where(p => p.ProductId == CurrentProductId).FirstOrDefault().ProductCode;
            } 
        }

        public string NewFolderName
        {
            get { return _newFolderName; }
            set
            {
                _newFolderName = value;
                NotifyPropertyChanged();
            }
        }
        private string _newFolderName;

        public string InputDialogTitle
        {
            get { return _inputDialogTitle; }
            set
            {
                _inputDialogTitle = value;
                NotifyPropertyChanged();
            }
        }
        private string _inputDialogTitle = "フォルダ作成";

        public string ListViewTitle
        {
            get { return _listViewTitle; }
            set
            {
                _listViewTitle = value;
                NotifyPropertyChanged();
            }
        }
        private string _listViewTitle = "すべてのフォルダ";

        public bool IsDeleteFolder
        {
            get { return _isDeleteFolder; }
            set
            {
                _isDeleteFolder = value;
                NotifyPropertyChanged();
            }
        }
        private bool _isDeleteFolder;

        public bool IsMoveFavoriteProduct
        {
            get { return _isMoveFavoriteProduct; }
            set
            {
                _isMoveFavoriteProduct = value;
                if (value)
                {
                    IsFavoriteFolderVisible = value;
                    ListViewTitle = "移動先のフォルダを選択してください。";
                }
                else ListViewTitle = "すべてのフォルダ";

                NotifyPropertyChanged();
            }
        }
        private bool _isMoveFavoriteProduct;

        public string CurrentFavoriteProductCount
        {
            get { return _currentFavoriteProductCount; }
            set
            {
                _currentFavoriteProductCount = $"{value} 件";
                NotifyPropertyChanged();
            }
        }
        private string _currentFavoriteProductCount;

        public VmFavorite()
        {
            Header.Parent = this;
            Header.OnBackButton_Clicked += Header_BackBtn_Clicked;
            Header.OnAddButton_Clicked += Header_OnAddButton_Clicked;

            if (AppData.FavoriteFolders.Count() > 0)
            {
                FavoriteFolders = AppData.FavoriteFolders;
            }
        }

        /// <summary>
        /// Viewのページサイズが変わった
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public override void OnPageSizeChanged(double width, double height)
        {
            base.OnPageSizeChanged(width, height);
            string[] notifies = { nameof(ProductColNum) };
            NotifyPropertyChanged(notifies);
        }

        private void Header_OnAddButton_Clicked(object sender, EventArgs e)
        {
            IsInputDialogVisible = true;
            IsRenameFavoriteFolder = false;
        }

        private async void Header_BackBtn_Clicked(object sender, EventArgs e)
        {
            using (var bussy = new IsBussyHolder(this))
            {
                if (await bussy.Set() == false) return;
                if (IsFavoriteProductVisible)
                {
                    IsFavoriteFolderVisible = true;
                    return;
                }
                await Navigator.PopModalAsync();
            }
        }

        public DelegateCommand CommandDialogCancel
        {
            get { return _commandDialogCancel = _commandDialogCancel ?? new DelegateCommand(() => { DialogCancel_Clicked(this, new EventArgs()); }); }
        }
        private DelegateCommand _commandDialogCancel;


        private async void DialogCancel_Clicked(VmFavorite vmFavorite, EventArgs eventArgs)
        {
            using (var bussy = new IsBussyHolder(this))
            {
                if (await bussy.Set() == false) return;
                IsInputDialogVisible = false;
            }
        }

        public DelegateCommand CommandDialogCreate
        {
            get { return _commandDialogCreate = _commandDialogCreate ?? new DelegateCommand(() => { DialogCreate_Clicked(this, new EventArgs()); }); }
        }
        private DelegateCommand _commandDialogCreate;


        private async void DialogCreate_Clicked(VmFavorite vmFavorite, EventArgs eventArgs)
        {
            using (var bussy = new IsBussyHolder(this))
            {
                if (await bussy.Set() == false) return;
                IsInputDialogVisible = false;
                if (IsRenameFavoriteFolder)
                {
                    AppData.RenameFavoriteFolder(CurrentFolderId, NewFolderName);
                    IsRenameFavoriteFolder = false;
                }
                else AppData.SaveFavoriteFolder(NewFolderName);
                FavoriteFolders = AppData.FavoriteFolders;

                NewFolderName = string.Empty;
            }
        }

        public DelegateCommand CommandMoveFavoriteProduct
        {
            get { return _commandMoveFavoriteProduct = _commandMoveFavoriteProduct ?? new DelegateCommand(() => { MoveFavoriteProduct_Clicked(this, new EventArgs()); }); }
        }
        private DelegateCommand _commandMoveFavoriteProduct;


        public ListView listView { get; set; }

        private void MoveFavoriteProduct_Clicked(VmFavorite vmFavorite, EventArgs eventArgs)
        {
            ChangeFavoriteProductFolder();
        }

        public async void ChangeFavoriteProductFolder()
        {
            using (var bussy = new IsBussyHolder(this))
            {
                if (await bussy.Set() == false) return;

                if (!string.IsNullOrEmpty(DestinationFolderId) && DestinationFolderId != CurrentFolderId)
                {
                    AppData.MoveFavoriteProduct(CurrentProductId, CurrentFolderId, DestinationFolderId);
                    FavoriteFolders = AppData.FavoriteFolders;
                    CurrentFavoriteProductCount = FavoriteFolders.Where( f => f.FolderId == CurrentFolderId ).FirstOrDefault().FavoriteProductCount.ToString();
                }

                DestinationFolderId = string.Empty;

                if (listView != null)
                {
                    listView.SelectedItem = null;
                }
            }
        }

        public DelegateCommand CommandMoveCancel
        {
            get { return _commandMoveCancel = _commandMoveCancel ?? new DelegateCommand(() => { MoveCancel_Clicked(this, new EventArgs()); }); }
        }
        private DelegateCommand _commandMoveCancel;

        private async void MoveCancel_Clicked(VmFavorite vmFavorite, EventArgs eventArgs)
        {
            using (var bussy = new IsBussyHolder(this))
            {
                if (await bussy.Set() == false) return;

                IsMoveFavoriteProduct = false;
                DestinationFolderId = string.Empty;

                if (listView != null)
                {
                    listView.SelectedItem = null;
                }
            }
        }

        public async void DeleteFavoriteFolder()
        {
            using (var bussy = new IsBussyHolder(this))
            {
                if (await bussy.Set() == false) return;
                AppData.DeleteFavoriteFolder(CurrentFolderId);
                FavoriteFolders = AppData.FavoriteFolders;
            }
        }

        public async void DeleteFavoriteProduct()
        {
            using (var bussy = new IsBussyHolder(this))
            {
                if (await bussy.Set() == false) return;
                AppData.DeleteFavoriteProduct(CurrentProductId);
                FavoriteFolders = AppData.FavoriteFolders;
                CurrentFavoriteProductCount = FavoriteFolders.Where(f => f.FolderId == CurrentFolderId).FirstOrDefault().FavoriteProductCount.ToString();
            }
        }
    }
}
