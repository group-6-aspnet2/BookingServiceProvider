namespace Domain.Responses;

public class BookingResult : ResponseResult
{
}

public class BookingResult<T> : BookingResult
{
    public T? Result { get; set; }
}