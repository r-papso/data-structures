﻿namespace SurveyApp.Model
{
    /// <summary>
    /// Model object used in <see cref="View.GenerateWindow"/> View
    /// </summary>
    public class GenerationCriteria
    {
        public bool RandomIds { get; set; }

        /// <summary>
        /// Number of generated locations
        /// </summary>
        public int LocationsCount { get; set; }
    }
}
