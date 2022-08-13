using MukaiTablet2.ViewModel.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace MukaiTablet2.View.Converter
{
    

    class DecimalToMoneyString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is Decimal))
                return null;

            return Decimal.Parse(value.ToString()).ToString("#");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            NumberStyles style = NumberStyles.Number | NumberStyles.AllowLeadingWhite | NumberStyles.AllowCurrencySymbol;
            //CultureInfo culture = new CultureInfo("ja-JP");

            Decimal valueDecimal;
            if (!Decimal.TryParse((string)value, style, culture, out valueDecimal))
            {
                return 0.0;
            }

            return valueDecimal;
        }
    }

    public class AccTaxPresenter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int)
            {
                var acc_tax = (int)value;
                return acc_tax == 1 ? "税込み" : "税抜き";
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BoolTaxPresenter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool)
            {
                var acc_tax = (bool)value;
                return acc_tax ? "税込み" : "税抜き";
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BooleanInverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool)
                return !((bool)value);

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BooleanToString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string trueString = "true";
            string falseString = "false";

            if (parameter is string)
            {
                var parameters = ((string)parameter).Split(';');
                trueString = parameters.GetLowerBound(0) >= 0 ? parameters[0] : trueString;
                falseString = parameters.GetUpperBound(0) >= 1 ? parameters[1] : falseString;
            }

            if (value is bool)
                return ((bool)value) ? trueString : falseString;

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    /// <summary>
    /// 数値からbool値に変換
    /// </summary>
    /// <remarks>
    /// trueに対応する値はparameterで設定
    /// </remarks>
    public class IntgerToBoolean : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var p = int.Parse((string)parameter);

            if ((value is int) && (parameter is string))
                return ((int)value == p) ? true : false;

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //if ((value is bool) && (bool)value && (parameter is string))
            //    return parameter;

            //return string.Empty;

            if ((value is bool) && (bool)value && (parameter is string))
                return parameter;

            return (string)"0";
        }
    }

    
    

    public class StringToBoolean : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((value is string) && (parameter is string))
                return ((string)value == (string)parameter);

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((value is bool) && (bool)value && (parameter is string))
                return parameter;

            return string.Empty;
        }
    }

    public class IsInternetConnectPresenter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool) {
                bool isTrue = (bool)value;
                return (isTrue) ? "接続中" : "未接続";
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BoolNegativeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool)
                return ((bool)value == false);
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
