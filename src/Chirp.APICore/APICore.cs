namespace Chirp.APICore;

using Chirp.Types;
using Utils;
using SimpleDB;

public class APICore {

    private IDatabaseRepository<Cheep> db;
//        = new(Path.Combine(AppContext.BaseDirectory, "Resources", "Data", "chirp_cli_db.csv"));

    public APICore(IDatabaseRepository<Cheep> database) {
        db = database;
    }

    public enum CheepStatusCode {
        SUCCESS,
        MISSING_AUTHOR,
        MISSING_MESSAGE,
        MISSING_TIMESTAMP,
        INVALID_TIMESTAMP
    }

    public string ToString(CheepStatusCode code ) {
        switch(code) {
            case APICore.CheepStatusCode.SUCCESS:
                return "Cheep'ed";
            case APICore.CheepStatusCode.MISSING_AUTHOR:
                return "Missing author name";
            case APICore.CheepStatusCode.MISSING_MESSAGE:
                return "Missing message";
            case APICore.CheepStatusCode.MISSING_TIMESTAMP:
                return "Missing timestamp";
            case APICore.CheepStatusCode.INVALID_TIMESTAMP:
                return "Invalid timestamp";
            default:
                return "UKNOWN ERROR";
        }
    }

    public void Write() {
        db.Write();
    }

    public CheepStatusCode Cheep(Dictionary<string, string> queryParameters) {

        if(!queryParameters.ContainsKey("author")) {
            return CheepStatusCode.MISSING_AUTHOR;
        }else if(!queryParameters.ContainsKey("message")) {
            return CheepStatusCode.MISSING_MESSAGE;
        }else if(!queryParameters.ContainsKey("timestamp")) {
            return CheepStatusCode.MISSING_TIMESTAMP;
        }

        var author = queryParameters["author"];;
        var message = queryParameters["message"];;
        var timestampStr = queryParameters["timestamp"];;

        long timestamp = 0;
        if(!long.TryParse(timestampStr, out timestamp)) {
            return CheepStatusCode.INVALID_TIMESTAMP;
        }

        db.Store(new Cheep(author, message, timestamp));

        return CheepStatusCode.SUCCESS;

    }
    public IEnumerable<Cheep> Cheeps(Dictionary<string, string> actualQueryParameters)
    {
        var predicates = new List<Func<Cheep, bool>>();
        var allQueryParameters = Enum.GetValues(typeof(QueryParameter)).Cast<QueryParameter>();

        foreach(APICore.QueryParameter x
                in 
                allQueryParameters) 
        {
            var queryKey = x.ToString();
            var queryValue = actualQueryParameters.GetValueOrDefault(queryKey, "");

            // If value was provided for queryKey, then add that rpedicate
            if(queryValue != "" && Query.KeyMatchesValue(x, queryValue)) {
                predicates.Add( Query.ToPredicate(x, queryValue) );
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
    }

    public enum QueryParameter
    {
        byUsers        =  1 << 0,
        notByUsers     =  1 << 1,
        beforeTime     =  1 << 2,
        afterTime      =  1 << 3,
        cheepContains  =  1 << 4
    }

    public static class Query {

        public static bool KeyMatchesValue(QueryParameter x, string value) {
            switch(x) 
            {
                case QueryParameter.beforeTime:
                    return StringUtils.IsInteger(value);
                case QueryParameter.afterTime:
                    return StringUtils.IsInteger(value);
                default:
                    return true;
            }

        }

        // value MUST be sanitized before this is called
        public static Func<Cheep, bool> ToPredicate(QueryParameter x, string value) 
        {
            switch(x) 
            {
                case QueryParameter.byUsers:
                    return (Cheep cheep) => {
                        if (value.Contains(',')) {
                            return value.Split(',').Select(x => x.ToLower()).Contains(cheep.Author.ToLower());
                        }
                        return cheep.Author.ToLower() == value.ToLower();
                    };
                case QueryParameter.notByUsers:
                    return (Cheep cheep) => {
                        if (value.Contains(',')) {
                            return !value.Split(',').Select(x => x.ToLower()).Contains(cheep.Author.ToLower());
                        }

                        return cheep.Author.ToLower() != value.ToLower();
                    };
                case QueryParameter.beforeTime:
                    return (Cheep cheep) => cheep.Timestamp < long.Parse(value);
                case QueryParameter.afterTime:
                    return (Cheep cheep) => long.Parse(value) < cheep.Timestamp;
                case QueryParameter.cheepContains:
                    return (Cheep cheep) => cheep.Message.ToLower().Contains(value.ToLower());
            }

            // This should never happen since all cases are covered in the switch statement
            return (Cheep cheep) => false;
        }


        public static String ToString(QueryParameter x) 
        {
            switch (x)
            {
                case QueryParameter.byUsers:
                    return "byUsers";
                case QueryParameter.notByUsers:
                    return "notByUsers";
                case QueryParameter.beforeTime:
                    return "beforeTime";
                case QueryParameter.afterTime:
                    return "afterTime";
                case QueryParameter.cheepContains:
                    return "cheepContains";
            }

            // This shoulder never happen
            return "";
        }
    }


}