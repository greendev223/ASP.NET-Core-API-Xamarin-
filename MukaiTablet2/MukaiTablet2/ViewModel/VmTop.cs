using Microsoft.Toolkit.Parsers.Rss;
using MukaiTablet2.Model;
using MukaiTablet2.MukaiWebService;
using MukaiTablet2.Util;
using MukaiTablet2.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MukaiTablet2.ViewModel
{
    class VmTop : VmBase
    {

        public static readonly double ScreenStandardWidth = 500;

        public static readonly double ScreenStandardHeight = 887;

        public static readonly double MenuItemStandardHeight = 100;

        public static readonly double MenuItemStandardWidth = 128;

        public double MenuItemScale
        {
            get { return _menuItemScale; }
            set { _menuItemScale = value; NotifyPropertyChanged(); }
        }
        private double _menuItemScale = 1;


        public double MenuItemHeightRequest
        {
            get { return _menuItemHeightRequest; }
            set { _menuItemHeightRequest = value; NotifyPropertyChanged(); }
        }
        private double _menuItemHeightRequest = MenuItemStandardHeight;

        public double MenuItemWidthRequest
        {
            get { return __menuItemWidthRequest; }
            set { __menuItemWidthRequest = value; NotifyPropertyChanged(); }
        }
        private double __menuItemWidthRequest = MenuItemStandardWidth;


        /// <summary>
        /// ログイン中アカウントの一覧の表示フラグ
        /// </summary>
        public bool IsSwitchAccountDialogVisible
        {
            get { return _isSwitchAccountDialogVisible; }
            set { _isSwitchAccountDialogVisible = value; NotifyPropertyChanged(); }
        }
        private bool _isSwitchAccountDialogVisible = false;

        /// <summary>
        /// ログイン中アカウントの一覧
        /// </summary>
        private List<LoginAccount> _loginAccountList;
        public List<LoginAccount> LoginAccountList
        {
            get { return _loginAccountList; }
            set { _loginAccountList = value; NotifyPropertyChanged(); }
        }

        /// <summary>
        /// アカウント切り替えボタンの表示フラグ
        /// </summary>
        public bool IsSwitchAccountVisible
        {
            get { return _isSwitchAccountVisible; }
            set { _isSwitchAccountVisible = value; NotifyPropertyChanged(); }
        }
        private bool _isSwitchAccountVisible = false;

        /// <summary>
        /// 注文ボタン
        /// </summary>
        public DelegateCommand CommandLedger
        {
            get { return _commandLedger = _commandLedger ?? new DelegateCommand(() => { BtnLedger(this, new EventArgs()); }); }
        }
        private DelegateCommand _commandLedger;

        private async void BtnLedger(object sender, EventArgs e)
        {
            using (var bussy = new IsBussyHolder(this))
            {
                if (await bussy.Set() == false) return;
                VmBase vm = new VmIndex();
                Page page = new PageIndex(vm);
                await Navigator.PushModalAsync(page);
            }
        }

        /// <summary>
        /// 発注カート
        /// </summary>
        public DelegateCommand CommandOrderChart
        {
            get { return _commandOrderChart = _commandOrderChart ?? new DelegateCommand(() => { ButtonOrderCart_Clicked(this, new EventArgs()); }); }
        }
        private DelegateCommand _commandOrderChart;

        private async void ButtonOrderCart_Clicked(object sender, EventArgs e)
        {
            using (var bussy = new IsBussyHolder(this))
            {
                if (await bussy.Set() == false) return;
                VmBase vm = new VmOrderCart();
                Page page = new PageOrderCart(vm);
                await Navigator.PushModalAsync(page);
            }
        }

        /// <summary>
        /// 発注履歴
        /// </summary>
        public DelegateCommand CommandOrderCHistory
        {
            get { return _commandOrderHistory = _commandOrderHistory ?? new DelegateCommand(() => { BtnOrderHistory_Clicked(this, new EventArgs()); }); }
        }
        private DelegateCommand _commandOrderHistory;
        private async void BtnOrderHistory_Clicked(object sender, EventArgs e)
        {
            using (var bussy = new IsBussyHolder(this))
            {
                if (await bussy.Set() == false) return;

                VmBase vm = new VmOrderHistory();
                Page page = new PageOrderHistory(vm);
                await Navigator.PushModalAsync(page);
            }
        }

        /// <summary>
        /// お気に入り
        /// </summary>
        public DelegateCommand CommandFavorite
        {
            get { return _cCommandFavorite = _cCommandFavorite ?? new DelegateCommand(() => { BtnFavorite_Clicked(this, new EventArgs()); }); }
        }
        private DelegateCommand _cCommandFavorite;
        private async void BtnFavorite_Clicked(object sender, EventArgs e)
        {
            using (var bussy = new IsBussyHolder(this))
            {
                if (await bussy.Set() == false) return;

                VmBase vm = new VmFavorite();
                Page page = new PageFavorite(vm);
                await Navigator.PushModalAsync(page);
            }
        }

        /// <summary>
        ///  売上日報
        /// </summary>
        public DelegateCommand CommandReport
        {
            get { return _commandReport = _commandReport ?? new DelegateCommand(() => { BtnReport_Clicked(this, new EventArgs()); }); }
        }
        private DelegateCommand _commandReport;
        private async void BtnReport_Clicked(object sender, EventArgs e)
        {
            using (var bussy = new IsBussyHolder(this))
            {
                if (await bussy.Set() == false) return;
                VmBase vm = new VmSaleReport();
                Page page = new PageSaleReport(vm);
                await Navigator.PushModalAsync(page,false);
            }
        }

        /// <summary>
        ///  設定
        /// </summary>
        public DelegateCommand CommandSetting
        {
            get { return _commandSetting = _commandSetting ?? new DelegateCommand(() => { BtnSetting_Clicked(this, new EventArgs()); }); }
        }
        private DelegateCommand _commandSetting;
        private async void BtnSetting_Clicked(object sender, EventArgs e)
        {
            using (var bussy = new IsBussyHolder(this))
            {
                if (await bussy.Set() == false) return;

                VmBase vm = new VmSetting();
                Page page = new PageSetting(vm);
                await Navigator.PushModalAsync(page);

            }
        }

        /// <summary>
        ///  ログアウト
        /// </summary>
        public DelegateCommand CommandLogout
        {
            get { return _commandLogout = _commandLogout ?? new DelegateCommand(() => { BtnLogout_Clicked(this, new EventArgs()); }); }
        }
        private DelegateCommand _commandLogout;
        private async void BtnLogout_Clicked(object sender, EventArgs e)
        {
            using (var bussy = new IsBussyHolder(this))
            {
                if (await bussy.Set() == false) return;

                await Navigator.PopToStartModalAsync(false);

            }
        }

        /// <summary>
        ///  アカウントを切り替え
        /// </summary>
        public DelegateCommand CommandSwitchAccount
        {
            get { return _commandSwitchAccount = _commandSwitchAccount ?? new DelegateCommand(() => { BtnSwitchAccount_Clicked(this, new EventArgs()); }); }
        }
        private DelegateCommand _commandSwitchAccount;
        private async void BtnSwitchAccount_Clicked(object sender, EventArgs e)
        {
            using (var bussy = new IsBussyHolder(this))
            {
                if (await bussy.Set() == false) return;

                IsSwitchAccountDialogVisible = true;

                List<LoginAccount> temp = new List<LoginAccount>();

                foreach (var account in AppData.GetLoginAccount())
                {
                    LoginAccount loginAccount = new LoginAccount();
                    loginAccount.StoreName = AppData.LocalarrayStore.Where( s => s.login == account).Select( s => s.umcname + s.sdcname).FirstOrDefault();
                    loginAccount.AccountName = account;
                    if (account == AppData.LastLoginUserId)
                    {
                        loginAccount.Background = Color.FromHex("#595959");
                        loginAccount.StoreName += "（ログイン中）";
                    }
                    temp.Add(loginAccount);
                }

                LoginAccountList = temp;
            }
        }

        /// <summary>
        ///  アカウント切り替えボックスを閉じる
        /// </summary>
        public DelegateCommand CommandCloseSwitchAccount
        {
            get { return _commandCloseSwitchAccount = _commandCloseSwitchAccount ?? new DelegateCommand(() => { BtnCloseSwitchAccount_Clicked(this, new EventArgs()); }); }
        }
        private DelegateCommand _commandCloseSwitchAccount;
        private async void BtnCloseSwitchAccount_Clicked(object sender, EventArgs e)
        {
            using (var bussy = new IsBussyHolder(this))
            {
                if (await bussy.Set() == false) return;
                IsSwitchAccountDialogVisible = false;
            }
        }

        /// RSS URLのデータをRssオブジェクト一覧に取得
        /// </summary>
        /// <returns></returns>
        public async Task<List<RssData>> GetRssData()
        {
            List<RssData> listData = new List<RssData>();
            var result = await ParseRss(RssData.RSS_URL);

            // RSSから日付とタイトルを１０件ほど表示
            result = result.Take(10);
            foreach (var item in result)
            {
                listData.Add(new RssData()
                {
                    Title = item.Title,
                    PublishDate = item.PublishDate.Date.ToString("yyyy/MM/dd"),
                    PublishTime = item.PublishDate.TimeOfDay.ToString(@"hh\:mm"),
                    FeedUrl = item.FeedUrl
                });
            }
            return listData;
        }

        /// <summary>
        /// RSS URLのデータを取得
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<RssSchema>> ParseRss(string url)
        {
            string feed = null;

            using (var client = new HttpClient())
            {
                feed = await client.GetStringAsync(url);
            }

            if (feed == null) return new List<RssSchema>();
            var parser = new RssParser();
            var rss = parser.Parse(feed);
            return rss;
        }

        public struct LoginAccount
        {
            public Color Background { get; set; }
            public string StoreName { get; set; }
            public string AccountName { get; set; }
        }
    }
}
