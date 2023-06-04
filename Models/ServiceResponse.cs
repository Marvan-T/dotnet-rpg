using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnet_rpg.Models
{
    /**
    This is a wrapper that you can send back to the frontend/client. Frontend is able
    to react to this additional information and read the actual data with the help 
    of HTTP interceptors. 

    We use generics when constructing these wrappers to use the correct types 
    **/
    public class ServiceResponse<T>
    {
        public T? Data { get; set; }
    
        public bool Success { get; set; } = true;

        public string Message { get; set; } = string.Empty; // think try/catch blocks in frontend (would be very helpful to have this message)
        
    }

}