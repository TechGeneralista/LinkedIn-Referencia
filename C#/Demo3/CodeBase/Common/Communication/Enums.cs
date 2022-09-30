namespace Common.Communication
{
    public enum Responses 
    { 
        IdNotExist, 
        NotAvailable, 
        Ok, 
        Nok,
        ErrorTriggerDisabled
    }

    public enum Commands 
    { 
        TriggerOnce, 
        GetResult 
    }
}
