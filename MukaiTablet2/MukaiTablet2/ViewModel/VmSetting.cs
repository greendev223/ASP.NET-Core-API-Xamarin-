using MukaiTablet2.Model;
using MukaiTablet2.View;
using MukaiTablet2.ViewModel.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MukaiTablet2.ViewModel
{
    public class VmSetting : VmBase
    {
        #region イベント
        #endregion

        #region プロパティ
        public VmHeader Header
        {
            get { return _header; }
            set { _header = value; NotifyPropertyChanged(); }
        }
        private VmHeader _header = new VmHeader("各種設定", useSearchButton: false, useBackButton: true);

        /// <summary>
        /// ネットワーク接続
        /// </summary>
        public bool IsInternetConnect
        {
            get { return (Connectivity.NetworkAccess == NetworkAccess.Internet); }
        }

        /// <summary>
        /// 最終画像更新
        /// </summary>
        public string ImageLastUpdateTime
        {
            get { return AppData.LastImageDownLoadDateTime; }
        }

        /// <summary>
        /// 最終データ更新日時
        /// </summary>
        public string DataLastUpdateTime
        {
            get { return AppData.LastDownLoadDateTime; }
        }

        /// <summary>
        /// ログイン店舗名
        /// </summary>
        public string LoginStore
        {
            get { return AppData.LastLoginShopMark + ":" + AppData.LastLoginShopName; }
        }

        #endregion

        #region 外部変数
        #endregion

        #region 内部変数
        #endregion

        #region ライフサイクル
        public VmSetting()
        {
            Header.OnBackButton_Clicked += BackBtn_Clicked;
            Header.Parent = this;

            Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;
        }



        /// <summary>
        /// 画面終了
        /// </summary>
        /// <param name="param"></param>
        public override async Task OnTerm(Object param)
        {
            await base.OnTerm(param);
            Connectivity.ConnectivityChanged -= Connectivity_ConnectivityChanged;
        }

        #endregion

        #region ユーザ操作
        /// <summary>
        /// 強制ダウンロード
        /// </summary>
        public DelegateCommand CommandForceDownload
        {
            get { return _commandForceDownload = _commandForceDownload ?? new DelegateCommand(() => { ForceDownload_Clicked(this, new EventArgs()); }); }
        }
        private DelegateCommand _commandForceDownload;

        private async void ForceDownload_Clicked(object sender, EventArgs e)
        {
            using (var bussy = new IsBussyHolder(this))
            {
                if (await bussy.Set() == false) return;
                if(Connectivity.NetworkAccess != NetworkAccess.Internet)
                {
                    await Navigator.CurrentPage.DisplayAlert("", "インターネットに接続されていません。", "OK");
                    return;
                }

                VmBase vm = new VmDownload(true, true);
                Page page = new PageDownload(vm);
                await Navigator.PushModalAsync(page);
            }
        }
        /// <summary>
        /// データの初期化
        /// </summary>
        public DelegateCommand CommandInitialize
        {
            get { return _commandInitialize = _commandInitialize ?? new DelegateCommand(() => { Initialize_Clicked(this, new EventArgs()); }); }
        }
        private DelegateCommand _commandInitialize;

        private async void Initialize_Clicked(object sender, EventArgs e)
        {
            using (var bussy = new IsBussyHolder(this))
            {
                if (await bussy.Set() == false) return;
                AppData.InitSettingData();
                await Navigator.PopToStartModalAsync(false);
            }
        }

        /// <summary>
        /// アカウント追加ボタンをクリックするコマンド
        /// </summary>
        public DelegateCommand CommandAddAccount
        {
            get { return _commandAddAccount = _commandAddAccount ?? new DelegateCommand(() => { AddAccount_Clicked(this, new EventArgs()); }); }
        }
        private DelegateCommand _commandAddAccount;

        /// <summary>
        /// アカウント追加ボタンをクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void AddAccount_Clicked(object sender, EventArgs e)
        {
            using (var bussy = new IsBussyHolder(this))
            {
                if (await bussy.Set() == false) return;
                VmBase vm = new VmLogin(true);
                Page page = new PageLogin(vm);
                await Navigator.PushModalAsync(page);

            }
        }

        /// <summary>
        /// ログアウト
        /// </summary>
        public DelegateCommand CommandLogout
        {
            get { return _commandLogout = _commandLogout ?? new DelegateCommand(() => { Logout_Clicked(this, new EventArgs()); }); }
        }
        private DelegateCommand _commandLogout;

        /// <summary>
        /// ログアウトボタンをクリックする
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Logout_Clicked(object sender, EventArgs e)
        {
            using (var bussy = new IsBussyHolder(this))
            {
                if (await bussy.Set() == false) return;
                AppData.LogoutAccount();
                if (AppData.GetLoginAccount().Count == 0)
                {
                    VmBase vm = new VmLogin();
                    Page page = new PageLogin(vm);
                    await Navigator.PushModalAsync(page);
                    return;
                }
            }
            await VmLogin.SwitchAccountLogin(this, AppData.GetLoginAccount().LastOrDefault());
        }

        private async void BackBtn_Clicked(object sender, EventArgs e)
        {
            using (var bussy = new IsBussyHolder(this))
            {
                if (await bussy.Set() == false) return;
                await Navigator.PopModalAsync();
            }
        }

        #endregion

        #region 外部関数
        #endregion

        #region 内部関数
        private void Connectivity_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            NotifyPropertyChanged(nameof(IsInternetConnect));
        }
        #endregion

    }
}
