using System.Windows;
using Microsoft.EntityFrameworkCore;
using Pepper.Cards.Database;

namespace Pepper.Table.Gui;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public static CardsDbContext CardsDbContext { get; set; }

    public App()
    {
        var optionsBulder = new DbContextOptionsBuilder<CardsDbContext>();
        optionsBulder.UseSqlite("Data Source=C:/sources/pepper-c1-tool/cards.db");
        CardsDbContext = new CardsDbContext(optionsBulder.Options);

    }
}
