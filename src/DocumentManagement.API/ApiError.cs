using System.ComponentModel.DataAnnotations;
using Common.System;

namespace DocumentManagement.API
{
    public class ApiError
    {
        public ApiError(string name, string message)
        {
            Ensure.NotNullOrEmpty(name, nameof(name));
            Ensure.NotNullOrEmpty(message, nameof(message));

            Name = name;
            Message = message;
        }

        /// <summary>
        /// Unique error name
        /// </summary>
        [Required]
        public string Name { get; }

        /// <summary>
        /// Meaningful error description
        /// </summary>
        [Required]
        public string Message { get; }
    }

    public static class ErrorExtensions
    {
        public static ApiError ConvertToApiError(this Error error)
        {
            return new ApiError(error.Name, error.Message);
        }
    }
}
