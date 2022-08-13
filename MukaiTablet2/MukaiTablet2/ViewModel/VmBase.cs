using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using MukaiTablet2.Util;

namespace MukaiTablet2.ViewModel
{
    public class VmBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler OnInitDone;

        protected bool mIsInit = false;
        public VmBase Parent = null;

        /// <summary>
        /// Viewへのプロパティ変化通知
        /// </summary>
        /// <param name="propertyName"></param>
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        /// <summary>
        /// Viewへのプロパティ変化通知
        /// </summary>
        /// <param name="propertyName"></param>
        protected void NotifyPropertyChanged(string[] propertyNames)
        {
            if (PropertyChanged != null)
            {
                foreach (string propertyName in propertyNames)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            }
        }

        public bool IsBussy
        {
            get
            {
                if (Parent != null) return Parent.IsBussy;　//一番親のIsBussyを取得する。
                return _isBussy;
            }
            set
            {
                if (Parent != null) //一番親のIsBussyをセットする。
                {
                    Parent.IsBussy = value;
                }
                else
                {
                    _isBussy = value;
                    Logger.Inst.WriteLine($"IsBussy:{_isBussy}");
                }
                NotifyPropertyChanged();
            }
        }
        private bool _isBussy;

        public bool IsUWP
        {
            get { return (Device.RuntimePlatform == Device.UWP); }
        }
        public bool IsAndroid
        {
            get { return (Device.RuntimePlatform == Device.Android); }
        }


        public bool IsSmartPhone
        {
            get { return (_isSmartPhone == null) ? false : (bool)_isSmartPhone; }
        }
        static bool? _isSmartPhone = null;

        protected double mPageWidth = 0;
        protected double mPageHeight = 0;



        /// <summary>
        /// VMの生成(画面表示)
        /// </summary>
        /// <param name="propertyName"></param>
        public virtual async Task OnInit(Object param = null)
        {
            await Task.Delay(0);
        }
        /// <summary>
        /// VMの破棄
        /// </summary>
        /// <param name="param"></param>
        public virtual async Task OnTerm(Object param = null)
        {
            await Task.Delay(0);
        }

        /// <summary>
        /// 画面表示
        /// </summary>
        /// <param name="param"></param>
        public virtual async Task OnAppear(Object param = null)
        {
            if (mIsInit == false)
            {
                await OnInit(param);
                mIsInit = true;
                OnInitDone?.Invoke(this, new EventArgs());
            }
        }
        /// <summary>
        /// 画面破棄
        /// </summary>
        public virtual void OnDisappear(Object param = null)
        {

        }

        /// <summary>
        /// ページサイズ変化
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <remarks>PageBaseから呼び出される </remarks>
        public virtual void OnPageSizeChanged(double width, double height)
        {
            mPageHeight = height;
            mPageWidth = width;

            if(_isSmartPhone == null)
            {
                //画面サイズが最初に決定した時に、H/Wいずれかが450を下回っていればスマホとする。
                _isSmartPhone = (mPageHeight < 450 || mPageWidth < 450);
                NotifyPropertyChanged(nameof(IsSmartPhone));
            }
        }


    }
}
