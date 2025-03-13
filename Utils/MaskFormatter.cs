using ApiElecateProspectsForm.Interfaces;
using System.Text;

namespace ApiElecateProspectsForm.Utils
{
    public class MaskFormatter : IMaskFormatter
    {
        // Function to apply the mask to the value
        public string ApplyMask(string value, string mask)
        {
            StringBuilder maskedValue = new();
            int valueIndex = 0;

            foreach (char maskChar in mask)
            {
                if (maskChar == '0')
                {
                    if (valueIndex < value.Length)
                    {
                        maskedValue.Append(value[valueIndex]);
                        valueIndex++;
                    }
                    else
                    {
                        maskedValue.Append('0'); // Fill with zeros if the value is shorter than the mask
                    }
                }
                else
                {
                    maskedValue.Append(maskChar);
                }
            }

            return maskedValue.ToString();
        }
    }
}
