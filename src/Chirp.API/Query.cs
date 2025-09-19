using Chirp.Types;

using Utils;

namespace Query {

    public enum QueryParamter
    {
        byUsers        =  1 << 0,
        notByUsers     =  1 << 1,
        beforeTime     =  1 << 2,
        afterTime      =  1 << 3,
        cheepContains  =  1 << 4
    }

    public static class Query {

        public static bool KeyMatchesValue(this QueryParamter x, string value) {
            switch(x) 
            {
                case QueryParamter.beforeTime:
                    return StringUtils.IsInteger(value);
                case QueryParamter.afterTime:
                    return StringUtils.IsInteger(value);
                default:
                    return true;
            }

        }

        // value MUST be sanitized before this is called
        public static Func<Cheep, bool> ToPredicate(this QueryParamter x, string value) 
        {
            switch(x) 
            {
                case QueryParamter.byUsers:
                    return (Cheep cheep) => {
                        if (value.Contains(',')) {
                            return value.Split(',').Select(x => x.ToLower()).Contains(cheep.Author.ToLower());
                        }
                        return cheep.Author.ToLower() == value.ToLower();
                    };
                case QueryParamter.notByUsers:
                    return (Cheep cheep) => {
                        if (value.Contains(',')) {
                            return !value.Split(',').Select(x => x.ToLower()).Contains(cheep.Author.ToLower());
                        }

                        return cheep.Author.ToLower() != value.ToLower();
                    };
                case QueryParamter.beforeTime:
                    return (Cheep cheep) => cheep.Timestamp < long.Parse(value);
                case QueryParamter.afterTime:
                    return (Cheep cheep) => long.Parse(value) < cheep.Timestamp;
                case QueryParamter.cheepContains:
                    return (Cheep cheep) => cheep.Message.ToLower().Contains(value.ToLower());
            }

            // This should never happen since all cases are covered in the switch statement
            return (Cheep cheep) => false;
        }


        public static String ToString(this QueryParamter x) 
        {
            switch (x)
            {
                case QueryParamter.byUsers:
                    return "byUsers";
                case QueryParamter.notByUsers:
                    return "notByUsers";
                case QueryParamter.beforeTime:
                    return "beforeTime";
                case QueryParamter.afterTime:
                    return "afterTime";
                case QueryParamter.cheepContains:
                    return "cheepContains";
            }

            // This shoulder never happen
            return "";
        }
    }

}