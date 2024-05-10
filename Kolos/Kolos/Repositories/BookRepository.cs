using Kolos.Models;
using Microsoft.Data.SqlClient;

namespace Kolos.Repositories;

public class BookRepository : IBookRepository
{
    private readonly IConfiguration _configuration;

    public BookRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<bool> DoesBookExist(int id)
    {
        var query = "SELECT 1 FROM books WHERE PK = @id";

        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@id", id);

        await connection.OpenAsync();
        
        var res = await command.ExecuteScalarAsync();
        
        return res is not null;
    }

    public async Task<BookDTO> GetBook(int id)
    {
        var query = @"SELECT B.PK AS BookId, title, genres.name FROM books B
                        JOIN books_genres ON B.PK = books_genres.FK_book
                        JOIN genres ON genres.PK = books_genres.FK_genre
                        WHERE B.PK = @id;";

        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@id", id);

        await connection.OpenAsync();
        
        var reader = await command.ExecuteReaderAsync();
        
        var bookIdOrdinal = reader.GetOrdinal("BookId");
        var titleOrdinal = reader.GetOrdinal("title");
        var nameOrdinal = reader.GetOrdinal("Name");
        
        BookDTO result = null;
        while (await reader.ReadAsync())
        {
            if (result == null)
            {
                result = new BookDTO()
                {
                    IdBook = reader.GetInt32(bookIdOrdinal),
                    Title = reader.GetString(titleOrdinal),
                    Genres = new List<string>()
                    {
                        reader.GetString(nameOrdinal)
                    }
                };
            }
            else
            {
                result.Genres.Add(reader.GetString(nameOrdinal));
            }
        }
        if (result == null){
            throw new Exception();
        }

        return result;
    }

    public async Task<bool> DoesTitleExist(string title)
    {
        var query = "SELECT 1 FROM books WHERE title = @title;";

        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@title", title);

        await connection.OpenAsync();
        
        var res = await command.ExecuteScalarAsync();
        
        return res is not null;
    }

    public async Task<bool> DoesGenreExist(int id)
    {
        var query = "SELECT 1 FROM genres WHERE PK = @id;";

        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@id", id);

        await connection.OpenAsync();
        
        var res = await command.ExecuteScalarAsync();
        
        return res is not null;
    }

    public async Task<int> AddBook(string title)
    {
        var query = "INSERT INTO books VALUES (@title);SELECT @@IDENTITY AS ID;";

        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@title", title);

        await connection.OpenAsync();
        
        var res = await command.ExecuteScalarAsync();

        if (res is null)
        {
            throw new Exception();
        }

        return Convert.ToInt32(res);
    }

    public async Task AddBookGenre(int idBook, int idGenre)
    {
        var query = "INSERT INTO books_genres VALUES (@idBook, @idGenre);";

        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@idBook", idBook);
        command.Parameters.AddWithValue("@idGenre", idGenre);

        await connection.OpenAsync();

        await command.ExecuteNonQueryAsync();
    }

    public async Task<string> GetGenreName(int idGenre)
    {
        var query = "SELECT name FROM genres WHERE PK = @idGenre";

        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@idGenre", idGenre);

        await connection.OpenAsync();

        var res = await command.ExecuteScalarAsync();
        
        if (res is null)
        {
            throw new Exception();
        }
        
        return Convert.ToString(res);
    }
}