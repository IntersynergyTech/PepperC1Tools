using Pepper.Cards.Data.DbModels;
using Pepper.Cards.Data.Enums;
using System.Globalization;
using System.Windows.Data;

namespace Pepper.Table.Gui.Converters
{
    internal class CardCornerValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var card = (Card)value;

            return $"{card.Value.ToCardDisplayString()}\n{card.Suit.ToSymbol()}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
