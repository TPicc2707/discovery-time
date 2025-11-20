namespace Theme.API.Models;

public class Theme
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public int Number { get; set; } = default!;
    public string Letter { get; set; } = default!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime CreatedDate { get; set; }
    public string CreatedBy { get; set; } = default!;
    public DateTime ModifiedDate { get; set; } = default!;
    public string ModifiedBy { get; set;} = default!;
}
