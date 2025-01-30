using System;

namespace WoT.Core.Errors
{
    /// <summary>
    /// An error that occurs during evaluation
    /// </summary>
    public class EvalError : Exception
    {
        public EvalError() : base() { }
        public EvalError(string messsage) : base(messsage) { }
        public EvalError(string messsage, Exception inner) : base(messsage, inner) { }
        public new string ToString()
        {
            return $"EvalError: {Message}";
        }

    }

    /// <summary>
    /// An error that occurs when a value is not in the set or range of allowed values
    /// </summary>
    public class RangeError : Exception
    {
        public RangeError() : base() { }
        public RangeError(string messsage) : base(messsage) { }
        public RangeError(string messsage, Exception inner) : base(messsage, inner) { }
        public new string ToString()
        {
            return $"RangeError: {Message}";
        }

    }

    /// <summary>
    /// An error that occurs when a reference is not valid
    /// </summary>
    public class ReferenceError : Exception
    {
        public ReferenceError() : base() { }
        public ReferenceError(string messsage) : base(messsage) { }
        public ReferenceError(string messsage, Exception inner) : base(messsage, inner) { }
        public new string ToString()
        {
            return $"ReferenceError: {Message}";
        }

    }

    /// <summary>
    /// An error that occurs when a value is not of the expected type
    /// </summary>
    public class TypeError : Exception
    {
        public TypeError() : base() { }
        public TypeError(string messsage) : base(messsage) { }
        public TypeError(string messsage, Exception inner) : base(messsage, inner) { }
        public new string ToString()
        {
            return $"TypeError: {Message}";
        }
    }

    /// <summary>
    /// An error that occurs when a URI is not valid
    /// </summary>
    public class URIError : Exception
    {
        public URIError() : base() { }
        public URIError(string messsage) : base(messsage) { }
        public URIError(string messsage, Exception inner) : base(messsage, inner) { }
        public new string ToString()
        {
            return $"URIError: {Message}";
        }

    }

    /// <summary>
    /// An error that occurs when a value is not found
    /// </summary>
    public class NotFoundError : Exception
    {
        public NotFoundError() : base() { }
        public NotFoundError(string messsage) : base(messsage) { }
        public NotFoundError(string messsage, Exception inner) : base(messsage, inner) { }
        public new string ToString()
        {
            return $"NotFoundError: {Message}";
        }
    }


    /// <summary>
    /// An error that occurs when a value is not supported
    /// </summary>
    public class NotSupportedError : Exception
    {
        public NotSupportedError() : base() { }
        public NotSupportedError(string messsage) : base(messsage) { }
        public NotSupportedError(string messsage, Exception inner) : base(messsage, inner) { }
        public new string ToString()
        {
            return $"NotSupportedError: {Message}";
        }
    }

    /// <summary>
    /// An error that occurs the operation is insecure.
    /// </summary>
    public class SecurityError : Exception
    {
        public SecurityError() : base() { }
        public SecurityError(string messsage) : base(messsage) { }
        public SecurityError(string messsage, Exception inner) : base(messsage, inner) { }
        public new string ToString()
        {
            return $"SecurityError: {Message}";
        }
    }

    /// <summary>
    /// An error that occurs the operation is insecure.
    /// </summary>
    public class SyntaxError : Exception
    {
        public SyntaxError() : base() { }
        public SyntaxError(string messsage) : base(messsage) { }
        public SyntaxError(string messsage, Exception inner) : base(messsage, inner) { }
        public new string ToString()
        {
            return $"SyntaxError: {Message}";
        }
    }



    /// <summary>
    /// An error that occurs when a value is not readable
    /// </summary>
    public class NotReadableError : Exception
    {
        public NotReadableError() : base() { }
        public NotReadableError(string messsage) : base(messsage) { }
        public NotReadableError(string messsage, Exception inner) : base(messsage, inner) { }
        public new string ToString()
        {
            return $"NotReadableError: {Message}";
        }
    }

    /// <summary>
    /// An error that occurs when operation is not allowed
    /// </summary>
    public class OperationError : Exception
    {
        public OperationError() : base() { }
        public OperationError(string messsage) : base(messsage) { }
        public OperationError(string messsage, Exception inner) : base(messsage, inner) { }
        public new string ToString()
        {
            return $"OperationError: {Message}";
        }
    }

    /// <summary>
    /// An error that occurs when a value is not allowed
    /// </summary>
    public class NotAllowedError : Exception
    {
        public NotAllowedError() : base() { }
        public NotAllowedError(string messsage) : base(messsage) { }
        public NotAllowedError(string messsage, Exception inner) : base(messsage, inner) { }
        public new string ToString()
        {
            return $"NotAllowedError: {Message}";
        }
    }
}
