using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using MukaiTablet2.MukaiWebService;
using MukaiTablet2.Util;
using System.Linq;
using System.IO;
using System.Runtime.Serialization;
using System.Threading;
using System.Net.Http;
using System.Net;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using Xamarin.Forms.Internals;

namespace MukaiTablet2.Model
{
    public class AppData
    {
        public static ArrayOfGoods LocalarrayGoods;
        public static ArrayOfJoinm LocalarrayJoin;
        public static ArrayOfIndex LocalarrayIndex;
        public static ArrayOfStore LocalarrayStore;
        public static ArrayOfCurrency LocalarrayCurrency;
        public static ArrayOfLim LocalarrayLim;
        public static ArrayOfAcc LocalarrayAcc;
        private static SettingData mSettingData;
        public static List<string> LocalarrayMtname;
        public static List<string> LocalarrayDename;
        public static List<string> LocalarrayGname2;
        public static List<string> LocalarraySecName1;
        public static List<string> LocalarraySecName2;
        public static List<string> LocalarraySecName3;
        public static ObservableCollection<FavoriteFolder> FavoriteFolders { get; set; } = new ObservableCollection<FavoriteFolder>();


        public static async Task Init()
        {
            IDepend dep = DependencyService.Get<IDepend>();
            string localPath = dep.GetLocalStoragePath();
            string sep = dep.GetSeparator();
            string settingFilePath = localPath + sep + "setting.xml";
            if (File.Exists(settingFilePath) == false)
            {
                mSettingData = new SettingData();
            }
            else
            {
                var storage = new DataStorageHelper<SettingData>("setting.xml");
                mSettingData = await storage.Load();
            }
        }

        public static async Task SaveSettingData()
        {
            var storage = new DataStorageHelper<SettingData>("setting.xml");
            await storage.Save(mSettingData);
        }

        public static void InitSettingData()
        {
            mSettingData.Init();
        }

        /// <summary>
        /// ログインしたアカウントを保存
        /// </summary>
        /// <param name="account"></param>
        public static void SaveLoginAccount(string account)
        {
            var storage = new DataStorageHelper<SettingData>("loginAccount.dat");
            if (!File.Exists(storage.FilePath))
            {
                var file = File.Create(storage.FilePath);
                file.Close();
            }

            List<string> loginData = File.ReadAllLines(storage.FilePath).ToList();

            // 同じアカウントを削除
            loginData.Distinct();
            if (loginData.Contains(account))
            {
                loginData.Remove(account);
            }
            loginData.Add(account);

            using (StreamWriter outputFile = new StreamWriter(storage.FilePath))
            {
                foreach (string acc in loginData)
                {
                    outputFile.WriteLine(acc);
                }
                   
            }
        }

        /// <summary>
        /// ログイン中アカウントをログアウト
        /// </summary>
        /// <returns></returns>
        public static List<string> LogoutAccount()
        {
            var storage = new DataStorageHelper<SettingData>("loginAccount.dat");
            if (!File.Exists(storage.FilePath))
            {
                var file = File.Create(storage.FilePath);
                file.Close();
                return new List<string>();
            }

            List<string> loginData = File.ReadAllLines(storage.FilePath).ToList();

            loginData.Distinct();

            if (!loginData.Contains(LastLoginUserId))
            {
                return loginData;
            }

            // ログインアカウントを削除
            loginData.Remove(LastLoginUserId);

            using (StreamWriter outputFile = new StreamWriter(storage.FilePath))
            {
                foreach (string acc in loginData)
                {
                    outputFile.WriteLine(acc);
                }
            }

            return loginData;
        }

        /// <summary>
        /// ログイン中アカウントを取得
        /// </summary>
        /// <returns></returns>
        public static List<string> GetLoginAccount()
        {
            var storage = new DataStorageHelper<SettingData>("loginAccount.dat");
            if (!File.Exists(storage.FilePath))
            {
                var file =  File.Create(storage.FilePath);
                file.Close();
            }

            return File.ReadAllLines(storage.FilePath).ToList();
        }

        /// <summary>
        /// 気に入り商品を取得
        /// </summary>
       public static void GetFavoriteFolder()
       {
            try
            {
                var storage = new DataStorageHelper<SettingData>($"Favorite\\{LastLoginUserId}.json");
                if (!File.Exists(storage.FilePath))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(storage.FilePath));
                    var file = File.Create(storage.FilePath);
                    file.Close();
                    FavoriteFolders = new ObservableCollection<FavoriteFolder>();
                    return;
                }

                FavoriteFolders = JsonConvert.DeserializeObject<ObservableCollection<FavoriteFolder>>(File.ReadAllText(storage.FilePath)) ??  new ObservableCollection<FavoriteFolder>();
                SetFavoriteProduct();
            }
            catch (Exception)
            {
                FavoriteFolders = new ObservableCollection<FavoriteFolder>();
            }
        }

        public static void DeleteFavoriteProduct(string productId)
        {
            var storage = new DataStorageHelper<SettingData>($"Favorite\\{LastLoginUserId}.json");
            if (!File.Exists(storage.FilePath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(storage.FilePath));
                var file = File.Create(storage.FilePath);
                file.Close();
            }
            
            var folder = FavoriteFolders.Where(f => f.FavoriteProducts.Any( p => p.ProductId == productId )).FirstOrDefault();

            var product = folder.FavoriteProducts.Where(p => p.ProductId == productId).FirstOrDefault();

            folder.FavoriteProducts.Remove(product);

            FavoriteFolders = new ObservableCollection<FavoriteFolder>(FavoriteFolders);

            SetFavoriteProduct();

            string jObject = JsonConvert.SerializeObject(FavoriteFolders);

            File.WriteAllText(storage.FilePath, jObject);
        }

        public static void DeleteFavoriteFolder(string folderId)
        {
            var storage = new DataStorageHelper<SettingData>($"Favorite\\{LastLoginUserId}.json");
            if (!File.Exists(storage.FilePath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(storage.FilePath));
                var file = File.Create(storage.FilePath);
                file.Close();
            }


            var folder = FavoriteFolders.Where(f => f.FolderId == folderId).FirstOrDefault();
            FavoriteFolders.Remove(folder);

            folder.FavoriteProducts.ForEach(p => 
            {
                LocalarrayGoods.Where(g => g.g_seqno == p.GSeqno).FirstOrDefault().isbookmark = false;
            });

            string jObject = JsonConvert.SerializeObject(FavoriteFolders);

            File.WriteAllText(storage.FilePath, jObject);
        }

        public static void RenameFavoriteFolder(string folderId, string folderNewName)
        {
            var storage = new DataStorageHelper<SettingData>($"Favorite\\{LastLoginUserId}.json");
            if (!File.Exists(storage.FilePath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(storage.FilePath));
                var file = File.Create(storage.FilePath);
                file.Close();
            }

            var folder = FavoriteFolders.Where(p => p.FolderId == folderId).FirstOrDefault();
            if (folder.FolderName == "未整理フォルダ")
            {
                FavoriteFolders.ForEach(f => f.FolderIndex++);
            }
            folder.FolderName = folderNewName;

            FavoriteFolders = new ObservableCollection<FavoriteFolder>(FavoriteFolders);

           string jObject = JsonConvert.SerializeObject(FavoriteFolders);

            File.WriteAllText(storage.FilePath, jObject);
        }



        public static void MoveFavoriteProduct(string productId, string srcFolderId, string destFolderId)
        {
            var storage = new DataStorageHelper<SettingData>($"Favorite\\{LastLoginUserId}.json");
            if (!File.Exists(storage.FilePath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(storage.FilePath));
                var file = File.Create(storage.FilePath);
                file.Close();
            }
            var srcFolder = FavoriteFolders.Where(f => f.FolderId == srcFolderId).FirstOrDefault();
            var product = srcFolder.FavoriteProducts.Where(p => p.ProductId == productId).FirstOrDefault();
            SaveFavoriteProduct(product, destFolderId);

            srcFolder.FavoriteProducts.Remove(product);

            FavoriteFolders = new ObservableCollection<FavoriteFolder>(FavoriteFolders);

            string jObject = JsonConvert.SerializeObject(FavoriteFolders);

            File.WriteAllText(storage.FilePath, jObject);
        }

        /// <summary>
        /// 気に入り商品を保存
        /// </summary>
        /// <param name="productFavorite"></param>
        public static void SaveFavoriteProduct(FavoriteProduct favoriteProduct, string folderId)
        {
            const string ADJUST_FOLDER = "未整理フォルダ";
            var storage = new DataStorageHelper<SettingData>($"Favorite\\{LastLoginUserId}.json");
            if (!File.Exists(storage.FilePath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(storage.FilePath));
                var file = File.Create(storage.FilePath);
                file.Close();
            }

            FavoriteFolder folder;

            if (folderId == string.Empty)
            {
                // 未調整フォルダが存在しない場合
                if (FavoriteFolders.All(p => p.FolderName != ADJUST_FOLDER))
                {
                    SaveFavoriteFolder(ADJUST_FOLDER);
                }

                folder = FavoriteFolders.Where(p => p.FolderIndex == 0).FirstOrDefault();
                folderId = folder.FolderId;
            }
            else folder = FavoriteFolders.Where(p => p.FolderId == folderId).FirstOrDefault();

            bool duplicateCheck = folder.FavoriteProducts.Where(p => p.ProductId == favoriteProduct.ProductId).Count() > 0 ? true : false;

            // 同じお気に入り商品を同じフォルダに保存する場合
            if (duplicateCheck)
            {
                return;
            }

            int initIndex = 1;
            folder.FavoriteProducts.ForEach(p => { p.ProductIndex = initIndex; initIndex++; });

            favoriteProduct.ProductIndex = 0;
            folder.FavoriteProducts.Add(favoriteProduct);

            folder.FavoriteProducts = new ObservableCollection<FavoriteProduct>(folder.FavoriteProducts.OrderBy(p => p.ProductIndex));

            LocalarrayGoods.Where(g => g.g_seqno == favoriteProduct.GSeqno).FirstOrDefault().isbookmark = true;

            string jObject = JsonConvert.SerializeObject(FavoriteFolders);
            File.WriteAllText(storage.FilePath, jObject);
        }

        public static void SaveFavoriteFolder(string folderName)
        {
            var storage = new DataStorageHelper<SettingData>($"Favorite\\{LastLoginUserId}.json");
            if (!File.Exists(storage.FilePath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(storage.FilePath));
                var file = File.Create(storage.FilePath);
                file.Close();
            }

            int initIndex = 2;
            // 再計算フォルダインデックス
            FavoriteFolders.ForEach(f => { if (f.FolderIndex != 0) f.FolderIndex = initIndex; initIndex++; });

            FavoriteFolders.Add(new FavoriteFolder() 
            {
                FolderId = FavoriteFolder.NewFolderId,
                FolderName = folderName,
                FolderIndex = folderName == "未整理フォルダ" ? 0 : 1,
            });

            FavoriteFolders = new ObservableCollection<FavoriteFolder>(FavoriteFolders.OrderBy(f => f.FolderIndex));

            string jObject = JsonConvert.SerializeObject(FavoriteFolders);
            File.WriteAllText(storage.FilePath, jObject);
        }

        /// <summary>
        /// アプリバージョン
        /// </summary>
        public static string AppVersion
        {
            get
            {
                IDepend dep = DependencyService.Get<IDepend>();
                return dep.GetAppVersion();
            }
        }

        /// <summary>
        /// 利用者認証コード
        /// </summary>
        public static string UserAuthCode
        {
            get { return mSettingData.UserAuthCode; }
            set { mSettingData.UserAuthCode = value; }
        }
        /// <summary>
        /// 最終ログイン店舗コードマーク（識別＋コード）
        /// </summary>
        public static string LastLoginShopMark
        {
            get { return mSettingData.LastLoginShopMark; }
            set { mSettingData.LastLoginShopMark = value; }
        }
        /// <summary>
        /// 最終ログイン店舗名
        /// </summary>
        public static string LastLoginShopName
        {
            get { return mSettingData.LastLoginShopName; }
            set { mSettingData.LastLoginShopName = value; }
        }

        /// <summary>
        /// ログイン中の店舗情報
        /// </summary>
        public static store LoginStore { get; set; }

        /// <summary>
        /// 最終ログインID
        /// </summary>
        public static string LastLoginUserId
        {
            get { return mSettingData.LastLoginUserId; }
            set { mSettingData.LastLoginUserId = value; }
        }
        /// <summary>
        /// 最終データダウンロード時間
        /// </summary>
        public static string LastDownLoadDateTime
        {
            get { return mSettingData.LastDownLoadDateTime; }
            set { mSettingData.LastDownLoadDateTime = value; }
        }
        /// <summary>
        /// 最終画像ダウンロード時間
        /// </summary>
        public static string LastImageDownLoadDateTime
        {
            get { return mSettingData.LastImageDownLoadDateTime; }
            set { mSettingData.LastImageDownLoadDateTime = value; }
        }
        /// <summary>
        /// 最終アプリケーション実行時間
        /// </summary>
        public static string LastApplicationOpenDateTime
        {
            get { return mSettingData.LastApplicationOpenDateTime; }
            set { mSettingData.LastApplicationOpenDateTime = value; }
        }



        public static async Task DownLoadImage(string shopmark, string userCode, IProgress<int> progressReporter, CancellationToken cancelToken)
        {
            //ダウンロード開始
            HttpClient httpClient = new HttpClient();
            Uri uri;
            //Windows.Storage.Streams.IBuffer stream;
            String destination;

            //Goodsデシリアライズ ここに来る時点でキャッシュ済みのはず・・・に掛ける
            //await GetAppDataGoodsXml(shopmark);
            var linqgoods = from g in LocalarrayGoods select g;

            //前回DL以降の差分画像ダウンロード（image_Datetime>=最終DL時間）
            if (string.IsNullOrEmpty(LastImageDownLoadDateTime))
            {
                LastImageDownLoadDateTime = "2015/06/01";
            }
            //訳あり仕様(今日以降が設定されたらリセットする)
            if (DateTime.Today < DateTime.Parse(LastImageDownLoadDateTime))
            {
                LastImageDownLoadDateTime = DateTime.Today.ToString();
            }
            var d = DateTime.Parse(LastImageDownLoadDateTime);
            //200件までとしてみる
            //linqgoods = linqgoods.Where(g => g.image_datetime >= d).OrderBy(o => o.image_datetime);
            linqgoods = linqgoods.Where(g => g.image_datetime >= d).OrderBy(o => o.image_datetime).Take(200);

            var maxCount = linqgoods.Count();
            int count = 0;
            int err = 0;
            foreach (var obj in linqgoods)
            {
                if (obj.image_name == "")
                {
                    break;
                }

                //キャンセル要求確認（キャンセルされた場合はOperationCanceledExceptionが発生）
                cancelToken.ThrowIfCancellationRequested();

                try
                {
                    uri = new Uri(obj.image_serveruri);
                    destination = Path.GetFileName(uri.LocalPath);

                    IDepend dep = DependencyService.Get<IDepend>();
                    string localStoragePath = dep.GetLocalStoragePath();
                    //ファイルの保存先

                    bool isErr = false;
                    await Task.Run(() =>
                    {
                        try
                        {
                            //非同期ダウンロードする
                            using (WebClient wc = new WebClient())
                            {
                                string fileName = Path.GetFileName(obj.image_serveruri);
                                wc.DownloadFile(obj.image_serveruri, localStoragePath + fileName);
                            }
                        }
                        catch (Exception e)
                        {
                            Logger.Inst.Assert(false, e.Message);
                            isErr = true;
                        }
                    });
                    if (isErr)
                    {
                        throw new Exception("DL Error");
                    }

                    //アプリ設定（最終画像DL時間）未来日の場合は更新しない
                    var today = DateTime.Today.AddDays(1).AddTicks(-1);
                    if (obj.image_datetime <= today)
                        AppData.LastImageDownLoadDateTime = obj.image_datetime.ToString();
                    count++;

                    Logger.Inst.WriteLine(((count * 100) / maxCount).ToString() + "%");
                    //進捗表示
                    progressReporter.Report((count * 100) / maxCount);
                }
                catch (FileNotFoundException ex)
                {
                    err++;
                    Logger.Inst.Assert(false, ex.Message);
                    //break;
                    continue;
                }
                catch (Exception ex)
                {
                    err++;
                    Logger.Inst.Assert(false, ex.Message);
                    //break;
                    continue;
                }
            }
        }

        public static async Task DownLoadStore(string userCode)
        {
            Logger.Inst.WriteLine("DownLoadStore Start");
            await DependencyService.Get<IDepend>().DownLoadStore(userCode);
            Logger.Inst.WriteLine("DownLoadStore DlDone");
            await LoadStoreXml();
            Logger.Inst.WriteLine("DownLoadStore End");
        }
        public static async Task DownLoadCurrency(string userCode, bool isForceDl)
        {
            Logger.Inst.WriteLine("DownLoadCurrency Start");
            await DependencyService.Get<IDepend>().DownLoadCurrency(userCode, isForceDl);
            Logger.Inst.WriteLine("DownLoadCurrency DlDone");
            await LoadCurrencyXml();
            Logger.Inst.WriteLine("DownLoadCurrency End");
        }
        public static async Task DownloadGoods(string shopmark, string userCode, bool isForceDl)
        {
            Logger.Inst.WriteLine("DownloadGoods Start");
            await DependencyService.Get<IDepend>().DownloadGoods(userCode, isForceDl);
            Logger.Inst.WriteLine("DownloadGoods DlDone");
            await LoadGoodsXml(shopmark);
            Logger.Inst.WriteLine("DownloadGoods End");
            await SaveAndLoadCategories(); //カテゴリ保存とロード

        }
        public static async Task DownLoadIndex(string userCode, bool isForceDl)
        {
            Logger.Inst.WriteLine("DownLoadIndex Start");
            await DependencyService.Get<IDepend>().DownLoadIndex(userCode, isForceDl);
            Logger.Inst.WriteLine("DownLoadIndex DlDone");
            await LoadIndexXml();
            Logger.Inst.WriteLine("DownLoadIndex End");
        }
        public static async Task DownLoadJoinm(string shopmark, string userCode, bool isForceDl)
        {
            Logger.Inst.WriteLine("DownLoadJoinm Start");
            await DependencyService.Get<IDepend>().DownLoadJoinm(userCode, isForceDl);
            Logger.Inst.WriteLine("DownLoadJoinm DlDone");
            await LoadJoinmXml();
            Logger.Inst.WriteLine("DownLoadJoinm End");
        }
        public static async Task DownLoadLimAcc(string shopmark, string userCode, bool isForceDl)
        {
            Logger.Inst.WriteLine("DownLoadLimAcc Start");
            await DependencyService.Get<IDepend>().DownLoadLimAcc(shopmark, userCode, isForceDl);
            Logger.Inst.WriteLine("DownLoadLimAcc End");
        }
        public static async Task DownLoadOrderHistory(string shopmark, string userCode, bool isForceDl)
        {
            Logger.Inst.WriteLine("DownLoadOrderHistory Start");
            await DependencyService.Get<IDepend>().DownLoadOrderHistory(shopmark, userCode, isForceDl);
            Logger.Inst.WriteLine("DownLoadOrderHistory End");
        }

        /// <summary>
        /// デシリアライズ　店舗データ
        /// </summary>
        public static async Task LoadStoreXml()
        {
            Logger.Inst.WriteLine("LoadStoreXml Start");

            //Storeデシリアライズ
            var storage = new DataStorageHelper<ArrayOfStore>("store.xml");

            //ストレージから読み込み
            LocalarrayStore = await storage.Load();
            Logger.Inst.WriteLine("LoadStoreXml End");

        }
        /// <summary>
        /// デシリアライズ　商品データ
        /// </summary>
        public static async Task LoadGoodsXml(string shopmark)
        {
            Logger.Inst.WriteLine("LoadGoodsXml Start");

            //Goodsデシリアライズ
            var storage = new DataStorageHelper<ArrayOfGoods>("goods.xml");

            //ストレージから読み込み
            LocalarrayGoods = await storage.Load();

            Logger.Inst.WriteLine("LoadGoodsXml LoadDone");

            //ローカルの発注を取得しプロパティにセットする
            var orders = await Order.GetLocalReq(shopmark);
            //全件検査
            foreach (req r in orders.Where(w => w.mkk == shopmark.Substring(0, 1) && w.shopcode == int.Parse(shopmark.Substring(1))))
            {
                SetGoodsOrder(r);
            }
            Logger.Inst.WriteLine("LoadGoodsXml OrderSet Done");

            //先に出荷停止、受注残データを読み込んでおく
            var lims = new DataStorageHelper<ArrayOfLim>("lim.xml", shopmark);
            var accs = new DataStorageHelper<ArrayOfAcc>("acc.xml", shopmark);
            LocalarrayLim = await lims.Load();
            LocalarrayAcc = await accs.Load();

            //goodsデータをループして追加
            foreach (goods g in LocalarrayGoods)
            {
                //廃盤が最優先につき、その他は参照不要
                if (g.isend == false)
                {
                    //引数の店舗マークから店舗を識別とコードに分離
                    string mkk = shopmark.Substring(0, 1);
                    int shopcode = int.Parse(shopmark.Substring(1));

                    //出荷制限データ（p.lmcode==""でない、かつlimに存在したら出荷可能）
                    if (g.p.lmcode == "")
                    {
                        g.islim = false;
                    }
                    else if (LocalarrayLim.Where(w => w.g_seqno == g.p.seqno && w.lmcode == g.p.lmcode && w.mkk == mkk && w.shopcode == shopcode).Count() > 0)
                    {
                        g.lim = LocalarrayLim.Where(w => w.g_seqno == g.p.seqno && w.lmcode == g.p.lmcode && w.mkk == mkk && w.shopcode == shopcode).Single();
                        g.islim = false;

                        //出荷制限かどうか（ヒットしたら可能）
                        DateTime ymd_s;
                        DateTime ymd_e;
                        if (g.lim != null)
                        {
                            //開始日キャスト
                            if (g.lim.ymd_s == "00000000")
                                ymd_s = new DateTime(0001, 1, 1);
                            else
                                ymd_s = new DateTime(int.Parse(g.lim.ymd_s.Substring(0, 4)), int.Parse(g.lim.ymd_s.Substring(4, 2)), int.Parse(g.lim.ymd_s.Substring(6, 2)));
                            //終了日キャスト
                            if (g.lim.ymd_e == "99999999")
                                ymd_e = new DateTime(9999, 12, 31);
                            else
                                ymd_e = new DateTime(int.Parse(g.lim.ymd_e.Substring(0, 4)), int.Parse(g.lim.ymd_e.Substring(4, 2)), int.Parse(g.lim.ymd_e.Substring(6, 2)));
                            //日付判定（ヒットしたら出荷可能 = false）
                            if (ymd_s <= DateTime.Now && ymd_e >= DateTime.Now)
                                g.islim = false;
                            else
                                g.islim = true;
                        }
                    }
                    else
                    {
                        g.islim = true;
                    }
                    //受注残データ
                    if (LocalarrayAcc.Where(w => w.g_seqno == g.g_seqno && w.mkk == mkk && w.shopcode == shopcode).Count() > 0)
                    {
                        var sumacc = new acc();
                        //複数あった場合、まとめる
                        foreach (acc a in LocalarrayAcc.Where(w => w.g_seqno == g.g_seqno && w.mkk == mkk && w.shopcode == shopcode).OrderByDescending(w => w.ymd))
                        {
                            sumacc.id = a.id;
                            sumacc.g_seqno = a.g_seqno;
                            sumacc.ymd = a.ymd;
                            sumacc.qty += a.qty;
                            sumacc.nos_num += a.nos_num;
                        }
                        g.acc = sumacc;
                        g.isacc = true;
                    }
                    //発注可否フラグ(isorderOK)
                    if (g.islim == false)
                    {
                        g.isorderOK = true;
                    }
                    else
                    {
                        g.isorderOK = false;
                    }
                }
                else
                {
                    //廃盤のときは他のプロパティがマークされないように
                    g.islim = false;
                    g.isacc = false;
                    g.isoos = false;
                    g.isorderOK = false;
                }
            }
            Logger.Inst.WriteLine("LoadGoodsXml EtcSet Done");
            Logger.Inst.WriteLine("LoadGoodsXml End");

        }

        ///// <summary>
        ///// デシリアライズ　ローカル発注データ
        ///// </summary>
        //public static async Task GetAppDataLocalReqXml(string shopmark)
        //{
        //    var storage = new DataStorageHelper<ArrayOfOrderhistory>("orderHistory.xml", shopmark);

        //    //ストレージから読み込み
        //    localarrayGoods = await storage.Load();

        //    //ローカルの発注を取得しプロパティにセットする
        //    var orders = await Order.GetLocalReq(shopmark);
        //    //全件検査
        //    foreach (req r in orders.Where(w => w.mkk == shopmark.Substring(0, 1) && w.shopcode == int.Parse(shopmark.Substring(1))))
        //    {
        //        SetGoodsOrder(r);
        //    }
        //    //ローカルの（将来はネットから）お気に入りを取得しプロパティにセットする
        //    var books = await Favorite.GetLocalFavorite(shopmark);
        //    //全件検査
        //    foreach (bookmark b in books.Where(w => w.shopmark == shopmark))
        //    {
        //        SetGoodsBookMark(b);
        //    }
        //}
        ///// <summary>
        ///// デシリアライズ　お気に入りデータ
        ///// </summary>
        //public static async Task GetAppDataGoodsXml(string shopmark)
        //{
        //    //Goodsデシリアライズ
        //    var storage = new DataStorageHelper<ArrayOfGoods>("goods.xml", shopmark);

        //    //ストレージから読み込み
        //    localarrayGoods = await storage.Load();

        //    //ローカルの発注を取得しプロパティにセットする
        //    var orders = await Order.GetLocalReq(shopmark);
        //    //全件検査
        //    foreach (req r in orders.Where(w => w.mkk == shopmark.Substring(0, 1) && w.shopcode == int.Parse(shopmark.Substring(1))))
        //    {
        //        SetGoodsOrder(r);
        //    }
        //    //ローカルの（将来はネットから）お気に入りを取得しプロパティにセットする
        //    var books = await Favorite.GetLocalFavorite(shopmark);
        //    //全件検査
        //    foreach (bookmark b in books.Where(w => w.shopmark == shopmark))
        //    {
        //        SetGoodsBookMark(b);
        //    }
        //}
        /// <summary>
        /// デシリアライズ　インデックスデータ
        /// </summary>
        public static async Task LoadIndexXml()
        {
            Logger.Inst.WriteLine("LoadIndexXml Start");

            var storage = new DataStorageHelper<ArrayOfIndex>("index.xml");

            //ストレージから読み込み
            LocalarrayIndex = await storage.Load();
            Logger.Inst.WriteLine("LoadIndexXml End");
        }
        /// <summary>
        /// デシリアライズ　階層データ
        /// </summary>
        public static async Task LoadJoinmXml()
        {
            Logger.Inst.WriteLine("LoadJoinmXml Start");
            var storage = new DataStorageHelper<ArrayOfJoinm>("joinm.xml");

            //ストレージから読み込み
            LocalarrayJoin = await storage.Load();
            Logger.Inst.WriteLine("LoadJoinmXml End");
        }


        public static async Task LoadDenamesXml()
        {
            Logger.Inst.WriteLine("LoadDenamesXml Start");
            var storage = new DataStorageHelper<List<string>>("denames.xml");

            //ストレージから読み込み
            LocalarrayDename = await storage.Load();
            Logger.Inst.WriteLine("LoadDenamesXml End");
        }
        public static async Task LoadMtnamesXml()
        {
            Logger.Inst.WriteLine("LoadMtnamesXml Start");
            var storage = new DataStorageHelper<List<string>>("mtnames.xml");

            //ストレージから読み込み
            LocalarrayMtname = await storage.Load();
            Logger.Inst.WriteLine("LoadMtnamesXml End");
        }

        public static async Task LoadGname2sXml()
        {
            Logger.Inst.WriteLine("LoadGname2sXml Start");
            var storage = new DataStorageHelper<List<string>>("gname2s.xml");

            //ストレージから読み込み
            LocalarrayGname2 = await storage.Load();
            Logger.Inst.WriteLine("LoadGname2sXml End");
        }

        public static async Task LoadSecName1sXml()
        {
            Logger.Inst.WriteLine("LoadSecName1sXml Start");
            var storage = new DataStorageHelper<List<string>>("secname1s.xml");

            //ストレージから読み込み
            LocalarraySecName1 = await storage.Load();
            Logger.Inst.WriteLine("LoadGname2sXml End");
        }
        public static async Task LoadSecName2sXml()
        {
            Logger.Inst.WriteLine("LoadSecName2sXml Start");

            var storage = new DataStorageHelper<List<string>>("secname2s.xml");

            //ストレージから読み込み
            LocalarraySecName2 = await storage.Load();
            Logger.Inst.WriteLine("LoadSecName2sXml End");

        }
        public static async Task LoadSecName3sXml()
        {
            Logger.Inst.WriteLine("LoadSecName3sXml Start");
            var storage = new DataStorageHelper<List<string>>("secname3s.xml");

            //ストレージから読み込み
            LocalarraySecName3 = await storage.Load();
            Logger.Inst.WriteLine("LoadSecName3sXml End");

        }


        /// <summary>
        /// デシリアライズ　通貨データ
        /// </summary>
        public static async Task LoadCurrencyXml()
        {
            Logger.Inst.WriteLine("LoadCurrencyXml Start");
            //デシリアライズ
            var storage = new DataStorageHelper<ArrayOfCurrency>("currency.xml");

            //ストレージから読み込み
            LocalarrayCurrency = await storage.Load();
            Logger.Inst.WriteLine("LoadCurrencyXml End");
        }

        /// <summary>
        /// お気に入りプロパティにセット
        /// </summary>
        public static void SetGoodsBookMarkInit(bookmark b)
        {
            //対象商品検査
            foreach (goods g in LocalarrayGoods.Where(w => w.g_seqno == b.goods.g_seqno))
            {
                g.isbookmark = false;
                g.bookmark = null;
            }
        }

        #region 発注数上書き
        ///// <summary>
        ///// 発注済みプロパティにセット＆加算（存在する場合は数量加算）
        ///// </summary>
        //public static void SetGoodsOrder(req r)
        //{
        //    //対象商品検査
        //    foreach (goods g in localarrayGoods.Where(w => w.g_seqno == r.goods.g_seqno))
        //    {
        //        //発注済みプロパティ
        //        g.isorder = true;
        //        //上書き加算or新規セット
        //        if (g.req == null)
        //        {
        //            g.req = r;
        //        }
        //        else
        //        {
        //            r.qty += g.req.qty;
        //            g.req = r;
        //        }
        //    }
        //}

        static void SetFavoriteProduct()
        {
            foreach (goods g in LocalarrayGoods)
            {
                g.isbookmark = FavoriteFolders.Any(f => f.FavoriteProducts.Any(p => p.GSeqno == g.g_seqno)) ? true : false;
            }
        }

        /// <summary>
        /// 発注済みプロパティにセット
        /// </summary>
        public static void SetGoodsOrder(req r)
        {
            //対象商品検査
            foreach (goods g in LocalarrayGoods.Where(w => w.g_seqno == r.goods.g_seqno))
            {
                //発注済みプロパティ
                g.isorder = true;
                //発注エンティティ
                g.req = r;
            }
        }
        #endregion

        /// <summary>
        /// 発注済みプロパティを初期化
        /// </summary>
        public static void SetGoodsOrderInit(req r)
        {
            //対象商品検査
            foreach (goods g in LocalarrayGoods.Where(w => w.g_seqno == r.goods.g_seqno))
            {
                //発注済みプロパティ
                g.isorder = false;
                //発注エンティティ
                g.req = null;
            }
        }

        /// <summary>
        /// シリアル化してクローンを作成
        /// </summary>
        /// <param name="g">元のgoodsインスタンス</param>
        /// <returns>返す新しいgoodsインスタンス</returns>
        public static goods GetGoodsClone(goods g)
        {
            // シリアル化した内容を保持しておくためのMemoryStreamを作成
            using (MemoryStream stream = new MemoryStream())
            {
                // バイナリシリアル化を行うためのフォーマッタを作成
                var serializer = new DataContractSerializer(typeof(goods));

                // 現在のインスタンスをシリアル化してMemoryStreamに格納
                serializer.WriteObject(stream, g);

                // ストリームの位置を先頭に戻す
                stream.Position = 0;

                // MemoryStreamに格納されている内容を逆シリアル化する
                var ret = (goods)serializer.ReadObject(stream);

                //循環する可能性のあるプロパティを初期化
                ret.req = null;
                ret.bookmark = null;

                //商品を戻す
                return ret;

            }
        }


        private static async Task SaveAndLoadCategories()
        {
            Logger.Inst.WriteLine("SaveAndLoadCategories Start");

            List<string> denames = new List<string>();  //形状
            List<string> mtnames = new List<string>();  //素材
            List<string> gname2s = new List<string>();
            List<string> secName1s = new List<string>();
            List<string> secName2s = new List<string>();
            List<string> secName3s = new List<string>();
            foreach (var goods in LocalarrayGoods)
            {
                if (string.IsNullOrEmpty(goods.p.dename) == false) denames.Add(goods.p.dename);
                if (string.IsNullOrEmpty(goods.p.mtname) == false) mtnames.Add(goods.p.mtname);
                if (string.IsNullOrEmpty(goods.c.gname2) == false) gname2s.Add(goods.c.gname2);
                if (string.IsNullOrEmpty(goods.p.secname1) == false) secName1s.Add(goods.c.gname2);
                if (string.IsNullOrEmpty(goods.p.secname2) == false) secName2s.Add(goods.c.gname2);
                if (string.IsNullOrEmpty(goods.p.secname3) == false) secName3s.Add(goods.c.gname2);
            }
            LocalarrayDename = denames.Distinct().ToList();
            LocalarrayDename.Sort();

            LocalarrayMtname = mtnames.Distinct().ToList();
            LocalarrayMtname.Sort();

            LocalarrayGname2 = gname2s.Distinct().ToList();
            LocalarrayGname2.Sort();

            LocalarraySecName1 = secName1s.Distinct().ToList();
            LocalarraySecName1.Sort();

            LocalarraySecName2 = secName1s.Distinct().ToList();
            LocalarraySecName2.Sort();

            LocalarraySecName3 = secName1s.Distinct().ToList();
            LocalarraySecName3.Sort();

            var storage = new DataStorageHelper<List<string>>("denames.xml");
            await storage.Save(LocalarrayDename);

            storage = new DataStorageHelper<List<string>>("mtnames.xml");
            await storage.Save(LocalarrayMtname);

            storage = new DataStorageHelper<List<string>>("gname2s.xml");
            await storage.Save(LocalarrayGname2);

            storage = new DataStorageHelper<List<string>>("secname1s.xml");
            await storage.Save(LocalarraySecName1);

            storage = new DataStorageHelper<List<string>>("secname2s.xml");
            await storage.Save(LocalarraySecName2);

            storage = new DataStorageHelper<List<string>>("secname3s.xml");
            await storage.Save(LocalarraySecName3);

            Logger.Inst.WriteLine("SaveAndLoadCategories End");

        }
    }

    public class SettingData
    {
        public string UserAuthCode { get; set; } = string.Empty;
        public string LastLoginShopMark { get; set; } = string.Empty;
        public string LastLoginShopName { get; set; } = string.Empty;
        public string LastLoginUserId { get; set; } = string.Empty;
        public string LastDownLoadDateTime { get; set; } = string.Empty;
        public string LastImageDownLoadDateTime { get; set; } = string.Empty;
        public string LastApplicationOpenDateTime { get; set; } = string.Empty;


        public void Init()
        {
            UserAuthCode = string.Empty;
            LastLoginShopMark = string.Empty;
            LastLoginShopName = string.Empty;
            LastLoginUserId = string.Empty;
            LastDownLoadDateTime = string.Empty;
            LastImageDownLoadDateTime = string.Empty;
            LastApplicationOpenDateTime = string.Empty;
        }

    }
}
