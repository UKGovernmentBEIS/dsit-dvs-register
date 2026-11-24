namespace DVSRegister.CommonUtility
{
    public static class ServiceLinks
    {
        public static string Latest(Guid providerGuid, Guid serviceGuid) =>
            $"/organisation/{providerGuid}/service/{serviceGuid}/version/latest";
    }
}
