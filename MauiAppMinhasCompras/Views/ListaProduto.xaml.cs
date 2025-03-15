using MauiAppMinhasCompras.Models;
using System.Collections.ObjectModel;

namespace MauiAppMinhasCompras.Views;

public partial class ListaProduto : ContentPage
{
	ObservableCollection<Produto> lista = new ObservableCollection<Produto>();
	public ListaProduto()
	{
		InitializeComponent();

		lst_produtos.ItemsSource = lista;
	}

    protected async override void OnAppearing()
    {
        List<Produto> tmp = await App.Db.GetAll();

		tmp.ForEach(i => lista.Add(i));
    }

    private void ToolbarItem_Clicked(object sender, EventArgs e)
    {
		try
		{
			Navigation.PushAsync(new Views.NovoProduto());

		} catch (Exception ex )
		{
			DisplayAlert("Ops", ex.Message, "Ok");
		}
    }

    private CancellationTokenSource debounceTimer;
    private async void txt_search_TextChanged(object sender, TextChangedEventArgs e)
    {
		string q = e.NewTextValue;

		lista.Clear();

        List<Produto> tmp = await App.Db.Search(q);

        tmp.ForEach(i => lista.Add(i));

        string query = e.NewTextValue ?? string.Empty;

        //IMPLEMENTANDO O DEBOUNCE
        debounceTimer?.Cancel();
        debounceTimer = new CancellationTokenSource();

        try
        {
            await Task.Delay(500, debounceTimer.Token); // Aguarda o usuário terminar de digitar

            lista.Clear();
            var produtosFiltrados = await App.Db.Search(query);
            produtosFiltrados.ForEach(produto => lista.Add(produto));
        }
        catch (TaskCanceledException)
        {
            // Ignora tarefas canceladas
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Erro ao buscar produtos: {ex.Message}", "Ok");
        }
    }

    private void ToolbarItem_Clicked_1(object sender, EventArgs e)
    {
		double soma = lista.Sum(i => i.Total);

		string msg = $"O total é {soma:C}";

		DisplayAlert("Total dos Produtos", msg, "Ok");
    }

    private void MenuItem_Clicked(object sender, EventArgs e)
    {
		
    }
}