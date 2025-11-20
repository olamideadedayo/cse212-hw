using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;

public static class SetsAndMaps
{
    private static string filePath;

    /// <summary>
    /// The words parameter contains a list of two character 
    /// words (lower case, no duplicates). Using sets, find an O(n) 
    /// solution for returning all symmetric pairs of words.  
    ///
    /// For example, if words was: [am, at, ma, if, fi], we would return :
    ///
    /// ["am & ma", "if & fi"]
    ///
    /// The order of the array does not matter, nor does the order of the specific words in each string in the array.
    /// at would not be returned because ta is not in the list of words.
    ///
    /// As a special case, if the letters are the same (example: 'aa') then
    /// it would not match anything else (remember the assumption above
    /// that there were no duplicates) and therefore should not be returned.
    /// </summary>
    /// <param name="words">An array of 2-character words (lowercase, no duplicates)</param>
   public static string[] FindPairs(string[] words)
{
    // TODO Problem 1 - ADD YOUR CODE HERE
    
    var seen = new HashSet<string>();
    var output = new List<string>();
    var paired = new HashSet<string>();

    foreach (var w in words)
        seen.Add(w);

    foreach (var w in words)
    {
        if (w.Length == 2 && w[0] == w[1]) continue; // skip "aa"

        var rev = new string(new[] { w[1], w[0] });

        if (seen.Contains(rev) && !paired.Contains(w) && !paired.Contains(rev) && string.CompareOrdinal(w, rev) < 0)
        {
            output.Add($"{w} & {rev}");
            paired.Add(w);
            paired.Add(rev);
        }
    }

    return output.ToArray(); // FIXED: return the built list
}


    /// <summary>
    /// Read a census file and summarize the degrees (education)
    /// earned by those contained in the file.  The summary
    /// should be stored in a dictionary where the key is the
    /// degree earned and the value is the number of people that 
    /// have earned that degree.  The degree information is in
    /// the 4th column of the file.  There is no header row in the
    /// file.
    /// </summary>
    /// <param name="filename">The name of the file to read</param>
    /// <returns>fixed array of divisors</returns>
    public static Dictionary<string, int> SummarizeDegrees(string filename)
{
    var degrees = new Dictionary<string, int>();
    foreach (var line in File.ReadLines(filename))
    {
        var fields = line.Split(",");
        // TODO Problem 2 - ADD YOUR CODE HERE

        if (fields.Length < 4) continue;
        var degree = fields[3].Trim();
        if (degree.Length == 0) continue;

        if (!degrees.ContainsKey(degree))
            degrees[degree] = 0;

        degrees[degree]++;
    }

    return degrees; // FIXED: return populated dictionary
}

    /// <summary>
    /// Determine if 'word1' and 'word2' are anagrams.  An anagram
    /// is when the same letters in a word are re-organized into a 
    /// new word.  A dictionary is used to solve the problem.
    /// 
    /// Examples:
    /// is_anagram("CAT","ACT") would return true
    /// is_anagram("DOG","GOOD") would return false because GOOD has 2 O's
    /// 
    /// Important Note: When determining if two words are anagrams, you
    /// should ignore any spaces.  You should also ignore cases.  For 
    /// example, 'Ab' and 'Ba' should be considered anagrams
    /// 
    /// Reminder: You can access a letter by index in a string by 
    /// using the [] notation.
    /// </summary>
   public static bool IsAnagram(string word1, string word2)
{
    // TODO Problem 3 - ADD YOUR CODE HERE

    string A = new string(word1.Where(c => !char.IsWhiteSpace(c)).Select(char.ToUpperInvariant).ToArray());
    string B = new string(word2.Where(c => !char.IsWhiteSpace(c)).Select(char.ToUpperInvariant).ToArray());

    if (A.Length != B.Length) return false;

    var freq = new Dictionary<char, int>();
    foreach (var ch in A)
    {
        if (!freq.ContainsKey(ch)) freq[ch] = 0;
        freq[ch]++;
    }

    foreach (var ch in B)
    {
        if (!freq.ContainsKey(ch)) return false;
        freq[ch]--;
        if (freq[ch] < 0) return false;
    }

    return true; // FIXED: return true when all checks pass
}


    /// <summary>
    /// This function will read JSON (Javascript Object Notation) data from the 
    /// United States Geological Service (USGS) consisting of earthquake data.
    /// The data will include all earthquakes in the current day.
    /// 
    /// JSON data is organized into a dictionary. After reading the data using
    /// the built-in HTTP client library, this function will return a list of all
    /// earthquake locations ('place' attribute) and magnitudes ('mag' attribute).
    /// Additional information about the format of the JSON data can be found 
    /// at this website:  
    /// 
    /// https://earthquake.usgs.gov/earthquakes/feed/v1.0/geojson.php
    /// 
    /// </summary>
   public static string[] EarthquakeDailySummary()
{
    const string uri = "https://earthquake.usgs.gov/earthquakes/feed/v1.0/summary/all_day.geojson";
    using var client = new HttpClient();
    using var getRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);
    using var response = client.Send(getRequestMessage);
    response.EnsureSuccessStatusCode();
    using var jsonStream = response.Content.ReadAsStream();
    using var reader = new StreamReader(jsonStream);
    var json = reader.ReadToEnd();
    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

    var featureCollection = JsonSerializer.Deserialize<FeatureCollection>(json, options);

    var output = new List<string>();

    if (featureCollection?.features != null)
    {
        foreach (var f in featureCollection.features)
        {
            var place = f?.properties?.place ?? "Unknown location";
            var mag = f?.properties?.mag;
            var magStr = mag.HasValue ? mag.Value.ToString() : "N/A";
            output.Add($"{place} : {magStr}");
        }
    }

    return output.ToArray();
}

    // JSON mapping classes for deserialization
    private class FeatureCollection
    {
        public List<Feature> features { get; set; } = new();
    }

    private class Feature
    {
        public Properties properties { get; set; } = new();
    }

    private class Properties
    {
        public string place { get; set; }
        public double? mag { get; set; }
    }

}


