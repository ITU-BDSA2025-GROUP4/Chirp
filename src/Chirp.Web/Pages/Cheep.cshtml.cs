using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Chirp.Core.Entities;
using Chirp.Core.Interfaces;
using Chirp.Core.Utils;

namespace Chirp.Razor.Pages;

public class CheepSubmitForm
{
    [BindProperty]
    public string Name { get; set; }

    [BindProperty]
    public string Cheep { get; set; }
}

[IgnoreAntiforgeryToken]
public class CheepModel : PageModel
{
    private readonly ICheepService _service;

    public CheepModel(ICheepService service)
    {
        _service = service;
    }

    public void OnGet() {}


    public IActionResult OnPostSubmit(CheepSubmitForm form) {

        if(form.Name == null || form.Cheep == null) {
 //           Console.WriteLine("NULL FORM");
            return BadRequest();
        }

//        Console.WriteLine("GOT: " + form.Name + " - " + form.Cheep);

        string time = TimestampUtils.DateTimeTimeStampToDateTimeString(
                DateTime.Now
        );

        var cheepDTO = new CheepDTO(form.Name, form.Cheep, time);

        bool wasAdded = _service.AddCheep(cheepDTO).Result;
//        Console.WriteLine("RESULT: " + result);

        if(wasAdded)
        return new OkResult();
        else return BadRequest();
    }
}