using SharedKernel.Operation;

namespace SharedKernel.Helper
{
    public static class RegionHelper
    {
        private static readonly Dictionary<string, List<string>> regionCountries = new(StringComparer.OrdinalIgnoreCase)
        {
            ["asia"] = new List<string>
            {
                "Australia", "Brunei", "Cambodia", "Hong Kong", "Macau", "India",
                "Indonesia", "Japan", "South Korea", "Laos", "Myanmar", "Malaysia",
                "New Zealand", "Pakistan", "Philippines", "Singapore", "Sri Lanka",
                "Taiwan", "Thailand", "Vietnam"
            },
            ["include"] = new List<string>
            {
                "USA", "Canada"
            },
            ["exclude"] = new List<string>
            {
                "Mongolia", "Nepal", "Tibet"
            },
            ["No Coverage"] = new List<string>
            {
                "Iran", "Syria", "Belarus", "Cuba", "Congo", "North Korea",
                "Somalia", "Sudan", "South Sudan", "Zimbawe"
            }
        };

        public static Result<string> GetDestinationRegion(List<string> countries)
        {
            if (countries == null || countries.Count == 0)
                return Result<string>.Failed("Unknown Region");

            var inputCountries = countries
                .Where(c => !string.IsNullOrWhiteSpace(c))
                .Select(c => c.Trim())
                .ToList();

            var asiaList = regionCountries["asia"];
            var includeList = regionCountries["include"];
            var noCoverageList = regionCountries["No Coverage"];

            // 1. Check for any No Coverage countries
            var noCoverage = inputCountries
                .FirstOrDefault(c => noCoverageList
                    .Contains(c, StringComparer.OrdinalIgnoreCase));
            if (noCoverage != null)
                return Result<string>.Failed($"We do not cover travel to {noCoverage}.");

            // 2. If all countries are in asia
            if (inputCountries.All(c => asiaList.Contains(c, StringComparer.OrdinalIgnoreCase)))
                return Result<string>.Success("asia");

            // 3. If any country is in include
            if (inputCountries.Any(c => includeList.Contains(c, StringComparer.OrdinalIgnoreCase)))
                return Result<string>.Success("include");

            // 4. If any country is not in asia, include, or no coverage => exclude
            if (inputCountries.Any(c =>
                !asiaList.Contains(c, StringComparer.OrdinalIgnoreCase) &&
                !includeList.Contains(c, StringComparer.OrdinalIgnoreCase) &&
                !noCoverageList.Contains(c, StringComparer.OrdinalIgnoreCase)))
                return Result<string>.Success("exclude");

            // 5. Fallback
            return Result<string>.Failed("Unknown Region");
        }
    }
}
