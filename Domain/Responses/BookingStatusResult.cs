namespace Domain.Responses;

public class BookingStatusResult : ResponseResult
{
}


public class BookingStatusResult<T> : BookingStatusResult
{
    public T? Result { get; set; }
}