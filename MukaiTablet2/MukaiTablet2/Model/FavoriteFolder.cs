using MukaiTablet2.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Text;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MukaiTablet2.Model
{
    public class FavoriteProduct
    {
        [JsonIgnore]
        public ObservableCollection<string> ListBind { get; set; } = new ObservableCollection<string>();

        private string productId;
        public string ProductId { get { return productId; } set { productId = value; ListBind.Clear(); ListBind.Add(value); } }

        public int ProductIndex { get; set; }

        public int GSeqno { get; set; }

        public string GCode { get; set; }

        public string MakerNo { get; set; }

        public int Vecode { get; set; }

        public string DpGname1 { get; set; }

        public string image_name { get; set; }

        public string image_serveruri { get; set; }

        public decimal Upprice { get; set; }

        [JsonIgnore]
        public string ProductCode { get { return $"{GCode}<{MakerNo}>({Vecode})"; } }

        [JsonIgnore]
        public ImageSource Image
        {
            get
            {
                ImageSource ret = null;
                IDepend dep = DependencyService.Get<IDepend>();
                string path = dep.GetLocalStoragePath() + image_name;

                //ローカルにファイルがなくネットワークに接続されていればダウンロードする。
                if (File.Exists(path) == false && Connectivity.NetworkAccess == NetworkAccess.Internet)
                {
                    try
                    {
                        using (WebClient wc = new WebClient())
                        {
                            wc.DownloadFile(image_serveruri, path);
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.Inst.WriteLine($"dl error {image_serveruri}: {e.Message} ");
                    }
                }
                //ファイルがあれば採用
                if (File.Exists(path))
                {
                    ret = ImageSource.FromFile(path);
                }
                return ret;
            }
        }

        [JsonIgnore]
        public static string NewProductId { get { return Guid.NewGuid().ToString(); } }
    }

    public class FavoriteFolder
    {
       
        public string FolderName { get; set; }

        public string FolderId { get; set; }


        public int FolderIndex { get; set; }

        [JsonIgnore]
        public int FavoriteProductCount { get { return FavoriteProducts.Count; } }

        public ObservableCollection<FavoriteProduct> FavoriteProducts { get; set; } = new ObservableCollection<FavoriteProduct>();

        [JsonIgnore]
        public static string NewFolderId { get { return Guid.NewGuid().ToString(); } }
    }
}
