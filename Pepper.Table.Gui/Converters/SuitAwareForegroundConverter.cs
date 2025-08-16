using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Pepper.Cards.Data.DbModels;
using Pepper.Cards.Data.Enums;

namespace Pepper.Table.Gui.Converters
{
    internal class SuitAwareForegroundConverter : IValueConverter
    {
        private static SolidColorBrush _red = new SolidColorBrush(Color.FromRgb(200, 0, 0));
        private static SolidColorBrush _black = new SolidColorBrush(Color.FromRgb(0, 0, 0));
        private static SolidColorBrush _blue = new SolidColorBrush(Color.FromRgb(40, 60, 180));
        private static SolidColorBrush _green = new SolidColorBrush(Color.FromRgb(0, 120, 0));

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var card = (Card)value;

            if (!card.DeckStyle.IsFourColour)
            {
                return card.Suit switch
                {
                    Suit.Spades => _black,
                    Suit.Hearts => _red,
                    Suit.Diamonds => _red,
                    Suit.Clubs => _black,

                    _ => _black
                };
            }
            else
            {
                return card.Suit switch
                {
                    Suit.Spades => _black, // Black
                    Suit.Hearts => _red, // Red
                    Suit.Diamonds => _blue,
                    Suit.Clubs => _green,

                    _ => _black
                };
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
