using AntaresPay.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntaresPay.Converters
{
    public class OperationTypeEnumToStringConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is OperationTypeEnum operationType)
            {
                return operationType switch
                {
                    OperationTypeEnum.Payment => "Pagamento",
                    OperationTypeEnum.Charge => "Cobrança",
                    _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
                };
            }
            return string.Empty;
        }
        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
