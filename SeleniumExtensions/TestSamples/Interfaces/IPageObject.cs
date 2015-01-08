using System;
using TestSamples.Objects;

namespace TestSamples.Interfaces
{
    public interface IPageObject
    {
        /// <summary>
        ///  Guarantee method for if the page object loaded successfully.
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns>ValidationResult object with pass/fail state as a bool and a list any error messages</returns>
        ValidationResult IsLoaded(TimeSpan timeout);

        /// <summary>
        /// Method used to HTML control object loaded in its correct state
        /// </summary>
        /// <returns>ValidationResult object with pass/fail state as a bool and a list any error messages</returns>
        ValidationResult ValidateDefaultRenderState();
    }
}
