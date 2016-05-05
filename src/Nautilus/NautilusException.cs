using System;
using System.Runtime.Serialization;
using Octopus.Client.Exceptions;

namespace Nautilus
{
    [Serializable]
    public class NautilusException : Exception
    {    
        public NautilusException(int errorCode, string message) : base(message) 
        { 
            ErrorCode = errorCode;
        }
        
        public NautilusException(int errorCode, string message, string messageDetails) : base(message) 
        { 
            ErrorCode = errorCode;
            MessageDetails = messageDetails;
        }
        
        public NautilusException(int errorCode, string message, Exception inner) : base(message, inner) 
        { 
            ErrorCode = errorCode;
        }
        
        protected NautilusException(SerializationInfo info, StreamingContext context) : base(info, context) 
        { }
        
        public int ErrorCode { get; }
        
        public string MessageDetails { get; }
        
        public static NautilusException UnknownError(Exception ex)
        {
            return new NautilusException(1, "An unknown error occured. See inner exception for details.", ex);
        }
        
        public static NautilusException OctopusError(OctopusException ex)
        {
            return new NautilusException(2, "An Octopus error occured. See inner exception for details.", ex);
        }
        
        public static NautilusException MachineNotRegistered(string machineName)
        {
            return new NautilusException(3, $"The target machine ({machineName}) is not registered with Octopus.");
        }
        
        public static NautilusException TentacleInstallationFailed(string messageDetails)
        {
            return new NautilusException(4, "Tentacle installation failed.", messageDetails);
        }
        
        public static NautilusException TentacleThumbprintNotFound(string messageDetails)
        {
            return new NautilusException(5, "Could not find the Tentacle thumbprint.", messageDetails);
        }
    }
}