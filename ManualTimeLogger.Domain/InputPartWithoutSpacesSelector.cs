using System.Linq;

namespace ManualTimeLogger.Domain
{
    public class InputPartWithoutSpacesSelector : IInputPartSelector
    {
        public string SectionMarker { get; }

        public InputPartWithoutSpacesSelector(string sectionMarker)
        {
            SectionMarker = sectionMarker;
        }

        public InputPartSelectorResult Get(string input)
        {
            if (!input.Contains(SectionMarker))
            {
                return InputPartSelectorResult.Failed($"Section marker {SectionMarker} not found");
            }

            if (input.Count(x => x.ToString() == SectionMarker) > 1)
            {
                return InputPartSelectorResult.Failed($"More than one section marker {SectionMarker} found");
            }

            return InputPartSelectorResult.Success(input.Substring(input.IndexOf(SectionMarker)).Replace(SectionMarker.ToString(), "").Split(' ')[0]);
        }
    }
}
