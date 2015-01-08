using System;
using System.Collections.Generic;
using System.Text;

namespace TestSamples.Objects
{
    public class ValidationResult
    {
        public ValidationResult()
        {
            ErrorMessages = new List<string>();
            ErrorCount = 0;
        }

        public List<string> ErrorMessages { get; set; }

        public bool Passed
        {
            get { return ErrorCount == 0; }
        }

        public int ErrorCount { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(String.Format("Validation Passed: {0}", Passed.ToString().ToUpper()));
            sb.AppendLine(String.Format("Error Count: {0}", ErrorCount));
            sb.AppendLine("ERROR MESSAGES:");
            foreach (var errorMessage in ErrorMessages)
            {
                sb.AppendLine(errorMessage);
            }
            return sb.ToString();
        }
    }
}
