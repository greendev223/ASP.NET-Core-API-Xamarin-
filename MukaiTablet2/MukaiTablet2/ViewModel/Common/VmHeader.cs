using System;
using System.Collections.Generic;
using System.Text;

namespace MukaiTablet2.ViewModel.Common
{
    public class VmHeader : VmBase
    {

        public string Title
        {
            get { return _title; }
            set { _title = value; NotifyPropertyChanged(); }
        }
        private string _title = string.Empty;

        public bool UseSeachButton
        {
            get { return _useSeachButton; }
            set { _useSeachButton = value; NotifyPropertyChanged(); }
        }
        private bool _useSeachButton;

        public bool UseBackButton
        {
            get { return _useBackButton; }
            set { _useBackButton = value; NotifyPropertyChanged(); }
        }
        private bool _useBackButton;

        public bool UseSubMenu
        {
            get { return _useSubMenu; }
            set { _useSubMenu = value; NotifyPropertyChanged(); }
        }
        private bool _useSubMenu;

        public bool UseAddButton
        {
            get { return _useAddButton; }
            set { _useAddButton = value; NotifyPropertyChanged(); }
        }
        private bool _useAddButton;

        public event EventHandler OnBackButton_Clicked;
        public event EventHandler OnSeachButton_Clicked;
        public event EventHandler OnSubMenu_Clicked;
        public event EventHandler OnAddButton_Clicked;

        public VmHeader() { }
        public VmHeader(string title, bool useSearchButton = false, bool useBackButton = true,bool useSubMenu = false, bool useAddButton = false)
        {
            UseBackButton = useBackButton;
            UseSeachButton = useSearchButton;
            UseSubMenu = useSubMenu;
            UseAddButton = useAddButton;
            Title = title;
        }

        /// <summary>
        /// 戻るボタン
        /// </summary>
        public DelegateCommand CommandBack
        {
            get { return _commandBack = _commandBack ?? new DelegateCommand(BtnBack_Clicked); }
        }
        private DelegateCommand _commandBack;

        private void BtnBack_Clicked()
        {
            OnBackButton_Clicked?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// 検索ボタン
        /// </summary>
        public DelegateCommand CommandSearch
        {
            get { return _commandSearch = _commandSearch ?? new DelegateCommand(BtnSeach_Clicked); }
        }
        private DelegateCommand _commandSearch;

        private void BtnSeach_Clicked()
        {
            OnSeachButton_Clicked?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// サブメニューボタン
        /// </summary>
        public DelegateCommand CommandSubMenu
        {
            get { return _commandSubMenu = _commandSubMenu ?? new DelegateCommand(BtnSubMenu_Clicked); }
        }
        private DelegateCommand _commandSubMenu;
        private void BtnSubMenu_Clicked()
        {
            OnSubMenu_Clicked?.Invoke(this, new EventArgs());
        }

        public DelegateCommand CommandAdd
        {
            get { return _commandAdd = _commandAdd ?? new DelegateCommand(BtnAdd_Clicked); }
        }
        private DelegateCommand _commandAdd;
        private void BtnAdd_Clicked()
        {
            OnAddButton_Clicked?.Invoke(this, new EventArgs());
        }
    }
}
