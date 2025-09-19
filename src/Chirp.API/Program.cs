using Query;
using Chirp.Types;
using SimpleDB;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

CsvDatabase<Cheep> db = new(Path.Combine(AppContext.BaseDirectory, "Resources", "Data", "chirp_cli_db.csv"));

app.MapGet(
    "/cheeps",
    (HttpRequest request, HttpResponse response) =>
    {
        var predicates = new List<Func<Cheep, bool>>();
        var allQueryParamters = Enum.GetValues(typeof(QueryParamter)).Cast<QueryParamter>();

        foreach(Query.QueryParamter x
                in 
                allQueryParamters) 
        {
            var queryKey = x.ToString();
            var queryValue = request.Query[queryKey].ToString();

            // If value was provided for queryKey, then add that rpedicate
            if(queryValue != "" && x.KeyMatchesValue(queryValue)) {
                predicates.Add( x.ToPredicate(queryValue) );
            }
        }

        if(predicates.Count() > 0) {
            // This applies all predicates on each cheep
            return db.Query((Cheep x) => predicates.Aggregate(true,
                        (acc, f) => acc && f(x)
                    ));
        }else {
            return db.ReadAll();
        }
});

app.MapGet(
    "/cheep",
    (HttpRequest request, HttpRequest response) => 
    {

        var author = request.Query["author"].ToString();
        var message = request.Query["message"].ToString();
        var timestampStr = request.Query["timestamp"].ToString();

        if(author == "") { return "Missing author"; } 
        if(message == "") { return "Missing message"; } 
        if(timestampStr == "") { return "Missing timestamp"; } 

        long timestamp = 0;
        if(!long.TryParse(timestampStr, out timestamp)) {
            return "Timestamp must be in unix time format";
        }

        db.Store(new Cheep(author, message, timestamp));

        return "Cheep'ed";

    }
);

app.MapGet("/", () => "Use /cheeps");

app.Run();