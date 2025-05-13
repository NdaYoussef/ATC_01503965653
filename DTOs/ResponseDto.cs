namespace EventManagmentTask.DTOs
{
    public class ResponseDto
    {
        public string Message { get; set; }
        public bool IsSucceeded { get; set; }
        
        public int StatusCode { get; set; }
        public object? Data { get; set; }
    }
}
