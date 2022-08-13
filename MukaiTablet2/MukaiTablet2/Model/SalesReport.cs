using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MukaiTablet2.Model;
using MukaiTablet2.MukaiWebService;
using MukaiTablet2.Util;

namespace MukaiTablet.Model
{
    /// <summary>
    /// 売上日報のBL
    /// </summary>
    class SalesReport
    {

        /// <summary>
        /// 対象年月
        /// </summary>
        public DateTime TargetMonth { get; set; }

        /// <summary>
        /// 売上日報のリスト
        /// </summary>
        public ArrayOfSal Sales { get; private set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SalesReport()
        {

        }
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="targetMonth">対象年月</param>
        public SalesReport(DateTime targetMonth)
        {
            this.TargetMonth = targetMonth;
        }

        /// <summary>
        /// ストレージ上のファイル名
        /// </summary>
        /// <param name="month">対象年月を表すDateTime</param>
        /// <returns>ストレージ上のファイル名</returns>
        public static string StorageFileName(DateTime month)
        {
            return string.Format("sal{0}.xml", month.ToString("yyyyMM"));
        }

        /// <summary>
        /// 対象年月の一か月の日付リスト
        /// </summary>
        /// <returns>日付のList</returns>
        private List<DateTime> GetDays()
        {
            var startDate = new DateTime(this.TargetMonth.Year, this.TargetMonth.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            return Enumerable.Range(0, 31).Select(d => startDate.AddDays(d)).Where(d => d <= endDate).ToList();
        }

        /// <summary>
        /// salクラスの初期化
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public sal GetInitSal(DateTime date)
        {
            //店舗情報から外税内税情報(0:税抜き／1:税込み)取得
            var store = AppData.LoginStore;
            int acc_tax = 0;
            if (store != null)
            {
                if (!int.TryParse(store.opt_1, out acc_tax))
                {
                    acc_tax = 0;
                }
            }

            //salクラスの初期化
            var sal = new sal();
            sal.mkk = AppData.LoginStore.mkk;
            sal.shopcode = AppData.LoginStore.shopcode;
            sal.yymm = this.TargetMonth.ToString("yyyyMM");
            sal.acc_tax = acc_tax;
            sal.sysymd = DateTime.Now;
            sal.pcname = new System.Net.NetworkCredential().Domain;
            sal.h_opt1 = string.Empty;
            sal.h_opt2 = string.Empty;
            sal.ymd = date;
            sal.daysale = 0;
            sal.opt1 = string.Empty;
            sal.opt2 = "\"" + DateTime.Now.ToString() + "\"";
            sal.t_opt1 = string.Empty;
            sal.t_opt2 = string.Empty;
            sal.holiday = 0;
            sal.valid = 1;
            return sal;
        }

        /// <summary>
        /// 追加
        /// </summary>
        public async Task Add(sal input)
        {
            var storage = new DataStorageHelper<ArrayOfSal>(StorageFileName(this.TargetMonth), AppData.LastLoginShopMark);

            //ストレージから読み込み
            this.Sales = await storage.Load();

            //常に１か月分を作成して追加
            var dateList = this.GetDays();
            foreach (var date in dateList)
            {
                var sal = this.Sales.FirstOrDefault(s => s.ymd == date);
                if (sal == null)
                {
                    //新規追加
                    sal = (date == input.ymd) ? input : this.GetInitSal(date);
                    this.Sales.Add(sal);
                }
                else
                {
                    //作成済みの場合は、同一日付なら入力値で置き換える
                    if (sal.ymd == input.ymd)
                    {
                        sal.mkk = input.mkk;
                        sal.shopcode = input.shopcode;
                        sal.yymm = input.yymm;
                        sal.acc_tax = input.acc_tax;
                        sal.sysymd = input.sysymd;
                        sal.pcname = input.pcname;
                        sal.h_opt1 = input.h_opt1;
                        sal.h_opt2 = input.h_opt2;
                        sal.ymd = input.ymd;
                        sal.daysale = input.daysale;
                        sal.opt1 = input.opt1;
                        sal.opt2 = input.opt2;
                        sal.t_opt1 = input.t_opt1;
                        sal.t_opt2 = input.t_opt2;
                        sal.holiday = input.holiday;
                        sal.valid = input.valid;
                    }
                }
            }
            //累計計算
            Recalc_sumsale();

            //店舗情報から外税内税情報(0:税抜き／1:税込み)取得および[0]に設定
            var store = AppData.LoginStore;
            int acc_tax = 0;
            if (store != null)
            {
                if (!int.TryParse(store.opt_1, out acc_tax))
                {
                    acc_tax = 0;
                }
            }
            if (this.Sales != null)
            {
                this.Sales[0].acc_tax = acc_tax;
            }

            //ストレージに書き込み
            await storage.Save(this.Sales);
        }

        /// <summary>
        /// ローカルに保存中の売上日報データからリスト読み込み
        /// </summary>
        /// <returns>売上日報のリスト</returns>
        public async Task LoadLocalData()
        {
            var storage = new DataStorageHelper<ArrayOfSal>(StorageFileName(this.TargetMonth), AppData.LastLoginShopMark);

            //ストレージから読み込み
            this.Sales = await storage.Load();

            //ストレージに保存していない場合は新規作成
            if (this.Sales.Count() != 0) return;
            var dateList = this.GetDays();
            foreach (var date in dateList)
            {
                //新規追加
                var sal = this.GetInitSal(date);
                this.Sales.Add(sal);
            }
            //累計計算
            Recalc_sumsale();
        }

        /// <summary>
        /// ローカルに保存中の売上日報データを削除し、リストを初期化する
        /// </summary>
        /// <returns></returns>
        public async Task DeleteLocalData()
        {
            DataStorageHelper<ArrayOfSal>.Delete(StorageFileName(this.TargetMonth), AppData.LastLoginShopMark);
            await LoadLocalData();
        }

        /// <summary>
        /// リストの累計を再計算する
        /// </summary>
        /// <returns></returns>
        public void Recalc_sumsale()
        {
            decimal calc = 0;
            foreach (var sal in this.Sales)
            {
                calc += sal.daysale;
                sal.sumsale = calc;
            }
        }
    }
}
