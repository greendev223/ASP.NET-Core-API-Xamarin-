using MukaiTablet2.Util;
using MukaiTablet2.ViewModel.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace MukaiTablet2.ViewModel
{
    public class VmItemSelect : VmBase
    {
        #region イベント
        #endregion

        #region プロパティ

        public VmHeader Header
        {
            get { return _header; }
            set { _header = value; NotifyPropertyChanged(); }
        }
        private VmHeader _header;


        public ObservableCollection<SelectItem> Items
        {
            get { return _items; }
            set { _items = value; NotifyPropertyChanged(); }
        }
        private ObservableCollection<SelectItem> _items;


        public int ItemColNum
        {
            get
            {
                if (mPageWidth == 0) return 1;
                int num = (int)Math.Floor(mPageWidth / 250);
                if (num <= 0) num = 1;
                return num;
            }
        }

        #endregion

        #region 外部変数
        #endregion

        #region 内部変数
        #endregion

        #region ライフサイクル
        public VmItemSelect(string title, ObservableCollection<SelectItem> items)
        {
            Items = items;
            _header = new VmHeader(title, useSearchButton: false, useBackButton: true);
            Header.OnBackButton_Clicked += BtnBack_Clicked;
            Header.Parent = this;
        }

        public VmItemSelect() { }

        public override void OnPageSizeChanged(double width, double height)
        {
            base.OnPageSizeChanged(width, height);

            NotifyPropertyChanged(nameof(ItemColNum));
        }
        #endregion


        #region ユーザ操作
        /// <summary>
        /// クリア
        /// </summary>
        public DelegateCommand CommandClear
        {
            get { return _commandClear = _commandClear ?? new DelegateCommand(BtnClear_Clicked); }
        }
        private DelegateCommand _commandClear;

        private async void BtnClear_Clicked()
        {
            using (var bussy = new IsBussyHolder(this))
            {
                if (await bussy.Set() == false) return;

                foreach (var item in Items)
                {
                    item.IsSelected = false;
                }
            }
        }

        /// <summary>
        /// 確定
        /// </summary>
        public DelegateCommand CommandCommit
        {
            get { return _commandCommit = _commandCommit ?? new DelegateCommand(BtnCommit_Clicked); }
        }
        private DelegateCommand _commandCommit;
        public async void BtnCommit_Clicked()
        {
            using (var bussy = new IsBussyHolder(this))
            {
                if (await bussy.Set() == false) return;
                await Navigator.PopModalAsync();
            }
        }

        private async void BtnBack_Clicked(object sender, EventArgs e)
        {
            using (var bussy = new IsBussyHolder(this))
            {
                if (await bussy.Set() == false) return;
                await Navigator.PopModalAsync();
            }
        }
        #endregion

        #region 外部関数
        #endregion

        #region 内部関数
        #endregion



    }


    /// <summary>
    /// アイテムクラス
    /// </summary>
    public class SelectItem :VmBase
    {
        public string Name
        {
            get { return _name; }
            set { _name = value; NotifyPropertyChanged(); }
        }
        private string _name;
        
        public bool IsSelected
        {
            get { return _isSelected; }
            set { _isSelected = value; NotifyPropertyChanged(); }
        }
        private bool _isSelected = false;

        public SelectItem(string name) { Name = name; }
        public SelectItem() { }

    }


}
