using System;
using System.Linq;

namespace ManualTimeLogger.Domain
{
    /// <summary>
    /// Gets the full text with spaces starting from the
    /// section marker until another section marker or the end of the input
    /// </summary>
    public class InputPartSelector
    {
        public string SectionMarker { get; }
        public string AllSectionMarkers { get; }
        private const string SpaceCharacter = " ";

        public InputPartSelector(string sectionMarker, string allSectionMarkers, bool allowSpaces)
        {
            if (string.IsNullOrEmpty(allSectionMarkers)) throw new ArgumentNullException(nameof(allSectionMarkers));
            if (!allSectionMarkers.Contains(sectionMarker)) throw new ArgumentException("Section marker should be contained in allSectionMarkers", nameof(sectionMarker));
            
            SectionMarker = sectionMarker;
            AllSectionMarkers = allowSpaces 
                ? allSectionMarkers
                : allSectionMarkers + SpaceCharacter;
        }

        public InputPartSelectorResult Get(string input)
        {
            if (!input.Contains(SectionMarker))
            {
                return InputPartSelectorResult.Failed($"Section marker {SectionMarker} not found");
            }

            if (input.Count(currentInputChar => currentInputChar.ToString() == SectionMarker) > 1)
            {
                return InputPartSelectorResult.Failed($"More than one section marker {SectionMarker} found");
            }

            var indexOfSectionMarker = input.IndexOf(SectionMarker, StringComparison.Ordinal);
            var result = input
                .Substring(indexOfSectionMarker)
                .TakeWhile(c => !IsNextSectionReached(c))
                .ToArray();

            return InputPartSelectorResult.Success(new string(result).Replace(SectionMarker, "").Trim(' '));
        }

        private bool IsNextSectionReached(char currentChar)
        {
            return AllSectionMarkers
                .Except(SectionMarker)
                .Contains(currentChar);
        }
    }
}
