﻿namespace DVSRegister.CommonUtility.Models
{
    public class GenericResponse
    {
        public bool Success { get; set; }
        public bool EmailSent { get; set; }

        public int InstanceId { get; set; }

        public string Data { get; set; }
    }
}
