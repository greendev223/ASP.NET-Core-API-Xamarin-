using MukaiTablet2.MukaiWebService;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MukaiTablet2
{
    /*
     * OS依存インタフェース
     */
    public interface  IDepend
    {
        string GetAppVersion();
        string GetLocalStoragePath();
        string GetLogDirPath();

        string GetSeparator();
        
        Task DownLoadStore(string userCode,bool isForceDl = false);
        Task DownLoadCurrency(string userCode, bool isForceDl = false);
        Task DownloadGoods(string userCode, bool isForceDl = false);
        Task DownLoadIndex(string userCode, bool isForceDl = false);
        Task DownLoadJoinm(string userCode, bool isForceDl = false);
        Task DownLoadLimAcc(string shopmark, string userCode, bool isForceDl = false);
        Task DownLoadOrderHistory(string shopmark, string userCode, bool isForceDl = false);

        Task<bool> ReqOrder(ArrayOfReq reqs, string userCode);

        //
        Task<int> GetSalUpdateDays();

        Task<bool> SetSal(ArrayOfSal sals, string userCode);

        //Android Only
        void DisabletSleep(bool isDisable);  

    }







}
