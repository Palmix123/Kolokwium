using Kolos.Models;

namespace Kolos.Repositories;

public interface IBookRepository
{
    Task<bool> DoesBookExist(int id);
    Task<BookDTO> GetBook(int id);
    Task<bool> DoesTitleExist(string title);
    Task<bool> DoesGenreExist(int id);
    Task<int> AddBook(string title);
    Task AddBookGenre(int idBook, int idGenre);
    Task<string> GetGenreName(int idGenre);
}