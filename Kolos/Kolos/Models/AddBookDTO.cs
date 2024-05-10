namespace Kolos.Models;

public class AddBookDTO
{
    public String Title { get; set; } = String.Empty;
    public List<int> Genres { get; set; }
}