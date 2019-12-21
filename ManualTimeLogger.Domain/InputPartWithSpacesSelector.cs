using System;
using System.Linq;

namespace ManualTimeLogger.Domain
{
    public class InputPartWithSpacesSelector : IInputPartSelector
    {
        public string SectionMarker { get; }
        public string AllSectionMarkers { get; }

        public InputPartWithSpacesSelector(string sectionMarker, string allSectionMarkers)
        {
            if (string.IsNullOrEmpty(allSectionMarkers)) throw new ArgumentNullException(nameof(allSectionMarkers));
            if (!allSectionMarkers.Contains(sectionMarker)) throw new ArgumentException("Section marker should be contained in allSectionMarkers", nameof(sectionMarker));
            
            SectionMarker = sectionMarker;
            AllSectionMarkers = allSectionMarkers;
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
