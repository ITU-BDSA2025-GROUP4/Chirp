using Chirp.APICore;
using Chirp.Types;

using SimpleDB;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
WebApplication app = builder.Build();


string dbPath = null;

for (int i = 0; i < args.Length; i++)
{
    if (args[i] == "--path")
    {
        if (i + 1 < args.Length)
        {
            dbPath = args[i+1];
               break;
        }
    }
}

dbPath ??= Path.Combine(AppContext.BaseDirectory, "Resources", "Data", "chirp_cli_db.csv");

IDatabaseRepository<Cheep> db; 
if(dbPath.EndsWith(".csv")) 
{
    db = new CsvDatabase<Cheep>(dbPath);
}
else 
{
    db = new SQLiteDatabase<Cheep>(dbPath);
}

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
        core.ToString(core.Cheep(QueryToDict(request.Query)))
);

app.MapGet("/", () => "Use /cheeps");

app.Run();

// Write DB to file before quitting
core.Write();