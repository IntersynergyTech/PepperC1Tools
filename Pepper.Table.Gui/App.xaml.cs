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
        CardsDbContext = new CardsDbContext();

    }
}
