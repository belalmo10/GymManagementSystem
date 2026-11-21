namespace GymManagement.BLL.Common
{
    public sealed record Result(bool success , string? message = null , ResultKind Kind = ResultKind.Ok)
    {
        public static Result Ok() => new Result(true);
        public static Result Fail(string msg)
        {
            return new Result(false , msg , ResultKind.Conflict);
        }

        public static Result ValidationError(string msg) => new Result(false , msg , ResultKind.ValidationFailed);

        public static Result NotFound(string msg = "Not Found")
        {
            return new Result(false , msg , ResultKind.NotFound);
        }
    }

    public sealed record Result<T>(bool success , T? value , string? message = null , ResultKind Kind = ResultKind.Ok)
    {
        public static Result<T> Ok(T value) => new Result<T>(true , value);
        public static Result<T> Fail(string msg) => new Result<T>(false , default , msg , ResultKind.Conflict);
        public static Result<T> NotFound(string msg) => new Result<T>(false , default , msg , ResultKind.NotFound);

    }
}
