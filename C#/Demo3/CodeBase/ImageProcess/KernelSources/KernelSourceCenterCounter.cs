namespace ImageProcess.KernelSources
{
    public static class KernelSourceCenterCounter
    {
        public static string FunctionName = "center_counter";

        public static string FunctionSource =
            @"

                __kernel void " + FunctionName + @"(global uchar* result, volatile global uint* counter) 
                {
                    int resultPos = get_global_id(0) + (get_global_id(1) * get_global_size(0)); 
                    
                    if(result[resultPos] == 1)
                        atomic_inc(counter);
                }
                
            ";

        
    }
}
