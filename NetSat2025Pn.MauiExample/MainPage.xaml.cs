using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;

namespace NetSat2025Pn.MauiExample;

public partial class MainPage : ContentPage
{
    class SelectedNode
    {
        public int Index { get; set; }
        public int Value { get; set; }
    }
    
    //Oggetto che rappresenta il nodo selezionato
    SelectedNode? selectedNode;

	public MainPage()
	{
		InitializeComponent();
        
        //Necessario per poter chimare chimare il metodo SetSelectedNode da javascript
        //Specifica il target per l'invocazione di metodi
        hybridWebView.SetInvokeJavaScriptTarget(this);
	}

    //Evento scatenato quando viene ricevuto un messaggio da javascript
	private void OnHybridWebViewRawMessageReceived(object sender, HybridWebViewRawMessageReceivedEventArgs e)
    {
        if (e.Message != null)
        {
            //Deserializzo il messaggio ricevuto da javascript per ottenere l'indice e il valore del nodo selezionato
            selectedNode = JsonSerializer.Deserialize<SelectedNode>(e.Message);
            if (selectedNode != null) {
                Slider.IsEnabled = true;
                Slider.Value = selectedNode.Value;
            } 
            else Slider.IsEnabled = false;
        }
	}

    //Invio un messaggio a javascript per cambiare il tipo di grafico
    private void ButtonBars_Clicked(object sender, EventArgs e)
    {
        hybridWebView.SendRawMessage("bar");
    }

    //Utilizzo il metodo EvaluateJavaScriptAsync per cambiare il tipo di grafico invece di usare i messaggi
    private async void ButtonLines_Clicked(object sender, EventArgs e)
    {        
        await hybridWebView.EvaluateJavaScriptAsync("myChart.config.type='line'; myChart.update();");
    }

    //Metodo chiamato da javascript per settare il nodo selezionato
    //Modalità alternativa all'uso di SendRawMessage
    public void SetSelectedNode(int index, int value)
    {
        selectedNode = new SelectedNode { Index = index, Value = value };
        Slider.IsEnabled = true;
        Slider.Value = selectedNode.Value;
    }

    //Quando il valore dello slider cambia, invio il nuovo valore al nodo selezionato chimanao il metodo setData di javascript
    private async void Slider_ValueChanged(object sender, ValueChangedEventArgs e)
    {
        var value = (int)e.NewValue;

        if (selectedNode != null)
        {
            var retVal = await hybridWebView.InvokeJavaScriptAsync("setData",
            HybridSampleJsContext.Default.Boolean, // JSON serialization info del valore di ritorno
            [selectedNode.Index,value], // Parameter values
            [HybridSampleJsContext.Default.Int32, HybridSampleJsContext.Default.Int32]); // JSON serialization info per ogni parametro
        }
    }
}
 
[JsonSerializable(typeof(int))]
[JsonSerializable(typeof(bool))]
internal partial class HybridSampleJsContext : JsonSerializerContext
{
    // This type's attributes specify JSON serialization info to preserve type structure
    // for trimmed builds.
}

