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

public class CheepModel : PageModel
{
    private readonly ICheepService _service;

    public CheepModel(ICheepService service)
    {
        _service = service;
    }

    public void OnGet() {}

    public void OnPostSubmit(CheepSubmitForm form) {

        if(form.Name == null || form.Cheep == null) {
 //           Console.WriteLine("NULL FORM");
            return;
        }

//        Console.WriteLine("GOT: " + form.Name + " - " + form.Cheep);

        string time = TimestampUtils.DateTimeTimeStampToDateTimeString(
                DateTime.Now
        );

        var cheepDTO = new CheepDTO(form.Name, form.Cheep, time);

        bool result = _service.AddCheep(cheepDTO).Result;

//        Console.WriteLine("RESULT: " + result);

    }
}