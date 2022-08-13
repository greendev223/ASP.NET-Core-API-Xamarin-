using MukaiTablet2.Model;
using MukaiTablet2.Util;
using MukaiTablet2.View;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MukaiTablet2.ViewModel
{
    public class VmDownload : VmBase
    {
        #region イベント
        #endregion

        #region プロパティ
        //ダウンロード完了
        public bool IsDownloadDone
        {
            get { return _isDownloadDone; }
            set { _isDownloadDone = value; NotifyPropertyChanged(); }
        }
        private bool _isDownloadDone = false;

        public string DownloadItem
        {
            get { return _downloadItem; }
            set { _downloadItem = value; NotifyPropertyChanged(); }
        }
        private string _downloadItem;

        public float Progress
        {
            get { return _progress; }
            set { _progress = value; NotifyPropertyChanged(); }
        }
        private float _progress;

        public bool IsCancelVisible
        {
            get { return _isCancelVisible; }
            set { _isCancelVisible = value; NotifyPropertyChanged(); }
        }
        private bool _isCancelVisible;

        public bool IsProgressVisible
        {
            get { return _isProgressVisible; }
            set { _isProgressVisible = value; NotifyPropertyChanged(); }
        }
        private bool _isProgressVisible;

        private bool mIsDownloadAll = true;

        #endregion

        #region 外部変数
        #endregion

        #region 内部変数
        private CancellationTokenSource mCancelTokenSource;
        private bool mIsForceDownload = false;
        private bool mIsPopFinished = false;
        #endregion

        #region ライフサイクル
        public VmDownload() { }
        public VmDownload(bool isForceDownload, bool isPopFinished = false, bool isDowloadAll = true)
        {
            mIsForceDownload = isForceDownload;
            mIsPopFinished = isPopFinished;
            mIsDownloadAll = isDowloadAll;
        }
        public override async Task OnInit(Object param)
        {
            var dep = DependencyService.Get<IDepend>();
            try
            {
                string userCode = AppData.UserAuthCode;
                string shopmark = AppData.LastLoginShopMark;

                //SLEEP禁止
                dep.DisabletSleep(true);

                using (var bussy = new IsBussyHolder(this))
                {
                    await bussy.Set();

                    // アカウント切り替えの時にダウンロードしない
                    if (mIsDownloadAll)
                    {
                        DownloadItem = "通貨設定";
                        Logger.Inst.WriteLine("DownLoadCurrency");
                        await AppData.DownLoadCurrency(userCode, mIsForceDownload);

                        DownloadItem = "台帳情報１";
                        Logger.Inst.WriteLine("DownLoadJoinm");
                        await AppData.DownLoadJoinm(shopmark, userCode, mIsForceDownload);

                        DownloadItem = "台帳情報２";
                        Logger.Inst.WriteLine("DownLoadIndex");
                        await AppData.DownLoadIndex(userCode, mIsForceDownload);
                    }

                    DownloadItem = "出荷停止、受注残";
                    Logger.Inst.WriteLine("DownLoadLimAcc");
                    await AppData.DownLoadLimAcc(shopmark, userCode, mIsForceDownload);

                    // アカウント切り替えの時にダウンロードしない
                    if (mIsDownloadAll)
                    {
                        DownloadItem = "商品情報";
                        Logger.Inst.WriteLine("DownloadGoods");
                        await AppData.DownloadGoods(shopmark, userCode, mIsForceDownload);
                    }

                    DownloadItem = "発注履歴";
                    Logger.Inst.WriteLine("DownLoadOrderHistory");
                    await AppData.DownLoadOrderHistory(shopmark, userCode, mIsForceDownload);
                }


                if (mIsDownloadAll)
                {
                    mCancelTokenSource = new CancellationTokenSource();
                    var progress = new Progress<int>(p => { Progress = p / 100.0f; });
                    IsCancelVisible = true;
                    IsProgressVisible = true;
                    DownloadItem = "商品イメージ";
                    await AppData.DownLoadImage(shopmark, userCode, progress, mCancelTokenSource.Token);
                    IsCancelVisible = false;
                }
              

                AppData.LastDownLoadDateTime = DateTime.Now.ToString();
                await AppData.SaveSettingData();
            }
            catch (OperationCanceledException ex)
            {
                Logger.Inst.WriteLine($"Canceled {ex.Message}");
                await Navigator.CurrentPage.DisplayAlert("", "ダウンロードがキャンセルされました。", "次へ");
            }
            catch (Exception e)
            {
                await Navigator.CurrentPage.DisplayAlert("", $"{e.Message}", "次へ");
            }
            finally
            {
                //SLEEP禁止解除
                dep.DisabletSleep(false);
            }

            if (mIsPopFinished)
            {
                await Navigator.PopModalAsync();
            }
            else
            {
                VmTop vm = new VmTop();
                Page page = new PageTop(vm);
                await Navigator.PushModalAsync(page);
            }
        }
        #endregion

        #region ユーザ操作
        /// <summary>
        /// キャンセル
        /// </summary>
        public DelegateCommand CommandCancel
        {
            get { return _commandCancel = _commandCancel ?? new DelegateCommand(BtnCancel_Clicked); }
        }
        private DelegateCommand _commandCancel;

        private void BtnCancel_Clicked()
        {
            mCancelTokenSource.Cancel();
        }
        #endregion

        #region 外部関数
        #endregion

        #region 内部関数
        #endregion





    }
}
