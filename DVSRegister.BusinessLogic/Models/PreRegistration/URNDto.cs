using System;
namespace DVSRegister.BusinessLogic.Models.PreRegistration
{
	public class URNDto
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public URNDto()
		{

		}

		public int Id { get; set; }

		public string? RegisteredDIPName { get; set; }

		public required string URN { get; set; }

		public DateTime? LastCheckedTimeStamp { get; set; }

        public DateTime? ReleasedTimeStamp { get; set; }

        public string? CheckedByCAB { get; set; }

        public int Validity { get; set; }

        public string? Status { get; set; }

    }
}