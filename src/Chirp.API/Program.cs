
using Chirp.APICore;
using Chirp.Types;

using SimpleDB;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

IDatabaseRepository<Cheep> db =
    new CsvDatabase<Cheep>(
            Path.Combine(AppContext.BaseDirectory, "Resources", "Data", "chirp_cli_db.csv")
);

APICore core = new APICore(db);

Dictionary<string, string> QueryToDict(IQueryCollection collection)
{
    return collection
        .Where(x => x.Value.ToString() != "")
        .ToDictionary(x => x.Key, x => x.Value.ToString());
}

app.MapGet(
    "/cheeps",
    (HttpRequest request, HttpResponse response) =>
        core.Cheeps(QueryToDict(request.Query))
);

app.MapGet(
    "/cheep",
    (HttpRequest request, HttpResponse response) =>
    core.ToString( core.Cheep(QueryToDict(request.Query)) )
);

app.MapGet("/", () => "Use /cheeps");

app.Run();

// Write DB to file before quitting
core.Write();