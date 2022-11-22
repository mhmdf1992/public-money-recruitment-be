using System;

namespace Repository.Exceptions{
    public class ResourceNotFoundException : Exception{
        public ResourceNotFoundException(string key):base($"Resource with key {key} not found"){
        }
    }
}