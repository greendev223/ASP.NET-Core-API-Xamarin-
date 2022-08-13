using MukaiTablet2.Model;
using MukaiTablet2.Util;
using MukaiTablet2.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;
using static MukaiTablet2.ViewModel.VmTop;

namespace MukaiTablet2.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PageTop : PageBase
    {
        private VmTop mTopVm;
        public PageTop(VmBase vm) : base(vm)
        {
            InitializeComponent();
            mTopVm = (VmTop)vm;

            this.Appearing += PageAppearing;
            SetupMargin();
            flexLayout.SizeChanged += FlexLayout_SizeChanged;
            listView_account.ItemAppearing += ListView_account_ItemAppearing;
        }

        /// <summary>
        /// ログイン中アカウントにスクロール
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListView_account_ItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            var switchAccount = e.Item.GetType().GetProperty(nameof(LoginAccount.AccountName)).GetValue(e.Item, null).ToString();
            if (switchAccount == AppData.LastLoginUserId)
            {
                listView_account.ScrollTo(e.Item, ScrollToPosition.MakeVisible, false);
            }
        }

        private void FlexLayout_SizeChanged(object sender, EventArgs e)
        {
            SetupMargin();
        }

        /// <summary>
        /// メニューアイテムのマージンを調整
        /// </summary>
        void SetupMargin()
        {
            mTopVm.MenuItemScale  = Application.Current.MainPage.Height / ScreenStandardHeight;
            mTopVm.MenuItemHeightRequest = MenuItemStandardHeight * mTopVm.MenuItemScale;
            mTopVm.MenuItemWidthRequest = MenuItemStandardWidth * mTopVm.MenuItemScale;

            double marginLeft = Application.Current.MainPage.Width % mTopVm.MenuItemWidthRequest;
            double marginTop = 25 * mTopVm.MenuItemScale;
            if (Application.Current.MainPage.Width > mTopVm.MenuItemWidthRequest * 6)
            {
                marginTop += 50 * mTopVm.MenuItemScale;
                marginLeft = Application.Current.MainPage.Width - mTopVm.MenuItemWidthRequest * 6;
            }

            if (AppData.GetLoginAccount().Count() < 2)
            {
                marginTop -= Application.Current.MainPage.Height * mTopVm.MenuItemScale / 17;
            }

            flexLayout.Margin = new Thickness(marginLeft / 2, marginTop , 0, 0);
        }
        /// <summary>
        /// RSS URLのクリックするイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public async void RssOnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null) return;

            // Feed URLを取得
            string feedUrl = e.SelectedItem.GetType().GetProperty(nameof(RssData.FeedUrl)).GetValue(e.SelectedItem, null).ToString() ?? "https://blog.livedoor.com/";

            // ブラウザでURLをアクセス
            await Launcher.OpenAsync(new Uri(feedUrl));

            ((ListView)sender).SelectedItem = null;
        }

        /// <summary>
        ///  切り替えアカウントを選択
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public async void AccountOnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null) return;
            
            ((ListView)sender).SelectedItem = null;

            // 切り替えアカウントを取得
            string switchAccount = e.SelectedItem.GetType().GetProperty(nameof(LoginAccount.AccountName)).GetValue(e.SelectedItem, null).ToString();

            if (switchAccount == AppData.LastLoginUserId)
            {
                return;
            }
            await VmLogin.SwitchAccountLogin(mTopVm, switchAccount);
            
        }

        private async void PageAppearing(object sender, EventArgs e)
        {
            // ログイン中アカウントが１以下の場合は切り替えボタンを非表示
            mTopVm.IsSwitchAccountVisible = (AppData.GetLoginAccount().Count() > 1);
            // インターネットが切れる場合
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                return;
            }
            try
            {
                // Rssデータを取得して、リストビューに反映
                listView_Rss.ItemsSource = await mTopVm.GetRssData();
            }
            catch (Exception)
            {
            }

        }
    }
}