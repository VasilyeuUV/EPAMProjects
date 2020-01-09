namespace epam_task4.Enums
{
    public enum EnumErrors
    {
        fileError = 1,              // file missing, unable to open file
        fileNameError,              // incorrect file name        
        fileContentError,           // incorrect file content
        managerError,               // incorrect manager 
        productError,               // incorrect product
        dateError,                  // incorrect data
        costError,                  // incorrect cost
        saveToDbError,
        fileWasSaved
    }   
}
