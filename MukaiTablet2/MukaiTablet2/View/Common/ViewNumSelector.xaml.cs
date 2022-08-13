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
    public partial class ViewNumSelector : ContentView
    {

        public object Num { get; set; }

        public object CommandMinus { get; set; }
        public object CommandPlus { get; set; }

        public ViewNumSelector()
        {
            InitializeComponent();
        }

        protected override void OnParentSet()
        {
            base.OnParentSet();

            {
                if (Num is Binding) LblNum.SetBinding(Label.TextProperty, (Binding)Num);
            }
            if (CommandMinus != null)
            {
                if (CommandMinus is Binding) BtnMinus.SetBinding(Button.CommandProperty, (Binding)CommandMinus);
            }
            if (CommandPlus != null)
            {
                if (CommandPlus is Binding) BtnPlus.SetBinding(Button.CommandProperty, (Binding)CommandPlus);
            }

        }


    }
}