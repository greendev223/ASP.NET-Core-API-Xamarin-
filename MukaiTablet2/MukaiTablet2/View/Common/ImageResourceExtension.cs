using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MukaiTablet2.View.Common
{
    [ContentProperty("Source")]

    public class ImageResourceExtension : IMarkupExtension
    {
        // XAMLコードで設定する属性（今回はクラスにContentProperty属性も指定する）
        public string Source { get; set; }

        // IMarkupExtensionインタフェースの実装
        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Source == null) return null;
            return ImageSource.FromResource(Source,GetType().Assembly);
        }
    }
}
