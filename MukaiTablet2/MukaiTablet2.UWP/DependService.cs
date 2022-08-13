using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using MukaiTablet2.Util;
using MukaiTablet2.UWP;
using MukaiTablet2.UWP.MukaiWebService;
using MukaiTablet2.Model;

[assembly: Dependency(typeof(DependService))]
namespace MukaiTablet2.UWP
{
    class DependService : IDepend
    {
        private readonly string mLocalStoragePath = Windows.Storage.ApplicationData.Current.LocalFolder.Path + @"\";
        public string GetAppVersion()
        {
            var version = Windows.ApplicationModel.Package.Current.Id.Version;
            return version.Major + "." + version.Minor + "." + version.Build + "." + version.Revision;
        }
        /*
         * ローカルストレージのPATHを取得
         * */
        public string GetLocalStoragePath()
        {

            return mLocalStoragePath;
        }

        public string GetLogDirPath()
        {
            return mLocalStoragePath + @"log\";
        }

        public string GetSeparator()
        {
            return @"\";
        }
        public async Task DownLoadStore(string userCode, bool isForceDl)
        {
            try
            {
                MukaiWebService.WebServiceSoapClient client = new MukaiWebService.WebServiceSoapClient();
                MukaiWebService.GetStoreResponse storeres = await client.GetStoreAsync(userCode, true);
                MukaiWebService.ArrayOfStore stores = storeres.Body.GetStoreResult;

                //保存
                var storage = new DataStorageHelper<MukaiWebService.ArrayOfStore>("store.xml");
                await storage.Save(stores);
            }
            catch (Exception e)
            {
                Logger.Inst.Assert(false, e.Message);
            }
        }
        public async Task DownLoadCurrency(string userCode, bool isForceDl)
        {
            try
            {
                MukaiWebService.WebServiceSoapClient client = new MukaiWebService.WebServiceSoapClient();
                MukaiWebService.GetCurrencyResponse currencyres = await client.GetCurrencyAsync(userCode, isForceDl);
                MukaiWebService.ArrayOfCurrency currency = currencyres.Body.GetCurrencyResult;

                var storage = new DataStorageHelper<MukaiWebService.ArrayOfCurrency>("currency.xml");
                await storage.Save(currency);
            }
            catch (Exception e)
            {
                Logger.Inst.Assert(false, e.Message);
            }
        }

        public async Task DownloadGoods(string userCode, bool isForceDl)
        {
            try
            {
                MukaiWebService.WebServiceSoapClient client = new MukaiWebService.WebServiceSoapClient();
                MukaiWebService.GetGoodsResponse goodsres = await client.GetGoodsAsync(userCode, isForceDl);
                MukaiWebService.ArrayOfGoods goods = goodsres.Body.GetGoodsResult;

                /*
                foreach (var good in goods)
                {
                    good.p.dename = DebugComplement.SeqNoToDename(good.p.seqno);
                    good.p.mtname = DebugComplement.SeqNoToMtname(good.p.seqno);
                }
                */


                //保存
                var storage = new DataStorageHelper<MukaiWebService.ArrayOfGoods>("goods.xml");

                await storage.Save(goods);
            }
            catch (Exception e)
            {
                Logger.Inst.Assert(false, e.Message);
            }
        }

        public async Task DownLoadIndex(string userCode, bool isForceDl)
        {
            try
            {
                MukaiWebService.WebServiceSoapClient client = new MukaiWebService.WebServiceSoapClient();
                MukaiWebService.GetIndexResponse indexres = await client.GetIndexAsync(userCode, isForceDl);
                MukaiWebService.ArrayOfIndex indexes = indexres.Body.GetIndexResult;

                //保存
                var storage = new DataStorageHelper<MukaiWebService.ArrayOfIndex>("index.xml");
                await storage.Save(indexes);
            }
            catch (Exception e)
            {
                Logger.Inst.Assert(false, e.Message);
            }
        }

        public async Task DownLoadJoinm(string userCode, bool isForceDl)
        {
            try
            {
                MukaiWebService.WebServiceSoapClient client = new MukaiWebService.WebServiceSoapClient();
                MukaiWebService.GetJoinmResponse joinmres = await client.GetJoinmAsync(userCode, isForceDl);
                MukaiWebService.ArrayOfJoinm Joinms = joinmres.Body.GetJoinmResult;

                //保存
                var storage = new DataStorageHelper<MukaiWebService.ArrayOfJoinm>("joinm.xml");
                await storage.Save(Joinms);
            }
            catch (Exception e)
            {
                Logger.Inst.Assert(false, e.Message);

            }
        }

        /// <summary>
        /// 出荷停止、受注残のダウンロード
        /// </summary>
        /// <returns></returns>
        public async Task DownLoadLimAcc(string shopmark, string userCode, bool isForceDl)
        {
            try
            {
                MukaiWebService.WebServiceSoapClient client = new MukaiWebService.WebServiceSoapClient();
                MukaiWebService.GetLimResponse lims = await client.GetLimAsync(shopmark, userCode, isForceDl);
                MukaiWebService.ArrayOfLim lim = lims.Body.GetLimResult;

                //保存

                var storage = new DataStorageHelper<MukaiWebService.ArrayOfLim>("lim.xml", shopmark);
                await storage.Save(lim);

                MukaiWebService.GetAccResponse accs = await client.GetAccAsync(shopmark, userCode, true);
                MukaiWebService.ArrayOfAcc acc = accs.Body.GetAccResult;

                var storage2 = new DataStorageHelper<MukaiWebService.ArrayOfAcc>("acc.xml", shopmark);
                await storage2.Save(acc);
            }
            catch (Exception e)
            {
                Logger.Inst.Assert(false, e.Message);
            }
        }

        /// <summary>
        /// 発注履歴のダウンロード
        /// </summary>
        /// <returns></returns>
        public async Task DownLoadOrderHistory(string shopmark, string userCode, bool isForceDl)
        {
            try
            {
                MukaiWebService.WebServiceSoapClient client = new MukaiWebService.WebServiceSoapClient();
                MukaiWebService.GetOrderHistoryResponse historyres = await client.GetOrderHistoryAsync(shopmark, userCode);
                MukaiWebService.ArrayOfOrderhistory history = historyres.Body.GetOrderHistoryResult;

                //保存
                IDepend dep = DependencyService.Get<IDepend>();
                string localPath = dep.GetLocalStoragePath();

                var storage = new DataStorageHelper<MukaiWebService.ArrayOfOrderhistory>("orderHistory.xml", shopmark);
                await storage.Save(history);
            }
            catch (Exception e)
            {
                Logger.Inst.Assert(false, e.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<int> GetSalUpdateDays()
        {
            int ret = 1;
            try
            {
                MukaiWebService.WebServiceSoapClient client = new MukaiWebService.WebServiceSoapClient();
                ret = await client.GetSalUpdateDaysAsync();
            }
            catch (Exception e)
            {
                Logger.Inst.Assert(false, e.Message);
            }
            return ret;

        }


        public async Task<bool> ReqOrder(MukaiTablet2.MukaiWebService.ArrayOfReq reqs, string userCode)
        {
            try
            {
                //一度ファイルに変換
                var storage = new DataStorageHelper<MukaiTablet2.MukaiWebService.ArrayOfReq>("tmpSetReq.xml");
                await storage.Save(reqs);

                //UWP型でロードしなおす
                var storage2 = new DataStorageHelper<MukaiWebService.ArrayOfReq>("tmpSetReq.xml");
                var sendReqs = await storage2.Load();


                MukaiWebService.WebServiceSoapClient client = new MukaiWebService.WebServiceSoapClient();
                var ret = await client.SetReqAsync(sendReqs, userCode);
                return ret.Body.SetReqResult.status;
            }
            catch (Exception e)
            {
                Logger.Inst.Assert(false, e.Message);
            }
            return false;
        }

        public async Task<bool> SetSal(MukaiTablet2.MukaiWebService.ArrayOfSal sals, string userCode)
        {
            try
            {
                //一度ファイルに変換
                var storage = new DataStorageHelper<MukaiTablet2.MukaiWebService.ArrayOfSal>("tmpSetSal.xml");
                await storage.Save(sals);

                //UWP型でロードしなおす
                var storage2 = new DataStorageHelper<MukaiWebService.ArrayOfSal>("tmpSetSal.xml");
                var sendReqs = await storage2.Load();


                MukaiWebService.WebServiceSoapClient client = new MukaiWebService.WebServiceSoapClient();
                var ret = await client.SetSalAsync(sendReqs, userCode);
                return ret.Body.SetSalResult.status;
            }
            catch (Exception e)
            {
                Logger.Inst.Assert(false, e.Message);
            }
            return false;
        }

        public void DisabletSleep(bool isDisable) { }
    }
}
