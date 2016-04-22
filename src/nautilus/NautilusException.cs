using System;
using System.Runtime.Serialization;

namespace Nautilus
{
    [Serializable]
    public class NautilusException : Exception
    {    
        public NautilusException(int errorCode, string message) : base(message) 
        { 
            ErrorCode = errorCode;
        }
        
        public NautilusException(int errorCode, string message, Exception inner) : base(message, inner) 
        { 
            ErrorCode = errorCode;
        }
        
        protected NautilusException(SerializationInfo info, StreamingContext context) : base(info, context) 
        { }
        
        public int ErrorCode { get; }
    }
}