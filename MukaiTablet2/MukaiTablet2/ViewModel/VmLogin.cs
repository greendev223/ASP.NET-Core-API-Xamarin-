using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Essentials;
using MukaiTablet2.Util;
using MukaiTablet2.Model;
using MukaiTablet2.View;
using System.Threading;
using System.Linq;
using MukaiTablet2.MukaiWebService;

namespace MukaiTablet2.ViewModel
{
    /// <summary>
    /// ログインページのViewModel
    /// </summary>
    class VmLogin : VmBase
    {
        #region =================== イベント ===========================
        #endregion
        #region =================== プロパティ ===========================
        public string UserName
        {
            get { return _userName; }
            set { _userName = value; NotifyPropertyChanged(); }
        }
        private string _userName;

        public string Password
        {
            get { return _password; }
            set { _password = value; NotifyPropertyChanged(); }
        }
        private string _password;

        public bool IsForceDownload
        {
            get { return _isForceDownload; }
            set { _isForceDownload = value; NotifyPropertyChanged(); }
        }
        private bool _isForceDownload;

        private bool _isAddAccount;

        #endregion


        #region =================== 変数 ===========================
        #endregion


        #region =================== ライフサイクル ===========================
        public VmLogin(bool isAddAccount = false)
        {
            this._isAddAccount = isAddAccount;
        }

        public override async Task OnInit(Object param)
        {
            await InitStore(this);

            if (_isAddAccount == false && AppData.GetLoginAccount().Count() > 0)
            {
                // auto login
                Login();
            }
        }

        static async Task InitStore(VmBase vmBase)
        {
            using (var bussy = new IsBussyHolder(vmBase))
            {
                await bussy.Set();

                //アプリケーションデータの初期化(Settingデータの読み込み)
                await AppData.Init();

                AppData.UserAuthCode = "mukai";

                //店情報のロード
                if (Connectivity.NetworkAccess == NetworkAccess.Internet)
                {
                    Logger.Inst.WriteLine("DownLoadStore");
                    await AppData.DownLoadStore(AppData.UserAuthCode);
                }
                else
                {
                    await AppData.LoadStoreXml();
                }
            }
        }
        public override async Task OnAppear(object param = null)
        {
            await base.OnAppear(param);

            AppData.LastApplicationOpenDateTime = DateTime.Now.ToString();

            if (AppData.LastLoginUserId != null)
            {
                UserName = AppData.LastLoginUserId;
            }
        }

        #endregion

        #region =================== ユーザ操作 ===========================

        /// <summary>
        /// ログインボタン
        /// </summary>
        public DelegateCommand CommandLogin
        {
            get { return _commandLogin = _commandLogin ?? new DelegateCommand(BtnLogin); }
        }
        private DelegateCommand _commandLogin;

        public void BtnLogin()
        {
            Login();
        }

        /// <summary>
        /// アカウント切り替えのログイン
        /// </summary>
        /// <param name="vmBase"></param>
        /// <param name="account"></param>
        /// <returns></returns>
        public static async Task SwitchAccountLogin(VmBase vmBase, string account)
        {
            await InitStore(vmBase);
            using (var bussy = new IsBussyHolder(vmBase))
            {
                if (await bussy.Set() == false) return;
                store login = (from s in AppData.LocalarrayStore
                                 where s.login == account
                                 select s).FirstOrDefault();
                if (login == null)
                {
                    //認証失敗
                    await Navigator.CurrentPage.DisplayAlert("エラー", "ユーザー名またはパスワードが違います。", "OK");
                    return;
                }

                //認証成功

                AppData.LastLoginUserId = account;
                AppData.LoginStore = login;
                AppData.LastLoginShopMark = login.mkk + login.shopcode.ToString("D4");
                AppData.LastLoginShopName = login.umcname + login.sdcname;

                //SettingData保存
                await AppData.SaveSettingData();

                // お気に入り商品を取得
                AppData.GetFavoriteFolder();

                bool isDownloadAll = false;
               
                if (Connectivity.NetworkAccess == NetworkAccess.Internet )
                {
                    // アカウント切り替えの時に24時間を超える時に ⇒　すべてダウンロード
                    if (string.IsNullOrEmpty(AppData.LastDownLoadDateTime) || DateTime.Parse(AppData.LastDownLoadDateTime).AddHours(24) < DateTime.Now)
                    {
                        isDownloadAll = true;
                    }

                    // 出荷停止、受注残、発注履歴だけダウンロード
                    var vmDownLoad = new VmDownload(true, false, isDownloadAll);
                    var pageDownload = new PageDownload(vmDownLoad);
                    await Navigator.PushModalAsync(pageDownload, true);
                }
            }
        }

        /// <summary>
        /// ログイン画面でログイン
        /// </summary>
        public async void Login()
        {
            using (var bussy = new IsBussyHolder(this))
            {
                if (await bussy.Set() == false) return;

                if (AppData.LocalarrayStore == null || AppData.LocalarrayStore.Count == 0)
                {
                    //店情報がない
                    await Navigator.CurrentPage.DisplayAlert("エラー", "店舗情報が正しく取得できていません。再起動してください。", "OK");
                    return;
                }

                store login = null;

                // オートログイン
                if (_isAddAccount == false && AppData.GetLoginAccount().Count > 0)
                {
                   var loginAcc =  AppData.GetLoginAccount().LastOrDefault();
                    UserName = loginAcc;
                    login = (from s in AppData.LocalarrayStore
                                 where s.login == loginAcc
                            select s).FirstOrDefault();
                }
                // add user or first login
                else
                { 
                    login = (from s in AppData.LocalarrayStore
                             where s.login == UserName && s.pass == Password
                             select s).FirstOrDefault();

                    // 最終のログインアカウントを保存
                    AppData.SaveLoginAccount(UserName);
                }

                if (login == null)
                {
                    //認証失敗
                    await Navigator.CurrentPage.DisplayAlert("エラー", "ユーザー名またはパスワードが違います。", "OK");
                    return;
                }

                //認証成功

                AppData.LastLoginUserId = UserName;
                AppData.LoginStore = login;
                AppData.LastLoginShopMark = login.mkk + login.shopcode.ToString("D4");
                AppData.LastLoginShopName = login.umcname + login.sdcname;

                //SettingData保存
                await AppData.SaveSettingData();

                if (Connectivity.NetworkAccess == NetworkAccess.Internet )
                {
                    //ネットワークに接続されていて、24時間以上経過していたら、DOWNLOAD
                    if (string.IsNullOrEmpty(AppData.LastDownLoadDateTime) || DateTime.Parse(AppData.LastDownLoadDateTime).AddHours(24) < DateTime.Now)
                    {
                        var vm = new VmDownload(true);
                        var page = new PageDownload(vm);
                        await Navigator.PushModalAsync(page, true);
                        return;
                    }

                    // アカウント追加の場合
                    if (_isAddAccount)
                    {
                        var vm = new VmDownload(true, false, false);
                        var page = new PageDownload(vm);
                        await Navigator.PushModalAsync(page, true);
                        return;
                    }
                }
             
                {
                    await AppData.LoadGoodsXml(AppData.LastLoginShopMark);
                    await AppData.LoadIndexXml();
                    await AppData.LoadJoinmXml();
                    await AppData.LoadCurrencyXml();
                    await AppData.LoadDenamesXml();
                    await AppData.LoadMtnamesXml();
                    await AppData.LoadGname2sXml();
                    await AppData.LoadSecName1sXml();
                    await AppData.LoadSecName2sXml();
                    await AppData.LoadSecName3sXml();

                    // お気に入り商品を取得
                    AppData.GetFavoriteFolder();

                    //TOPページ
                    VmBase vm = new VmTop();
                    Page page = new PageTop(vm);
                    await Navigator.PushModalAsync(page, true);

                    Password = ""; //戻ってい来た時のためにパスワードは初期化
                }

            }
        }
        #endregion
        #region =================== 内部関数 ===========================
        #endregion

    }
}
