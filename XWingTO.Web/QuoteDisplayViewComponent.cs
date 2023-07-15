namespace XWingTO.Web;

public class QuoteDisplayViewComponent : ViewComponent
{
	private readonly List<string> _quotes;

	public QuoteDisplayViewComponent(List<string> quotes)
	{
		_quotes = quotes;
	}

	public IViewComponentResult Invoke()
	{
		string quote = _quotes.Random();

		return View("Default",quote);
	}
}