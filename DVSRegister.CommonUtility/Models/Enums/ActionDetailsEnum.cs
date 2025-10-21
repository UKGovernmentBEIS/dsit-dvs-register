namespace DVSRegister.CommonUtility.Models.Enums
{
    public enum ActionDetailsEnum
    {
        //The keys should match with the ActionDetailsKey column's in ActionDetails table
        CR_APR,                    
        CR_Rej,                    
        CR_Restore,                
        CR_SentBack,                
        CR_DeclinedByProvider,      
        PI_Primary_Pass,            
        PI_SentBack,               
        PI_Primary_Fail,           
        PI_Fail,                   
        PI_ProviderPublish,         
        PI_ServicePublish,     
        PI_ServiceRePublish,      
        ServiceNameUpdate,         
        ServiceUpdates,            
        ProviderContactUpdate,  
        BusinessDetailsUpdate           

    }
}
