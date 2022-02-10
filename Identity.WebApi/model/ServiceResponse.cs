using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Identity.WebApi.model
{
	public class ServiceResponse<T>
	{
		public bool isSuccess { get; set; }
		public T Response { get; set; }
		public string Message { get; set; }
		public Guid ResponseID { get; set; }
		public int ResultCode { get; set; }
		public ServiceResponse(params object[] args)
		{
			ResponseID = Guid.NewGuid();
			if (typeof(T).IsArray == false && typeof(T) != typeof(string))
			{
				Response = (T)Activator.CreateInstance(typeof(T), args);
				
			}
		}
	}
}

