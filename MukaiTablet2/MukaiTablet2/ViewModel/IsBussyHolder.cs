using MukaiTablet2.Util;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MukaiTablet2.ViewModel
{
    public class IsBussyHolder : IDisposable
    {
        private VmBase mVm;
        private bool mIsSet = false;


        public IsBussyHolder(VmBase vm)
        {
            mVm = vm;
        }


        /// <summary>
        /// Setに失敗するとfalseを返す。
        /// </summary>
        /// <returns></returns>
        public async Task<bool> Set()
        {
            lock (this)
            {
                if (mVm.IsBussy)
                {
                    Logger.Inst.WriteLine("IsBussy ignore");
                    return false;
                }
                mVm.IsBussy = true;
                mIsSet = true;
            }
            await Task.Delay(50);   //クルクル表示のためUIタスクを一度明け渡す
            return true;
        }


        public void Dispose()
        {
            if (mIsSet)
            {
                mVm.IsBussy = false;
            }
        }
    }
}
