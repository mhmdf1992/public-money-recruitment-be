using System;

namespace Repository.Exceptions
{
    public class CRUDOperationException : Exception
    {
        public CRUDOperationException(CRUDOperation operation) : base($"An unkown error occured on {operation}"){
        }
    }
}
