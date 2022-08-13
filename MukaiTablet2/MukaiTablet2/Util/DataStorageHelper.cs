using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.IO;
using Xamarin.Forms;
using System.Xml;
using System.Xml.Linq;

namespace MukaiTablet2.Util
{

    /// <summary>
    /// データストアのLoad／Saveユーティリティ
    /// </summary>
    /// <typeparam name="T">対象オブジェクトのデータ型</typeparam>
    public class DataStorageHelper<T> where T : class, new()
    {
        /// <summary>
        /// ファイル名
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DataStorageHelper(string fileName)
        {
            IDepend dep = DependencyService.Get<IDepend>();
            string path = dep.GetLocalStoragePath() + fileName;
            this.FilePath = path;
        }

        public DataStorageHelper(string fileName, string shopmark)
        {
            IDepend dep = DependencyService.Get<IDepend>();
            string sep = dep.GetSeparator();
            string path = dep.GetLocalStoragePath() + shopmark + sep + fileName;
            this.FilePath = path;
            string dirName = System.IO.Path.GetDirectoryName(path);
            if (Directory.Exists(dirName) == false) Directory.CreateDirectory(dirName);
        }


        public async Task<T> Load()
        {
            T ret = null;
            //保存先フォルダ
            await Task.Run(() =>
            {
                try
                {
                    //デシリアライズ
                    var serializer = new XmlSerializer(typeof(T));
                    using (StreamReader sr = new StreamReader(FilePath))
                    {
                        ret = (T)serializer.Deserialize(sr);
                    }
                }
                catch (Exception e)
                {
                    Logger.Inst.Assert(false, e.Message);
                    ret = new T();
                }
            });

            return ret;
        }

        /// <summary>
        /// 指定したオブジェクトをデータストアに書き込む
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public async Task Save(T obj)
        {
            await Task.Run(() =>
            {
                try
                {
                    //シリアライズ
                    var serializer = new XmlSerializer(typeof(T));
                    using (StreamWriter sw = new StreamWriter(FilePath))
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
        /// <summary>
        /// ファイルを削除する
        /// </summary>
        /// <param name="fileName">ファイル名</param>
        /// <param name="subFolderName">サブフォルダ名</param>
        /// <returns></returns>
        public static void Delete(string fileName, string subFolderName = "")
        {
            IDepend dep = DependencyService.Get<IDepend>();
            string sep = dep.GetSeparator();
            string path = dep.GetLocalStoragePath() + subFolderName + sep + fileName;

            //ファイルが無かったら抜ける。
            if (File.Exists(path) == false) return;

            try
            {
                File.Delete(path);
            }
            catch (Exception e)
            {
                Logger.Inst.Assert(false, e.Message);
                throw;
            }
        }

        /// <summary>
        /// 現在のファイルを削除する
        /// </summary>
        public void Delete()
        {
            if (File.Exists(FilePath) == false) return;
            try
            {
                File.Delete(FilePath);
            }
            catch (Exception e)
            {
                Logger.Inst.Assert(false, e.Message);
                throw;
            }
        }
    }
}
