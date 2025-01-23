using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;

namespace NetSat2025Pn.MauiExample;

public partial class MainPage : ContentPage
{

    SelectedNode? selectedNode;

	public MainPage()
	{
		InitializeComponent();
        hybridWebView.SetInvokeJavaScriptTarget(this);
	}

    public void SetSelectedNode(int index, int value)
    {
        selectedNode = new SelectedNode { Index = index, Value = value };
        Slider.IsEnabled = true;
        Slider.Value = selectedNode.Value;
    }

	private void OnHybridWebViewRawMessageReceived(object sender, HybridWebViewRawMessageReceivedEventArgs e)
    {
        if (e.Message != null)
        {
            //deserializzo il messaggio ricevuto in due valori interi Index e Value
            selectedNode = JsonSerializer.Deserialize<SelectedNode>(e.Message);
            if (selectedNode != null) {
                Slider.IsEnabled = true;
                Slider.Value = selectedNode.Value;
            } 
            else Slider.IsEnabled = false;
        }
	}

    private async void Slider_ValueChanged(object sender, ValueChangedEventArgs e)
    {
        var value = (int)e.NewValue;

        if (selectedNode != null)
        {
            var retVal = await hybridWebView.InvokeJavaScriptAsync("setData",
            HybridSampleJsContext.Default.Boolean, // JSON serialization info for return value
            [selectedNode.Index,value], // Parameter values
            [HybridSampleJsContext.Default.Int32, HybridSampleJsContext.Default.Int32]); // JSON serialization info for each parameter
        }
    }

    private void ButtonBars_Clicked(object sender, EventArgs e)
    {
        hybridWebView.SendRawMessage("bar");
    }

    private void ButtonLines_Clicked(object sender, EventArgs e)
    {
        hybridWebView.SendRawMessage("line");
    }
}
 
[JsonSerializable(typeof(int))]
[JsonSerializable(typeof(bool))]
internal partial class HybridSampleJsContext : JsonSerializerContext
{
    // This type's attributes specify JSON serialization info to preserve type structure
    // for trimmed builds.
}

class SelectedNode
{
    public int Index { get; set; }
    public int Value { get; set; }
}