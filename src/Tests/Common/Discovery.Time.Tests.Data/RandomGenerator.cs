namespace Discovery.Time.Tests.Data;

public class RandomGenerator
{
    private readonly Random _random = new Random();

    public long RandomNumber(int length)
    {
        try
        {
            string numberString = string.Empty;
            for (int i = 0; i < length; i++)
                numberString = String.Concat(numberString, _random.Next(10));

            var test = long.Parse(numberString);

            return long.Parse(numberString);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }

    public int RandomNumberBetweenMinAndMax(int min, int max)
    {
        return _random.Next(min, max);
    }

    public string RandomString(int size, bool lowerCase = false)
    {
        var builder = new StringBuilder(size);

        char offset = lowerCase ? 'a' : 'A';
        const int lettersOffset = 26;

        for (var i = 0; i < size; i++)
        {
            var @char = (char)_random.Next(offset, offset + lettersOffset);
            builder.Append(@char);
        }

        return lowerCase ? builder.ToString().ToLower() : builder.ToString();
    }
}
