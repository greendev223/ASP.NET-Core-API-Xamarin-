using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using MukaiTablet2.MukaiWebService;
using MukaiTablet2.Util;

using System.IO;


namespace MukaiTablet2.Model
{
    class Order
    {
        /// <summary>
        /// ストレージ上のファイル名
        /// </summary>
        const string DATA_FILE_NAME = "order.xml";

        /// <summary>
        /// 発注カートの内容
        /// </summary>
        public static ArrayOfReq LocalArrayReq;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Order()
        {
        }
        /// <summary>
        /// ローカルに保存中の発注データを取得
        /// </summary>
        /// <param name="shopmark">店舗マーク</param>
        public static async Task<ArrayOfReq> GetLocalReq(string shopmark)
        {
            //Orderデシリアライズ
            var storage = new DataStorageHelper<ArrayOfReq>(DATA_FILE_NAME, shopmark);

            //ストレージから読み込み
            LocalArrayReq = await storage.Load();
            
            return LocalArrayReq;
        }
        /// <summary>
        /// ローカル発注データの初期化（発注送信後に呼ばれる）
        /// </summary>
        public static async Task Init()
        {
            //削除
            DataStorageHelper<ArrayOfReq>.Delete(DATA_FILE_NAME, AppData.LastLoginShopMark);

            //発注カート内容をクリアし、ストレージに空で作成
            LocalArrayReq = new MukaiWebService.ArrayOfReq();
            var storage = new DataStorageHelper<ArrayOfReq>(DATA_FILE_NAME, AppData.LastLoginShopMark);
            await storage.Save(LocalArrayReq);
        }
        /// <summary>
        /// ローカル発注データへ追加
        /// </summary>
        /// <param name="req">発注データ１件</param>
        public static async Task Add(req req)
        {
            //Orderデシリアライズ
            var storage = new DataStorageHelper<ArrayOfReq>(DATA_FILE_NAME, AppData.LastLoginShopMark);

            //ストレージから読み込み
            LocalArrayReq = await storage.Load();

            #region 発注数上書き
            ////追加
            //r.guid = Guid.NewGuid().ToString();
            //localarrayReq.Add(r);

            //同一アイテムが有ったら置き換える
            var sameReq = LocalArrayReq.FirstOrDefault(x => x.goods.g_seqno == req.goods.g_seqno);
            if (sameReq == null)
            {
                //無ければ新規追加
                req.guid = Guid.NewGuid().ToString();
                LocalArrayReq.Add(req);

            }
            else
            {
                //置き換え
                req.guid = sameReq.guid;
                LocalArrayReq.Insert(LocalArrayReq.IndexOf(sameReq), req);
                LocalArrayReq.Remove(sameReq);
            }
            #endregion

            //シリアライズ
            await storage.Save(LocalArrayReq);
            
        }
        /// <summary>
        /// ローカル発注データから削除
        /// </summary>
        /// <param name="g">発注１件</param>
        public static async Task Delete(req r)
        {
            //Orderデシリアライズ
            var storage = new DataStorageHelper<ArrayOfReq>(DATA_FILE_NAME, AppData.LastLoginShopMark);

            //ストレージから読み込み
            LocalArrayReq = await storage.Load();

            //削除（guidで探す）
            for (int i = 0; i < LocalArrayReq.Count; i++)
            {
                if (LocalArrayReq[i].guid == r.guid)
                    LocalArrayReq.RemoveAt(i);
            }

            //シリアライズ
            await storage.Save(LocalArrayReq);
        }
        /// <summary>
        /// 送信済みの発注取り消し（ローカル発注データへキャンセルデータの追加）
        /// </summary>
        /// <param name="r"></param>
        //public async void Cancel(req r)
        //{
        //    var ret = new ArrayOfReq();

        //}

    }

    public class OrderSummary
    {
        private string _rname;
        public string rname{
            get { return _rname; }
            set { _rname = value; }
        }

        private int _rcode;
        public int rcode
        {
            get { return _rcode; }
            set { _rcode = value; }
        }

        private decimal _qty;
        public decimal qty
        {
            get { return _qty; }
            set { _qty = value; }
        }

        private string _qtystr;
        public string qtystr
        {
            get { return _qtystr; }
            set { _qtystr = value; }
        }

        private decimal _price;
        public decimal price
        {
            get { return _price; }
            set { _price = value; }
        }

        private string _pricestr;
        public string pricestr
        {
            get { return _pricestr; }
            set { _pricestr = value; }
        }

    }
}
