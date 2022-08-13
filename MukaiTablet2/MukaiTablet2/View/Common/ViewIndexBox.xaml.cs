using MukaiTablet2.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MukaiTablet2.View.Common
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ViewIndexBox : ContentView
    {
        public string MyName { get; set; }
        public Color TagColor { get; set; }

        public event EventHandler Clicked;

        public object BindName { get; set; }
        public object BindColor { get; set; }

        public object BindCommand { get; set; }

        public ViewIndexBox()
        {
            InitializeComponent();
        }

        protected override void OnParentSet()
        {
            base.OnParentSet();

            if (MyName != null) LblName.Text = MyName;
            if (TagColor != null) BvTag.BackgroundColor = TagColor;
            if (Clicked != null) { BtnAccept.Clicked += Clicked; }
            if(BindName != null)
            {
                if (BindName is Binding) LblName.SetBinding(Label.TextProperty, (Binding)BindName);
            }
            if (BindColor != null)
            {
                if (BindColor is Binding) BvTag.SetBinding(Label.BackgroundColorProperty, (Binding)BindColor);
            }
            if (BindCommand != null)
            {
                if (BindCommand is Binding) BtnAccept.SetBinding(Button.CommandProperty, (Binding)BindCommand);
            }

        }
    }
}