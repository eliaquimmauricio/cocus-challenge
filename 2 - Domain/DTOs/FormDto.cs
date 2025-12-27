namespace Cocus.Domain.DTOs;

public class FormDto
{
	public int Id { get; set; } = 0;
	public FormMode Mode { get; set; }
	public string ControllerName { get; set; } = string.Empty;
	public string PageTitle { get; set; } = string.Empty;
	public string CancelUrl { get; set; } = string.Empty;

	public bool IsReadOnly => Mode == FormMode.Delete || Mode == FormMode.Details;
	public bool IsDelete => Mode == FormMode.Delete;
	public List<FormField> Fields { get; set; } = new();
}

public class FormField
{
	public string Name { get; set; } = string.Empty;
	public string Label { get; set; } = string.Empty;
	public string PlaceHolder { get; set; } = string.Empty;
	public dynamic Value { get; set; } = default!;

}

public enum FormMode
{
	Create,
	Edit,
	Delete,
	Details
}
