using System;
using System.Collections.Generic;
using System.Text;

namespace WoT.Errors
{
    class EvalError : Exception
    {
        public EvalError() : base() { }
        public EvalError(string messsage) : base(messsage) { }
        public EvalError(string messsage, Exception inner) : base(messsage, inner) { }
        public new string ToString()
        {
            return $"EvalError: {this.Message}";
        }

    }
    class RangeError : Exception
    {
        public RangeError() : base() { }
        public RangeError(string messsage) : base(messsage) { }
        public RangeError(string messsage, Exception inner) : base(messsage, inner) { }
        public new string ToString()
        {
            return $"RangeError: {this.Message}";
        }

    }
    class ReferenceError : Exception
    {
        public ReferenceError() : base() { }
        public ReferenceError(string messsage) : base(messsage) { }
        public ReferenceError(string messsage, Exception inner) : base(messsage, inner) { }
        public new string ToString()
        {
            return $"ReferenceError: {this.Message}";
        }

    }
    class TypeError : Exception
    {
        public TypeError() : base() { }
        public TypeError(string messsage) : base(messsage) { }
        public TypeError(string messsage, Exception inner) : base(messsage, inner) { }
        public new string ToString()
        {
            return $"TypeError: {this.Message}";
        }
    }
    class URIError : Exception
    {
        public URIError() : base() { }
        public URIError(string messsage) : base(messsage) { }
        public URIError(string messsage, Exception inner) : base(messsage, inner) { }
        public new string ToString()
        {
            return $"URIError: {this.Message}";
        }

    }
    class NotFoundError : Exception
    {
        public NotFoundError() : base() { }
        public NotFoundError(string messsage) : base(messsage) { }
        public NotFoundError(string messsage, Exception inner) : base(messsage, inner) { }
        public new string ToString()
        {
            return $"NotFoundError: {this.Message}";
        }
    }

    class NotSupportedError : Exception
    {
        public NotSupportedError() : base() { }
        public NotSupportedError(string messsage) : base(messsage) { }
        public NotSupportedError(string messsage, Exception inner) : base(messsage, inner) { }
        public new string ToString()
        {
            return $"NotSupportedError: {this.Message}";
        }
    }

    class SyntaxErrpr : Exception
    {
        public SyntaxErrpr() : base() { }
        public SyntaxErrpr(string messsage) : base(messsage) { }
        public SyntaxErrpr(string messsage, Exception inner) : base(messsage, inner) { }
        public new string ToString()
        {
            return $"SyntaxErrpr: {this.Message}";
        }
    }

    class NotReadableError: Exception
    {
        public NotReadableError() : base() { }
        public NotReadableError(string messsage) : base(messsage) { }
        public NotReadableError(string messsage, Exception inner) : base(messsage, inner) { }
        public new string ToString()
        {
            return $"NotReadableError: {this.Message}";
        }
    }

    class OperationError : Exception
    {
        public OperationError() : base() { }
        public OperationError(string messsage) : base(messsage) { }
        public OperationError(string messsage, Exception inner) : base(messsage, inner) { }
        public new string ToString()
        {
            return $"OperationError: {this.Message}";
        }
    }

    class NotAllowedError : Exception
    {
        public NotAllowedError() : base() { }
        public NotAllowedError(string messsage) : base(messsage) { }
        public NotAllowedError(string messsage, Exception inner) : base(messsage, inner) { }
        public new string ToString()
        {
            return $"NotAllowedError: {this.Message}";
        }
    }
}
