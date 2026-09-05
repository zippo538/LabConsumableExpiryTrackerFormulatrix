using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LabConsumableExpiryTracker.DTOs
{
    public class ApiResponseDTO<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public List<string> Errors { get; set; } = new List<string>();

        public static ApiResponseDTO<T> SuccessResult<T>(T data, string message = "Success")
        {
            return new ApiResponseDTO<T>
            {
                Success = true,
                Message = message,
                Data = data
            };
        }

        public static ApiResponseDTO<T> ErrorResult(string message, List<string>? errors = null)
        {
            return new ApiResponseDTO<T>
            {
                Success = false,
                Message = message,
                Errors = errors ?? new List<string>()
            };
        }

    }
}