
namespace Utils {

    public class StringUtils {
        public static bool IsInteger(string x) {
            if(x.Length == 0) return false;
            return x.Select(x => Char.IsDigit(x)).Aggregate((x, y) => x && y);
        }
    }

}