using MukaiTablet2.Model;
using MukaiTablet2.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace MukaiTablet2.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PageFavorite : PageBase
    {
        private VmFavorite mFavoriteVm;
        public PageFavorite(VmBase vm) : base(vm)
        {
            InitializeComponent();
            this.BindingContext = vm;
            mFavoriteVm = (VmFavorite)vm;
        }

        public void FavoriteProductOnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null) return;

            ((ListView)sender).SelectedItem = null;
        }

        public void FavoriteFolderOnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null) return;

            string selectedFolderId = e.SelectedItem.GetType().GetProperty(nameof(FavoriteFolder.FolderId)).GetValue(e.SelectedItem, null).ToString();

            // フォルダに入りたい場合
            if (mFavoriteVm.IsMoveFavoriteProduct == false)
            {
                //// フォルダIDを取得
                mFavoriteVm.CurrentFolderId = selectedFolderId;
                ((ListView)sender).SelectedItem = null;
            }
            // フォルダの移動の場合
            else
            {
                mFavoriteVm.DestinationFolderId = selectedFolderId;
            }
            mFavoriteVm.listView = ((ListView)sender);
        }

        public void OnChangeFavoriteFolderName(object sender, EventArgs e)
        {
            mFavoriteVm.IsFolderRightClick = true;
            mFavoriteVm.CurrentFolderId = ((MenuItem)sender).CommandParameter.ToString();
            mFavoriteVm.IsRenameFavoriteFolder = true;
            mFavoriteVm.IsFolderRightClick = false;

        }

        public async void OnDeleteFavoriteFolder(object sender, EventArgs e)
        {

            mFavoriteVm.IsFolderRightClick = true;
            mFavoriteVm.CurrentFolderId = ((MenuItem)sender).CommandParameter.ToString();

            bool answer = await Navigator.CurrentPage.DisplayAlert("確認", $"{mFavoriteVm.CurrentFolderName}を削除しますか？", "はい", "いいえ");
            if (answer)
            {
                mFavoriteVm.DeleteFavoriteFolder();
            }
            mFavoriteVm.IsFolderRightClick = false;

        }

        public async void OnChangeFolder(object sender, EventArgs e)
        {
            var folder = AppData.FavoriteFolders.Select(f => (FolderName: f.FolderName, FolderId: f.FolderId));

            string CANCEL = "キャンセル";

            string selectedFolder = await Navigator.CurrentPage.DisplayActionSheet("移動先を選択してください。", CANCEL, null,
               folder.Select((f, idx) =>
               {
                   string result = f.FolderName;
                   for (int i = 0; i < idx; i++)
                   {
                       result = string.Concat(result, " ");
                   }
                   return result;

               }).ToArray());

            if (selectedFolder == CANCEL || string.IsNullOrEmpty(selectedFolder))
            {
                return;
            }

            mFavoriteVm.DestinationFolderId = folder.ElementAt(selectedFolder.Count(c => c == ' ')).FolderId;
            mFavoriteVm.CurrentProductId = ((MenuItem)sender).CommandParameter.ToString();
            mFavoriteVm.ChangeFavoriteProductFolder();
            return;
        }

        public async void OnDeleteProduct(object sender, EventArgs e)
        {
            mFavoriteVm.CurrentProductId = ((MenuItem)sender).CommandParameter.ToString();
            bool answer = await Navigator.CurrentPage.DisplayAlert("確認", $"{mFavoriteVm.CurrentProductCode}を削除しますか？", "はい", "いいえ");
            if (answer)
            {
                mFavoriteVm.DeleteFavoriteProduct();
            }
        }
    }
}