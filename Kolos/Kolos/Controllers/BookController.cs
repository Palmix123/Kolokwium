using System.Transactions;
using Kolos.Models;
using Kolos.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Kolos.Controllers;

[ApiController]
public class BookController : ControllerBase
{
    private readonly IBookRepository _bookRepository;

    public BookController(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    [HttpGet]
    [Route("api/books{id:int}/genres")]
    public async Task<IActionResult> GetBook(int id)
    {
        if (!await _bookRepository.DoesBookExist(id))
        {
            return NotFound($"Book with id {id} doesn't exist");
        }

        return Ok(await _bookRepository.GetBook(id));
    }
    
    [HttpPost]
    [Route("api/books")]
    public async Task<IActionResult> AddBook(AddBookDTO bookDto)
    {
        if (await _bookRepository.DoesTitleExist(bookDto.Title))
        {
            return NotFound($"Book with title '{bookDto.Title}' already exist");
        }

        foreach (var idGenre in bookDto.Genres)
        {
            if (!await _bookRepository.DoesGenreExist(idGenre))
            {
                return NotFound($"Genre with id {idGenre} doesn't exist");
            }
        }
        
        BookDTO result = new BookDTO()
        {
            Title = bookDto.Title,
            Genres = new List<string>()
        };
        int idBook = -1; 
        
        using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            idBook = await _bookRepository.AddBook(bookDto.Title);
            result.IdBook = idBook;
            
            foreach (var idGenre in bookDto.Genres)
            {
                await _bookRepository.AddBookGenre(idBook, idGenre);
                var genreName = await _bookRepository.GetGenreName(idGenre);
                result.Genres.Add(genreName);
            }
            scope.Complete();
        }

        if (idBook == -1) // teorytycznie tego nie trzeba dodawac 
        {
            throw new Exception();
        }

        return Created(Request.Path.Value ?? $"api/books/{idBook}/genres", result);
    }
}