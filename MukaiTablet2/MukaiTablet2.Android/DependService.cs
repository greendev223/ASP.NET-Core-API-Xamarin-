using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;
using MukaiTablet2.Droid;
using System.Threading.Tasks;
using MukaiTablet2.Util;
using System.Xml.Serialization;
using System.IO;
using System.Reflection;
using MukaiTablet2.Model;
using MukaiTablet2.MukaiWebService;

[assembly: Dependency(typeof(DependService))]
namespace MukaiTablet2.Droid
{
    using ArrayOfStore = List<MukaiWebService.store>;
    using ArrayOfCurrency = List<MukaiWebService.currency>;
    using ArrayOfGoods = List<MukaiWebService.goods>;
    using ArrayOfIndex = List<MukaiWebService.index>;
    using ArrayOfJoinm = List<MukaiWebService.joinm>;
    using ArrayOfLim = List<MukaiWebService.lim>;
    using ArrayOfAcc = List<MukaiWebService.acc>;
    using ArrayOfOrderhistory = List<MukaiWebService.orderhistory>;

    public class DependService : IDepend
    {
        private readonly string mLocalStoragePath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal) + "/";

        public string GetAppVersion()
        {
            Context context = Android.App.Application.Context;
            return context.PackageManager.GetPackageInfo(context.PackageName, 0).VersionName;
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
            return mLocalStoragePath + @"log/";
        }

        public string GetSeparator() { return "/"; }

        public async Task DownLoadStore(string userCode,bool isForceDl)
        {

            await Task.Run(async () =>
            {
                try
                {
                    MukaiWebService.WebService client = new MukaiWebService.WebService();
                    MukaiWebService.store[] tmp = client.GetStore(userCode, true);
                    ArrayOfStore stores = new ArrayOfStore(tmp);

                    var xmlAttributes = new XmlAttributes();
                    var xmlAttributeOverrides = new XmlAttributeOverrides();
                    xmlAttributes.Xmlns = false;
                    xmlAttributeOverrides.Add(typeof(MukaiWebService.store), xmlAttributes);

                    var storage = new SoapSerializer<ArrayOfStore>("store.xml");
                    await storage.Serialize(stores, xmlAttributeOverrides);
                }
                catch (Exception e)
                {
                    Logger.Inst.Assert(false, e.Message);
                }
            });
        }
        public async Task DownLoadCurrency(string userCode, bool isForceDl)
        {
            await Task.Run(async () =>
            {
                try
                {
                    MukaiWebService.WebService client = new MukaiWebService.WebService();
                    MukaiWebService.currency[] tmp = client.GetCurrency(userCode, isForceDl);
                    ArrayOfCurrency currency = new ArrayOfCurrency(tmp);

                    var xmlAttributes = new XmlAttributes();
                    var xmlAttributeOverrides = new XmlAttributeOverrides();
                    xmlAttributes.Xmlns = false;
                    xmlAttributeOverrides.Add(typeof(MukaiWebService.currency), xmlAttributes);

                    var storage = new SoapSerializer<ArrayOfCurrency>("currency.xml");
                    await storage.Serialize(currency, xmlAttributeOverrides);
                }
                catch (Exception e)
                {
                    Logger.Inst.Assert(false, e.Message);
                }
            });
        }

        public async Task DownloadGoods(string userCode,bool isForceDl)
        {
            await Task.Run(async () =>
            {
                try
                {
                    MukaiWebService.WebService client = new MukaiWebService.WebService();
                    MukaiWebService.goods[] tmp = client.GetGoods(userCode, isForceDl);
                    ArrayOfGoods goods = new ArrayOfGoods(tmp);

                    var xmlAttributes = new XmlAttributes();
                    var xmlAttributeOverrides = new XmlAttributeOverrides();
                    xmlAttributes.Xmlns = false;
                    xmlAttributeOverrides.Add(typeof(MukaiWebService.goods), xmlAttributes);
                    xmlAttributeOverrides.Add(typeof(MukaiWebService.goods_p), xmlAttributes);
                    xmlAttributeOverrides.Add(typeof(MukaiWebService.goods_c), xmlAttributes);
                    xmlAttributeOverrides.Add(typeof(MukaiWebService.end), xmlAttributes);
                    xmlAttributeOverrides.Add(typeof(MukaiWebService.oos), xmlAttributes);
                    xmlAttributeOverrides.Add(typeof(MukaiWebService.res), xmlAttributes);
                    xmlAttributeOverrides.Add(typeof(MukaiWebService.lim), xmlAttributes);
                    xmlAttributeOverrides.Add(typeof(MukaiWebService.acc), xmlAttributes);
                    xmlAttributeOverrides.Add(typeof(MukaiWebService.bookmark), xmlAttributes);
                    xmlAttributeOverrides.Add(typeof(MukaiWebService.req), xmlAttributes);

                    /*
                    foreach(var good in goods)
                    {
                        good.p.dename = DebugComplement.SeqNoToDename(good.p.seqno);
                        good.p.mtname = DebugComplement.SeqNoToMtname(good.p.seqno);
                    }
                    */

                    //保存
                    var storage = new SoapSerializer<ArrayOfGoods>("goods.xml");
                    await storage.Serialize(goods, xmlAttributeOverrides);
                }
                catch (Exception e)
                {
                    Logger.Inst.Assert(false, e.Message);
                }
            });
        }

        public async Task DownLoadIndex(string userCode,bool isForceDl)
        {
            await Task.Run(async () =>
            {
                try
                {
                    MukaiWebService.WebService client = new MukaiWebService.WebService();
                    MukaiWebService.index[] tmp = client.GetIndex(userCode, isForceDl);
                    ArrayOfIndex indexes = new ArrayOfIndex(tmp);
                    //保存

                    var xmlAttributes = new XmlAttributes();
                    var xmlAttributeOverrides = new XmlAttributeOverrides();
                    xmlAttributes.Xmlns = false;
                    xmlAttributeOverrides.Add(typeof(MukaiWebService.index), xmlAttributes);

                    var storage = new SoapSerializer<ArrayOfIndex>("index.xml");
                    await storage.Serialize(indexes, xmlAttributeOverrides);
                }
                catch (Exception e)
                {
                    Logger.Inst.Assert(false, e.Message);
                }
            });
        }

        public async Task DownLoadJoinm(string userCode,bool isForceDl)
        {
            await Task.Run(async () =>
            {
                try
                {
                    MukaiWebService.WebService client = new MukaiWebService.WebService();
                    MukaiWebService.joinm[] tmp = client.GetJoinm(userCode, isForceDl);
                    ArrayOfJoinm Joinms = new ArrayOfJoinm(tmp);

                    var xmlAttributes = new XmlAttributes();
                    var xmlAttributeOverrides = new XmlAttributeOverrides();
                    xmlAttributes.Xmlns = false;
                    xmlAttributeOverrides.Add(typeof(MukaiWebService.joinm), xmlAttributes);

                    //保存
                    var storage = new SoapSerializer<ArrayOfJoinm>("joinm.xml");
                    await storage.Serialize(Joinms, xmlAttributeOverrides);
                }
                catch (Exception e)
                {
                    Logger.Inst.Assert(false, e.Message);
                }
            });
        }

        /// <summary>
        /// 出荷停止、受注残のダウンロード
        /// </summary>
        /// <returns></returns>
        public async Task DownLoadLimAcc(string shopmark, string userCode,bool isForceDl)
        {
            await Task.Run(async () =>
            {
                try
                {
                    MukaiWebService.WebService client = new MukaiWebService.WebService();
                    MukaiWebService.lim[] tmp = client.GetLim(shopmark, userCode, isForceDl);
                    ArrayOfLim lim = new ArrayOfLim(tmp);

                    var xmlAttributes = new XmlAttributes();
                    xmlAttributes.Xmlns = false;
                    var xmlAttributeOverrides = new XmlAttributeOverrides();
                    xmlAttributeOverrides.Add(typeof(MukaiWebService.lim), xmlAttributes);

                    //保存
                    var storage = new SoapSerializer<ArrayOfLim>("lim.xml", shopmark);
                    await storage.Serialize(lim, xmlAttributeOverrides);

                    var xmlAttributeOverrides2 = new XmlAttributeOverrides();
                    xmlAttributeOverrides2.Add(typeof(MukaiWebService.acc), xmlAttributes);

                    MukaiWebService.acc[] tmp2 = client.GetAcc(shopmark, userCode, isForceDl);
                    ArrayOfAcc acc = new ArrayOfAcc(tmp2);

                    var storage2 = new SoapSerializer<ArrayOfAcc>("acc.xml", shopmark);
                    await storage2.Serialize(acc, xmlAttributeOverrides2);
                }
                catch (Exception e)
                {
                    Logger.Inst.Assert(false, e.Message);
                }
            });
        }

        /// <summary>
        /// 発注履歴のダウンロード
        /// </summary>
        /// <returns></returns>
        public async Task DownLoadOrderHistory(string shopmark, string userCode,bool isForceDl)
        {
            await Task.Run(async () =>
            {
                try
                {
                    MukaiWebService.WebService client = new MukaiWebService.WebService();
                    MukaiWebService.orderhistory[] tmp = client.GetOrderHistory(shopmark, userCode);
                    ArrayOfOrderhistory history = new ArrayOfOrderhistory(tmp);

                    //保存
                    IDepend dep = DependencyService.Get<IDepend>();
                    string localPath = dep.GetLocalStoragePath();

                    var xmlAttributes = new XmlAttributes();
                    xmlAttributes.Xmlns = false;
                    var xmlAttributeOverrides = new XmlAttributeOverrides();
                    xmlAttributeOverrides.Add(typeof(MukaiWebService.orderhistory), xmlAttributes);
                    xmlAttributeOverrides.Add(typeof(MukaiWebService.goods), xmlAttributes);
                    xmlAttributeOverrides.Add(typeof(MukaiWebService.goods_p), xmlAttributes);
                    xmlAttributeOverrides.Add(typeof(MukaiWebService.goods_c), xmlAttributes);
                    xmlAttributeOverrides.Add(typeof(MukaiWebService.end), xmlAttributes);
                    xmlAttributeOverrides.Add(typeof(MukaiWebService.oos), xmlAttributes);
                    xmlAttributeOverrides.Add(typeof(MukaiWebService.res), xmlAttributes);
                    xmlAttributeOverrides.Add(typeof(MukaiWebService.lim), xmlAttributes);
                    xmlAttributeOverrides.Add(typeof(MukaiWebService.acc), xmlAttributes);
                    xmlAttributeOverrides.Add(typeof(MukaiWebService.bookmark), xmlAttributes);
                    xmlAttributeOverrides.Add(typeof(MukaiWebService.req), xmlAttributes);
                    var storage = new SoapSerializer<ArrayOfOrderhistory>("orderHistory.xml", shopmark);
                    await storage.Serialize(history, xmlAttributeOverrides);
                }
                catch (Exception e)
                {
                    Logger.Inst.Assert(false, e.Message);
                }
            });
        }

        /// <summary>
        /// 先月売上の変更可能日
        /// </summary>
        /// <returns></returns>
        public async Task<int> GetSalUpdateDays()
        {
            int ret = await Task<int>.Run(() =>
            {
                int day = 1;
                try
                {
                    MukaiWebService.WebService client = new MukaiWebService.WebService();
                    day = client.GetSalUpdateDays();
                }
                catch (Exception e)
                {
                    Logger.Inst.Assert(false, e.Message);
                }
                return day;
            });
            return ret;
        }


        public class ArrayOfReq : System.Collections.ObjectModel.ObservableCollection<MukaiWebService.req> { }

        public async Task<bool> ReqOrder(MukaiTablet2.MukaiWebService.ArrayOfReq reqs, string userCode)
        {
            bool ret = false;
            await Task.Run(() =>
            {
                try
                {
                    //型変換
                    MukaiWebService.req[] sendReqs = new MukaiWebService.req[reqs.Count];
                    for (int ii = 0; ii < reqs.Count; ii++)
                    {
                        sendReqs[ii] = ConvPclToDroid(reqs[ii]);
                    }

                    //送信
                    MukaiWebService.WebService client = new MukaiWebService.WebService();
                    var webRet = client.SetReq(sendReqs, userCode);
                    ret = webRet.status;
                }
                catch (Exception e)
                {
                    Logger.Inst.Assert(false, e.Message);
                }
            });
            return ret;
        }

        public async Task<bool> SetSal(ArrayOfSal sals, string userCode)
        {
            bool ret = false;
            await Task.Run(() =>
            {
                try
                {
                    //型変換
                    MukaiWebService.sal[] sendReqs = new MukaiWebService.sal[sals.Count];
                    for (int ii = 0; ii < sals.Count; ii++)
                    {
                        sendReqs[ii] = ConvPclToDroid(sals[ii]);
                    }

                    /*
                    foreach (var item in sendReqs)
                    {
                        Logger.Inst.WriteLine($"ymd:{item.ymd.ToString()} valid:{item.valid} holiday:{item.holiday} daysale:{item.daysale} sumsale:{item.sumsale} ");
                    }
                    */

                    //送信
                    MukaiWebService.WebService client = new MukaiWebService.WebService();
                    var webRet = client.SetSal(sendReqs, userCode);
                    ret = webRet.status;
                }
                catch (Exception e)
                {
                    Logger.Inst.Assert(false, e.Message);
                }
            });
            return ret;
        }

        private static PowerManager.WakeLock mSleepWakeLock = null;
        public void DisabletSleep(bool isDisable)
        {
            if (isDisable)
            {
                if (mSleepWakeLock != null)
                {
                    Logger.Inst.Assert(false, "mSleepWakeLock not release");
                    mSleepWakeLock.Release();
                }

                Context context = Android.App.Application.Context;
                PowerManager powerManager = context.GetSystemService(Context.PowerService) as PowerManager;
                mSleepWakeLock = powerManager.NewWakeLock(WakeLockFlags.ScreenBright | WakeLockFlags.OnAfterRelease, "non_sleep");

                // スリープOFF
                mSleepWakeLock.Acquire();
            }
            else
            {
                if (mSleepWakeLock == null)
                {
                    Logger.Inst.Assert(false, "mSleepWakeLock not Acquire");
                    return;
                }
                mSleepWakeLock.Release();
                mSleepWakeLock = null;
            }

        }

        #region 型変換
        private Droid.MukaiWebService.req ConvPclToDroid(MukaiTablet2.MukaiWebService.req from)
        {
            if (from == null) return null;
            Droid.MukaiWebService.req to = new Droid.MukaiWebService.req();
            to.bkk = from.bkk;
            to.bodycount = from.bodycount;
            to.gcode1 = from.gcode1;
            to.gcode2 = from.gcode2;
            to.goods = ConvPclToDroid(from.goods);
            to.guid = from.guid;
            to.h_id = from.h_id;
            to.h_opt1 = from.h_opt1;
            to.h_opt2 = from.h_opt2;
            to.id = from.id;
            to.mkk = from.mkk;
            to.opt1 = from.opt1;
            to.opt2 = from.opt2;
            to.pcname = from.pcname;
            to.qty = from.qty;
            to.r_id = from.r_id;
            to.shopcode = from.shopcode;
            to.vid = from.vid;
            to.ymd = from.ymd;
            return to;
        }

        private Droid.MukaiWebService.goods ConvPclToDroid(MukaiTablet2.MukaiWebService.goods from)
        {
            if (from == null) return null;
            Droid.MukaiWebService.goods to = new MukaiWebService.goods();
            to.acc = ConvPclToDroid(from.acc);
            to.bookmark = ConvPclToDroid(from.bookmark);
            to.c = ConvPclToDroid(from.c);
            to.dp_gname1 = from.dp_gname1;
            to.dp_gname2 = from.dp_gname2;
            to.end = ConvPclToDroid(from.end);
            to.gcode = from.gcode;
            to.g_seqno = from.g_seqno;
            to.hasChild = from.hasChild;
            to.image_datetime = from.image_datetime;
            to.image_localuri = from.image_localuri;
            to.image_name = from.image_name;
            to.image_serveruri = from.image_serveruri;
            to.isacc = from.isacc;
            to.isbookmark = from.isbookmark;
            to.iscache = from.iscache;
            to.isend = from.isend;
            to.islim = from.islim;
            to.isnew = from.isnew;
            to.isoos = from.isoos;
            to.isorder = from.isorder;
            to.isorderOK = from.isorderOK;
            to.jancode = from.jancode;
            to.jank = from.jank;
            to.lim = ConvPclToDroid(from.lim);
            to.oos = ConvPclToDroid(from.oos);
            to.p = ConvPclToDroid(from.p);
            to.req = ConvPclToDroid(from.req);
            to.res = ConvPclToDroid(from.res);
            to.searchtext = from.searchtext;
            return to;
        }

        private Droid.MukaiWebService.acc ConvPclToDroid(MukaiTablet2.MukaiWebService.acc from)
        {
            if (from == null) return null;
            MukaiWebService.acc to = new MukaiWebService.acc();
            to.g_seqno = from.g_seqno;
            to.id = from.id;
            to.mkk = from.mkk;
            to.nos_num = from.nos_num;
            to.qty = from.qty;
            to.shopcode = from.shopcode;
            to.ymd = from.ymd;
            return to;
        }

        private Droid.MukaiWebService.bookmark ConvPclToDroid(MukaiTablet2.MukaiWebService.bookmark from)
        {
            if (from == null) return null;
            Droid.MukaiWebService.bookmark to = new MukaiWebService.bookmark();
            to.goods = ConvPclToDroid(from.goods);
            to.guid = from.guid;
            to.isbookmark = from.isbookmark;
            to.i_seqno = from.i_seqno;
            to.ordersuryo = from.ordersuryo;
            to.shopmark = from.shopmark;
            to.ymd = from.ymd;
            return to;
        }

        private Droid.MukaiWebService.goods_c ConvPclToDroid(MukaiTablet2.MukaiWebService.goods_c from)
        {
            if (from == null) return null;
            Droid.MukaiWebService.goods_c to = new MukaiWebService.goods_c();
            to.chgdatetime = from.chgdatetime;
            to.entdatetime = from.entdatetime;
            to.gcode1 = from.gcode1;
            to.gcode2 = from.gcode2;
            to.gname2 = from.gname2;
            to.gname2_e = from.gname2_e;
            to.id = from.id;
            to.jancode = from.jancode;
            to.jank = from.jank;
            to.seqno = from.seqno;
            return to;
        }

        private Droid.MukaiWebService.end ConvPclToDroid(MukaiTablet2.MukaiWebService.end from)
        {
            if (from == null) return null;
            Droid.MukaiWebService.end to = new MukaiWebService.end();
            to.g_seqno = to.g_seqno;
            to.id = from.id;
            to.ymd = from.ymd;
            return to;
        }

        private Droid.MukaiWebService.lim ConvPclToDroid(MukaiTablet2.MukaiWebService.lim from)
        {
            if (from == null) return null;
            Droid.MukaiWebService.lim to = new MukaiWebService.lim();
            to.g_seqno = from.g_seqno;
            to.id = from.id;
            to.lmcode = from.lmcode;
            to.mkk = from.mkk;
            to.shopcode = from.shopcode;
            to.ymd_e = from.ymd_e;
            to.ymd_s = from.ymd_s;
            return to;
        }
        private Droid.MukaiWebService.oos ConvPclToDroid(MukaiTablet2.MukaiWebService.oos from)
        {
            if (from == null) return null;
            Droid.MukaiWebService.oos to = new MukaiWebService.oos();
            to.g_seqno = from.g_seqno;
            to.id = from.id;
            to.ymd = from.ymd;
            return to;
        }
        private Droid.MukaiWebService.goods_p ConvPclToDroid(MukaiTablet2.MukaiWebService.goods_p from)
        {
            if (from == null) return null;
            Droid.MukaiWebService.goods_p to = new MukaiWebService.goods_p();
            to.chgdatetime = from.chgdatetime;
            to.clcode1 = from.clcode1;
            to.clcode2 = from.clcode2;
            to.clname1 = from.clname1;
            to.clname1_e = from.clname1_e;
            to.clname2 = from.clname2;
            to.clname2_e = from.clname2_e;
            to.decode = from.decode;
            to.dename = from.dename;
            to.dtk = from.dtk;
            to.entdatetime = from.entdatetime;
            to.g2cnt = from.g2cnt;
            to.gcode1 = from.gcode1;
            to.gname1 = from.gname1;
            to.gname1_e = from.gname1_e;
            to.id = from.id;
            to.jancode = from.jancode;
            to.jank = from.jank;
            to.jpg_no = from.jpg_no;
            to.jpg_size = from.jpg_size;
            to.jpg_time = from.jpg_time;
            to.jpg_ymd = from.jpg_ymd;
            to.kicode = from.kicode;
            to.kiname = from.kiname;
            to.kiname_e = from.kiname_e;
            to.lmcode = from.lmcode;
            to.makerno = from.makerno;
            to.mtcode = from.mtcode;
            to.mtname = from.mtname;
            to.net = from.net;
            to.netname = from.netname;
            to.page_no = from.page_no;
            to.rcode = from.rcode;
            to.rname = from.rname;
            to.rname_e = from.rname_e;
            to.seccode1 = from.seccode1;
            to.seccode2 = from.seccode2;
            to.seccode3 = from.seccode3;
            to.secname1 = from.secname1;
            to.secname2 = from.secname2;
            to.secname3 = from.secname3;
            to.seqno = from.seqno;
            to.ship_ent = from.ship_ent;
            to.tname = from.tname;
            to.tname_e = from.tname_e;
            to.upprice = from.upprice;
            to.upprice_e = from.upprice_e;
            to.uremark = from.uremark;
            to.uremark_e = from.uremark_e;
            to.vecode = from.vecode;
            to.xtxt1 = from.xtxt1;
            return to;
        }
        private Droid.MukaiWebService.res ConvPclToDroid(MukaiTablet2.MukaiWebService.res from)
        {
            if (from == null) return null;
            Droid.MukaiWebService.res to = new MukaiWebService.res();
            to.ave_qty = from.ave_qty;
            to.avg_string = from.avg_string;
            to.g_seqno = from.g_seqno;
            to.id = from.id;
            to.shopnum = from.shopnum;
            return to;
        }

        private Droid.MukaiWebService.sal ConvPclToDroid(MukaiTablet2.MukaiWebService.sal from)
        {
            if (from == null) return null;
            var to = new MukaiWebService.sal();
            to.acc_tax = from.acc_tax;
            to.bodycount = from.bodycount;
            to.daysale = from.daysale;
            to.guid = from.guid;
            to.holiday = from.holiday;
            to.h_id = from.h_id;
            to.h_opt1 = from.h_opt1;
            to.h_opt2 = from.h_opt2;
            to.id = from.id;
            to.mkk = from.mkk;
            to.opt1 = from.opt1;
            to.opt2 = from.opt2;
            to.pcname = from.pcname;
            to.shopcode = from.shopcode;
            to.sumsale = from.sumsale;
            to.sysymd = from.sysymd;
            to.t_id = from.t_id;
            to.t_opt1 = from.t_opt1;
            to.t_opt2 = from.t_opt2;
            to.valid = from.valid;
            to.ymd = from.ymd;
            to.yymm = from.yymm;
            return to;
       }


        #endregion

    }

        /// <summary>
        /// Android SOAP用専用シリアライザ(XMLNSを削除するため)
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        class SoapSerializer<T1> where T1 : class, new()
    {
        /// <summary>
        /// ファイル名
        /// </summary>
        private string mFilePath { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SoapSerializer(string fileName)
        {
            IDepend dep = DependencyService.Get<IDepend>();
            string path = dep.GetLocalStoragePath() + fileName;
            this.mFilePath = path;
        }

        public SoapSerializer(string fileName, string shopmark)
        {
            IDepend dep = DependencyService.Get<IDepend>();
            string sep = dep.GetSeparator();
            string path = dep.GetLocalStoragePath() + shopmark + sep + fileName;
            this.mFilePath = path;
            string dirName = System.IO.Path.GetDirectoryName(path);
            if (Directory.Exists(dirName) == false) Directory.CreateDirectory(dirName);
        }

        /// <summary>
        /// 指定したオブジェクトをデータストアに書き込む
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public async Task Serialize(T1 obj, XmlAttributeOverrides overrides)
        {
            await Task.Run(() =>
            {
                try
                {
                    //Xmlns属性を削除しなければ、デシリアライズできないため
                    var serializer = new XmlSerializer(typeof(T1), overrides);
                    using (StreamWriter sw = new StreamWriter(mFilePath))
                    {
                        serializer.Serialize(sw, obj);
                    }
                }
                catch (Exception e)
                {
                    Logger.Inst.Assert(false, e.Message);
                }
            });
        }
    }

}