namespace Cocus.Domain.ViewModels;

public class TableRow
{
	public int Id = 0;
	public IEnumerable<string> Values { get; set; } = [];
}

public class TableViewModel
{
	public IEnumerable<string> Headers { get; set; } = [];
	public IEnumerable<TableRow> Rows { get; set; } = [];
	public bool ShowActions { get; set; } = false;
	public string EmptyStateIcon { get; set; } = "fas fa-inbox";
	public string EmptyStateMessage { get; set; } = "No data available";
}
