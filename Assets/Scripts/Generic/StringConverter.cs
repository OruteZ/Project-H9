public static class StringConverter
{
    public static string CovertToCamelCase(string str)
    {
        if (string.IsNullOrEmpty(str))
            return str;
        
        // return all lowered
        var ret = char.ToLower(str[0]).ToString();
        
        // add other letter lowered
        for (int i = 1; i < str.Length; i++)
        {
            ret += char.ToLower(str[i]);
        }
        
        return ret;
    }
    
    public static string CovertToPascalCase(string str)
    {
        if (string.IsNullOrEmpty(str))
            return str;
        
        // Capitalize the first letter
        var ret = char.ToUpper(str[0]).ToString();
        
        // add other letter lowered
        for (int i = 1; i < str.Length; i++)
        {
            ret += char.ToLower(str[i]);
        }

        return ret;
    }
    
    public static string CovertToSnakeCase(string str)
    {
        if (string.IsNullOrEmpty(str))
            return str;

        return string.Join("_", str.Split(' ')).ToLower();
    }
}