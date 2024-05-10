namespace Kolos.Models;

public class BookDTO
{
    public int IdBook { get; set; }
    public String Title { get; set; } = String.Empty;
    public List<string> Genres { get; set; }
}
